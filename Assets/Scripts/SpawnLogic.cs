using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnLogic : MonoBehaviour {
    public Transform playergo;
    private List<Transform> spawnLocs = null;

    private GameObject trackedplayer = null;

	void Start() {
        spawnLocs = new List<Transform>();
        foreach(Transform child in transform){
            child.LookAt(transform);
            spawnLocs.Add(child);
        }
	}

    //
    // Api
    //

    // Spawns the player at an index in [0-7] inclusive
    void SpawnPlayerAtIndex(int index){
        Debug.Assert(0 <= index && index <= spawnLocs.Count);
        Debug.Assert(trackedplayer == null);
        Transform loc = spawnLocs[index];
        trackedplayer = ((Transform)Instantiate(playergo, loc.position, loc.rotation)).gameObject;
		// Simple movement fix?
		trackedplayer.SetActive(true);
    }

    // Kills the player
    void RemovePlayer(){
        print("SpawnLogic.cs! Remove Player Called!...");
        print("SpawnLogic.cs!" + (trackedplayer == null ? "null" : trackedplayer.ToString()));
        if(trackedplayer != null){
            print("SpawnLogic.cs! Destroyed trackedplayer!!!");
            trackedplayer.SetActive(false);
            Destroy(trackedplayer);
            trackedplayer = null;
        }
    }
}
