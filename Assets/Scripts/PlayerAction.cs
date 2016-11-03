using UnityEngine;
using System.Collections;

public class PlayerAction : MonoBehaviour {

    // Did player trigger Action this frame?
    // Set in Update()
    bool fDidAction = false;

    public bool IsActive {get{return fDidAction;}}

	// Use this for initialization
	void Start () { }

	// Update is called once per frame
	void Update () {
        fDidAction = Input.GetKeyDown("space");
	}
}
