using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System;
using System.IO;
using System.Text;

public class Logic : MonoBehaviour {

    // Coroutines

    public GameObject Environments = null;
    public GameObject CanvasCoord = null;

    private bool fplayerfoundtarget = false;

    // Callback given to Triggers / Objects to let our guy know we're done
    private void TriggerCallback(){
        fplayerfoundtarget = true;
    }

    private object OnFindTarget(){
        print("GOOD JOB U FOUND TARGET");
        fplayerfoundtarget = false;
        // Freeze controls
        // TODO
        return new WaitForSeconds(2); // XXX Post wait time, not in Scene. Should it be? Or should it be 0 always?
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

    public IEnumerator RunNormalScene(Scene s){
        print("RunScene(): Starting");

        foreach(object o in E.YieldFrom(ShowGrayScreen(s.objShowIndex, s.showTime, s.greyScreenTime)))
            yield return o;

        // Generic GrayScreen Logic
        GameObject curenv = GetEnvGO(Environments, s.envIndex);

        curenv.BroadcastMessage("SpawnPlayerAtIndex", s.playerSpawnIndex);
        curenv.BroadcastMessage("ActivateObjTriggerAtIndex", new ObjSpawner.TriggerInfo(s.objSpawnIndex, TriggerCallback));
        curenv.BroadcastMessage("ShowLandmark", s.landmarkSpawnIndex);

        // Specific scene logic

        float curtime = Time.time;
        yield return new WaitUntil(() => fplayerfoundtarget || Time.time - curtime >= s.envTime);

        print(String.Format("RunScene(): fplayerfoundtarget: '{0}'", fplayerfoundtarget));

        if(fplayerfoundtarget){
            yield return OnFindTarget();
        }else{
            print("Go find the target!!!!");

            // Freeze player controls
            GameObject player = GameObject.FindWithTag("Player");
            player.SendMessage("DisableInput");

            // Turn player towards object
            foreach(object o in E.YieldFrom((player.GetComponent<PlayerAction>() as PlayerAction).PlayerLookTowards()))
                yield return o;

            player.SendMessage("EnableInput");

            // Wait Until they find the object
            yield return new WaitUntil(() => fplayerfoundtarget);

            yield return OnFindTarget();
        }

        curenv.BroadcastMessage("RemovePlayer");
        curenv.BroadcastMessage("DeactiveateTriggers");
        curenv.BroadcastMessage("HideLandmark");

        print("RunScene(): Done");
    }

    // Placed in environment where they can explore for 2-3 mins, no objects.
    public IEnumerator RunExploreScene(Scene s){
        print("RunExploreScene(): Starting");

        // Doesn't have GrayScreen

        GameObject curenv = GetEnvGO(Environments, s.envIndex);
        curenv.BroadcastMessage("SpawnPlayerAtIndex", s.playerSpawnIndex);
        yield return new WaitForSeconds(s.envTime);
        curenv.BroadcastMessage("RemovePlayer");

        print("RunExploreScene(): Done");
    }

    public IEnumerator RunSearchFindScene(Scene s){
        System.Diagnostics.Debug.Assert(false, "Not Implemented");
        return null;
    }

    public IEnumerator RunScene(Scene s){
        // TODO Maybe refactor to class with inheritance?
        switch(s.mode){
            case "normal":
                foreach(object o in E.YieldFrom(RunNormalScene(s)))
                    yield return o;
                break;
            case "explore":
                foreach(object o in E.YieldFrom(RunExploreScene(s)))
                    yield return o;
                break;
            case "searchfind":
                foreach(object o in E.YieldFrom(RunSearchFindScene(s)))
                    yield return o;
                break;
            default:
                System.Diagnostics.Debug.Assert(false, String.Format("mode is invalid: '{0}'", s.mode));
                return false;
        }
    }

    public IEnumerator RunAllScenes(IEnumerable<Scene> scenes){
        int counter = 0;
        foreach(Scene s in scenes){
            Debug.Log(String.Format("RunAllScenes(): Running scene number: {0}", counter));

            foreach(object o in E.YieldFrom(RunScene(s)))
                yield return o;

            Debug.Log(String.Format("RunAllScenes(): Done running scene number: {0}", counter));
            counter += 1;

            yield return new WaitForSeconds(2.0f);
        }
        Debug.Log("RunAllScenes(): Done running all scenes!");
    }

    // Init

	void Start () {
        // Read Json file to configure stuff
        //

        string jsonpath = @"config.json";
        if (!File.Exists(jsonpath)){
            // Just end it all
            Debug.Assert(false);
            Debug.LogError(String.Format("Couldn't load config file at: {0}", jsonpath));
            Application.Quit();
        }

        string configjson = File.ReadAllText(jsonpath);
        Config config = Config.CreateFromJSON(configjson);

        StartCoroutine("RunAllScenes", config.scenes);
	}
}
