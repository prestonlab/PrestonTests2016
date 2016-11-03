using UnityEngine;
using System.Collections;

using System;
using System.IO;
using System.Text;

public class Logic : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // Read Json file to configure stuff
        string jsonpath = @"config.json";
        if (!File.Exists(jsonpath)){
            // End it all
            Debug.Assert(false);
            Debug.LogError(String.Format("Couldn't load config file at: {0}", jsonpath));
            Application.Quit();
        }

        string configjson = File.ReadAllText(jsonpath);
        Debug.Log(configjson);

        //Config config = Config.CreateFromJSON(configjson);
	}

	// Update is called once per frame
	void Update () {

	}
}
