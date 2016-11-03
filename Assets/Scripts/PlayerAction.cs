using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityStandardAssets.Characters.FirstPerson;

public class PlayerAction : MonoBehaviour {

    // Did player trigger Action this frame?
    bool fDidAction = false;

    public bool IsActive {get{return fDidAction;}}

	void Update () {
        fDidAction = Input.GetKeyDown("space");
	}

    // Messages

    public void FreezeControls(){
        GameObject go = transform.gameObject;
        (go.GetComponent<CharacterController>() as CharacterController).enabled = false;
        (go.GetComponent<FirstPersonController>() as FirstPersonController).enabled = false;
    }

}
