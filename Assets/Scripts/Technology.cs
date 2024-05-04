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
    float researchPoints;
    TMP_FontAsset font;
    RawImage tip;
    TextMeshProUGUI tipText;
    GameObject button;
    Image image;
    float buttonPositionX;
    float buttonPositionY;
    public bool completed=false;
    public Technology(string data){
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
        baseResearchTime = int.Parse(splitData[current]);
        researchPoints = 0;
        
    }
    public void generateButton(Canvas parent, TMP_FontAsset font, RawImage tipImage, TextMeshProUGUI tipText, float posX,float posY,int sizeX,int sizeY){
        this.font = font;
        tip = tipImage;
        this.tipText = tipText;
        buttonPositionX =posX;
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
        
        if (completed)
        {
            image.color = Color.green;
            text.text = name+"(Completed)";
        }
        else if (unlocked == 1)
        {
            image.color = Color.black;
            text.text = name;
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
        GlobalVariables.Instance.currentTechnology=id-1;
        CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Down);
        }
    }
  private void OnMouseEnter() {
       
        tip.gameObject.SetActive(true);
        if (completed)
        {
            string effects = "";
            foreach (string eff in effectDesc)
            {
                effects += "->" + eff + "\n";
            }
            tipText.SetText("<color=green>" + effects + "</color>");
        }
        else if(unlocked == 1){
            CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Clickable);
            string effects="";
            foreach(string eff in effectDesc){
                effects+="->"+eff+"\n";
            }
            tipText.SetText("<color=white>"+desc+"</color>\n"+"<color=yellow>Research time: "+getResearchTime()+" months</color>"+"\n<color=red>"+effects+"</color>");
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
    public void applyEffectsAndComplete(){
        completed=true;

        for(int i = 0; i < type.Count; i++) 
        {
            if (type[i] == "researchRate")
            {
                float val =(1f+value[i]/100f);
                if (effect[i] == "more")
                    GlobalVariables.Instance.researchRate *= val;
                if (effect[i] == "less")
                    GlobalVariables.Instance.researchRate /= val;

            }
            else if (type[i] == "baseAttack")
            {
                float val = (value[i] / 100f);
                if (effect[i] == "increased")
                    GlobalVariables.Instance.increasedBaseAttackOfShips += val;
                if (effect[i] == "reduced")
                    GlobalVariables.Instance.increasedBaseAttackOfShips -= val;
            }
            else if(type[i] == "baseDefense")
            {
                float val = (value[i] / 100f);
                if (effect[i] == "increased")
                    GlobalVariables.Instance.increasedBaseDefenceOfShips += val;
                if (effect[i] == "reduced")
                    GlobalVariables.Instance.increasedBaseDefenceOfShips -= val;
            }
            else if(type[i] == "baseSpeed")
            {
                float val = (value[i] / 100f);
                if (effect[i] == "increased")
                    GlobalVariables.Instance.increasedBaseSpeedOfShips += val;
                if (effect[i] == "reduced")
                    GlobalVariables.Instance.increasedBaseSpeedOfShips -= val;
            }
            else if(type[i] == "attack")
            {
                float val = (value[i] / 100f);
                if (effect[i] == "increased")
                    GlobalVariables.Instance.increasedTotalAttackOfShips += val;
                if (effect[i] == "reduced")
                    GlobalVariables.Instance.increasedTotalAttackOfShips -= val;
            }
            else if(type[i] == "defense")
            {
                float val = (value[i] / 100f);
                if (effect[i] == "increased")
                    GlobalVariables.Instance.increasedTotalDefenceOfShips += val;
                if (effect[i] == "reduced")
                    GlobalVariables.Instance.increasedTotalDefenceOfShips -= val;
            }
            else if(type[i] == "speed")
            {
                float val = (value[i] / 100f);
                if (effect[i] == "increased")
                    GlobalVariables.Instance.increasedTotalSpeedOfShips += val;
                if (effect[i] == "reduced")
                    GlobalVariables.Instance.increasedTotalSpeedOfShips -= val;
            }
            else if(type[i] == "expGain")
            {
                float val = (value[i] / 100f);
                if (effect[i] == "increased")
                    GlobalVariables.Instance.pilotExpMultiplier += val;
                if (effect[i] == "reduced")
                    GlobalVariables.Instance.pilotExpMultiplier -= val;
            }
            else if(type[i] == "travelSpeed")
            {
                float val = (value[i] / 100f);
                if (effect[i] == "increased")
                    GlobalVariables.Instance.hyperspaceTravelSpeedModifier += val;
                if (effect[i] == "reduced")
                    GlobalVariables.Instance.hyperspaceTravelSpeedModifier -= val;
            }
            else if(type[i] == "pilotQuality")
            {
                float val = (value[i] / 100f);
                if (effect[i] == "increased")
                    GlobalVariables.Instance.rarityMultiplier += val;
                if (effect[i] == "reduced")
                    GlobalVariables.Instance.rarityMultiplier -= val;
            }
            else if(type[i] == "upkeepShip")
            {
                float val = (value[i]/100f);
                if (effect[i] == "increased")
                    GlobalVariables.Instance.shipUpkeepMultiplier += val;
                if (effect[i] == "reduced")
                    GlobalVariables.Instance.shipUpkeepMultiplier -= val;
            }
            else if(type[i] == "levelPilot")
            {
                int val = value[i];
                if (effect[i] == "increased")
                    GlobalVariables.Instance.shipUpkeepMultiplier += val;
                if (effect[i] == "reduced")
                    GlobalVariables.Instance.shipUpkeepMultiplier -= val;
            }
            else if(type[i] == "travelAlternative")
            {
                int val = value[i];
                if (effect[i] == "increased")
                    GlobalVariables.Instance.unlockedSystemAlternatives += val;
                if (effect[i] == "reduced")
                    GlobalVariables.Instance.unlockedSystemAlternatives -= val;
                GlobalVariables.Instance.refreshAvailableSystems = true;
            }
            else
            {
                if (type[i].Substring(0,8)=="resource") // so far only implemented for single digit number as I don't predict any more than this
                {
                    char asChar = type[i][type[i].Length - 1];
                    int resourceId = asChar - '0';
                    resourceId--;
                    if (effect[i] == "increased" || effect[i] == "reduced")
                    {
                        float val = (value[i] / 100);
                        if (effect[i] == "increased") GlobalVariables.Instance.listOfResources[resourceId].addToMultiplier(val);
                        if(effect[i] == "reduced") GlobalVariables.Instance.listOfResources[resourceId].addToMultiplier(-val);
                    }
                    if (effect[i] == "additional" )
                    {
                        int val = value[i];
                        Debug.Log(val);
                        GlobalVariables.Instance.listOfResources[resourceId].addToBase(val);       
                    }

                }
            }




        }
    }
    public string getName(){
        return name;
    }
    public string getResearchTime(){
        double timeRemaining = Math.Ceiling((baseResearchTime - researchPoints) / GlobalVariables.Instance.researchRate);
        return timeRemaining.ToString()+" months";
    }
    public void skipTime(int months){
        researchPoints+=months*GlobalVariables.Instance.researchRate;
        if(researchPoints>=baseResearchTime)applyEffectsAndComplete();

    }
}
