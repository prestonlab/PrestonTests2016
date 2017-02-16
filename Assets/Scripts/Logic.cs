﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System;
using System.IO;
using System.Text;

using Eppy; // Tuple<T1, T2>

public class Logic : MonoBehaviour {

    public GameObject Environments = null;
    public GameObject CanvasCoord = null;

    public Config globalConfig = null; // Global settings, like actionKey

    private bool fplayerfoundtarget = false;

    private void ResetCallbacks(){
        fplayerfoundtarget = false;
    }

    // Callback given to Triggers / Objects to let our guy know we're done
    private void TriggerCallback(){
        fplayerfoundtarget = true;
    }

    private IEnumerator OnFindTarget(GameObject player){
        print("GOOD JOB U FOUND TARGET");
        fplayerfoundtarget = false;
        // Freeze controls
        player.SendMessage("DisableInput");
        yield return new WaitForSeconds(.5f); // TODO Configurable, global
        player.SendMessage("EnableInput");
    }

    // Helper
    private GameObject GetEnvGO(GameObject environments, int envIndex){
        return environments.transform.GetChild(envIndex).gameObject;
    }

    // objShowIndex: index of object to show
    // showTime: time we show the object
    // greyScreenTime: time we show the plus
    private IEnumerator ShowGrayScreen(int objShowIndex, float showTime, float greyScreenTime){
        CanvasCoord.SendMessage("ShowGray");
        print("ShowGrayScreen(): Enabled grayscreen");

        CanvasCoord.SendMessage("ShowImage", objShowIndex);
        print(String.Format("ShowGrayScreen(): Enabled Image {0}", objShowIndex));

        yield return new WaitForSeconds(showTime);

        CanvasCoord.SendMessage("HideImage");
        print(String.Format("ShowGrayScreen(): Disabled Image {0}", objShowIndex));

        CanvasCoord.SendMessage("ShowPlus");

        yield return new WaitForSeconds(greyScreenTime);

        CanvasCoord.SendMessage("HidePlus");
        CanvasCoord.SendMessage("HideGray");
        print("ShowGrayScreen(): Disabled grayscreen");
    }

    public IEnumerator RunNormalScene(Scene s, Logger logger){
        print("RunNormalScene(): Starting");

        // Show GrayScreen
        foreach(object o in E.YieldFrom(ShowGrayScreen(s.objShowIndex, s.showTime, s.greyScreenTime)))
            yield return o;

        // Env we'll be sending message to
        GameObject curenv = GetEnvGO(Environments, s.envIndex);

        curenv.BroadcastMessage("SpawnPlayerAtIndex", s.playerSpawnIndex);
        curenv.BroadcastMessage("ActivateObjTriggerAtIndex",
                new ObjSpawner.TriggerInfo(s.objSpawnIndex, TriggerCallback, s.objShowIndex));
        curenv.BroadcastMessage("ShowLandmark", s.landmarkSpawnIndex);

        // Get player reference
        GameObject player = GameObject.FindWithTag("Player");

        // Setup logger, using environment info component
        EnvInfo envinfo = (EnvInfo)curenv.GetComponent<EnvInfo>();
        logger.StartTrial(envinfo.GetActiveTriggerObj().transform.position, player, envinfo.GetOrigin());

        //
        // Specific scene logic
        //

        float curtime = Time.time;
        yield return new WaitUntil(() => Input.GetKeyDown(globalConfig.actionKey) || Time.time - curtime >= s.envTime);

        // Check if player found target after the press
        // An ObjTrigger would have used the callback if it existed

        print(String.Format("RunScene(): fplayerfoundtarget: '{0}'", fplayerfoundtarget));

        if(fplayerfoundtarget){
            print("You found the target without having to be shown it!");
            curenv.BroadcastMessage("ShowSelf");

            // Turn player towards object
            foreach(object o in E.YieldFrom((player.GetComponent<PlayerAction>() as PlayerAction).PlayerLookTowards()))
                yield return o;

            foreach(object o in E.YieldFrom(OnFindTarget(player)))
                yield return o;
            curenv.BroadcastMessage("HideSelf");
        }else{
            print("You didn't find the target in time. Let me show it to you...");

            // TODO Turn the target on! Show the billboarded sprite!
            curenv.BroadcastMessage("ShowSelf");

            // Freeze player controls
            player.SendMessage("DisableInput");

            // Turn player towards object
            foreach(object o in E.YieldFrom((player.GetComponent<PlayerAction>() as PlayerAction).PlayerLookTowards()))
                yield return o;

            player.SendMessage("EnableInput");
            print("...now go find the target!");

            yield return new WaitUntil(() => fplayerfoundtarget);

            foreach(object o in E.YieldFrom(OnFindTarget(player)))
                yield return o;
            curenv.BroadcastMessage("HideSelf");
        }

        logger.EndTrial();

        curenv.BroadcastMessage("RemovePlayer");
        curenv.BroadcastMessage("DeactiveateTriggers");
        curenv.BroadcastMessage("HideLandmark");

        foreach(object o in E.YieldFrom(ShowGrayScreen(0, 0.0f, s.greyScreenTimeTwo)))
            yield return o;

        print("Scene(): Done");
    }

    // Placed in environment where they can explore for 2-3 mins, no objects.
    public IEnumerator RunExploreScene(Scene s, Logger logger){
        print("ExploreScene(): Starting");

        // Doesn't have GrayScreen

        GameObject curenv = GetEnvGO(Environments, s.envIndex);
        curenv.BroadcastMessage("SpawnPlayerAtIndex", s.playerSpawnIndex);
        yield return new WaitForSeconds(s.envTime);
        curenv.BroadcastMessage("RemovePlayer");

        print("ExploreScene(): Done");
    }

    // In environment, obj in the world, go get it, repeat
    public IEnumerator RunSearchFindScene(Scene s, Logger logger){
        print("SearchFindScene(): Starting");

        // Doesn't have GrayScreen

        GameObject curenv = GetEnvGO(Environments, s.envIndex);
        curenv.BroadcastMessage("SpawnPlayerAtIndex", s.playerSpawnIndex);

        int counter = 0;
        foreach(SearchObj so in s.searchObjs){
            print(String.Format("SearchFindScene(): Player looking for target {0}", counter));

            ObjSpawner.TriggerInfo curti = new ObjSpawner.TriggerInfo(so.objSpawnIndex, TriggerCallback, so.objSpriteIndex);
            print(String.Format("SearchFindScene(): curTriggerInfo: {0}", curti));

            curenv.BroadcastMessage("ActivateObjTriggerAtIndex", curti);

            yield return new WaitUntil(() => fplayerfoundtarget);
            ResetCallbacks();

            print("SearchFindScene(): Player found target");

            curenv.BroadcastMessage("DeactiveateTriggers");

            counter += 1;
        }

        curenv.BroadcastMessage("RemovePlayer");

        print("SearchFindScene(): Done");
    }

    public IEnumerator RunScene(Scene s, Logger logger){
        // TODO Maybe refactor to class with inheritance?
        switch(s.mode){
            case "normal":
                foreach(object o in E.YieldFrom(RunNormalScene(s, logger)))
                    yield return o;
                break;
            case "explore":
                foreach(object o in E.YieldFrom(RunExploreScene(s, logger)))
                    yield return o;
                break;
            case "searchfind":
                foreach(object o in E.YieldFrom(RunSearchFindScene(s, logger)))
                    yield return o;
                break;
            default:
                System.Diagnostics.Debug.Assert(false, String.Format("scene mode is invalid: '{0}'", s.mode));
                return false;
        }
    }

    public IEnumerator RunAllScenes(Tuple<IEnumerable<Scene>, Logger> tup){
        IEnumerable<Scene> scenes = tup.Item1;
        Logger logger = tup.Item2;
        logger.StartPhase(globalConfig.phaseName);

        int counter = 0;
        foreach(Scene s in scenes){
            print(String.Format("AllScenes(): Running scene number: {0}", counter));

            foreach(object o in E.YieldFrom(RunScene(s, logger)))
                yield return o;

            print(String.Format("RunAllScenes(): Done running scene number: {0}", counter));
            counter += 1;
        }

        logger.EndPhase();

        print("AllScenes(): Done running all scenes!");
    }

	void Start(){
        // Read Json file to configure stuff
        //

        string jsonpath = @"config.json";
        if(!File.Exists(jsonpath)){
            Debug.LogError(String.Format("Couldn't load config file at: {0}", jsonpath));
            Debug.Assert(false);
            Application.Quit();
        }

        string configjson = File.ReadAllText(jsonpath);
        Config config = Config.Create(configjson);
        globalConfig = config;

        // Setup logger
        Logger logger = GetComponent<Logger>();
        logger.InitLogger();

        StartCoroutine("RunAllScenes", new Tuple<IEnumerable<Scene>, Logger>(config.scenes, logger));
	}
}
