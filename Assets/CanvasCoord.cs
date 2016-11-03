using UnityEngine;
using System.Collections;

public class CanvasCoord : MonoBehaviour {
    public GameObject grayScreenGo = null;

    public void GrayOn(){
        grayScreenGo.SetActive(true);
    }

    public void GrayOff(){
        grayScreenGo.SetActive(false);
    }

}
