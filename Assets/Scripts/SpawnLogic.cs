using UnityEngine;
using System.Collections;

public class SpawnLogic : MonoBehaviour {
	void Awake() {
        foreach(Transform child in transform){
            child.LookAt(transform);
        }
	}
}
