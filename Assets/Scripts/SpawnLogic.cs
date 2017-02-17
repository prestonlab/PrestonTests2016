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
        print("SPAWNED THE FUCKIN PALYER");
        Transform loc = spawnLocs[index];
        trackedplayer = ((Transform)Instantiate(playergo, loc.position, loc.rotation)).gameObject;
    }

    // Kills the player
    void RemovePlayer(){
        if(trackedplayer)
            Destroy(trackedplayer);
    }
}
