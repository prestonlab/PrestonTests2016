using System;
using UnityEngine;

// A trigger object, with a trigger component attached
public class ObjTrigger : MonoBehaviour {

    private Action curcallback = null;

    public void SetInfo(ObjSpawner.TriggerInfo ti){
        curcallback = ti.callback;
        if(ti.spriteIndex.HasValue){
            BroadcastMessage("SetSprite", ti.spriteIndex);
        }
    }

    public void OnTriggerStay(Collider other) {
        if(other.gameObject.GetComponent<PlayerAction>() != null)
            curcallback();
    }

    public void ClearInfo(){
        curcallback = null;
        transform.gameObject.SetActive(false);
    }
}
