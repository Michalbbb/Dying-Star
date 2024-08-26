using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.NetworkInformation;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SetStatusBar : MonoBehaviour
{

    [SerializeField] RawImage rawImage;
    [SerializeField] Canvas targetSize;
    [SerializeField] Button skipMonth;
    [SerializeField] Button skipToNewSystems;
    [SerializeField] Button skipTillNextEvent;
    [SerializeField] TextMeshProUGUI skipTillNextEventTXT;
    [SerializeField] TextMeshProUGUI skipToNewSystemsTXT;
    List<GameObject> resourceValue;
    List<GameObject> incomeValue;

    TextMeshProUGUI researchInfo;

    TextMeshProUGUI explorationTime;
    private int skipToSystems=1;
    private int skipToEvent=1;
    
    public string file="TextFiles/Technology.txt";
 
    string fileText;
    GameObject date;
    // Start is called before the first frame update
    void Start()
    {
        if (!GlobalVariables.Instance.isTechnologyGenerated) generateTechnology();
        skipToSystems=GlobalVariables.Instance.refreshExplorationTime;
        resourceValue=new List<GameObject>();
        incomeValue=new List<GameObject>();
        float x=targetSize.GetComponent<RectTransform>().sizeDelta.x;
        float y=targetSize.GetComponent<RectTransform>().sizeDelta.y;
        float rIX =x;
        float rIY=y/8;
        if(rawImage!=null && targetSize !=null){
            rawImage.GetComponent<RectTransform>().sizeDelta=new Vector2(rIX,rIY);
        }
        rawImage.GetComponent<RectTransform>().localPosition=new Vector3(0,y/2-rIY/2,0);
        float ICON_SIZE=x*0.04f;
        float startingX=-targetSize.GetComponent<RectTransform>().sizeDelta.x/2+ICON_SIZE/2;
        float SPACE_BETWEEN=x*0.07f;
        foreach(Resource res in GlobalVariables.Instance.listOfResources){
            GameObject gmObject = new GameObject("Resource");
            RawImage image = gmObject.AddComponent<RawImage>();
            gmObject.transform.SetParent(rawImage.transform);
            image.GetComponent<RectTransform>().sizeDelta=new Vector2(ICON_SIZE,ICON_SIZE);
            image.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
            image.GetComponent<RectTransform>().localPosition=new Vector3(startingX,0,0);
            GameObject name=new GameObject("Name"); 
            GameObject value=new GameObject("Value");
            GameObject inc=new GameObject("Income");
            TextMeshProUGUI txt=name.AddComponent<TextMeshProUGUI>();
            name.transform.SetParent(rawImage.transform);
            TextMeshProUGUI val=value.AddComponent<TextMeshProUGUI>();
            value.transform.SetParent(rawImage.transform);
            TextMeshProUGUI income=inc.AddComponent<TextMeshProUGUI>();
            income.transform.SetParent(rawImage.transform); 
            txt.GetComponent<RectTransform>().sizeDelta=new Vector2(ICON_SIZE,rIY*0.2f);
            txt.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
            txt.GetComponent<RectTransform>().localPosition=new Vector3(startingX,rIY/2-rIY*0.1f,0);
            txt.SetText(res.getName());
            txt.fontSize=8;
            txt.enableAutoSizing=true;
            txt.alignment=TextAlignmentOptions.Center;
            val.GetComponent<RectTransform>().sizeDelta=new Vector2(x*0.025f,rIY*0.2f);
            val.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
            val.GetComponent<RectTransform>().localPosition=new Vector3(startingX+ICON_SIZE*0.9f,rIY/2-ICON_SIZE/2,0);
            val.SetText(res.getAmountAsString());
            val.fontSize=8;
            val.enableAutoSizing=true;
            val.alignment=TextAlignmentOptions.Center;
            resourceValue.Add(value);

            income.GetComponent<RectTransform>().sizeDelta=new Vector2(x*0.025f,rIY*0.2f);
            income.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
            income.GetComponent<RectTransform>().localPosition=new Vector3(startingX+ICON_SIZE*0.9f,rIY/2-ICON_SIZE/2-rIY*0.2f,0);
            income.SetText("<color=yellow>+"+res.getIncomeAsString()+"</color>");
            income.fontSize=8;
            income.enableAutoSizing=true;
            income.alignment=TextAlignmentOptions.Center;
            incomeValue.Add(inc);

            startingX+=SPACE_BETWEEN;
            image.texture = res.getImage();
            
        }
        GameObject research=new GameObject("Research");
        TextMeshProUGUI rTxt=research.AddComponent<TextMeshProUGUI>();
        research.transform.SetParent(rawImage.transform);
        rTxt.GetComponent<RectTransform>().sizeDelta=new Vector2(x*0.14f,rIY);
        rTxt.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
        rTxt.GetComponent<RectTransform>().localPosition=new Vector3(startingX+x*0.07f,0,0);
        rTxt.alignment = TextAlignmentOptions.Center;
        if(GlobalVariables.Instance.currentTechnology!=-1){
            skipToEvent=GlobalVariables.Instance.technologies[GlobalVariables.Instance.currentTechnology].getResearchTimeAsInt();
            rTxt.SetText(GlobalVariables.Instance.technologies[GlobalVariables.Instance.currentTechnology].getName()+"\n"+"<color=yellow>"+GlobalVariables.Instance.technologies[GlobalVariables.Instance.currentTechnology].getResearchTime()+"</color>");     
        }
        else rTxt.SetText("<color=red>Technology not chosen.</color>");
        rTxt.fontSize=20;
        rTxt.enableAutoSizing=true;
        rTxt.alignment=TextAlignmentOptions.Center;
        researchInfo=rTxt;
        startingX+=x*0.23f;

        GameObject exploration=new GameObject("Exploration");
        TextMeshProUGUI eTxt=exploration.AddComponent<TextMeshProUGUI>();
        exploration.transform.SetParent(rawImage.transform);
        eTxt.GetComponent<RectTransform>().sizeDelta=new Vector2(x*0.14f,rIY);
        eTxt.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
        eTxt.GetComponent<RectTransform>().localPosition=new Vector3(startingX+0.07f,0,0);
        eTxt.alignment = TextAlignmentOptions.Center;
        if(GlobalVariables.Instance.currentlyExploring.Count>0){
                int minExploration=361; // 360 is max roll duration
                foreach(GenerateSystem sys in GlobalVariables.Instance.currentlyExploring){
                    if(minExploration>sys.getExplorationTime())minExploration=sys.getExplorationTime();
                }
                if(GlobalVariables.Instance.currentTechnology==-1)skipToEvent=minExploration;
                if(minExploration<skipToEvent)skipToEvent=minExploration;
                eTxt.SetText($"Shortest exploration will end in:\n<color=yellow>{minExploration} months</color>");
        }
        else eTxt.SetText("<color=red>Pilots are waiting to be assigned.</color>");
        explorationTime=eTxt;
        eTxt.fontSize=20;
        eTxt.enableAutoSizing=true;
        eTxt.alignment=TextAlignmentOptions.Center;
        startingX+=x*0.15f;
        GameObject dateHolder=new GameObject("Date");
        TextMeshProUGUI dTxt=dateHolder.AddComponent<TextMeshProUGUI>();
        dateHolder.transform.SetParent(rawImage.transform);
        dTxt.GetComponent<RectTransform>().sizeDelta=new Vector2(x*0.14f,rIY);
        dTxt.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
        dTxt.GetComponent<RectTransform>().localPosition=new Vector3(startingX+0.07f,0,0);
        dTxt.alignment = TextAlignmentOptions.Center;
        dTxt.fontSize=25;
        dTxt.enableAutoSizing=true;
        date=dateHolder;
        updateDate();
        startingX+=x*0.10f;
        float buttonSize = rIY*0.5f < x*0.06f ? rIY*0.5f : x*0.06f;
        skipMonth.GetComponent<RectTransform>().sizeDelta=new Vector2(buttonSize,buttonSize);
        skipTillNextEvent.GetComponent<RectTransform>().sizeDelta=new Vector2(buttonSize,buttonSize);
        skipToNewSystems.GetComponent<RectTransform>().sizeDelta=new Vector2(buttonSize,buttonSize);
        skipMonth.GetComponent<RectTransform>().localPosition=new Vector3(startingX,-rawImage.GetComponent<RectTransform>().sizeDelta.y/2+buttonSize/2+15,0);
        startingX+=buttonSize*1.6f;
        skipToNewSystems.GetComponent<RectTransform>().localPosition=new Vector3(startingX,-rawImage.GetComponent<RectTransform>().sizeDelta.y/2+buttonSize/2+15,0);
        startingX+=buttonSize*1.6f;
        skipTillNextEvent.GetComponent<RectTransform>().localPosition=new Vector3(startingX,-rawImage.GetComponent<RectTransform>().sizeDelta.y/2+buttonSize/2+15,0);
        
        updateSkipTxt();



    }
    void generateTechnology(){
        if (File.Exists(Path.Combine(Application.streamingAssetsPath,file)))
        {
            // Read all text from the file
            fileText = File.ReadAllText(Path.Combine(Application.streamingAssetsPath,file));
            string [] data=fileText.Split("\n");
            for(int i=0;i<data.Length;i++)
              GlobalVariables.Instance.technologies.Add(new Technology(data[i]));
            GlobalVariables.Instance.isTechnologyGenerated = true;
        }
        else{
            Debug.Log("NOT FOUND");
            Debug.Log(Path.Combine(Application.streamingAssetsPath,file));
        }
    }
    private void Update() {
        if(GlobalVariables.Instance.updateGameStatus){
            int iterator=0;
            foreach(Resource res in GlobalVariables.Instance.listOfResources){
                resourceValue[iterator].GetComponent<TextMeshProUGUI>().SetText(res.getAmountAsString());
                incomeValue[iterator].GetComponent<TextMeshProUGUI>().SetText("<color=yellow>+"+res.getIncomeAsString()+"</color>");
                iterator++;
            }
            skipToSystems=GlobalVariables.Instance.refreshExplorationTime;
            if(GlobalVariables.Instance.currentTechnology!=-1){
                skipToEvent=GlobalVariables.Instance.technologies[GlobalVariables.Instance.currentTechnology].getResearchTimeAsInt();
                researchInfo.SetText(GlobalVariables.Instance.technologies[GlobalVariables.Instance.currentTechnology].getName()+"\n"+"<color=yellow>"+GlobalVariables.Instance.technologies[GlobalVariables.Instance.currentTechnology].getResearchTime()+"</color>");
            }
            else{
                researchInfo.SetText("<color=red>Technology not chosen.</color>");
            }
            if(GlobalVariables.Instance.currentlyExploring.Count>0){
                int minExploration=361; // 360 is max roll duration
                foreach(GenerateSystem sys in GlobalVariables.Instance.currentlyExploring){
                    if(minExploration>sys.getExplorationTime())minExploration=sys.getExplorationTime();
                }
                if(GlobalVariables.Instance.currentTechnology==-1)skipToEvent=minExploration;
                if(minExploration<skipToEvent)skipToEvent=minExploration;
                explorationTime.SetText($"Shortest exploration will end in:\n<color=yellow>{minExploration} months</color>");
            }
            else explorationTime.SetText("<color=red>Pilots are waiting to be assigned.</color>");
            updateSkipTxt();
            updateDate();
            GlobalVariables.Instance.updateGameStatus=false;
        }
    }
    private void updateSkipTxt(){
        skipTillNextEventTXT.SetText("SKIP "+skipToEvent+" MONTHS");
        skipToNewSystemsTXT.SetText("SKIP "+skipToSystems+" MONTHS");
    }
    public void monthForward(){
            GlobalVariables.Instance.skipTime(1);
    }

    public void forwardTillNextEvent(){
            GlobalVariables.Instance.skipTime(skipToEvent);
    }
    public void forwardToNewSystems(){
            GlobalVariables.Instance.skipTime(skipToSystems);
    }

    private void updateDate(){
        date.GetComponent<TextMeshProUGUI>().SetText("Date\n"+"01."+GlobalVariables.Instance.getMonth()+"."+GlobalVariables.Instance.year.ToString()+"\n"+GlobalVariables.Instance.getRemainingTime());
    }
}
