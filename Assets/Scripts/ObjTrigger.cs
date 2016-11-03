using System;
using UnityEngine;
using System.Collections;

public class ObjTrigger : MonoBehaviour {

    private Action callback = null;

    public void SetInfo(Action cb){
        callback = cb;
    }

    public void ClearInfo(){
        callback = null;
        transform.gameObject.SetActive(false);
    }

    public void OnTriggerStay(Collider other) {
        if(other.gameObject.GetComponent<PlayerAction>().IsActive){
            callback();
        }
    }
}
