using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Technology{
    int id;
    int unlocked;
    string name;
    string desc;
    List<string> effectDesc=new List<string>();
    List<string> type=new List<string>();
    List<string> effect=new List<string>();
    List<int> value=new List<int>();
    int baseResearchTime;
    Canvas parent;
    TMP_FontAsset font;
    RawImage tip;
    TextMeshProUGUI tipText;
    GameObject button;
    Image image;
    float buttonPositionX;
    float buttonPositionY;
    bool completed=false;
    public Technology(string data,Canvas canv,TMP_FontAsset font,RawImage tipImage,TextMeshProUGUI text){
        string [] splitData=data.Split(';');
        id=int.Parse(splitData[0]);
        unlocked=int.Parse(splitData[1]);
        name=splitData[2];
        desc=splitData[3];
        int current=5;
        for(int i=0;i<int.Parse(splitData[4]);i++){
            effectDesc.Add(splitData[current]);
            current++;
            type.Add(splitData[current]);
            current++;
            effect.Add(splitData[current]);
            current++;
            value.Add(int.Parse(splitData[current]));
            current++;
        }
        baseResearchTime=int.Parse(splitData[current]);
        parent=canv;
        this.font=font;
        tip=tipImage;
        tipText=text;
    }
    public void generateButton(float posX,float posY,int sizeX,int sizeY){
        buttonPositionX=posX;
        buttonPositionY=posY;
        button = new GameObject("Technology "+id);

        Button buttonComponent = button.AddComponent<Button>();

        RectTransform rectTransform = button.AddComponent<RectTransform>();

        rectTransform.SetParent(parent.transform); // Attach the button to the current GameObject
        rectTransform.localPosition = new Vector3(posX, posY, 0); // Set position
        rectTransform.sizeDelta = new Vector2(sizeX, sizeY); // Set size
        
        GameObject textObject = new GameObject("Text");
        TextMeshProUGUI text=textObject.AddComponent<TextMeshProUGUI>();
        textObject.transform.SetParent(buttonComponent.transform);
        image = button.AddComponent<Image>();
        if(unlocked==1){
            image.color=Color.black;
            text.text=name;
        }
        else {
            image.color=Color.red;
            text.text="???";
        }
       
        
        text.fontSize=20;
        text.font=this.font;
        text.alignment=TextAlignmentOptions.Center;
        textObject.GetComponent<RectTransform>().localPosition=new Vector3(0,0,0);

        
        // Add a listener to the button to handle clicks
        buttonComponent.onClick.AddListener(OnButtonClick);
        EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { OnMouseEnter(); });


            eventTrigger.triggers.Add(entry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) => { OnMouseExit(); });

            eventTrigger.triggers.Add(exitEntry);
    }
    void OnButtonClick()
    {
       
        if(!completed&&unlocked == 1){
        GlobalVariables.Instance.changeTechnology=true;
        GlobalVariables.Instance.currentTechnology=id;
        CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Down);
        }
    }
  private void OnMouseEnter() {
       
        tip.gameObject.SetActive(true);
        if(unlocked == 1){
            CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Clickable);
            string effects="";
            foreach(string eff in effectDesc){
                effects+="->"+eff+"\n";
            }
            tipText.SetText("<color=white>"+desc+"</color>\n"+"<color=yellow>Research time: "+baseResearchTime+" months</color>"+"\n<color=green>"+effects+"</color>"); // UPDATE THIS LATER
        }
        else {
            tipText.SetText("Unknown technology.");
            tipText.color=Color.red;
        }
       
        tip.GetComponent<RectTransform>().localPosition=new Vector3(buttonPositionX+50,buttonPositionY+200,0);
    }
    private void OnMouseExit() {
        CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Default);
        tip.gameObject.SetActive(false);
        
    }
    public void resetToBase(){
        if(image!=null){
            if(!completed&&unlocked==1)image.color=Color.black;
            if(unlocked==0)image.color=Color.red;
        }
    }
    public void setToActive(){
        image.color=new Color(255/255f, 153/255f, 0/255f);
    }
    public void changeCompleteState(){
        completed=true;
    }
}
