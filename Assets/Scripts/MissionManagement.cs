using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionManagement : MonoBehaviour
{

    public delegate void MyAction();
    public static event MyAction doAction;
    [SerializeField] RawImage missionMainCanvas;
    [SerializeField] TextMeshProUGUI missionDetail;
    [SerializeField] TextMeshProUGUI missionName;
    [SerializeField] Button launchButton;

    [SerializeField] GameObject panelAv;
    [SerializeField] GameObject areaAv;
    [SerializeField] GameObject scrollAv;
    [SerializeField] GameObject gridAv;
    [SerializeField] RawImage rwAv;
    [SerializeField] GameObject panelEx;
    [SerializeField] GameObject areaEx;
    [SerializeField] GameObject scrollEx;
    [SerializeField] GameObject gridEx;
    [SerializeField] RawImage rwEx;
    [SerializeField] TextMeshProUGUI time;
    [SerializeField] TextMeshProUGUI [] rewards;
    [SerializeField] RawImage report;
    [SerializeField] TextMeshProUGUI reportTxt;
    [SerializeField] TextMeshProUGUI amountOfPilots;
    [SerializeField] RawImage systemSpace;
    [SerializeField] GameObject grid;
    [SerializeField] GameObject panel;
    [SerializeField] GameObject area;
    [SerializeField] GameObject scroll;
    [SerializeField] TextMeshProUGUI totalPower;
    [SerializeField] TextMeshProUGUI logMissionName;
    [SerializeField] TextMeshProUGUI logMissionDetails;
    [SerializeField] TextMeshProUGUI logEventLog;
    [SerializeField] TextMeshProUGUI logExplorationTime;
    [SerializeField] TextMeshProUGUI logHourglass;

    [SerializeField] TextMeshProUGUI [] logRewards;
    [SerializeField] TextMeshProUGUI logFinalReward;

    [SerializeField] GameObject hideRewards;
    [SerializeField] GameObject hideTime;



    // Start is called before the first frame update
    void Start()
    {
     if(missionDetail.text=="NULL")missionMainCanvas.gameObject.SetActive(false);  
     
     refreshPilots();
    }
    private void OnEnable() {
        Pilot.doAction+=refreshPilots;
        refreshPilots();
    }
    private void OnDisable() {
        Pilot.doAction-=refreshPilots;
        
    }
    public void pickMission(int number){ 
     if(number>GlobalVariables.Instance.availableSpaceSystems.Count-1) return;
        setMission(number);

    }
    public void hideMission(){
     missionMainCanvas.gameObject.SetActive(false);   

    }
    public void setLaunchedMissions(){
        int childCount = systemSpace.transform.childCount;

        
            for (int i = childCount - 1; i >= 0; i--)
            {
            Transform child = systemSpace.transform.GetChild(i);
            if (child.name == "System") Destroy(child.gameObject);
            }
            float width=area.GetComponent<RectTransform>().sizeDelta.x;
            float height=width*2/3;
            float SPACE_BETWEEN=Screen.height/36;
            panel.GetComponent<RectTransform>().sizeDelta = new Vector2(width,panel.GetComponent<RectTransform>().sizeDelta.y);
            area.GetComponent<RectTransform>().sizeDelta = new Vector2(width,area.GetComponent<RectTransform>().sizeDelta.y);
            scroll.GetComponent<RectTransform>().sizeDelta = new Vector2(width,scroll.GetComponent<RectTransform>().sizeDelta.y);
            grid.GetComponent<GridLayoutGroup>().cellSize=new Vector2(width,height);
            grid.GetComponent<GridLayoutGroup>().spacing=new Vector2(0,SPACE_BETWEEN);
            List<GenerateSystem> allSystems=new List<GenerateSystem>();
            allSystems.AddRange(GlobalVariables.Instance.explored);
            allSystems.AddRange(GlobalVariables.Instance.currentlyExploring);
            allSystems.Sort(new SystemComparer());
            systemSpace.GetComponent<RectTransform>().sizeDelta=new Vector2(width,(height+SPACE_BETWEEN)*allSystems.Count-SPACE_BETWEEN); 
            float y=0;  
            GlobalVariables.Instance.equipment.Sort(new EquipmentComparer());
            foreach(GenerateSystem sys in allSystems){
                GameObject system=new GameObject("System");
                RawImage systemImage=system.AddComponent<RawImage>();
                systemImage.GetComponent<RectTransform>().SetParent(systemSpace.transform);
                systemImage.GetComponent<RectTransform>().sizeDelta=new Vector2(width*0.8f,height*0.8f);
                systemImage.GetComponent<RectTransform>().localPosition=new Vector3(0,y,0);
                systemImage.color=new Color(0.7264151f,0.7264151f,0.7264151f);
                systemImage.texture=sys.getImage();
                GameObject txt = new GameObject("System");
                TextMeshProUGUI text=txt.AddComponent<TextMeshProUGUI>();
                text.GetComponent<RectTransform>().SetParent(systemImage.transform);
                text.alignment=TextAlignmentOptions.Center;
                text.font=tooltipManager._instance.globalFont;
                text.enableAutoSizing=true;
                text.fontSizeMax=40;
                text.fontSizeMin=10;
                text.SetText(sys.getInfo());
                text.GetComponent<RectTransform>().sizeDelta=new Vector2(width,height);
                text.GetComponent<RectTransform>().localPosition=new Vector3(0,0,0);
                Button buttonComponent = system.AddComponent<Button>();
                buttonComponent.onClick.AddListener(() => loadLogs(sys));
                y-=width+SPACE_BETWEEN;
            }
    }
    private void loadLogs(GenerateSystem system){
        hideRewards.SetActive(true);
        hideTime.SetActive(true);
        logMissionName.SetText(system.getInfo());
        logMissionDetails.SetText(system.getDescription());
        logEventLog.SetText(system.getReport());
        if(system.isOver())
        {
            logExplorationTime.SetText(system.getBaseExplorationTime()+" months");
            logHourglass.SetText("Exploration duration");
        }
        else 
        {
            logExplorationTime.SetText(system.getExplorationTime()+" months");
            logHourglass.SetText("Remaining time");

        }
        string text;
        for(int i=0;i<logRewards.Length;i++){
            
            if(system.isExplorationOver)
            {
                
                if(system.isSuccess)logRewards[i].SetText(system.getRewards(i)+"<color=green>\n[Granted]</color>");
                else logRewards[i].SetText("<color=red>0\n[Failed]</color>");
            }
            else{
                logRewards[i].SetText(system.getRewards(i).ToString());
            }
        }
        if(system.isExplorationOver){
            if(system.isSuccess)text="<color=green>Chance to find habitable planet permanently increased (Stackable)</color>";
            else text="<color=red>Chance to find habitable planet did NOT increase</color>";
        }
        else text="When habitable planet not found:\n<color=green>Permanently increases chance to find habitable planet</color>";
        logFinalReward.SetText(text);
    }
    public void acceptMission(){
     if(doAction!=null)doAction();
     missionMainCanvas.gameObject.SetActive(false);   

    }
    void setMission(int number){
    missionMainCanvas.gameObject.SetActive(true);
    var system=GlobalVariables.Instance.availableSpaceSystems[number];
    missionDetail.SetText(system.getDescription());
    missionName.SetText(system.getInfo());
    time.SetText(system.getExplorationTime()+" months");
    for(int i=0;i<rewards.Length;i++){
        rewards[i].SetText(system.getRewards(i).ToString());
    }
    if(!system.isExplorationActive&&!system.isExplorationOver){
    report.gameObject.SetActive(false);
    launchButton.gameObject.SetActive(true);
    panelAv.gameObject.SetActive(true);
    panelEx.SetActive(true);
    launchButton.onClick.RemoveAllListeners();
    launchButton.onClick.AddListener(() => {system.launchExploration();refreshPilots();acceptMission();});
    }
    else{
        launchButton.gameObject.SetActive(false);
        panelAv.gameObject.SetActive(false);
        panelEx.SetActive(false);
        reportTxt.SetText(system.getReport());
        report.gameObject.SetActive(true);
    }
    if(GlobalVariables.Instance.waitingToBeAssignedToExploration.Count<1) launchButton.interactable=false;

    }
    string getTotalPower(){
        if(GlobalVariables.Instance.waitingToBeAssignedToExploration.Count<1) return "Total power:\n0 (+0% harmony bonus)";
        int sum=0;
        float [] harmonyBonus= {1f,1.0301f,1.05f,1.1f};
        int [] weight = {1,10,100,1000};
        int uniqueClasses=0;
        int sumOfWeight=0;
        foreach(Pilot p in GlobalVariables.Instance.waitingToBeAssignedToExploration)
        {
            sum+=p.getPower();
            sumOfWeight+=weight[p.getPilotType()];
        }
        while(sumOfWeight>0)
        {
            if(sumOfWeight%10!=0)uniqueClasses++;
            sumOfWeight/=10;
        }       

        sum=(int)(sum*harmonyBonus[uniqueClasses-1]);
        float bonus=harmonyBonus[uniqueClasses-1];
        bonus-=1f;
        bonus*=100f;
        return "Total power:\n"+sum+" (+"+(int)bonus+"% harmony bonus)";
    }
    void refreshPilots(){
        totalPower.SetText(getTotalPower());
        if(GlobalVariables.Instance.waitingToBeAssignedToExploration.Count<1) launchButton.interactable=false;
        else launchButton.interactable=true;
        int max;
        if (GlobalVariables.Instance.hyperdrives>4)max=4;
        else max=GlobalVariables.Instance.hyperdrives;
        if(GlobalVariables.Instance.waitingToBeAssignedToExploration.Count==4||GlobalVariables.Instance.waitingToBeAssignedToExploration.Count>=GlobalVariables.Instance.hyperdrives)
        amountOfPilots.SetText($"<color=red>{GlobalVariables.Instance.waitingToBeAssignedToExploration.Count}/{max}");
        else amountOfPilots.SetText($"{GlobalVariables.Instance.waitingToBeAssignedToExploration.Count}/{max}");
        int childCount = rwAv.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
            {
                Transform child = rwAv.transform.GetChild(i);
                if (child.name == "Pilot") Destroy(child.gameObject);
            }
        float width=areaAv.GetComponent<RectTransform>().sizeDelta.x;
        float height=Screen.height / 9;
        float SPACE_BETWEEN=Screen.height/36;
        panelAv.GetComponent<RectTransform>().sizeDelta = new Vector2(width,panelAv.GetComponent<RectTransform>().sizeDelta.y);
        areaAv.GetComponent<RectTransform>().sizeDelta = new Vector2(width,areaAv.GetComponent<RectTransform>().sizeDelta.y);
        scrollAv.GetComponent<RectTransform>().sizeDelta = new Vector2(width,scrollAv.GetComponent<RectTransform>().sizeDelta.y);
        gridAv.GetComponent<GridLayoutGroup>().cellSize=new Vector2(width,height);
        gridAv.GetComponent<GridLayoutGroup>().spacing=new Vector2(0,SPACE_BETWEEN);
        childCount=rwEx.transform.childCount;
        rwAv.GetComponent<RectTransform>().sizeDelta=new Vector2(width,(height+SPACE_BETWEEN)*GlobalVariables.Instance.recruitedPilots.Count-SPACE_BETWEEN); 
        float y=0;  
        foreach(Pilot p in GlobalVariables.Instance.recruitedPilots)
            {
                p.generatePowerPortrait(rwAv,width,height,y);
                y-=width+SPACE_BETWEEN;
            }
        childCount = rwEx.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
            {
                Transform child = rwEx.transform.GetChild(i);
                if (child.name == "Pilot") Destroy(child.gameObject);
            }
        panelEx.GetComponent<RectTransform>().sizeDelta = new Vector2(width,panelEx.GetComponent<RectTransform>().sizeDelta.y);
        areaEx.GetComponent<RectTransform>().sizeDelta = new Vector2(width,areaEx.GetComponent<RectTransform>().sizeDelta.y);
        scrollEx.GetComponent<RectTransform>().sizeDelta = new Vector2(width,scrollEx.GetComponent<RectTransform>().sizeDelta.y);
        gridEx.GetComponent<GridLayoutGroup>().cellSize=new Vector2(width,height);
        gridEx.GetComponent<GridLayoutGroup>().spacing=new Vector2(0,SPACE_BETWEEN);

        rwEx.GetComponent<RectTransform>().sizeDelta=new Vector2(width,(height+SPACE_BETWEEN)*GlobalVariables.Instance.waitingToBeAssignedToExploration.Count-SPACE_BETWEEN); 
        y=0;  
        foreach(Pilot p in GlobalVariables.Instance.waitingToBeAssignedToExploration)
            {
                p.generatePowerPortrait(rwEx,width,height,y,false);
                y-=width+SPACE_BETWEEN;
            }
    }

    
}
