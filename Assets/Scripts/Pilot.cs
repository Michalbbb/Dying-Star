using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pilot 
{
    public delegate void MyAction();
    public static event MyAction doAction;
    string shipClass; //// 0 - Skirmisher, 1 - Destroyer, 2 - Cruiser, 3 - Support
    string pilotRarity; // Ordinary, Advanced, Veteran, Renowned, Exalted 
    int rarityAsInt;
    int pilotType;
    List<Ability> abilities;
    string name;
    string surname;
    int level;
    SkillTree skillTree;
    int id;
    float exp;
    int requiredExp;

    Equipment [] equipment;

    float baseCritChance;
    float baseCritMultiplier;
    float baseAttack;
    float baseDefence;
    float baseSpeed;
    float baseHealth;


    bool equipmentBinded;

    int baseMovementPoints;
    float damageMultiplier = 1f;

    int movementPoints;
    float healingMultiplier=1f;
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
        exp=0;
        requiredExp=GlobalVariables.Instance.requiredExpPerLevel[level-1];
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
        equipment=new Equipment[3];
        for(int i=0;i<3;i++){
            equipment[i]=null;
        }
        equipmentBinded=false;
    }
    public int getPilotType(){
        return pilotType;
    }
    public void addExp(float expAmount){
        exp+=expAmount*GlobalVariables.Instance.pilotExpMultiplier;
        while(exp>=requiredExp&&level<GlobalVariables.Instance.maxLevel){
            exp-=requiredExp;
            level++;
            skillTree.addSkillPoints(1);
            requiredExp=GlobalVariables.Instance.requiredExpPerLevel[level-1];
        }
        
    }
    public void generatePowerPortrait(RawImage rawImage,float width,float height,float  y, bool add=true){
        refreshStats();
        GameObject pilotPortrait = new GameObject("Pilot");
        RawImage portraitImageComponent = pilotPortrait.AddComponent<RawImage>();
        
        portraitImageComponent.GetComponent<RectTransform>().SetParent(rawImage.transform);
        portraitImageComponent.GetComponent<RectTransform>().localPosition = new Vector3(0, y, 0); // Set position
        portraitImageComponent.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height); // Set size
        portraitImageComponent.color = Color.black;
        // Name ,  , Level , Power , Class
        GameObject nameTxt = new GameObject("name");
        TextMeshProUGUI nTxt = nameTxt.AddComponent<TextMeshProUGUI>();
        nameTxt.transform.SetParent(pilotPortrait.transform);
        nTxt.fontSize = 15;
        nTxt.GetComponent<RectTransform>().sizeDelta = new Vector2(pilotPortrait.GetComponent<RectTransform>().sizeDelta.x*0.9f,50);
        nTxt.font = tooltipManager._instance.globalFont;
        nTxt.alignment = TextAlignmentOptions.Center;
        nTxt.color = Color.white;
        nTxt.text = name+" "+surname+" ("+level+"lvl)\n"+"Ship class: "+shipClass+"\nPower: "+getPower();
        nameTxt.GetComponent<RectTransform>().localPosition = new Vector3(0, -height/4, 0);
        // Quality
        GameObject qualityTxt = new GameObject("quality");
        TextMeshProUGUI qTxt = qualityTxt.AddComponent<TextMeshProUGUI>();
        qualityTxt.transform.SetParent(pilotPortrait.transform);
        qTxt.fontSize = 17;
        qTxt.font = tooltipManager._instance.globalFont;
        qTxt.alignment = TextAlignmentOptions.Center;
        qTxt.color = GlobalVariables.Instance.bgPilotColors[rarityAsInt];
        qTxt.text = "<b>"+pilotRarity+" pilot</b>";
        qualityTxt.GetComponent<RectTransform>().localPosition = new Vector3(0, +height / 4, 0);
        //Button to assign as exploration team
        Button x= pilotPortrait.AddComponent<Button>();
        x.onClick.RemoveAllListeners();
        if(add)x.onClick.AddListener(() => addToWaitingList() );
        else x.onClick.AddListener(() => removeFromWaitingList() );
        
            
    }
    void addToWaitingList(){
        GlobalVariables.Instance.addToWaitingList(this);
        if(doAction!=null)doAction();
    }
    void removeFromWaitingList(){
        GlobalVariables.Instance.removeFromWaitingList(this);
        if(doAction!=null)doAction();
    }
    public string getClass() { return shipClass; }
    public string getFullName(){ return name+" "+surname; }
    private void generatePrice(int rarity)
    {
        int baseAmount = rarity + 1;
        metalPrice = Random.Range(baseAmount * 8, baseAmount * 12 + 1)*100;
        if (baseAmount >= 4)
        {
            titanPrice = Random.Range(baseAmount * 5, baseAmount * 15 + 1) * 5; // 100-300  /  500 - 1500  25*5 = 250 * 2 = 500
            if (baseAmount == 5) titanPrice *= 4;
        }
        
    }
    public void generatePassiveTree(RawImage canv, float optionalWidthOffset=0,float optionalHeightOffset=0){
            if (canv == null) return;
            float textHeight=20;
            float width= canv.GetComponent<RectTransform>().sizeDelta.x/5;
            float height= canv.GetComponent <RectTransform>().sizeDelta.y/8;
            float baseX = -canv.GetComponent<RectTransform>().sizeDelta.x/2+width/2+optionalWidthOffset+canv.GetComponent<RectTransform>().sizeDelta.x/2*(1f-3.6f/5);  
            float baseY = canv.GetComponent<RectTransform>().sizeDelta.y/2-height/2-textHeight*2-optionalHeightOffset;  
            float y=baseY;
            GameObject passiveSkillsTxt = new GameObject("skill");
            TextMeshProUGUI psTxt=passiveSkillsTxt.AddComponent<TextMeshProUGUI>();
            psTxt.alignment = TextAlignmentOptions.Center;
            passiveSkillsTxt.transform.SetParent(canv.transform);
            psTxt.fontSizeMin=10;
            psTxt.enableAutoSizing=true;
            psTxt.font = tooltipManager._instance.globalFont;
            psTxt.alignment = TextAlignmentOptions.Center;
            psTxt.text = "(Unspent)\nskill points:\n"+skillTree.getSkillPoints().ToString();
            passiveSkillsTxt.GetComponent<RectTransform>().sizeDelta=new Vector2(width,textHeight*2);
            passiveSkillsTxt.GetComponent<RectTransform>().localPosition=new Vector3(canv.GetComponent<RectTransform>().sizeDelta.x/2-width/2,canv.GetComponent<RectTransform>().sizeDelta.y/2-textHeight,0);
            foreach(PassiveSkill ps in skillTree.getRegion1()){
                    GameObject skillButton = new GameObject("skill");
                    RawImage imageComponent = skillButton.AddComponent<RawImage>();
                    skillButton.GetComponent<RectTransform>().SetParent(canv.transform);
                    skillButton.GetComponent<RectTransform>().sizeDelta = new Vector2(width,height);
                    skillButton.GetComponent<RectTransform>().localPosition = new Vector3(baseX,y,0);
                    y-=height*1.2f+textHeight*2.2f;
                    imageComponent.texture = ps.returnIcon();
                    Button buttonComponent = skillButton.AddComponent<Button>();
                    
                    BoxCollider collider = skillButton.AddComponent<BoxCollider>();
                    collider.isTrigger = true;
                    collider.size = new Vector3(width, height, 0);
                    skillButton.AddComponent<toolTip>();
                    skillButton.GetComponent<toolTip>().message=ps.getProperDesc();
                    setData(skillButton);
                    GameObject nameTxt = new GameObject("name");
                    TextMeshProUGUI nTxt=nameTxt.AddComponent<TextMeshProUGUI>();
                    nTxt.alignment = TextAlignmentOptions.Center;
                    nameTxt.transform.SetParent(skillButton.transform);
                    nTxt.fontSize = 5;
                    nTxt.fontSizeMin=10;
                    nTxt.enableAutoSizing=true;
                    nTxt.font = tooltipManager._instance.globalFont;
                    nTxt.alignment = TextAlignmentOptions.Center;
                    nTxt.text = ps.getName();
                    nameTxt.GetComponent<RectTransform>().sizeDelta=new Vector2(width,textHeight);
                    nameTxt.GetComponent<RectTransform>().localPosition=new Vector3(0,height/2+textHeight/2,0);
                    GameObject lvlText = new GameObject("level");
                    TextMeshProUGUI lTxt=lvlText.AddComponent<TextMeshProUGUI>();
                    lTxt.alignment = TextAlignmentOptions.Center;
                    lvlText.transform.SetParent(skillButton.transform);
                    lTxt.fontSizeMin=10;
                    lTxt.enableAutoSizing=true;
                    lTxt.font = tooltipManager._instance.globalFont;
                    lTxt.alignment = TextAlignmentOptions.Center;
                    lTxt.text = ps.getLevel();
                    lvlText.GetComponent<RectTransform>().sizeDelta=new Vector2(width,textHeight);
                    lvlText.GetComponent<RectTransform>().localPosition=new Vector3(0,-height/2-textHeight/2,0);
                    buttonComponent.onClick.AddListener(() => levelUpOnClick(ps,skillButton.GetComponent<toolTip>(),lTxt,psTxt));

            }
            y=baseY;
            baseX+=width*1.3f;
            foreach(PassiveSkill ps in skillTree.getRegion2()){
                    GameObject skillButton = new GameObject("skill");
                    RawImage imageComponent = skillButton.AddComponent<RawImage>();
                    skillButton.GetComponent<RectTransform>().SetParent(canv.transform);
                    skillButton.GetComponent<RectTransform>().sizeDelta = new Vector2(width,height);
                    skillButton.GetComponent<RectTransform>().localPosition = new Vector3(baseX,y,0);
                    y-=height*1.2f+textHeight*2.2f;
                    imageComponent.texture = ps.returnIcon();
                    Button buttonComponent = skillButton.AddComponent<Button>();
                    BoxCollider collider = skillButton.AddComponent<BoxCollider>();
                    collider.isTrigger = true;
                    collider.size = new Vector3(width, height, 0);
                    skillButton.AddComponent<toolTip>();
                    skillButton.GetComponent<toolTip>().message=ps.getProperDesc();
                    setData(skillButton);
                    GameObject nameTxt = new GameObject("name");
                    TextMeshProUGUI nTxt=nameTxt.AddComponent<TextMeshProUGUI>();
                    nTxt.alignment = TextAlignmentOptions.Center;
                    nameTxt.transform.SetParent(skillButton.transform);
                    nTxt.fontSize = 5;
                    nTxt.fontSizeMin=10;
                    nTxt.enableAutoSizing=true;
                    nTxt.font = tooltipManager._instance.globalFont;
                    nTxt.alignment = TextAlignmentOptions.Center;
                    nTxt.text = ps.getName();
                    nameTxt.GetComponent<RectTransform>().sizeDelta=new Vector2(width,textHeight);
                    nameTxt.GetComponent<RectTransform>().localPosition=new Vector3(0,height/2+textHeight/2,0);
                    GameObject lvlText = new GameObject("level");
                    TextMeshProUGUI lTxt=lvlText.AddComponent<TextMeshProUGUI>();
                    lTxt.alignment = TextAlignmentOptions.Center;
                    lvlText.transform.SetParent(skillButton.transform);
                    lTxt.fontSizeMin=10;
                    lTxt.enableAutoSizing=true;
                    lTxt.font = tooltipManager._instance.globalFont;
                    lTxt.alignment = TextAlignmentOptions.Center;
                    lTxt.text = ps.getLevel();
                    lvlText.GetComponent<RectTransform>().sizeDelta=new Vector2(width,textHeight);
                    lvlText.GetComponent<RectTransform>().localPosition=new Vector3(0,-height/2-textHeight/2,0);
                    buttonComponent.onClick.AddListener(() => levelUpOnClick(ps,skillButton.GetComponent<toolTip>(),lTxt,psTxt));
            }
            y=baseY;
            baseX+=width*1.3f;
            foreach(PassiveSkill ps in skillTree.getRegion3()){
                    GameObject skillButton = new GameObject("skill");
                    RawImage imageComponent = skillButton.AddComponent<RawImage>();
                    skillButton.GetComponent<RectTransform>().SetParent(canv.transform);
                    skillButton.GetComponent<RectTransform>().sizeDelta = new Vector2(width,height);
                    skillButton.GetComponent<RectTransform>().localPosition = new Vector3(baseX,y,0);
                    y-=height*1.2f+textHeight*2.2f;
                    imageComponent.texture = ps.returnIcon();
                    Button buttonComponent = skillButton.AddComponent<Button>();
                    BoxCollider collider = skillButton.AddComponent<BoxCollider>();
                    collider.isTrigger = true;
                    collider.size = new Vector3(width, height, 0);
                    skillButton.AddComponent<toolTip>();
                    skillButton.GetComponent<toolTip>().message=ps.getProperDesc();
                    setData(skillButton);
                    GameObject nameTxt = new GameObject("name");
                    TextMeshProUGUI nTxt=nameTxt.AddComponent<TextMeshProUGUI>();
                    nTxt.alignment = TextAlignmentOptions.Center;
                    nameTxt.transform.SetParent(skillButton.transform);
                    nTxt.fontSize = 5;
                    nTxt.fontSizeMin=10;
                    nTxt.enableAutoSizing=true;
                    nTxt.font = tooltipManager._instance.globalFont;
                    nTxt.alignment = TextAlignmentOptions.Center;
                    nTxt.text = ps.getName();
                    nameTxt.GetComponent<RectTransform>().sizeDelta=new Vector2(width,textHeight);
                    nameTxt.GetComponent<RectTransform>().localPosition=new Vector3(0,height/2+textHeight/2,0);
                    GameObject lvlText = new GameObject("level");
                    TextMeshProUGUI lTxt=lvlText.AddComponent<TextMeshProUGUI>();
                    lTxt.alignment = TextAlignmentOptions.Center;
                    lvlText.transform.SetParent(skillButton.transform);
                    lTxt.fontSize = 5;
                    lTxt.fontSizeMin=10;
                    lTxt.enableAutoSizing=true;
                    lTxt.font = tooltipManager._instance.globalFont;
                    lTxt.alignment = TextAlignmentOptions.Center;
                    lTxt.text = ps.getLevel();
                    lvlText.GetComponent<RectTransform>().sizeDelta=new Vector2(width,textHeight);
                    lvlText.GetComponent<RectTransform>().localPosition=new Vector3(0,-height/2-textHeight/2,0);
                    buttonComponent.onClick.AddListener(() => levelUpOnClick(ps,skillButton.GetComponent<toolTip>(),lTxt,psTxt));

                    
            }

    }
    public bool isAnyPointUnspent(){
        if(skillTree.getSkillPoints()>0) return true;
        return false;
    }
    public int getLevel(){
        return level;
    }
    public string getEquipmentDesc(int number){
            if(equipment[number]==null){
                return "Slot is empty";
            }
            else return equipment[number].getDescription();
    }
    public Texture2D getEquipmentIcon(int number){
        if(equipment[number]==null){
                return Resources.Load<Texture2D>("ph");
        }
        else return equipment[number].getIcon();
    }
    public void equipFirstItem(Equipment itemToEquip){
        if(equipment[0]!=null)equipment[0].detach();
        equipment[0]=itemToEquip;
        equipment[0].attach(this); 
    }
    public void equipSecondItem(Equipment itemToEquip){
        if(equipment[1]!=null)equipment[1].detach(); 
        equipment[1]=itemToEquip;
        equipment[1].attach(this); 
    }
    public void equipThirdItem(Equipment itemToEquip){
        if(equipment[2]!=null)equipment[2].detach(); 
        equipment[2]=itemToEquip;
        equipment[2].attach(this); 
    }
    public bool equipItem(Equipment itemToEquip){
        foreach(Equipment eq in equipment){
            if(eq==itemToEquip) return false;
        }
        for(int i=0;i<equipment.Length;i++){
            if(equipment[i]==null){
                equipment[i]=itemToEquip;
                equipment[i].attach(this); 
                GlobalVariables.Instance.refreshPilotsInHangar=true;
                return true;
            }
        }
        
        equipment[0].detach();
        equipment[0]=itemToEquip;
        equipment[0].attach(this);
        GlobalVariables.Instance.refreshPilotsInHangar=true;
        return true;
        
    }
    public bool isEquipmentAssigned(int number){
        if(equipment[number]==null) return false;
        return true;
    }
    public void forceRemove(int number){
        if(number<0||number>2) return;
        if(equipment[number]==null) return;
        equipment[number].detach();
    }
    public void removeEquipment(Equipment item){
        for(int i=0;i<equipment.Length;i++){
            if(item==equipment[i]) {
                equipment[i]=null;
            }
        }
    }
    public void bindEquipment(){
        for(int i=0;i<equipment.Length;i++){
            if(equipment[i]!=null){
                GlobalVariables.Instance.equipment.Remove(equipment[i]);
            }
        }
        equipmentBinded=true;
    }
    public void unbindEquipment(){
        if(!equipmentBinded) return;
        for(int i=0;i<equipment.Length;i++){
            if(equipment[i]!=null){
                GlobalVariables.Instance.equipment.Add(equipment[i]);
            }
        }
        equipmentBinded=false;
    }
    public int getPower(){
        float critChanceNormalized= critChance > 100 ? 100 : critChance;
        float power=attack*(1-critChanceNormalized/100f)+attack*(critChanceNormalized/100f)*(critMultiplier/100f)+defence+health/8+speed*10;
        power+=(movementPoints-baseMovementPoints)*30;
        int powerMultiplier=0;

        foreach(Ability ab in abilities){
            power+=ab.getPower(shipClass);
            if(ab.isHealingAbility())powerMultiplier++;
        }
        if(shipClass=="Support") power+=(healingMultiplier-1f)*500*powerMultiplier; // 5 power per 1% over 100
        return (int)power;

    }
    private void setData(GameObject button){
         EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { passiveEnter(); });


            eventTrigger.triggers.Add(entry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) => { passiveExit(); });

            eventTrigger.triggers.Add(exitEntry);

            EventTrigger.Entry downEntry = new EventTrigger.Entry();
            downEntry.eventID=EventTriggerType.PointerDown;
            downEntry.callback.AddListener((data) => { passiveDown(); });

            eventTrigger.triggers.Add(downEntry);

            EventTrigger.Entry upEntry = new EventTrigger.Entry();
            upEntry.eventID=EventTriggerType.PointerUp;
            upEntry.callback.AddListener((data) => { passiveUp(); });

            eventTrigger.triggers.Add(upEntry);
    }
    private void passiveEnter()
    {	CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Clickable);      
    }
    private void passiveExit()
    {
        
            CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Default);
        
    }
    void passiveUp()
    {
        CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Clickable);
    }
    void passiveDown()
    {
        CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Down);
    }
    private void levelUpOnClick(PassiveSkill ps,toolTip tip,TextMeshProUGUI lvlText,TextMeshProUGUI skillPoints){  
        skillTree.levelUp(ps);
        tip.message=ps.getProperDesc();
        tip.forceRefersh();
        lvlText.SetText(ps.getLevel());
        skillPoints.SetText("(Unspent)\nskill points:\n"+skillTree.getSkillPoints().ToString());
        GlobalVariables.Instance.refreshPilotsInHangar=true;
    }
    public void generatePortrait(Canvas canvas,float posX,float posY,float screenDividedByThisNumber=3f,RawImage rawImage=null)
    {
        refreshStats();
        
        // Components Widths
        float width = Screen.width / screenDividedByThisNumber;
        float height = Screen.height / (screenDividedByThisNumber*1.5f);
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
        if(rawImage==null)imageComponent.GetComponent<RectTransform>().SetParent(canvas.transform);
        else imageComponent.GetComponent<RectTransform>().SetParent(rawImage.transform);
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
        nTxt.GetComponent<RectTransform>().sizeDelta = new Vector2(pilotPortrait.GetComponent<RectTransform>().sizeDelta.x*0.9f,50);
        nTxt.font = tooltipManager._instance.globalFont;
        nTxt.alignment = TextAlignmentOptions.Center;
        nTxt.color = Color.white;
        nTxt.text = name+" "+surname;
        nameTxt.GetComponent<RectTransform>().localPosition = new Vector3(0, -portraitHeight / 4, 0);
        // quality
        GameObject qualityTxt = new GameObject("quality");
        TextMeshProUGUI qTxt = qualityTxt.AddComponent<TextMeshProUGUI>();
        qualityTxt.transform.SetParent(pilotPortrait.transform);
        qTxt.fontSize = 17;
        qTxt.font = tooltipManager._instance.globalFont;
        qTxt.alignment = TextAlignmentOptions.Center;
        qTxt.color = GlobalVariables.Instance.bgPilotColors[rarityAsInt];
        qTxt.text = "<b>"+pilotRarity+" pilot</b>";
        qualityTxt.GetComponent<RectTransform>().localPosition = new Vector3(0, +portraitHeight / 4, 0);
        //Ability images + tooltips
        GameObject abilityOne = new GameObject("Ability1");
        GameObject abilityTwo = new GameObject("Ability2");
        GameObject abilityThree = new GameObject("Ability3");
        abilityOne.transform.SetParent(pilot.transform);
        abilityTwo.transform.SetParent(pilot.transform);
        abilityThree.transform.SetParent(pilot.transform);
        RawImage imageOne = abilityOne.AddComponent<RawImage>();
        RawImage imageTwo = abilityTwo.AddComponent<RawImage>();
        RawImage imageThree = abilityThree.AddComponent<RawImage>();
        imageOne.GetComponent<RectTransform>().sizeDelta = new Vector2(abilityWidth, abilityHeight);
        imageTwo.GetComponent<RectTransform>().sizeDelta = new Vector2(abilityWidth, abilityHeight);
        imageThree.GetComponent<RectTransform>().sizeDelta = new Vector2(abilityWidth, abilityHeight);

        imageOne.GetComponent<RectTransform>().localPosition = new Vector3(-width / 2 + abilityWidth / 2, -height / 2 + abilityHeightOffset, 0); ;
        imageTwo.GetComponent<RectTransform>().localPosition = new Vector3(-width / 2 + abilityWidth * 1.7f, -height / 2 + abilityHeightOffset, 0);
        imageThree.GetComponent<RectTransform>().localPosition = new Vector3(-width / 2 + abilityWidth * 2.9f, -height / 2 + abilityHeightOffset, 0);
        BoxCollider colliderOne = abilityOne.AddComponent<BoxCollider>();
        colliderOne.isTrigger = true;
        colliderOne.size = new Vector3(abilityWidth, abilityHeight, 0);
        abilityOne.AddComponent<toolTip>();
        abilityOne.GetComponent<toolTip>().message = abilities[0].getInfo();
        BoxCollider colliderTwo = abilityTwo.AddComponent<BoxCollider>();
        colliderTwo.isTrigger = true;
        colliderTwo.size = new Vector3(abilityWidth, abilityHeight, 0);
        colliderTwo.AddComponent<toolTip>();
        colliderTwo.GetComponent<toolTip>().message = abilities[1].getInfo();
        BoxCollider colliderThree = abilityThree.AddComponent<BoxCollider>();
        colliderThree.isTrigger = true;
        colliderThree.size = new Vector3(abilityWidth, abilityHeight, 0);
        colliderThree.AddComponent<toolTip>();
        colliderThree.GetComponent<toolTip>().message = abilities[2].getInfo();
        imageOne.texture = abilities[0].getIcon();
        imageTwo.texture = abilities[1].getIcon();
        imageThree.texture = abilities[2].getIcon(); 
        

        // lvl
        GameObject lvlTxt = new GameObject("lvl");
        TextMeshProUGUI lTxt = lvlTxt.AddComponent<TextMeshProUGUI>();
        lvlTxt.transform.SetParent(pilot.transform);
        lTxt.fontSize = 14;
        lTxt.font = tooltipManager._instance.globalFont;
        lTxt.alignment = TextAlignmentOptions.Center;
        if(rarityAsInt==4)lTxt.color = Color.black;
        else lTxt.color = Color.white;
        lTxt.text = "lvl.<b>"+level +"</b>";
        if(recruited&&skillTree.getSkillPoints()>0){
            lTxt.text+="(!)";
        }
        lvlTxt.GetComponent<RectTransform>().localPosition = new Vector3(width / 2.5f, height / 2.5f, 0);
        // class
        GameObject clTxt = new GameObject("class");
        TextMeshProUGUI cTxt = clTxt.AddComponent<TextMeshProUGUI>();
        clTxt.transform.SetParent(pilot.transform);
        cTxt.fontSize = 16;
        cTxt.font = tooltipManager._instance.globalFont;
        cTxt.alignment = TextAlignmentOptions.Center;
        if(rarityAsInt==4)cTxt.color = Color.black;
        else cTxt.color = Color.white;
        cTxt.text = shipClass;
        clTxt.GetComponent<RectTransform>().localPosition = new Vector3(width / 7, height / 2.5f, 0);
        // stats
        GameObject statsTxt = new GameObject("stats");
        TextMeshProUGUI sTxt = statsTxt.AddComponent<TextMeshProUGUI>();
        statsTxt.transform.SetParent(pilot.transform);
        sTxt.fontSize = 13;
        
        sTxt.alignment = TextAlignmentOptions.Center;
        if(rarityAsInt==3)sTxt.color = Color.white;
        else sTxt.color = Color.black;
        sTxt.text = recruited ? getStats(true) : getStats();
        statsTxt.GetComponent<RectTransform>().localPosition = new Vector3(width / 6, -height / 10, 0);
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
            meTxt.fontSize = 14;
            meTxt.alignment = TextAlignmentOptions.Center;
            if(rarityAsInt==4) meTxt.color = Color.black;
            else meTxt.color = Color.white;
            meTxt.font = tooltipManager._instance.globalFont;
            meTxt.text = metalPrice.ToString();
            metTxt.GetComponent<RectTransform>().localPosition = new Vector3(width / 3 + resourceWidth * 1.3f, height / 4 - resourceHeight * 0.5f, 0);
            GameObject titTxt = new GameObject("pricetxt");
            TextMeshProUGUI tiTxt = titTxt.AddComponent<TextMeshProUGUI>();
            titTxt.transform.SetParent(pilot.transform);
            tiTxt.fontSize = 14;
            tiTxt.font = tooltipManager._instance.globalFont;
            tiTxt.alignment = TextAlignmentOptions.Center;
            if(rarityAsInt==4)tiTxt.color = Color.black;
            else tiTxt.color = Color.white;
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

            EventTrigger.Entry downEntry = new EventTrigger.Entry();
            downEntry.eventID=EventTriggerType.PointerDown;
            downEntry.callback.AddListener((data) => { OnMouseDown(buttonImage); });

            eventTrigger.triggers.Add(downEntry);

            EventTrigger.Entry upEntry = new EventTrigger.Entry();
            upEntry.eventID=EventTriggerType.PointerUp;
            upEntry.callback.AddListener((data) => { OnMouseUp(buttonImage); });

            eventTrigger.triggers.Add(upEntry);


        }
        else{
            Button button = pilot.AddComponent<Button>();
            pilot.GetComponent<Button>().onClick.AddListener(onClick);
        }
    }
    void OnMouseUp(RawImage buttonImage){
        if (buttonImage.color != Color.red) CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Clickable);
    }
    void OnMouseDown(RawImage buttonImage){
        if (buttonImage.color != Color.red) CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Down);
    }
    private void OnButtonClick()
    {
        if (GlobalVariables.Instance.listOfResources[2].check(metalPrice) && GlobalVariables.Instance.listOfResources[3].check(titanPrice))
        {

            id=GlobalVariables.Instance.idCounter++;

            GlobalVariables.Instance.recruitedPilots.Add(this);
            GlobalVariables.Instance.listOfResources[2].spend(metalPrice);
            GlobalVariables.Instance.listOfResources[3].spend(titanPrice);
            GlobalVariables.Instance.pilotsInPool.Remove(this);
            GlobalVariables.Instance.updateAether = true;
            GlobalVariables.Instance.updateGameStatus=true;

            recruited=true;
        }
        else
        {
            Debug.Log("Cannot purchase ( This message should never appear )");
        }


    }
    private void onClick(){
        if(GlobalVariables.Instance.pilotId!=this){
            GlobalVariables.Instance.chosenPilot=true;
            GlobalVariables.Instance.pilotId=this;
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
        var gv = GlobalVariables.Instance;
        float totalAttackIncrease=gv.increasedTotalAttackOfShips;
        float baseAttackIncrease=gv.increasedBaseAttackOfShips;
        int flatAttackIncrease=0;
        float totalDefenceIncrease=gv.increasedTotalDefenceOfShips;
        float baseDefenceIncrease=gv.increasedBaseDefenceOfShips;
        int flatDefenceIncrease=0;
        float totalHealthIncrease=1f;
        float baseHealthIncrease=1f;
        int flatHealthIncrease=0;
        float totalSpeedIncrease=gv.increasedBaseSpeedOfShips;
        float baseSpeedIncrease=gv.increasedBaseSpeedOfShips;
        int flatSpeedIncrease=0;
        movementPoints=0;
        healingMultiplier=1f;
        critChance=0;
        critMultiplier=0;

        string stats="";
        for(int i=0;i<equipment.Length;i++){
            if(equipment[i]!=null){
                if(stats!="")stats+=";";
                stats+=equipment[i].getStats();

            }
        }
        
        foreach(PassiveSkill ps in skillTree.getTree()){
             if(ps.getStats()!=""){
                if(stats!="")stats+=";";
                stats+=ps.getStats();
             }
        }
        string [] data=stats.Split(";");
            for(int i=0;i<data.Length;i+=3){
                if(data.Length<3) break;
                int value = int.Parse(data[i+2]);
                if(data[i]=="attack"){
                    if(data[i+1]=="flat") flatAttackIncrease+=value;
                    if(data[i+1]=="base") baseAttackIncrease+=value/100f;
                    if(data[i+1]=="percent") totalAttackIncrease+=value/100f;

                }
                else if(data[i]=="defence"){
                    if(data[i+1]=="flat") flatDefenceIncrease+=value;
                    if(data[i+1]=="base") baseDefenceIncrease+=value/100f;
                    if(data[i+1]=="percent") totalDefenceIncrease+=value/100f;
                }
                else if(data[i]=="speed"){
                    if(data[i+1]=="flat") flatSpeedIncrease+=value;
                    if(data[i+1]=="base") baseSpeedIncrease+=value/100f;
                    if(data[i+1]=="percent") totalSpeedIncrease+=value/100f;
                }
                else if(data[i]=="health"){
                    if(data[i+1]=="flat") flatHealthIncrease+=value;
                    if(data[i+1]=="base") baseHealthIncrease+=value/100f;
                    if(data[i+1]=="percent") totalHealthIncrease+=value/100f;
                }
                else if(data[i]=="critChance"){
                    critChance+=value;
                }
                else if(data[i]=="critMultiplier"){
                    critMultiplier+=value;
                }
                else if(data[i]=="healing"){
                    healingMultiplier+=value/100f;
                }
                else if(data[i]=="movementPoints"){
                    movementPoints+=value;
                }

            }
            

        










        critChance += (int)baseCritChance;
        critMultiplier += (int)baseCritMultiplier;
        defence = (int)(((baseDefence*baseDefenceIncrease)+flatDefenceIncrease)*totalDefenceIncrease);
        attack = (int)(((baseAttack*baseAttackIncrease)+flatAttackIncrease)*totalAttackIncrease);
        speed = (int)(((baseSpeed*baseSpeedIncrease)+flatSpeedIncrease)*totalSpeedIncrease);
        health = (int)(((baseHealth*baseHealthIncrease)+flatHealthIncrease)*totalHealthIncrease);
        movementPoints += baseMovementPoints;
    }
    public string getStats(bool advanced=false)
    {   
        
        if(!advanced) return $"Attack:{(int)baseAttack}\nDefence:{(int)baseDefence}\nSpeed:{(int)baseSpeed}\nHealth:{(int)baseHealth}\nCrit Chance:{baseCritChance}%\nCrit Multiplier:{baseCritMultiplier}%\nMovement points:{baseMovementPoints}";
        return $"Attack:<b>{attack}</b>({(int)baseAttack}+{attack-(int)baseAttack})\nDefence:<b>{defence}</b>({(int)baseDefence}+{defence-(int)baseDefence})\nSpeed:<b>{speed}</b>({(int)baseSpeed}+{speed-(int)baseSpeed})\nHealth:<b>{health}</b>({(int)baseHealth}+{health-(int)baseHealth})\nCrit Chance:<b>{critChance}%</b>({baseCritChance}%+{critChance-baseCritChance}%)\nCrit Multiplier:<b>{critMultiplier}%</b>({baseCritMultiplier}%+{critMultiplier-baseCritMultiplier}%)\nMovement points:<b>{movementPoints}</b>({baseMovementPoints}+{movementPoints-baseMovementPoints})"+
        $"\n\n<b>Experience: {exp}/{requiredExp} </b>";
        
    }
    private void mouseEnter(RawImage buttonImage)
    {
        if (buttonImage.color != Color.red){
        buttonImage.color = GlobalVariables.Instance.baseSelectedButtonColor;
        CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Clickable);
        }
            
            
    }
    private void mouseExit(RawImage buttonImage)
    {
        if (buttonImage.color != Color.red) {
            buttonImage.color = Color.black;
            CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Default);
        }
    }

}
