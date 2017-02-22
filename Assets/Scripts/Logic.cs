using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System;
using System.IO;
using System.Text;

using Eppy; // Tuple<T1, T2>

public class Logic : MonoBehaviour {
    private float IntroGreyScreenTime = 4.0f;

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
        ResetCallbacks();
        yield return new WaitForSeconds(globalConfig.pauseTime);
    }

    // Helper
    private GameObject GetEnvGO(GameObject environments, int envIndex){
        return environments.transform.GetChild(envIndex).gameObject;
    }

    // objShowIndex: index of object to show
    // showTime: time we show the object
    // greyScreenTime: time we show the plus
    private IEnumerator ShowGrayScreen(int objShowIndex, float showTime, float greyScreenTime){
        if(showTime <= 0.0f && greyScreenTime <= 0.0f){
            // End immediately
            yield break;
        }

        CanvasCoord.SendMessage("ShowGray");
        print("ShowGrayScreen(): Enabled grayscreen");

        if(showTime > 0.0f){
            CanvasCoord.SendMessage("ShowImage", objShowIndex);
            print(String.Format("ShowGrayScreen(): Enabled Image {0}", objShowIndex));

            yield return new WaitForSeconds(showTime);

            CanvasCoord.SendMessage("HideImage");
            print(String.Format("ShowGrayScreen(): Disabled Image {0}", objShowIndex));
        }

        if(greyScreenTime > 0.0f){
            CanvasCoord.SendMessage("ShowPlus");

            yield return new WaitForSeconds(greyScreenTime);

            CanvasCoord.SendMessage("HidePlus");
        }

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

        // Setup player
        if(s.playerSpawnIndex >= 0){
            // Player index of -1 or less implies we dont respawn player. Used for searchfind.
            curenv.BroadcastMessage("RemovePlayer");
            curenv.BroadcastMessage("SpawnPlayerAtIndex", s.playerSpawnIndex);
        }
        GameObject player = GameObject.FindWithTag("Player");
        player.SendMessage("EnableInput");

        // Setup trigger object
        curenv.BroadcastMessage("ActivateObjTriggerAtIndex",
                new ObjSpawner.TriggerInfo(s.objSpawnIndex, TriggerCallback, s.objShowIndex));
        if(s.showObjAlways)
            curenv.BroadcastMessage("ShowSelf");

        Vector3 objpos = GameObject.FindWithTag("GoalTrigger").transform.position;

        // Setup Landmark
        curenv.BroadcastMessage("ShowLandmark", s.landmarkSpawnIndex);

        // Setup logger, using environment info component
        EnvInfo envinfo = (EnvInfo)curenv.GetComponent<EnvInfo>();
        logger.StartTrial(envinfo.GetActiveTriggerObj().transform.position, player, envinfo.GetOrigin());

        //
        // Specific scene logic
        //

        float curtime = Time.time;
        yield return new WaitUntil(() => Input.GetKeyDown(globalConfig.actionKey) || Time.time - curtime >= s.envTime);

        // Freeze player controls
        player.SendMessage("DisableInput");

        // Check if player found target after the press

        // True if the player pressed actionKey on a target
        bool fplayeractivatetarget = !(Time.time - curtime >= s.envTime) &&
            (Vector3.Distance(objpos, player.transform.position) <= globalConfig.objTriggerRadius);

        print(String.Format("RunScene(): fplayerfoundtarget: '{0}'", fplayeractivatetarget));

        if(fplayeractivatetarget){
            print("You found the target without having to be shown it!");
            curenv.BroadcastMessage("ShowSelf");

            // Freeze player controls
            player.SendMessage("DisableInput");

            // Turn player towards object
            foreach(object o in E.YieldFrom((player.GetComponent<PlayerAction>() as PlayerAction).PlayerLookTowards()))
                yield return o;


            foreach(object o in E.YieldFrom(OnFindTarget(player)))
                yield return o;

            curenv.BroadcastMessage("HideSelf");
        }else{
            print("You didn't find the target in time. Let me show it to you...");

            // Turn the target on! Show the billboarded sprite!
            curenv.BroadcastMessage("ShowSelf");


            // Turn player towards object
            foreach(object o in E.YieldFrom((player.GetComponent<PlayerAction>() as PlayerAction).PlayerLookTowards()))
                yield return o;

            // Reset target
            ResetCallbacks();

            // Unfreeze controls
            player.SendMessage("EnableInput");

            print("...now go find the target!");

            yield return new WaitUntil(() => fplayerfoundtarget);

            // Freeze player controls
            player.SendMessage("DisableInput");

            foreach(object o in E.YieldFrom(OnFindTarget(player)))
                yield return o;
            curenv.BroadcastMessage("HideSelf");
        }

        logger.EndTrial();

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

        // Get player reference
        GameObject player = GameObject.FindWithTag("Player");

        // Setup logger, using environment info component
        EnvInfo envinfo = (EnvInfo)curenv.GetComponent<EnvInfo>();
        logger.StartTrial(Vector3.zero, player, envinfo.GetOrigin()); // No destination in explore mode

        yield return new WaitForSeconds(s.envTime);

        logger.EndTrial();

        curenv.BroadcastMessage("RemovePlayer");

        print("ExploreScene(): Done");
    }

    // In environment, obj in the world, go get it, repeat
    public IEnumerator RunScene(Scene s, Logger logger){
        // Maybe refactor to class with inheritance?
        switch(s.mode){
            case "normal":
                foreach(object o in E.YieldFrom(RunNormalScene(s, logger)))
                    yield return o;
                break;
            case "explore":
                foreach(object o in E.YieldFrom(RunExploreScene(s, logger)))
                    yield return o;
                break;
            default:
                System.Diagnostics.Debug.Assert(false, String.Format("scene mode is invalid: '{0}'", s.mode));
                return false;
        }
    }

    IEnumerator IntroGreyScreen(float plusTime){
        CanvasCoord.SendMessage("ShowGray");
        print("IntroGreyScreen(): Enabled grayscreen");

        CanvasCoord.SendMessage("ShowPlus");

        print("IntroGreyScreen(): Waiting for '5' key press...");
        yield return new WaitUntil(() => Input.GetKeyDown("5"));
        print("IntroGreyScreen(): ...Got it!");

        yield return new WaitForSeconds(plusTime);
        CanvasCoord.SendMessage("HidePlus");

        CanvasCoord.SendMessage("HideGray");
        print("IntroGreyScreen(): Disabled grayscreen");
    }

    public IEnumerator RunAllScenes(Tuple<IEnumerable<Scene>, Logger> tup){
        // Show the intro screen first
        foreach(object o in E.YieldFrom(IntroGreyScreen(IntroGreyScreenTime)))
            yield return o;

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
