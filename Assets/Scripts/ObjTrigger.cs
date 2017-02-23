using System;
using UnityEngine;

// A trigger object, with a trigger component attached
public class ObjTrigger : MonoBehaviour {

    private Action curcallback = null;
    private Billboard billboard = null;
    private Renderer rend = null;

    void Awake(){
        billboard = GetComponent<Billboard>();
        billboard.enabled = true;

        rend = GetComponent<Renderer>();
        rend.enabled = false;
    }

    // Messages for showing and hiding the sprite
    //
    public void ShowSelf(){
        rend.enabled = true;
    }

    public void HideSelf(){
        rend.enabled = false;
    }

    // Messages for dealing with information

    public void SetSprite(int spriteIndex){
        rend.material.mainTexture = (Texture)Resources.Load(spriteIndex.ToString());
    }

    public void SetInfo(ObjSpawner.TriggerInfo ti){
        curcallback = ti.callback;
        if(ti.spriteIndex.HasValue){
            SendMessage("SetSprite", ti.spriteIndex);
        }
    }

    public void ClearInfo(){
        curcallback = null;
        rend.enabled = false;
        transform.gameObject.SetActive(false);
    }

    public void OnTriggerStay(Collider other) {
        if(other.gameObject.GetComponent<PlayerAction>() != null)
            curcallback();
    }
}
