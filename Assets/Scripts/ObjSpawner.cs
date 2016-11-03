using System;
using UnityEngine;
using System.Collections;

public class ObjSpawner : MonoBehaviour {
    // Controller for all objects, by which I mean triggers

    // Message passing class
    public class TriggerInfo {
        public TriggerInfo(int i, Action cb){
            index = i;
            callback = cb;
        }
        public readonly int index;
        public readonly Action callback;
    }

    private GameObject lastactivated = null;

    public void ActivateObjTriggerAtIndex(TriggerInfo ti){
        GameObject go = transform.GetChild(ti.index).gameObject;
        lastactivated = go;
        go.SetActive(true);
        go.SendMessage("SetInfo", ti.callback);
    }

    public void DeactiveateTriggers(){
        lastactivated.SendMessage("ClearInfo");
        lastactivated = null;
    }
}

