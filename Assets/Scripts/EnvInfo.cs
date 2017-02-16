using UnityEngine;
using System.Collections;

// Holds queryable information about each environment
public class EnvInfo : MonoBehaviour {

    // Traverses GO hierarchy to get active trigger object position
    public GameObject GetActiveTriggerObj(){
        return GameObject.FindWithTag("GoalTrigger");
    }

    // Returns origin of this environment, we're using the center as origin
    public Vector3 GetOrigin(){
        return transform.Find("Center").position;
    }
}
