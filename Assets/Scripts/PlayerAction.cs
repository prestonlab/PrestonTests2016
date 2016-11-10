using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityStandardAssets.Characters.FirstPerson;

public class PlayerAction : MonoBehaviour {

    // Did player trigger Action this frame?
    bool fDidAction = false;

    public bool IsActive {get{return fDidAction;}}

    private Transform child;

    void Start(){
        child = transform.GetChild(0);
    }

	void Update () {
        fDidAction = Input.GetKeyDown("space");
	}

    // Coroutines
    readonly float lookslerptime = 2.0f;
    public IEnumerator PlayerLookTowards(){
        Vector3 goalpos = GameObject.FindWithTag("GoalTrigger").transform.position - transform.position;
        Quaternion goalrot = Quaternion.LookRotation(goalpos);
        Quaternion goalrotflat = Quaternion.Euler(0, goalrot.eulerAngles.y, 0);

        Quaternion initrot = transform.rotation;
        float curtime = Time.time;
        while(Time.time - curtime < lookslerptime){
            float prop = (Time.time - curtime) / lookslerptime;
            transform.rotation = Quaternion.Slerp(initrot, goalrotflat, prop);
            child.rotation = Quaternion.Slerp(initrot, goalrotflat, prop);
            yield return null;
        }
    }

    // Messages

    public void DisableInput(){
        GameObject go = transform.gameObject;
        (go.GetComponent<FirstPersonController>() as FirstPersonController).enabled = false;
    }

    public void EnableInput(){
        GameObject go = transform.gameObject;
        (go.GetComponent<FirstPersonController>() as FirstPersonController).enabled = true;
    }

    public void FreezePlayer(){
        GameObject go = transform.gameObject;
        (go.GetComponent<CharacterController>() as CharacterController).enabled = false;
    }

    public void UnFreezePlayer(){
        GameObject go = transform.gameObject;
        (go.GetComponent<CharacterController>() as CharacterController).enabled = true;;
    }
}
