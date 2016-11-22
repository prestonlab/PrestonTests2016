using UnityEngine;
using System.Collections;

public class LandmarkSpawner : MonoBehaviour {
    public void Start(){
        HideLandmark();
    }

    public void ShowLandmark(int index){
        print(string.Format("ShowLandmark({0})", index));
        transform.GetChild(index).gameObject.SetActive(true);
    }

    public void HideLandmark(){
        foreach(Transform child in transform)
            child.gameObject.SetActive(false);
    }
}
