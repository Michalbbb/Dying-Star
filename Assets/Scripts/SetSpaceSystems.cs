using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SetSpaceSystems : MonoBehaviour
{
    [SerializeField] RawImage [] alternatives;
    [SerializeField] Button[] alternativesButtons;
    [SerializeField] TextMeshProUGUI [] alternativesText;
    [SerializeField] TextMeshProUGUI label;
    [SerializeField] RawImage refreshSystemsTime;
    [SerializeField] TextMeshProUGUI refreshSystemsTimeTxt;

    [SerializeField] Button [] menuButtons;
    [SerializeField] RawImage menuPanel;
    [SerializeField] TextMeshProUGUI menuText;

    [SerializeField] Canvas canvas;

    [SerializeField] GameObject grid;
    [SerializeField] RawImage pilotOverview;
    [SerializeField] TextMeshProUGUI pilotName;
    [SerializeField] RawImage pilotSpace;
    [SerializeField] GameObject panel;
    [SerializeField] GameObject area;
    [SerializeField] GameObject scroll;
    [SerializeField] RawImage factory;

    [SerializeField] RawImage pilotMenu;

    [SerializeField] RawImage equipmentMenu;
    [SerializeField] RawImage logsOverview;
     [SerializeField] GameObject hideRewards;
    [SerializeField] GameObject hideTime;
    EquipmentManagement equipmentManagement;

    Pilot currentPilot;

    string current="Not chosen.";


    private void OnEnable(){
        GenerateEquipment.doAction+=refreshEquipment;
        MissionManagement.doAction+=refreshAfterAcceptingMission;
        MissionManagement.doAction+=refreshSystems;
        
    }
    private void OnDisable() {
        GenerateEquipment.doAction-=refreshEquipment;
        MissionManagement.doAction-=refreshAfterAcceptingMission;
        MissionManagement.doAction-=refreshSystems;

        
    }
    private void Start() {
         if(GlobalVariables.Instance.availableSpaceSystems.Count==0){
            GlobalVariables.Instance.refreshSpaceSystemList();
         }
        equipmentManagement=equipmentMenu.GetComponent<EquipmentManagement>();
        GlobalVariables.Instance.chosenPilot=false;
        GlobalVariables.Instance.pilotId=null;
        hideRewards.SetActive(false);
        hideTime.SetActive(false);
        showSystems();
        pilotMenu.gameObject.SetActive(false);
        equipmentMenu.gameObject.SetActive(false);
        setPilots();
       
    }
    
    private void Update(){
        
        if(GlobalVariables.Instance.chosenPilot){
            GlobalVariables.Instance.chosenPilot=false;
            currentPilot=GlobalVariables.Instance.pilotId;
            updateChosenPilot();
        }
        if(GlobalVariables.Instance.refreshPilotsInHangar){
            GlobalVariables.Instance.refreshPilotsInHangar=false;
            refreshPilotsList();
        }
    }
    public void showSystems(){
        if(current=="cs") return;
        current="cs";
        hidePilots();
        hideHD();
        hideLogs();
        menuButtons[0].GetComponent<RawImage>().color=Color.white;
      
        ShowPreview.rw=menuButtons[0].GetComponent<RawImage>();
        label.SetText("Exploration");
        
        float x=Screen.width/3f;
        float y=Screen.height/3.3f;
        float width=-canvas.GetComponent<RectTransform>().sizeDelta.x/2 + x*0.8f;
        float height=canvas.GetComponent<RectTransform>().sizeDelta.y/8;
        foreach(Button alt in alternativesButtons){
            alt.enabled=true;
            alt.GetComponent<RectTransform>().sizeDelta = new Vector2(x,y);
        }
        for(int i=0;i<alternativesButtons.Length;i+=2){
            alternativesButtons[i].GetComponent<RectTransform>().localPosition = new Vector3(width,height,0);
            alternativesButtons[i+1].GetComponent<RectTransform>().localPosition = new Vector3(width+x*1.15f,height,0);
            height-=y*1.15f;
        }
        foreach(RawImage alt in alternatives) alt.enabled=true;
        foreach(TextMeshProUGUI alt in alternativesText) alt.enabled=true;
        
        for(int i = 0; i < GlobalVariables.Instance.unlockedSystemAlternatives; i++)
        {
            alternativesButtons[i].GetComponent<Button>().interactable = true;
        }
        int iterator=0;
        foreach(GenerateSystem sys in GlobalVariables.Instance.availableSpaceSystems){
            alternatives[iterator].GetComponent<RawImage>().texture=sys.getImage();
            alternatives[iterator].GetComponent<RawImage>().color = Color.white;
            alternativesText[iterator].text=sys.getInfo();
            iterator++;
        }
        refreshSystemsTime.gameObject.SetActive(true);
        refreshSystemsTimeTxt.SetText(GlobalVariables.Instance.refreshExplorationTime+"\nmonths");
    }
    public void refreshSystems(){
        int iterator=0;
        foreach(GenerateSystem sys in GlobalVariables.Instance.availableSpaceSystems){
            alternativesText[iterator].text=sys.getInfo();
            iterator++;
        }
    }
    public void showPilots(){
        if(current=="ps") return;
        current="ps";
        hideSystems();
        hideHD();
        hideLogs();
        
        panel.SetActive(true);
        pilotOverview.gameObject.SetActive(true);
        menuButtons[2].GetComponent<RawImage>().color=Color.white;
  
        ShowPreview.rw=menuButtons[2].GetComponent<RawImage>();
        label.SetText("Pilots");

    }
    public void showHD(){
        if(current=="hd") return;
        current="hd";
        hideSystems();
        hidePilots();
        hideLogs();
        factory.gameObject.SetActive(true);
        menuButtons[3].GetComponent<RawImage>().color=Color.white;
        ShowPreview.rw=menuButtons[3].GetComponent<RawImage>();
        label.SetText("Factory");

    }
    public void showLogs(){
        if(current=="sl") return;
        current="sl";
        hideSystems();
        hidePilots();
        hideHD();
        menuButtons[1].GetComponent<RawImage>().color=Color.white;
        logsOverview.gameObject.SetActive(true);
        ShowPreview.rw=menuButtons[1].GetComponent<RawImage>();
        label.SetText("Event logs");
    }
    private void hideLogs(){
        logsOverview.gameObject.SetActive(false);
        menuButtons[1].GetComponent<RawImage>().color=Color.black;

    }
    private void hideSystems(){
        menuButtons[0].GetComponent<RawImage>().color=Color.black;
        foreach(Button alt in alternativesButtons)
        {
            alt.enabled = false;
        }
        refreshSystemsTime.gameObject.SetActive(false);
        foreach(RawImage alt in alternatives) alt.enabled=false;
        foreach(TextMeshProUGUI alt in alternativesText) alt.enabled=false;
    }
    private void hidePilots(){
        menuButtons[2].GetComponent<RawImage>().color=Color.black;
        panel.SetActive(false);
        pilotOverview.gameObject.SetActive(false);
        

    }
    private void hideHD(){
        menuButtons[3].GetComponent<RawImage>().color=Color.black;
        factory.gameObject.SetActive(false);
    }
    private void setPilots(){

            float BUTTON_X=Screen.width / 4;
            float BUTTON_Y=Screen.height / 6;
            float SPACE_BETWEEN=Screen.height/18;
            panel.GetComponent<RectTransform>().sizeDelta = new Vector2(BUTTON_X,panel.GetComponent<RectTransform>().sizeDelta.y);
            area.GetComponent<RectTransform>().sizeDelta = new Vector2(BUTTON_X,area.GetComponent<RectTransform>().sizeDelta.y);
            scroll.GetComponent<RectTransform>().sizeDelta = new Vector2(BUTTON_X,scroll.GetComponent<RectTransform>().sizeDelta.y);
            grid.GetComponent<GridLayoutGroup>().cellSize=new Vector2(BUTTON_X,BUTTON_Y);
            grid.GetComponent<GridLayoutGroup>().spacing=new Vector2(0,SPACE_BETWEEN);
            List<Pilot> allPilots=new List<Pilot>();
            allPilots.AddRange(GlobalVariables.Instance.recruitedPilots);
            allPilots.AddRange(GlobalVariables.Instance.waitingToBeAssignedToExploration);
            allPilots.Sort(new PilotComparer());
            pilotSpace.GetComponent<RectTransform>().sizeDelta=new Vector2(BUTTON_X,(BUTTON_Y+SPACE_BETWEEN)*allPilots.Count-SPACE_BETWEEN); 
            int x=5;
            float y=0;  
            
            foreach(Pilot p in allPilots){
                p.generatePortrait(null,x,y,4,pilotSpace);
                y-=BUTTON_Y+SPACE_BETWEEN;
            }
    }
    private void refreshPilotsList(){
        clear(pilotSpace,"Pilot");
        setPilots();
    }
    public void updateChosenPilot(){
        if(currentPilot==null) return;
        clear(pilotOverview,"skill");
        pilotName.SetText(currentPilot.getFullName());
        pilotMenu.gameObject.SetActive(true);
        equipmentMenu.gameObject.SetActive(false);
        refreshEquipment();
    }
    public void refreshAfterAcceptingMission(){
        clear(pilotSpace,"Pilot");
        clear(pilotOverview,"skill");
        pilotMenu.gameObject.SetActive(false);
        equipmentMenu.gameObject.SetActive(false);
        pilotName.SetText("Choose pilot from the  list");
        GlobalVariables.Instance.pilotId=null;
        setPilots();
    }
    public void showSkillTree(){
        if(currentPilot==null) return;
        equipmentMenu.gameObject.SetActive(false);
        clear(pilotOverview,"skill");
        currentPilot.generatePassiveTree(pilotOverview,pilotMenu.GetComponent<RectTransform>().sizeDelta.x/2,pilotName.GetComponent<RectTransform>().sizeDelta.y);
    }
    public void showEquipment(){
        if(currentPilot==null) return;
        clear(pilotOverview,"skill");
        equipmentMenu.gameObject.SetActive(true);
        
    }
    public void refreshEquipment(){
        
        equipmentManagement.refreshEquipment(currentPilot);
        equipmentManagement.refreshSlots(currentPilot);
        refreshPilotsList();
    }
    private void clear(RawImage target,string name){
        int childCount = target.transform.childCount;

        
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = target.transform.GetChild(i);
            if (child.name == name) Destroy(child.gameObject);
        }
    }
    public void removeEquipmentIfExist(int number){
        if(currentPilot==null) return;
        currentPilot.forceRemove(number);
        refreshEquipment();
    }
}
