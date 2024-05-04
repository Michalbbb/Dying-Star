using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class MakeIcon : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    void Start()
    {
        string phOfPh = "ph";
        string namePh = "Tyrion Lannister";
        int lvlPh = 1;
        string abiOne = "ability1";
        string abiTwo = "ability2";
        string abiThree = "ability3";
        string sClass="<b>Destroyer</b>";
        string pQuality = "<b><color=yellow>Renowned pilot</color></b>";
        string stats = "Attack:222\nDefence:222\nSpeed:30\nHealth:2222\nCrit Chance:15%\nCrit Multiplier:180%\nMovement points:8";
        bool recruted=false;
        int metalPrice = 1000;
        int titanPrice = 50;
        // Components Widths
        int width = Screen.width / 3;
        int height = Screen.height / 4;
        float portraitWidth = width * 0.4f;
        float portraitHeight = height * 0.6f;
        float abilityWidth = width * 0.1334f;
        float abilityHeight = height * 0.3f;
        float abilityHeightOffset = height * 0.2f;
        float resourceWidth = width * 0.07f;
        float resourceHeight = resourceWidth;
        // Frame

        GameObject pilot = new GameObject("Pilot");
        RawImage imageComponent = pilot.AddComponent<RawImage>();
        imageComponent.GetComponent<RectTransform>().SetParent(canvas.transform);
        imageComponent.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0); // Set position
        imageComponent.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height); // Set size
        imageComponent.color = Color.magenta;
        // Window that will hold name and quality
        GameObject pilotPortrait = new GameObject("Portrait");
        RawImage portraitImageComponent = pilotPortrait.AddComponent<RawImage>();
        
        portraitImageComponent.GetComponent<RectTransform>().SetParent(pilot.transform);
        portraitImageComponent.GetComponent<RectTransform>().localPosition = new Vector3(-width/2+portraitWidth/2.02f, height/2-portraitHeight/2.02f, 0); 
        portraitImageComponent.GetComponent<RectTransform>().sizeDelta = new Vector2(portraitWidth,portraitHeight);
        portraitImageComponent.color = Color.black;
        // name
        GameObject nameTxt = new GameObject("name");
        TextMeshProUGUI nTxt = nameTxt.AddComponent<TextMeshProUGUI>();
        nameTxt.transform.SetParent(pilotPortrait.transform);
        nTxt.fontSize = 15;
        nTxt.alignment = TextAlignmentOptions.Center;
        nTxt.color = Color.white;
        nTxt.text = namePh;
        nameTxt.GetComponent<RectTransform>().localPosition = new Vector3(0, -portraitHeight/4, 0);
        // quality
        GameObject qualityTxt = new GameObject("quality");
        TextMeshProUGUI qTxt = qualityTxt.AddComponent<TextMeshProUGUI>();
        qualityTxt.transform.SetParent(pilotPortrait.transform);
        qTxt.fontSize = 17;
        qTxt.alignment = TextAlignmentOptions.Center;
        qTxt.color = Color.white;
        qTxt.text = pQuality;
        qualityTxt.GetComponent<RectTransform>().localPosition = new Vector3(0, +portraitHeight / 4, 0);
        //Ability images + tooltips
        GameObject abilityOne = new GameObject("Ability1");
        GameObject abilityTwo = new GameObject("Ability2");
        GameObject abilityThree = new GameObject("Ability3");
        RawImage imageOne = abilityOne.AddComponent<RawImage>();
        BoxCollider colliderOne = abilityOne.AddComponent<BoxCollider>();
        colliderOne.isTrigger = true;
        colliderOne.size = new Vector3(abilityWidth, abilityHeight, 40);
        abilityOne.AddComponent<toolTip>();
        abilityOne.GetComponent<toolTip>().message = abiOne;
        RawImage imageTwo = abilityTwo.AddComponent<RawImage>();
        BoxCollider colliderTwo = abilityTwo.AddComponent<BoxCollider>();
        colliderTwo.isTrigger = true;
        colliderTwo.size = new Vector3(abilityWidth, abilityHeight, 40);
        colliderTwo.AddComponent<toolTip>();
        colliderTwo.GetComponent<toolTip>().message = abiTwo;
        RawImage imageThree = abilityThree.AddComponent<RawImage>();
        BoxCollider colliderThree = abilityThree.AddComponent<BoxCollider>();
        colliderThree.isTrigger = true;
        colliderThree.size = new Vector3(abilityWidth, abilityHeight, 40);
        colliderThree.AddComponent<toolTip>();
        colliderThree.GetComponent<toolTip>().message = abiThree;
        imageOne.texture=Resources.Load<Texture2D>(phOfPh);
        imageTwo.texture=Resources.Load<Texture2D>(phOfPh);
        imageThree.texture=Resources.Load<Texture2D>(phOfPh);
        abilityOne.transform.SetParent(pilot.transform);
        abilityTwo.transform.SetParent(pilot.transform);
        abilityThree.transform.SetParent(pilot.transform);
        imageOne.GetComponent<RectTransform>().sizeDelta = new Vector2(abilityWidth, abilityHeight);
        imageTwo.GetComponent<RectTransform>().sizeDelta = new Vector2(abilityWidth, abilityHeight);
        imageThree.GetComponent<RectTransform>().sizeDelta = new Vector2(abilityWidth, abilityHeight);

        imageOne.GetComponent<RectTransform>().localPosition = new Vector3(-width / 2 + abilityWidth / 2, -height/2+abilityHeightOffset, 0);;
        imageTwo.GetComponent<RectTransform>().localPosition = new Vector3(-width / 2 + abilityWidth * 1.5f, -height / 2 + abilityHeightOffset, 0);
        imageThree.GetComponent<RectTransform>().localPosition = new Vector3(-width / 2 + abilityWidth * 2.5f, -height / 2 + abilityHeightOffset, 0);

        // lvl
        GameObject lvlTxt = new GameObject("lvl");
        TextMeshProUGUI lTxt = lvlTxt.AddComponent<TextMeshProUGUI>();
        lvlTxt.transform.SetParent(pilot.transform);
        lTxt.fontSize = 20;
        //lTxt.font = {font} <- TODO DUNE FONT
        lTxt.alignment = TextAlignmentOptions.Center;
        lTxt.color = Color.black;
        lTxt.text = lvlPh + "lvl.";
        lvlTxt.GetComponent<RectTransform>().localPosition = new Vector3(width/2.5f, height/2.5f, 0);
        // class
        GameObject clTxt = new GameObject("class");
        TextMeshProUGUI cTxt = clTxt.AddComponent<TextMeshProUGUI>();
        clTxt.transform.SetParent(pilot.transform);
        cTxt.fontSize = 20;
        cTxt.alignment = TextAlignmentOptions.Center;   
        cTxt.color = Color.black;
        cTxt.text = sClass;
        clTxt.GetComponent<RectTransform>().localPosition = new Vector3(width/7, height / 2.5f, 0);
        // stats
        GameObject statsTxt = new GameObject("stats");
        TextMeshProUGUI sTxt = statsTxt.AddComponent<TextMeshProUGUI>();
        statsTxt.transform.SetParent(pilot.transform);
        sTxt.fontSize = 13;
        sTxt.alignment = TextAlignmentOptions.Center;
        sTxt.color = Color.black;
        sTxt.text = stats;
        statsTxt.GetComponent<RectTransform>().localPosition = new Vector3(width / 7, -height/10, 0);
        if (!recruted)
        {
            // images of resources
            GameObject mPrice = new GameObject("price1");
            GameObject tPrice = new GameObject("price2");
            RawImage metImage = mPrice.AddComponent<RawImage>();
            RawImage titImage = tPrice.AddComponent<RawImage>();
            metImage.texture = Resources.Load<Texture2D>("Icons/Metals");
            titImage.texture = Resources.Load<Texture2D>("Icons/Titan");
            mPrice.GetComponent<RectTransform>().SetParent(pilot.transform);
            mPrice.GetComponent<RectTransform>().localPosition = new Vector3(width / 3, height / 4 - resourceHeight * 0.5f, 0);
            mPrice.GetComponent<RectTransform>().sizeDelta = new Vector2(resourceWidth, resourceHeight);
            tPrice.GetComponent<RectTransform>().SetParent(pilot.transform);
            tPrice.GetComponent<RectTransform>().localPosition = new Vector3(width / 3, height / 4 - resourceHeight * 1.8f, 0);
            tPrice.GetComponent<RectTransform>().sizeDelta = new Vector2(resourceWidth, resourceHeight);
            // price text
            GameObject metTxt = new GameObject("pricetxt");
            TextMeshProUGUI meTxt = metTxt.AddComponent<TextMeshProUGUI>();
            metTxt.transform.SetParent(pilot.transform);
            meTxt.fontSize = 15;
            meTxt.alignment = TextAlignmentOptions.Center;
            meTxt.color = Color.black;
            meTxt.text = metalPrice.ToString();
            metTxt.GetComponent<RectTransform>().localPosition = new Vector3(width / 3 + resourceWidth * 1.3f, height / 4 - resourceHeight * 0.5f, 0);
            GameObject titTxt = new GameObject("pricetxt");
            TextMeshProUGUI tiTxt = titTxt.AddComponent<TextMeshProUGUI>();
            titTxt.transform.SetParent(pilot.transform);
            tiTxt.fontSize = 15;
            tiTxt.alignment = TextAlignmentOptions.Center;
            tiTxt.color = Color.black;
            tiTxt.text = titanPrice.ToString();
            titTxt.GetComponent<RectTransform>().localPosition = new Vector3(width / 3 + resourceWidth * 1.3f, height / 4 - resourceHeight * 1.8f, 0);
            // RecruitButton 
            GameObject button = new GameObject("Recruit");
            Button buttonComponent = button.AddComponent<Button>();
            RectTransform rectTransform = button.AddComponent<RectTransform>();
            rectTransform.SetParent(pilot.transform); // Attach the button to the current GameObject
            rectTransform.localPosition = new Vector3(width / 3 + resourceWidth * 0.9f, height / 4 - resourceHeight * 3.1f, 0);
            rectTransform.sizeDelta = new Vector2(resourceWidth*2, resourceHeight); // Set size

            GameObject textObject = new GameObject("recruitText");
            TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
            text.GetComponent<RectTransform>().sizeDelta = new Vector2(resourceWidth * 1.8f, resourceHeight);
            textObject.transform.SetParent(buttonComponent.transform);
            text.enableAutoSizing = true;
            text.fontSizeMin = 10f;
            RawImage buttonImage = button.AddComponent<RawImage>();

            bool canRecruit;
            if (Random.Range(0, 2) == 1) canRecruit = false;
            else canRecruit = true;
            if (canRecruit)
            {
                buttonImage.color = Color.black;
                buttonComponent.interactable = true;
                text.color = Color.white;
                text.text = "Recruit";
            }
            else
            {
                buttonImage.color = Color.red;
                buttonComponent.interactable = false;
                text.color = Color.white;
                text.text = "Lack of funds";
            }


            text.fontSize = 15;
            text.alignment = TextAlignmentOptions.Center;
            textObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);


            // Add a listener to the button to handle clicks
            buttonComponent.onClick.AddListener(OnButtonClick);

            EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { mouseEnter(buttonImage); });


            eventTrigger.triggers.Add(entry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) => { mouseExit(buttonImage); });

            eventTrigger.triggers.Add(exitEntry);


        }
        
  
    }
    
    private void OnButtonClick()
    {
        Debug.Log("Clicked.");
        
    }
    private void mouseEnter( RawImage buttonImage)
    {
        if (buttonImage.color != Color.red) buttonImage.color = GlobalVariables.Instance.baseSelectedButtonColor;
    }
    private void mouseExit( RawImage buttonImage)
    {
        if (buttonImage.color != Color.red) buttonImage.color = Color.black;
    }


}
