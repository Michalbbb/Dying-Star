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

   
    // Start is called before the first frame update
    public string file="Assets/TextFiles/Technology.txt";
 
    string fileText;
    void Start()
    {
        if (!GlobalVariables.Instance.isTechnologyGenerated) generateTechnology();
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

    void generateTechnology(){
        if (File.Exists(file))
        {
            // Read all text from the file
            fileText = File.ReadAllText(file);
            string [] data=fileText.Split("\n");
            for(int i=0;i<data.Length;i++)
              GlobalVariables.Instance.technologies.Add(new Technology(data[i]));
            GlobalVariables.Instance.isTechnologyGenerated = true;
        }
    }

    void setButtons()
    {
        if (!GlobalVariables.Instance.isTechnologyGenerated) return;
        float startingY = (canvas.GetComponent<RectTransform>().sizeDelta.y / 2) - (text.GetComponent<RectTransform>().sizeDelta.y * 3);
        float startingX = -(canvas.GetComponent<RectTransform>().sizeDelta.x / 2);
        const int BUTTON_X = 200;
        const int BUTTON_Y = 100;
        const int SPACE_BETWEEN = 150;
        startingX += BUTTON_X;
        float x = startingX;
        float y = startingY;

        foreach (Technology tech in GlobalVariables.Instance.technologies)
        {
            tech.generateButton(canvas,font,tip,tipText,x, y, BUTTON_X, BUTTON_Y);
            x += BUTTON_X + SPACE_BETWEEN;
            if (x >= (canvas.GetComponent<RectTransform>().sizeDelta.x / 2 - (BUTTON_X)))
            {
                x = startingX;
                y -= (BUTTON_Y + SPACE_BETWEEN);
            }

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
