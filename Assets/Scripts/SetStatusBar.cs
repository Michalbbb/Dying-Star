using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetStatusBar : MonoBehaviour
{

    [SerializeField] RawImage rawImage;
    [SerializeField] Canvas targetSize;
    [SerializeField] Button skipMonth;
    [SerializeField] Button skipTillNextEvent;
    List<GameObject> resourceValue;
    List<GameObject> incomeValue;

    GameObject researchTime;
    TextMeshProUGUI techName;

    GameObject explorationTime;

    GameObject date;
    // Start is called before the first frame update
    void Start()
    {
        resourceValue=new List<GameObject>();
        incomeValue=new List<GameObject>();
        float x=targetSize.GetComponent<RectTransform>().sizeDelta.x;
        float y=targetSize.GetComponent<RectTransform>().sizeDelta.y;
        if(rawImage!=null && targetSize !=null){
            rawImage.GetComponent<RectTransform>().sizeDelta=new Vector2(x,y/8);
        }
        rawImage.GetComponent<RectTransform>().localPosition=new Vector3(0,y/2-y/16,0);
        const float ICON_SIZE=50;
        float startingX=-targetSize.GetComponent<RectTransform>().sizeDelta.x/2+ICON_SIZE;
        float SPACE_BETWEEN=ICON_SIZE+50;
        foreach(Resource res in GlobalVariables.Instance.listOfResources){
            GameObject gmObject = new GameObject("Resource");
            RawImage image = gmObject.AddComponent<RawImage>();
            gmObject.transform.SetParent(targetSize.transform);
            image.GetComponent<RectTransform>().sizeDelta=new Vector2(ICON_SIZE,ICON_SIZE);
            image.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
            image.GetComponent<RectTransform>().localPosition=new Vector3(startingX,y/2-y/14,0);
            GameObject name=new GameObject("Name"); 
            GameObject value=new GameObject("Value");
            GameObject inc=new GameObject("Income");
            TextMeshProUGUI txt=name.AddComponent<TextMeshProUGUI>();
            name.transform.SetParent(targetSize.transform);
            TextMeshProUGUI val=value.AddComponent<TextMeshProUGUI>();
            value.transform.SetParent(targetSize.transform);
            TextMeshProUGUI income=inc.AddComponent<TextMeshProUGUI>();
            income.transform.SetParent(targetSize.transform);
            txt.GetComponent<RectTransform>().sizeDelta=new Vector2(80,30);
            txt.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
            txt.GetComponent<RectTransform>().localPosition=new Vector3(startingX+15,y/2-y/32,0);
            txt.SetText(res.getName());
            txt.fontSize=15;
            
            val.GetComponent<RectTransform>().sizeDelta=new Vector2(50,50);
            val.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
            val.GetComponent<RectTransform>().localPosition=new Vector3(startingX+ICON_SIZE+10,y/2-y/10,0);
            val.SetText(res.getAmountAsString());
            val.fontSize=15;
            resourceValue.Add(value);

            income.GetComponent<RectTransform>().sizeDelta=new Vector2(50,50);
            income.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
            income.GetComponent<RectTransform>().localPosition=new Vector3(startingX+ICON_SIZE+10,y/2-y/8,0);
            income.SetText("<color=yellow>+"+res.getIncomeAsString()+"</color>");
            income.fontSize=13;
            incomeValue.Add(inc);

            startingX+=SPACE_BETWEEN;
            image.texture = res.getImage();
            
        }
        GameObject research=new GameObject("Research");
        TextMeshProUGUI rTxt=research.AddComponent<TextMeshProUGUI>();
        research.transform.SetParent(targetSize.transform);
        rTxt.GetComponent<RectTransform>().sizeDelta=new Vector2(200,30);
        rTxt.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
        rTxt.GetComponent<RectTransform>().localPosition=new Vector3(startingX+100,y/2-y/28,0);
        rTxt.alignment = TextAlignmentOptions.Center;
        if(GlobalVariables.Instance.currentTechnology!=-1){
            rTxt.SetText(GlobalVariables.Instance.technologies[GlobalVariables.Instance.currentTechnology].getName());
            GameObject time=new GameObject("Time");
            TextMeshProUGUI tTxt=time.AddComponent<TextMeshProUGUI>();
            time.transform.SetParent(targetSize.transform);
            tTxt.GetComponent<RectTransform>().sizeDelta=new Vector2(200,30);
            tTxt.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
            tTxt.GetComponent<RectTransform>().localPosition=new Vector3(startingX+100,y/2-y/12,0);
            tTxt.SetText("<color=yellow>"+GlobalVariables.Instance.technologies[GlobalVariables.Instance.currentTechnology].getResearchTime()+"</color>");
            tTxt.alignment = TextAlignmentOptions.Center;
            tTxt.fontSize=20;
            researchTime=time;

        }
        else rTxt.SetText("<color=red>Technology not chosen.</color>");
        rTxt.fontSize=20;
        techName=rTxt;
        startingX+=300;

        GameObject exploration=new GameObject("Exploration");
        TextMeshProUGUI eTxt=exploration.AddComponent<TextMeshProUGUI>();
        exploration.transform.SetParent(targetSize.transform);
        eTxt.GetComponent<RectTransform>().sizeDelta=new Vector2(200,30);
        eTxt.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
        eTxt.GetComponent<RectTransform>().localPosition=new Vector3(startingX,y/2-y/28,0);
        eTxt.alignment = TextAlignmentOptions.Center;
        if(GlobalVariables.Instance.chosenSpaceSystem!=null){
            eTxt.SetText(GlobalVariables.Instance.chosenSpaceSystem.getInfo());
            GameObject time=new GameObject("Time");
            TextMeshProUGUI tTxt=time.AddComponent<TextMeshProUGUI>();
            time.transform.SetParent(targetSize.transform);
            tTxt.GetComponent<RectTransform>().sizeDelta=new Vector2(200,30);
            tTxt.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
            tTxt.GetComponent<RectTransform>().localPosition=new Vector3(startingX,y/2-y/12,0);
            tTxt.SetText("<color=yellow>"+"TODO"+"</color>");
            tTxt.alignment = TextAlignmentOptions.Center;
            tTxt.fontSize=20;
            explorationTime=time;

        }
        else eTxt.SetText("<color=red>Pilots are waiting to be assigned.</color>");
        eTxt.fontSize=20;
        startingX+=165;
        GameObject dateHolder=new GameObject("Date");
        TextMeshProUGUI dTxt=dateHolder.AddComponent<TextMeshProUGUI>();
        dateHolder.transform.SetParent(targetSize.transform);
        dTxt.GetComponent<RectTransform>().sizeDelta=new Vector2(120,70);
        dTxt.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
        dTxt.GetComponent<RectTransform>().localPosition=new Vector3(startingX,y/2-y/16,0);
        dTxt.SetText("Date\n"+"01."+GlobalVariables.Instance.getMonth()+"."+GlobalVariables.Instance.year.ToString());
        dTxt.alignment = TextAlignmentOptions.Center;
        dTxt.fontSize=25;
        date=dateHolder;
        startingX+=100;
        skipMonth.GetComponent<RectTransform>().localPosition=new Vector3(startingX,y/2-y/16,0);
        startingX+=65;
        skipTillNextEvent.GetComponent<RectTransform>().localPosition=new Vector3(startingX,y/2-y/16,0);
        



    }
    private void Update() {
        if(GlobalVariables.Instance.updateGameStatus){
            int iterator=0;
            foreach(Resource res in GlobalVariables.Instance.listOfResources){
                resourceValue[iterator].GetComponent<TextMeshProUGUI>().SetText(res.getAmountAsString());
                incomeValue[iterator].GetComponent<TextMeshProUGUI>().SetText(res.getIncomeAsString());
                iterator++;
            }
            if(GlobalVariables.Instance.currentTechnology!=-1){
                researchTime.GetComponent<TextMeshProUGUI>().SetText("<color=yellow>" + GlobalVariables.Instance.technologies[GlobalVariables.Instance.currentTechnology].getResearchTime()+"</color>");
            }
            else{
                if(researchTime!=null)researchTime.GetComponent<TextMeshProUGUI>().SetText("");
                techName.SetText("<color=red>Technology not chosen.</color>");
            }
            if(explorationTime!=null){
                explorationTime.GetComponent<TextMeshProUGUI>().SetText("<color=yellow>"+"TODO"+"</color>");
            }
            updateDate();
            GlobalVariables.Instance.updateGameStatus=false;
        }
    }
    public void monthForward(){
            GlobalVariables.Instance.skipTime(1);
    }

    public void forwardTillNextEvent(){
            GlobalVariables.Instance.skipTime(120);

    }

    private void updateDate(){
        date.GetComponent<TextMeshProUGUI>().SetText("Date\n"+"01."+GlobalVariables.Instance.getMonth()+"."+GlobalVariables.Instance.year.ToString());
    }
}
