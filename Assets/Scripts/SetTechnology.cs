using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetTechnology : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TMP_FontAsset font;
    [SerializeField] RawImage tip;
    [SerializeField] TextMeshProUGUI tipText;

   
    void Start()
    {
        
        setButtons();
         RectTransform[] children = canvas.GetComponentsInChildren<RectTransform>();
         int childrenCount=children.Length;
          children[3].SetSiblingIndex(childrenCount-1);
           children[4].SetSiblingIndex(childrenCount);
       

    }
    private void Update() {
        if(GlobalVariables.Instance.changeTechnology){
            GlobalVariables.Instance.changeTechnology=false;
            changeResearch();
        }
    }

    

    void setButtons()
    {
        if (!GlobalVariables.Instance.isTechnologyGenerated) return;
        float startingY = (canvas.GetComponent<RectTransform>().sizeDelta.y / 2) - (text.GetComponent<RectTransform>().sizeDelta.y * 3);
        float startingX = -canvas.GetComponent<RectTransform>().sizeDelta.x/2 ;
         int BUTTON_X = Screen.width/7;
         int BUTTON_Y = Screen.height/5;
         int SPACE_BETWEEN = Screen.height/15;
        startingX += BUTTON_X;
        float x = startingX;
        float y = startingY;
    
        foreach (Technology tech in GlobalVariables.Instance.technologies)
        {
            if (x >= (canvas.GetComponent<RectTransform>().sizeDelta.x/2))
            {
                x = startingX;
                y -= (BUTTON_Y + SPACE_BETWEEN);
            }
            tech.generateButton(canvas,font,tip,tipText,x, y, BUTTON_X, BUTTON_Y);
            x += BUTTON_X + SPACE_BETWEEN;
            

        }
        if (GlobalVariables.Instance.currentTechnology != -1)
        {
            GlobalVariables.Instance.technologies[GlobalVariables.Instance.currentTechnology].setToActive();
        }
    }
    public void changeResearch(){
            foreach(Technology tech in GlobalVariables.Instance.technologies){
                tech.resetToBase();
            }
            GlobalVariables.Instance.technologies[GlobalVariables.Instance.currentTechnology].setToActive();
    }
}
