using UnityEngine;
using System.Collections;

public class ObjSprite : MonoBehaviour {

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

    public void ClearInfo(){
        rend.enabled = false;
    }
}
