using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpawnLogic : MonoBehaviour {
    public Transform playergo;
    private List<Transform> spawnLocs = null;

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
        Instantiate(playergo, spawnLocs[index].position, Quaternion.identity);
    }
}
