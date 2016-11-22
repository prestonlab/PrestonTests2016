using System;
using UnityEngine;
using System.Collections;

public class ObjSpawner : MonoBehaviour {
    // Controller for all objects, by which I mean triggers
    // Those objects can also have sprites that always face the camera on them

    // Message passing class
    public class TriggerInfo {
        public TriggerInfo(int i, Action cb, int? si = null){
            index = i;
            callback = cb;
            spriteIndex = si;
        }
        public readonly int index; // Index of our children to activate
        public readonly Action callback; // Callback object will call when its found
        public readonly int? spriteIndex; // Show the billboard if not null
        public override String ToString(){
            return String.Format("index: {0}, callback: {1}, spriteIndex: {2}", index, callback, spriteIndex);
        }
    }

    private GameObject lastactivated = null;

    public void ActivateObjTriggerAtIndex(TriggerInfo ti){
        GameObject go = transform.GetChild(ti.index).gameObject;
        lastactivated = go;
        go.SetActive(true);
        go.SendMessage("SetInfo", ti);
    }

    public void DeactiveateTriggers(){
        lastactivated.SendMessage("ClearInfo");
        lastactivated = null;
    }
}

