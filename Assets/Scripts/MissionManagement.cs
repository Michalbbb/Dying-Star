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
    // Start is called before the first frame update
    void Start()
    {
     missionMainCanvas.gameObject.SetActive(false);   
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
    launchButton.onClick.RemoveAllListeners();
    launchButton.onClick.AddListener(() => {system.launchExploration();refreshPilots();acceptMission();});

    }
    void refreshPilots(){
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
