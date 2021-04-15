using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class CanvasCoord : MonoBehaviour {
    void Start(){
        imageComp = imageGo.GetComponent<Image>();
    }

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
    public Image imageComp = null;
    public void ShowImage(int index){
        Debug.Log(String.Format("ShowImage called w/ index {0}", index));

        // Load and build Sprite from a texture in Resources folder
        Sprite sprite = Resources.Load<Sprite>(index.ToString());

        // Activate GO and assign sprite
        imageGo.SetActive(true);
		imageComp.sprite = sprite;
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
