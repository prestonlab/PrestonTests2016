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
        Quaternion initrot = transform.rotation;

        Quaternion goalrot = Quaternion.LookRotation(
                GameObject.FindWithTag("GoalTrigger").transform.position - transform.position);

        float curtime = Time.time;
        while(Time.time - curtime < lookslerptime){
            float prop = (Time.time - curtime) / lookslerptime;
            transform.rotation = Quaternion.Slerp(initrot, goalrot, prop);
            child.rotation = Quaternion.Slerp(initrot, goalrot, prop);
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
