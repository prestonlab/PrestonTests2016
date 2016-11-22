using System;
using UnityEngine;

public class ObjTrigger : MonoBehaviour {

    private Action curcallback = null;
    private Billboard billboard = null;
    private Renderer rend = null;

    void Awake(){
        billboard = GetComponent<Billboard>();
        billboard.enabled = true;

        rend = GetComponent<Renderer>();
    }

    public void SetSprite(int spriteIndex){
        rend.material.mainTexture = (Texture)Resources.Load(spriteIndex.ToString());
    }

    public void SetInfo(ObjSpawner.TriggerInfo ti){
        curcallback = ti.callback;
        if(ti.spriteIndex != null){
            SendMessage("SetSprite", ti.spriteIndex);
            billboard.enabled = true;
        }
    }

    public void ClearInfo(){
        curcallback = null;
        billboard.enabled = false;
        transform.gameObject.SetActive(false);
    }

    public void OnTriggerStay(Collider other) {
        if(other.gameObject.GetComponent<PlayerAction>().IsActive)
            curcallback();
    }
}
