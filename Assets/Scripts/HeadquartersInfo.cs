using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Information 
{
    string name;
    string desc;
    int unlocked;
    int id;
    RawImage parent;
    TMP_FontAsset font;
    TextMeshProUGUI details;
    GameObject button;
    Image image;
    public Information(string data,RawImage rawImage,TMP_FontAsset font,TextMeshProUGUI description){
        string [] split = data.Split(";");
        id=int.Parse(split[0]);
        name=split[1];
        desc=split[2];
        unlocked=int.Parse(split[3]);
        parent=rawImage;
        this.font=font;
        details=description;
    }
    public bool generateButton(float posX,float posY,int sizeX,int sizeY){
        if(unlocked!=1) return false;
        button = new GameObject("Info");

        Button buttonComponent = button.AddComponent<Button>();

        RectTransform rectTransform = button.AddComponent<RectTransform>();

        rectTransform.SetParent(parent.transform); // Attach the button to the current GameObject
        rectTransform.localPosition = new Vector3(posX, posY, 0); // Set position
        rectTransform.sizeDelta = new Vector2(sizeX, sizeY); // Set size
        
        GameObject textObject = new GameObject("Text");
        TextMeshProUGUI text=textObject.AddComponent<TextMeshProUGUI>();
        textObject.transform.SetParent(buttonComponent.transform);
        image = button.AddComponent<Image>();
        image.color=new Color(0.4986246f,0.2018512f,0.5283019f);
        text.text=name;
       
       
        
        text.fontSize=18;
        text.font=this.font;
        text.alignment=TextAlignmentOptions.Center;
        textObject.GetComponent<RectTransform>().localPosition=new Vector3(0,0,0);
        textObject.GetComponent<RectTransform>().sizeDelta=new Vector2(sizeX,sizeY);

        
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

            EventTrigger.Entry downEntry = new EventTrigger.Entry();
            downEntry.eventID=EventTriggerType.PointerDown;
            downEntry.callback.AddListener((data) => { OnMouseDown(); });

            eventTrigger.triggers.Add(downEntry);

            EventTrigger.Entry upEntry = new EventTrigger.Entry();
            upEntry.eventID=EventTriggerType.PointerUp;
            upEntry.callback.AddListener((data) => { OnMouseUp(); });

            eventTrigger.triggers.Add(upEntry);
    return true;
    }
    void OnMouseUp(){
            if(unlocked == 1)CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Clickable);
    }
    void OnMouseDown(){
        if(unlocked == 1)CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Down);
    }
    void OnButtonClick()
    {
       
        if(unlocked == 1){
        GlobalVariables.Instance.changeInfoHighlight=true;
        details.text=desc;
        GlobalVariables.Instance.currentInfo=id;
        }
    }
  private void OnMouseEnter() {
            CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Clickable);
    }
    private void OnMouseExit() {
        CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Default);
        
    }
    public void resetToBase(){
        if(button!=null){
            image.color=new Color(0.4986246f,0.2018512f,0.5283019f);
        }
    }
    public void setToActive(){
        image.color=new Color(255/255f, 153/255f, 0/255f);
    }
}
