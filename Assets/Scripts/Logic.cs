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

    public IEnumerator RunScene(Scene s){
        // Show the gray screen
        print("RunScene(): Starting");

        CanvasCoord.SendMessage("ShowGray");
        print("RunScene(): Enabled grayscreen");

        CanvasCoord.SendMessage("ShowImage", s.objShowIndex);
        print(String.Format("RunScene(): Enabled Image {0}", s.objShowIndex));

        yield return new WaitForSeconds(s.showTime);

        CanvasCoord.SendMessage("HideImage");
        print(String.Format("RunScene(): Disabled Image {0}", s.objShowIndex));

        CanvasCoord.SendMessage("ShowPlus");

        yield return new WaitForSeconds(s.greyScreenTime);

        CanvasCoord.SendMessage("HidePlus");
        CanvasCoord.SendMessage("HideGray");
        print("RunScene(): Disabled grayscreen");

        GameObject curenv = Environments.transform.GetChild(s.envIndex).gameObject;

        curenv.BroadcastMessage("SpawnPlayerAtIndex", s.playerSpawnIndex);
        curenv.BroadcastMessage("ActivateObjTriggerAtIndex", new ObjSpawner.TriggerInfo(s.objSpawnIndex, TriggerCallback));
        curenv.BroadcastMessage("ShowLandmark", s.landmarkSpawnIndex);

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
