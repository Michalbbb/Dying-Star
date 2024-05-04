using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pilot 
{
    string shipClass; //// 0 - Skirmisher, 1 - Destroyer, 2 - Cruiser, 3 - Support
    string pilotRarity; // Ordinary, Advanced, Veteran, Renowned, Exalted 
    int rarityAsInt;
    int pilotType;
    List<Ability> abilities;
    string name;
    string surname;
    int level;
    SkillTree skillTree;
    

    float baseCritChance;
    float baseCritMultiplier;
    float baseAttack;
    float baseDefence;
    float baseSpeed;
    float baseHealth;

    int baseMovementPoints;
    float damageMultiplier = 1f;
    float healingMultiplier = 1f;

    int movementPoints;
    int critChance;
    int critMultiplier;
    int attack;
    int defence;
    int speed;
    int health;

    bool canMoveAfterAction;
    bool didMakeAction;


    bool recruited;
    int metalPrice;
    int titanPrice;
    

    public Pilot(int pilotType,int rarity,float critChance,float critMultiplier,float attack,float defence,float speed,float health,string name,string surname)
    {
        recruited = false;
        this.pilotType = pilotType;
        rarityAsInt = rarity;
        baseMovementPoints = 5;
        canMoveAfterAction = false;
        if(pilotType == 0) { shipClass = "Skirmisher"; baseMovementPoints += 2;damageMultiplier -= 0.2f; canMoveAfterAction = true; }
        if(pilotType == 1) { shipClass = "Destroyer"; }
        if(pilotType == 2) { shipClass = "Cruiser"; }
        if(pilotType == 3) { shipClass = "Support"; }
        abilities = GenerateAbility.Instance.generateTwo(pilotType);
        level = GlobalVariables.Instance.startingLevelOfPilots;
        pilotRarity = GlobalVariables.Instance.pilotRarities[rarity];
        baseCritChance = critChance;
        baseCritMultiplier = 100+critMultiplier;
        baseAttack = attack;
        baseDefence= defence;
        baseSpeed = speed;
        baseHealth = health;
        this.name = name;
        this.surname = surname;
        skillTree = GeneratePassiveSkill.Instance.makeSkillTree(pilotType, rarity);
        generatePrice(rarity);
    }
    public string getClass() { return shipClass; }
    public string getAllInfo() // Probably not needed anyomre
    {
        return name+" "+surname+" Class: "+shipClass+" Pilot quality:"+pilotRarity+"\nCrit chance: "+baseCritChance+"\nCrit multiplier: "+baseCritMultiplier+
            "\nAttack: "+baseAttack+ "\nDefence: " + baseDefence + "\nSpeed: " + baseSpeed + "\nHealthpool: " + baseHealth ;
    }
    public void checkSkillTree() // Will be outdated any time soon
    {
        skillTree.getBasicInfo();
    }
    private void generatePrice(int rarity)
    {
        int baseAmount = rarity + 1;
        metalPrice = Random.Range(baseAmount * 8, baseAmount * 12 + 1)*100;
        if (baseAmount >= 4)
        {
            titanPrice = Random.Range(baseAmount * 5, baseAmount * 15 + 1) * 5;
            if (baseAmount == 5) titanPrice *= 4;
        }
        
    }
    public void generatePortrait(Canvas canvas,float posX,float posY)
    {
        refreshStats();
        
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
        imageComponent.GetComponent<RectTransform>().localPosition = new Vector3(posX, posY, 0); // Set position
        imageComponent.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height); // Set size
        imageComponent.color = GlobalVariables.Instance.bgPilotColors[rarityAsInt] ;
        // Window that will hold name and quality
        GameObject pilotPortrait = new GameObject("Portrait");
        RawImage portraitImageComponent = pilotPortrait.AddComponent<RawImage>();

        portraitImageComponent.GetComponent<RectTransform>().SetParent(pilot.transform);
        portraitImageComponent.GetComponent<RectTransform>().localPosition = new Vector3(-width / 2 + portraitWidth / 2.02f, height / 2 - portraitHeight / 2.02f, 0);
        portraitImageComponent.GetComponent<RectTransform>().sizeDelta = new Vector2(portraitWidth, portraitHeight);
        portraitImageComponent.color = Color.black;
        // name
        GameObject nameTxt = new GameObject("name");
        TextMeshProUGUI nTxt = nameTxt.AddComponent<TextMeshProUGUI>();
        nameTxt.transform.SetParent(pilotPortrait.transform);
        nTxt.fontSize = 15;
        nTxt.alignment = TextAlignmentOptions.Center;
        nTxt.color = Color.white;
        nTxt.text = name+" "+surname;
        nameTxt.GetComponent<RectTransform>().localPosition = new Vector3(0, -portraitHeight / 4, 0);
        // quality
        GameObject qualityTxt = new GameObject("quality");
        TextMeshProUGUI qTxt = qualityTxt.AddComponent<TextMeshProUGUI>();
        qualityTxt.transform.SetParent(pilotPortrait.transform);
        qTxt.fontSize = 17;
        qTxt.alignment = TextAlignmentOptions.Center;
        qTxt.color = GlobalVariables.Instance.bgPilotColors[rarityAsInt];
        qTxt.text = "<b>"+pilotRarity+" pilot</b>";
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
        abilityOne.GetComponent<toolTip>().message = abilities[0].getInfo();
        RawImage imageTwo = abilityTwo.AddComponent<RawImage>();
        BoxCollider colliderTwo = abilityTwo.AddComponent<BoxCollider>();
        colliderTwo.isTrigger = true;
        colliderTwo.size = new Vector3(abilityWidth, abilityHeight, 40);
        colliderTwo.AddComponent<toolTip>();
        colliderTwo.GetComponent<toolTip>().message = abilities[1].getInfo();
        RawImage imageThree = abilityThree.AddComponent<RawImage>();
        BoxCollider colliderThree = abilityThree.AddComponent<BoxCollider>();
        colliderThree.isTrigger = true;
        colliderThree.size = new Vector3(abilityWidth, abilityHeight, 40);
        colliderThree.AddComponent<toolTip>();
        colliderThree.GetComponent<toolTip>().message = abilities[2].getInfo();
        imageOne.texture = abilities[0].getIcon();
        imageTwo.texture = abilities[1].getIcon();
        imageThree.texture = abilities[2].getIcon(); 
        abilityOne.transform.SetParent(pilot.transform);
        abilityTwo.transform.SetParent(pilot.transform);
        abilityThree.transform.SetParent(pilot.transform);
        imageOne.GetComponent<RectTransform>().sizeDelta = new Vector2(abilityWidth, abilityHeight);
        imageTwo.GetComponent<RectTransform>().sizeDelta = new Vector2(abilityWidth, abilityHeight);
        imageThree.GetComponent<RectTransform>().sizeDelta = new Vector2(abilityWidth, abilityHeight);

        imageOne.GetComponent<RectTransform>().localPosition = new Vector3(-width / 2 + abilityWidth / 2, -height / 2 + abilityHeightOffset, 0); ;
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
        lTxt.text = level + "lvl.";
        lvlTxt.GetComponent<RectTransform>().localPosition = new Vector3(width / 2.5f, height / 2.5f, 0);
        // class
        GameObject clTxt = new GameObject("class");
        TextMeshProUGUI cTxt = clTxt.AddComponent<TextMeshProUGUI>();
        clTxt.transform.SetParent(pilot.transform);
        cTxt.fontSize = 20;
        cTxt.alignment = TextAlignmentOptions.Center;
        cTxt.color = Color.black;
        cTxt.text = shipClass;
        clTxt.GetComponent<RectTransform>().localPosition = new Vector3(width / 7, height / 2.5f, 0);
        // stats
        GameObject statsTxt = new GameObject("stats");
        TextMeshProUGUI sTxt = statsTxt.AddComponent<TextMeshProUGUI>();
        statsTxt.transform.SetParent(pilot.transform);
        sTxt.fontSize = 13;
        sTxt.alignment = TextAlignmentOptions.Center;
        sTxt.color = Color.black;
        sTxt.text = getStats();
        statsTxt.GetComponent<RectTransform>().localPosition = new Vector3(width / 7, -height / 10, 0);
        if (!recruited)
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
            rectTransform.sizeDelta = new Vector2(resourceWidth * 2, resourceHeight); // Set size

            GameObject textObject = new GameObject("recruitText");
            TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
            text.GetComponent<RectTransform>().sizeDelta = new Vector2(resourceWidth * 1.8f, resourceHeight);
            textObject.transform.SetParent(buttonComponent.transform);
            text.enableAutoSizing = true;
            text.fontSizeMin = 10f;
            RawImage buttonImage = button.AddComponent<RawImage>();

            
            if (canRecruit())
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
        if (GlobalVariables.Instance.listOfResources[2].check(metalPrice) && GlobalVariables.Instance.listOfResources[3].check(titanPrice))
        {
            GlobalVariables.Instance.recruitedPilots.Add(this);
            GlobalVariables.Instance.listOfResources[2].spend(metalPrice);
            GlobalVariables.Instance.listOfResources[3].spend(titanPrice);
            GlobalVariables.Instance.pilotsInPool.Remove(this);
            GlobalVariables.Instance.updateAether = true;
        }
        else
        {
            Debug.Log("Cannot purchase ( This message should never appear )");
        }


    }
    private bool canRecruit()
    {
        if (!GlobalVariables.Instance.listOfResources[2].check(metalPrice)) return false;
        if (!GlobalVariables.Instance.listOfResources[3].check(titanPrice)) return false;
        return true;

    }
    public void refreshStats()
    {
        critChance = (int)baseCritChance;
        critMultiplier = (int)baseCritMultiplier;
        defence = (int)baseDefence;
        attack = (int)baseAttack;
        speed = (int)baseSpeed;
        health = (int)baseHealth;
        movementPoints = baseMovementPoints;
    }
    public string getStats()
    {
        string stats=$"Attack:{attack}\nDefence:{defence}\nSpeed:{speed}\nHealth:{health}\nCrit Chance:{critChance}%\nCrit Multiplier:{critMultiplier}%\nMovement points:{movementPoints}";
        return stats;
    }
    private void mouseEnter(RawImage buttonImage)
    {
        if (buttonImage.color != Color.red) buttonImage.color = GlobalVariables.Instance.baseSelectedButtonColor;
    }
    private void mouseExit(RawImage buttonImage)
    {
        if (buttonImage.color != Color.red) buttonImage.color = Color.black;
    }

}
