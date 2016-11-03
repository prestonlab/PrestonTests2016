using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System;
using System.IO;
using System.Text;

public class Logic : MonoBehaviour {

    // Coroutines

    public GameObject CanvasCoord = null;

    public IEnumerator RunScene(Scene scene){
        // Show the gray screen
        Debug.Log("RunScene(): Starting");

        CanvasCoord.SendMessage("GrayOn");
        Debug.Log("RunScene(): Enabled grayscreen");

        yield return new WaitForSeconds(2.0f);
        CanvasCoord.SendMessage("GrayOff");
        Debug.Log("RunScene(): Disabled grayscreen");

        Debug.Log("RunScene(): Done");
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

        // Maybe create an object instead of using a coroutine? How do you express "wait forever until..." with a coroutine?
        // JK LOL Here it is: https://docs.unity3d.com/ScriptReference/WaitUntil.html
        StartCoroutine("RunAllScenes", config.scenes);
	}
}
