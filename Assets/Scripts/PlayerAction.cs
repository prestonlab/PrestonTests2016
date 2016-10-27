using UnityEngine;
using System.Collections;

public class PlayerAction : MonoBehaviour {

    // Did player trigger Action this frame?
    // Set in Update()
    bool fDidAction = false;

	// Use this for initialization
	void Start () { }

	// Update is called once per frame
	void Update () {
        fDidAction = fDidAction || Input.GetKeyDown("space");
	}

    private void ActivateAction(){
        // TODO Log player pos, trigger something in the object
        Debug.Log(string.Format("Triggered lol at {0}", transform.position));
    }

    void FixedUpdate(){
        if(fDidAction){
            ActivateAction();
            fDidAction = false;
        }
    }
}
