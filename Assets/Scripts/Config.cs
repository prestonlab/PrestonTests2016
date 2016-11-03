using UnityEngine;
using System.Collections;
using System;

// https://forum.unity3d.com/threads/how-to-load-an-array-with-jsonutility.375735/

[Serializable]
public class Scene {
    // Index of the object we want to show to the player
    // An object is a 2d rendering of a nondescript, abstract geometric model.
    // It will be displayed on a grey screen
    public int objShowIndex;
    public float showTime; // How long the object is on screen, seconds
    public float greyScreenTime; // How long the grey screen is shown, seconds

    public int envIndex; // The environment the player is in
    public float envTime; // The environment the player is in
    public int objSpawnIndex; // Index of position object spawns in
    public int playerSpawnIndex; // Index of position player spawns in
    public int landmarkSpawnIndex; // Index of position landmark spawns in
}

// Config is a list of scenes and the player's settings
[Serializable]
public class Config {
    // TODO Player speed etc
    public Scene[] scenes;

    // Given the text of a JSON object, build the config class
    public static Config CreateFromJSON(string jsonString){
        return JsonUtility.FromJson<Config>(jsonString);
    }
}
