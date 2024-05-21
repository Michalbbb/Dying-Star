using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class GenerateEquipment : MonoBehaviour
{
    public delegate void MyAction();
    public static event MyAction doAction;
    [SerializeField] TextMeshProUGUI instructions;
    [SerializeField] TextMeshProUGUI previewText;

    [SerializeField] Button buildButton;

    [SerializeField] Button [] buttons;

    [SerializeField] RawImage currency1;
    [SerializeField] TextMeshProUGUI cur1value;
    [SerializeField] RawImage currency2;
    [SerializeField] TextMeshProUGUI cur2value;

    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI resourceMetal;
    [SerializeField] TextMeshProUGUI resourceTitan;
    [SerializeField] TextMeshProUGUI resourceQuame;
    [SerializeField] RawImage equipmentPeek;
    [SerializeField] RawImage equipmentIcon;
    [SerializeField] TextMeshProUGUI equipmentDesc;



    string type;
    string quality;
    // Update is called once per frame
    private void Start() {
    equipmentPeek.gameObject.SetActive(false);
    hideCurrency();
    type="";
    quality="";
    refresh();
    refreshResources();
    }

    public void switchType(string value){
        type = value;
        refresh();
        
    }
    public void switchQuality(string value){
        quality = value;
        refresh();
    }
    public void refresh(){
        setDescription();
        if(type=="")
        {
            buildButton.gameObject.SetActive(false);
            instructions.SetText("You need to pick type of equipment before you proceed.");
            foreach(Button b in buttons) b.interactable=false;
        
        }
        string setAsPreview="";
        if(type=="hyperdrive"){
            setAsPreview+="Hyperdrive";
            foreach(Button b in buttons) b.interactable=false;
            instructions.SetText("Quality is not available for hyperdrive.");
            buildButton.gameObject.SetActive(true);
            previewText.SetText(setAsPreview);
            currency2.texture=Resources.Load<Texture2D>("Icons/Quame");
            setCurrency(GlobalVariables.Instance.hyperdriveMetalPrice,GlobalVariables.Instance.hyperdriveQuamePrice,true);
            return;
        }
        else{
           currency2.texture=Resources.Load<Texture2D>("Icons/Titan");
        }
        if(type=="energyBooster")setAsPreview+="Energy Booster";
        if(type=="supportModule")setAsPreview+="Support Module";
        if(type=="forceField")setAsPreview+="Force Field";
        if(type!=""){
            foreach(Button b in buttons) b.interactable=true;
            instructions.SetText("Pick quality");
        }
        buildButton.gameObject.SetActive(true);
        currency2.texture=Resources.Load<Texture2D>("Icons/Titan");
        if(quality=="ordinary"){
            setAsPreview+="(Ordinary)";
            setCurrency(equipmentConstValues.Instance.metalPrice[0],equipmentConstValues.Instance.titanPrice[0]);
        }
        else if(quality=="excellent"){
            setAsPreview+="<color=blue>(Excellent)</color>";
            setCurrency(equipmentConstValues.Instance.metalPrice[1],equipmentConstValues.Instance.titanPrice[1]);
        }
        else if(quality=="superior"){
            setAsPreview+="<color=yellow>(Superior)</color>";
            setCurrency(equipmentConstValues.Instance.metalPrice[2],equipmentConstValues.Instance.titanPrice[2]);
        }
        else buildButton.gameObject.SetActive(false);



        if(setAsPreview!="")previewText.SetText(setAsPreview);

    }
    private void setCurrency(int cur1,int cur2,bool hype=false){
        currency1.gameObject.SetActive(true);
        currency2.gameObject.SetActive(true);
        cur1value.gameObject.SetActive(true);
        cur2value.gameObject.SetActive(true);
        description.gameObject.SetActive(true);
        string value1;
        string value2;
        buildButton.interactable=true;
        if(cur1<=GlobalVariables.Instance.listOfResources[2].getAmount()){
            value1=cur1.ToString();
        }
        else{
            value1="<color=red>"+cur1.ToString()+"</color>";
            buildButton.interactable=false;
        }
        int compareTo= hype ? GlobalVariables.Instance.listOfResources[1].getAmount() : GlobalVariables.Instance.listOfResources[3].getAmount();
        if(cur2<=compareTo)
        value2=cur2.ToString();
        else {
                value2="<color=red>"+cur2.ToString()+"</color>";
                buildButton.interactable=false;
        }
        cur1value.SetText(value1);
        cur2value.SetText(value2);
    }
    private void hideCurrency(){
        currency1.gameObject.SetActive(false);
        currency2.gameObject.SetActive(false);
        cur1value.gameObject.SetActive(false);
        cur2value.gameObject.SetActive(false);
        description.gameObject.SetActive(false);
    }
    public void buildItem(){
        if(type=="hyperdrive"){
            GlobalVariables.Instance.hyperdrives++;
            GlobalVariables.Instance.listOfResources[1].spend(GlobalVariables.Instance.hyperdriveQuamePrice);
            GlobalVariables.Instance.listOfResources[2].spend(GlobalVariables.Instance.hyperdriveMetalPrice);

        }
        else{
            Equipment newPiece=new Equipment(type,quality);
            equipmentPeek.gameObject.SetActive(true);
            equipmentIcon.texture=newPiece.getIcon();
            equipmentDesc.SetText(newPiece.getDescription());
            int number=0;
            if(quality=="excellent")number=1;
            if(quality=="superior")number=2;
            GlobalVariables.Instance.listOfResources[2].spend(equipmentConstValues.Instance.metalPrice[number]);
            GlobalVariables.Instance.listOfResources[3].spend(equipmentConstValues.Instance.titanPrice[number]);
            GlobalVariables.Instance.equipment.Add(newPiece);
            if(doAction!=null){
            doAction();
        }
            
        }
        
        refresh();
        refreshResources();
    }
    private void setDescription(){
        if(type==""){return;}
        if(type=="hyperdrive"){
            description.SetText("Current hyperdrives: "+GlobalVariables.Instance.hyperdrives.ToString());
            return;
        }
        if(quality=="") hideCurrency();
        string data="";
        float multiplier;
        if(quality=="excellent") multiplier=1.3f;
        else if(quality=="superior") multiplier=1.8f;
        else multiplier=1f;
        if(type=="energyBooster"){
            data+="Base attack: "+(int)(equipmentConstValues.Instance.energyBoosterMinAttack*multiplier)+" - "+(int)(equipmentConstValues.Instance.energyBoosterMaxAttack*multiplier);
            data+="\nBase speed: "+(int)(equipmentConstValues.Instance.energyBoosterMinSpeed*multiplier)+" - "+(int)(equipmentConstValues.Instance.energyBoosterMaxSpeed*multiplier);

        }
        if(type=="supportModule"){
            data+="Base health: "+(int)(equipmentConstValues.Instance.supportModuleMinHealth*multiplier)+" - "+(int)(equipmentConstValues.Instance.supportModuleMaxHealth*multiplier);
            data+="\nBase speed: "+(int)(equipmentConstValues.Instance.supportModuleMinSpeed*multiplier)+" - "+(int)(equipmentConstValues.Instance.supportModuleMaxSpeed*multiplier);
        }
        if(type=="forceField"){
            data+="Base health: "+(int)(equipmentConstValues.Instance.forceFieldMinHealth*multiplier)+" - "+(int)(equipmentConstValues.Instance.forceFieldMaxHealth*multiplier);
            data+="\nBase defence: "+(int)(equipmentConstValues.Instance.forceFieldMinDefence*multiplier)+" - "+(int)(equipmentConstValues.Instance.forceFieldMaxDefence*multiplier);
            
        }
        data+="\n+2 additional random stats.";
        description.SetText(data);
    }
    private void refreshResources(){
            resourceMetal.SetText(GlobalVariables.Instance.listOfResources[2].getAmount().ToString());
            resourceTitan.SetText(GlobalVariables.Instance.listOfResources[3].getAmount().ToString());
            resourceQuame.SetText(GlobalVariables.Instance.listOfResources[1].getAmount().ToString());
    }
}
