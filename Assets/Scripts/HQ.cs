using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HQ : MonoBehaviour
{
    // Start is called before the first frame update
     public RawImage tip;
    public TextMeshProUGUI tipText;
    public Canvas tipCanvas;
    private void OnMouseUpAsButton() {
        SceneManager.LoadScene("HQ");
    }
    private void OnMouseEnter() {
        tip.gameObject.SetActive(true);
        tipText.SetText("Here you can view all currently available information.");
        tipText.color=Color.cyan;

    }
   private void OnMouseOver() {
        Vector3 mousePosition=Input.mousePosition;
        float x= mousePosition.x*tipCanvas.GetComponent<RectTransform>().localScale.x;
        float xOffset=tip.GetComponent<RectTransform>().rect.width/1.95f;
        float y=mousePosition.y*tipCanvas.GetComponent<RectTransform>().localScale.y;
        float yOffset=tip.GetComponent<RectTransform>().rect.height/1.95f;
        float z=tipCanvas.transform.position.z-23;
        float zOffset=-23; // Just to make sure it's visible at front
        
        if(Screen.width<x+xOffset) xOffset=-xOffset;
        if(Screen.height<y+yOffset) yOffset=-yOffset;

        tip.transform.position=new Vector3(x+xOffset,y+yOffset,z+zOffset);
    }
    private void OnMouseExit() {
        tip.gameObject.SetActive(false);
        
    }
}
