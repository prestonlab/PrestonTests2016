using UnityEngine;
using System.Collections;
using System;

public class CanvasCoord : MonoBehaviour {
    // Gray
    public GameObject grayScreenGo = null;
    public void ShowGray(){
        grayScreenGo.SetActive(true);
    }

    public void HideGray(){
        grayScreenGo.SetActive(false);
    }

    // Image
    public GameObject imageGo = null;
    public void ShowImage(int index){
        Debug.Log(String.Format("ShowImage called w/ index {0}", index));
        imageGo.SetActive(true);
    }

    public void HideImage(){
        imageGo.SetActive(false);
    }

    // Plus
    public GameObject plusGo = null;
    public void ShowPlus(){
        plusGo.SetActive(true);
    }

    public void HidePlus(){
        plusGo.SetActive(false);
    }
}
