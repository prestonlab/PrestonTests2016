using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {
    void Update() {
        Vector3 goalpos = Camera.main.transform.position;
        goalpos.y = transform.position.y;
        transform.LookAt(goalpos, Vector3.up);
    }
}
