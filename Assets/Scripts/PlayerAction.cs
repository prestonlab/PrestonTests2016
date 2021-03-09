using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityStandardAssets.Characters.FirstPerson;

public class PlayerAction : MonoBehaviour {

    private Config globalConfig = null; // A reference to the global config object, which holds keybinding configurations and other things

    private Transform child;

    private SimpleMovement fpscont; // Enable/Disable input
    private CharacterController charcont; // Freeze/Unfreeze player

    void Awake(){
        globalConfig = GameObject.Find("Logic").GetComponent<Logic>().globalConfig;
        child = transform.GetChild(0);
        GameObject go = transform.gameObject;
        fpscont = (SimpleMovement)go.GetComponent<SimpleMovement>();
        charcont = (CharacterController)go.GetComponent<CharacterController>();
    }


    // Coroutines
    public IEnumerator ViewSlerp(Quaternion start, Quaternion end, float time, Transform[] transforms) {
        // Slerps between current look direction and end over time, set rotations of the transforms in transforms
        Quaternion initrot = start;
        float inittime = Time.time;
        while(Time.time - inittime < time){
            float prop = (Time.time - inittime) / time;
            Quaternion newrot = Quaternion.Slerp(initrot, end, prop);
            foreach(Transform t in transforms)
                t.rotation = newrot;
            yield return null;
        }
    }

    public IEnumerator PlayerLookTowards(){
        Vector3 goalpos = GameObject.FindWithTag("GoalTrigger").transform.position - transform.position;
        Quaternion goalrot = Quaternion.LookRotation(goalpos);
        Quaternion goalrotflat = Quaternion.Euler(0, goalrot.eulerAngles.y, 0);
        yield return StartCoroutine(ViewSlerp(transform.rotation, goalrotflat, globalConfig.lookSlerpTime, new Transform[]{transform, child}));
    }

    public IEnumerator Pan(float time, float angle){
        Quaternion goalrot = Quaternion.Euler(angle, transform.rotation.eulerAngles.y, 0);
        yield return StartCoroutine(ViewSlerp(child.rotation, goalrot, time, new Transform[]{child}));
    }

    // Messages

    public void DisableInput(){
        fpscont.enabled = false;
    }

    public void EnableInput(){
        fpscont.enabled = true;
    }

    public void FreezePlayer(){
        charcont.enabled = false;
    }

    public void UnFreezePlayer(){
        charcont.enabled = true;
    }
}
