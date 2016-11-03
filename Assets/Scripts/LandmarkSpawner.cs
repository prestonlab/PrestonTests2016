using UnityEngine;
using System.Collections;

public class LandmarkSpawner : MonoBehaviour {
    public void Start(){
        HideLandmark();
    }

    public void ShowLandmark(int index){
        transform.GetChild(index).gameObject.SetActive(true);
    }

    public void HideLandmark(){
        foreach(Transform child in transform)
            child.gameObject.SetActive(false);
    }
}
