using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GlobalVariables {
    private static GlobalVariables instance = null;
    private static readonly object padlock = new object();

    public static GlobalVariables Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new GlobalVariables();
                }
                return instance;
            }
        }
    }
    public bool isTechnologyGenerated;
    public List<Resource> listOfResources;
    public List<Technology> technologies;
    public List<Pilot> recruitedPilots;
    public List<Equipment> equipment;
    public List<Pilot> pilotsInPool;
    public List<Pilot> waitingToBeAssignedToExploration;
    public bool updateAether;

    public bool firstRollDone;

    public int freeDrafts;

    public int basicDraftNullsPrice;
    public bool refreshPilotsInHangar;
    public int encreDraftNullsPrice;
    public int encreDraftEncrePrice;
    public bool chosenPilot;
    public Pilot pilotId;
    public int idCounter;
    public int hyperdriveQuamePrice;
    public int hyperdriveMetalPrice;
    public int unlockedSystemAlternatives;
    public string getDateAsString(){
        string m="";
        if(month<10)m+="0";
        m+=month.ToString();
        return "<color=#99ccff>["+m+"."+year+"]</color>";
    }
    GlobalVariables()
    {
        forceRestart();
    }
    public void forceRestart(){
        refreshPilotsInHangar=false;
        isTechnologyGenerated=false;
        month=2;
        year = 541;
        researchRate = 1.0f;
        increasedBaseAttackOfShips = 1.0f;
        increasedBaseDefenceOfShips = 1.0f;
        increasedBaseSpeedOfShips = 1.0f;
        increasedTotalAttackOfShips = 1.0f;
        increasedTotalDefenceOfShips = 1.0f;
        increasedTotalSpeedOfShips = 1.0f;
        pilotExpMultiplier = 1.0f;
        hyperspaceTravelSpeedModifier= 1.0f;
        startingLevelOfPilots = 1;
        rarityMultiplier= 1.0f;
        refreshExplorationTime=36;
        currentTechnology=-1; 
        changeTechnology=false;
        currentInfo=0;
        changeInfoHighlight=false;
        listOfResources = new List<Resource>();
        availableSpaceSystems= new List<GenerateSystem>();
        technologies = new List<Technology>();
        listOfResources.Add(new Resource(1, "Nulls","Basic currency",400000,"Nulls",1000));
        listOfResources.Add(new Resource(2, "Quame","Special mineral used for building hyper drives",100,"Quame",0)); // DEFAULT: 100
        listOfResources.Add(new Resource(3, "Metals","Basic currency for building ships",4000,"Metals",1));
        listOfResources.Add(new Resource(4, "Titan v2","Advanced currency for building ships",400,"Titan",0));
        listOfResources.Add(new Resource(5, "Encre","Special material for better drafts.",1000,"Encre",0));
        recruitedPilots= new List<Pilot>();
        pilotsInPool = new List<Pilot>();
        equipment=new List<Equipment>();
        waitingToBeAssignedToExploration=new List<Pilot>();
        updateAether = false;
        firstRollDone = false;
        freeDrafts=2;
        basicDraftNullsPrice=100000;
        encreDraftNullsPrice=100000;
        encreDraftEncrePrice=1000;
        hyperdrives=3;
        hyperdriveQuamePrice=400;
        hyperdriveMetalPrice=600;
        unlockedSystemAlternatives=3;
        idCounter=0;
        successfulExplorations=0;
        explored=new List<GenerateSystem>();
        currentlyExploring=new List<GenerateSystem>();
    }
    
    public void victoryScreen(){
    SceneManager.LoadScene("Victory");

    }
    public void checkIfGameIsOver(){
        if(hyperdrives<1&&!listOfResources[1].check(hyperdriveQuamePrice)&&currentlyExploring.Count==0){
            SceneManager.LoadScene("Loss");
        }
    }
    public void addToWaitingList(Pilot p){
        if(hyperdrives>waitingToBeAssignedToExploration.Count&&waitingToBeAssignedToExploration.Count<4){
        if(recruitedPilots.Remove(p))waitingToBeAssignedToExploration.Add(p);
        }
    }
    public void removeFromWaitingList(Pilot p){
        if(waitingToBeAssignedToExploration.Remove(p))recruitedPilots.Add(p);

    }
    public readonly int[] pilotStats = { 600, 630, 660, 800, 1000 }; // base stats for each rarity
    public readonly string[] pilotRarities = { "Ordinary", "Advanced", "Veteran", "Renowned", "Exalted" };
    public readonly string[] missionDifficulty = { "Minimal", "Relatively low", "Significant", "Lethal", "Extreme" };
    public readonly int[] requiredExpPerLevel = {400,700,1500,2000,3000,4000,6000,8000,10000,15000,0};

    public readonly int[] baseChanceToFindHabitablePlanet = {-10,0,0,3,15};
    public readonly int[] chanceToFindHabitablePlanetPerExploredPlanet = {1,1,2,3,4};
    public readonly float[] expQuameMultiplier = {0.1f,0.2f,0.5f,1,2};
    public readonly float[] expNullsMultiplier = {1,1,1.5f,1.5f,3};
    public readonly float[] expTitanMultiplier = {0.3f,0.5f,1,2,5};
    public readonly float[] expEncreMultiplier = {0.3f,0.5f,1,2,5};
    public readonly float[] expMetalMultiplier = {1,1,1.5f,1.5f,3};

    public readonly int[] missionLevelEstimatedPowerValue = { 750, 1500, 2500, 5000, 9000 };

    
    public int currentTechnology; // -1 means no technology
    public bool changeTechnology;
    public int currentInfo;
    public bool changeInfoHighlight;

    public Color[] bgPilotColors = { new Color(0.6320754f, 0.6320754f, 0.6320754f), new Color(0.253382f, 0.735849f, 0.446885f),new Color(0.6f,0.7050772f,0.8784314f),
    new Color(0.6197724f,0f,1f),new Color(0.8773585f,0.8649189f,0.2772784f)};
    public Color baseSelectedButtonColor = new Color(0.4986246f, 0.2018512f, 0.5283019f);

    public const int AVAILABLE_CLASSES=5;
    public  string[] CLASS_NAMES = { "A_class", "K_class","M_class","G_class","F_class" }; 
    
    public int[] AVAILABLE_VARIANTS={3,3,3,3,3};

    public List<GenerateSystem> availableSpaceSystems;
    public List<GenerateSystem> currentlyExploring;
    public List<GenerateSystem> explored;
    public int successfulExplorations;
    

    public int month;
    public int year ;
    public float researchRate ;
    public float increasedBaseAttackOfShips ;
    public float increasedBaseDefenceOfShips ;
    public float increasedBaseSpeedOfShips;
    public float increasedTotalAttackOfShips;
    public float increasedTotalDefenceOfShips ;
    public float increasedTotalSpeedOfShips;
    public float pilotExpMultiplier ;
    public float hyperspaceTravelSpeedModifier;
    public int startingLevelOfPilots;
    public float rarityMultiplier;
    public int refreshExplorationTime;

    public int maxLevel=11;
    public int hyperdrives;


    public string getMonth(){
        if(month<10){
            return "0"+month.ToString();
        }
        return month.ToString();
    }
    private int refreshExplorationBaseTime=36;
    public void skipTime(int months){
        refreshExplorationTime-=months;
        if(refreshExplorationTime<=0){
            refreshSpaceSystemList();
        }
        month+=months;
        if(month>12){year+=month/12;month=month%12;}
        if(month==0){year-=1;month+=12;}
        foreach(Resource res in listOfResources){
            if(res!=null)res.skipTime(months);
        }
        if(currentTechnology != -1){
            technologies[currentTechnology].skipTime(months);
            if (technologies[currentTechnology].completed) currentTechnology = -1;
        }
        for(int i=currentlyExploring.Count-1;i>=0;i--){
            currentlyExploring[i].skipTime(months);
        }
        foreach(Pilot p in recruitedPilots){
            p.addExp(months);
        }
        updateGameStatus=true;
    }   
    public void refreshSpaceSystemList(){
            refreshExplorationTime=refreshExplorationBaseTime;
            availableSpaceSystems.Clear();
            for(int i=0;i<unlockedSystemAlternatives;i++){
                GenerateSystem NewSystem=new GenerateSystem();
                availableSpaceSystems.Add(NewSystem);

            }
        
    }
    public bool updateGameStatus;
}
