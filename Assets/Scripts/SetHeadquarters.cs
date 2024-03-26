using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetHeadquarters : MonoBehaviour
{
    [SerializeField] RawImage rawImage;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI details;
    [SerializeField] TMP_FontAsset font;


    // Start is called before the first frame update
    public string file="Assets/TextFiles/Informations.txt";
    List<Information> informations=new List<Information>();
    string fileText;
    void Start()
    {
       setButtons();
       

    }
    private void Update() {
        if(GlobalVariables.Instance.changeInfoHighlight){
            GlobalVariables.Instance.changeInfoHighlight=false;
            changeHighlight();
        }
    }

    void setButtons(){
        if (File.Exists(file))
        {
            // Read all text from the file
            fileText = File.ReadAllText(file);
            string [] data=fileText.Split("\n");
            const int BUTTON_X=165;
            const int BUTTON_Y=55;
            const int SPACE_BETWEEN=55;

            rawImage.GetComponent<RectTransform>().sizeDelta=new Vector2(rawImage.GetComponent<RectTransform>().sizeDelta.x,(BUTTON_Y+SPACE_BETWEEN)*data.Length-SPACE_BETWEEN); 

            for(int i=0;i<data.Length;i++){
            String [] checkIfCorrect=data[i].Split(';');
            if(checkIfCorrect.Length!=4){
                continue;
            }
            informations.Add(new Information(data[i],rawImage,font,details));
            }
            
            int x=5;
            float y=0;            
            foreach(Information info in informations){
                if(info.generateButton(x,y,BUTTON_X,BUTTON_Y))y-=(BUTTON_Y+SPACE_BETWEEN);
                
               
                
            }
            if(GlobalVariables.Instance.currentInfo!=0){
                informations[GlobalVariables.Instance.currentInfo-1].setToActive();
            }
        }
    }
    public void changeHighlight(){
            foreach(Information info in informations){
                info.resetToBase();
            }
            informations[GlobalVariables.Instance.currentInfo-1].setToActive();
    }
}
