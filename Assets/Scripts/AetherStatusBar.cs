using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AetherStatusBar : MonoBehaviour
{
    [SerializeField] RawImage rawImage;
    [SerializeField] Canvas canvas;
    List<GameObject> resourceValue;
    [SerializeField] Button encreDraft;
    [SerializeField] Button basicDraft;

    [SerializeField] TextMeshProUGUI bdNulls;
    [SerializeField] TextMeshProUGUI edNulls;
    [SerializeField] TextMeshProUGUI edEncre;

    


    // Start is called before the first frame update
    void Start()
    {
        
        resourceValue=new List<GameObject>();
        float x=canvas.GetComponent<RectTransform>().sizeDelta.x;
        float y=canvas.GetComponent<RectTransform>().sizeDelta.y;
        if(rawImage!=null && canvas !=null){
            rawImage.GetComponent<RectTransform>().sizeDelta=new Vector2(x/2.5f,y/8);
        }
        rawImage.GetComponent<RectTransform>().localPosition=new Vector3(x/2.5f,y/2-y/16,0);
        float ICON_SIZE=y/12;
        float startingX=-rawImage.GetComponent<RectTransform>().sizeDelta.x/2+ICON_SIZE/1.5f;
        float SPACE_BETWEEN=ICON_SIZE+20;
        foreach(Resource res in GlobalVariables.Instance.listOfResources){
            if(res.getName().Equals("Quame")){continue;}
            GameObject gmObject = new GameObject("Resource");
            RawImage image = gmObject.AddComponent<RawImage>();
            gmObject.transform.SetParent(rawImage.transform);
            image.GetComponent<RectTransform>().sizeDelta=new Vector2(ICON_SIZE,ICON_SIZE);
            image.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
            image.GetComponent<RectTransform>().localPosition=new Vector3(startingX,0,0);
            GameObject name=new GameObject("Name"); 
            GameObject value=new GameObject("Value");
            TextMeshProUGUI txt=name.AddComponent<TextMeshProUGUI>();
            name.transform.SetParent(rawImage.transform);
            TextMeshProUGUI val=value.AddComponent<TextMeshProUGUI>();
            value.transform.SetParent(rawImage.transform);
            txt.GetComponent<RectTransform>().sizeDelta=new Vector2(80,30);
            txt.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
            txt.alignment=TextAlignmentOptions.Center;
            txt.GetComponent<RectTransform>().localPosition=new Vector3(startingX,ICON_SIZE*0.6f,0);

            txt.SetText(res.getName());
            txt.fontSize=20;
            
            val.GetComponent<RectTransform>().sizeDelta=new Vector2(80,30);
            val.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
            val.alignment=TextAlignmentOptions.Center;
            val.GetComponent<RectTransform>().localPosition=new Vector3(startingX,-ICON_SIZE*0.6f,0);
            val.SetText(res.getAmountAsString());
            val.fontSize=20;
            resourceValue.Add(value);
            startingX+=SPACE_BETWEEN;
            image.texture = res.getImage();
            }
            setButtons();
            updateBasicDraft();
            updateEncreDraft();
            



    }
    private void setButtons(){
        if(encreDraft==null||basicDraft==null){return;}
        float width=Screen.width/5;
        float height=Screen.height/8;
        float x=-canvas.GetComponent<RectTransform>().sizeDelta.x/2+width*0.6f;
        float y=canvas.GetComponent<RectTransform>().sizeDelta.y/2 - height/2 - height*2;
        float spaceY = canvas.GetComponent<RectTransform>().sizeDelta.y/4;
        encreDraft.GetComponent<RectTransform>().sizeDelta=new Vector2(width,height);
        basicDraft.GetComponent<RectTransform>().sizeDelta=new Vector2(width,height);
        encreDraft.GetComponent<RectTransform>().localPosition=new Vector3(x,y,0);
        basicDraft.GetComponent<RectTransform>().localPosition=new Vector3(x,y-spaceY,0);

        bdNulls.GetComponent<RectTransform>().sizeDelta=new Vector2(basicDraft.GetComponent<RectTransform>().sizeDelta.x/2,bdNulls.GetComponent <RectTransform>().sizeDelta.y);
        edNulls.GetComponent<RectTransform>().sizeDelta=new Vector2(encreDraft.GetComponent<RectTransform>().sizeDelta.x/2,edNulls.GetComponent <RectTransform>().sizeDelta.y);
        edEncre.GetComponent<RectTransform>().sizeDelta=new Vector2(encreDraft.GetComponent<RectTransform>().sizeDelta.x/2,edEncre.GetComponent <RectTransform>().sizeDelta.y);

        bdNulls.GetComponent<RectTransform>().localPosition=new Vector3(x-basicDraft.GetComponent<RectTransform>().sizeDelta.x/3.7f,y-spaceY+bdNulls.GetComponent<RectTransform>().sizeDelta.y*1.7f,0);
        edNulls.GetComponent<RectTransform>().localPosition=new Vector3(x-encreDraft.GetComponent<RectTransform>().sizeDelta.x/3.7f,y+edNulls.GetComponent<RectTransform>().sizeDelta.y*1.7f,0);
        edEncre.GetComponent<RectTransform>().localPosition=new Vector3(x-encreDraft.GetComponent<RectTransform>().sizeDelta.x/3.7f,edNulls.transform.localPosition.y+edEncre.GetComponent<RectTransform>().sizeDelta.y*1f,0);
    }
    private void Update() {
        if(GlobalVariables.Instance.updateGameStatus){
            int iterator=0;
            foreach(Resource res in GlobalVariables.Instance.listOfResources){
                if(!res.getName().Equals("Quame")){
                resourceValue[iterator].GetComponent<TextMeshProUGUI>().SetText(res.getAmountAsString());
                iterator++;
                }
            }
            GlobalVariables.Instance.updateGameStatus=false;
            updateBasicDraft();
            updateEncreDraft();
        }
    }
    private void updateBasicDraft(){
        var gv = GlobalVariables.Instance;
        string bdText;
        if(gv.freeDrafts>0){
            bdText=$"<color=green>Free Draft({gv.freeDrafts})</color>";
            basicDraft.interactable=true;
        }
        else{
            int nullPrice=gv.basicDraftNullsPrice/1000;
            if(gv.listOfResources[0].check(gv.basicDraftNullsPrice)){
            bdText=$"<color=green>Price: {nullPrice}k nulls</color>";
            basicDraft.interactable=true;
            }
            else {
            bdText=$"<color=red>Price: {nullPrice}k nulls</color>";
            basicDraft.interactable=false;
            }
        }
        bdNulls.SetText(bdText);
    }
    private void updateEncreDraft(){
        var gv = GlobalVariables.Instance;
        string edText;
        string edEncText;
        if(gv.listOfResources[0].check(gv.encreDraftNullsPrice)&&gv.listOfResources[4].check(gv.encreDraftEncrePrice)){
            encreDraft.interactable=true;
        }
        else encreDraft.interactable=false;
        int nullPrice=gv.encreDraftNullsPrice/1000;
        int encrePrice=gv.encreDraftEncrePrice;
        if(gv.listOfResources[0].check(gv.encreDraftNullsPrice)){
            edText=$"<color=green>Price: {nullPrice}k nulls</color>";
        }
        else  edText=$"<color=red>Price: {nullPrice}k nulls</color>";
        if(gv.listOfResources[4].check(gv.encreDraftEncrePrice)){
            edEncText=$"<color=green>Price: {encrePrice} encre</color>";
        }
        else  edEncText=$"<color=red>Price: {encrePrice} encre</color>";
        edEncre.SetText(edEncText);
        edNulls.SetText(edText);

    }
}
