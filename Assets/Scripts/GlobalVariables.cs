using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    public bool isTechnologyGenerated = false;
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
    public bool refreshPilotsInHangar=false;
    public int encreDraftNullsPrice;
    public int encreDraftEncrePrice;
    public bool chosenPilot;
    public Pilot pilotId;
    public int idCounter;
    public int hyperdriveQuamePrice;
    public int hyperdriveMetalPrice;
    GlobalVariables()
    {
        listOfResources = new List<Resource>();
        technologies = new List<Technology>();
        listOfResources.Add(new Resource(1, "Nulls","Basic currency",400000,"Nulls",1000));
        listOfResources.Add(new Resource(2, "Quame","Special mineral used for building hyper drives",100,"Quame",0));
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
        
        idCounter=0;
        successfulExplorations=0;
        explored=new List<GenerateSystem>();
        currentlyExploring=new List<GenerateSystem>();
    }
    public void victoryScreen(){
        // TODO 
    }
    public void checkIfGameIsOver(){
        if(hyperdrives<1&&!listOfResources[1].check(hyperdriveQuamePrice)){
            // GAMEOVER TODO SCREEN
        }
    }
    public void addToWaitingList(Pilot p){
        if(recruitedPilots.Remove(p))waitingToBeAssignedToExploration.Add(p);
    }
    public void removeFromWaitingList(Pilot p){
        if(waitingToBeAssignedToExploration.Remove(p))recruitedPilots.Add(p);

    }
    public readonly int[] pilotStats = { 600, 630, 660, 700, 750 }; // base stats for each rarity
    public readonly string[] pilotRarities = { "Ordinary", "Advanced", "Veteran", "Renowned", "Exalted" };
    public readonly string[] missionDifficulty = { "Very low", "Low", "Medium", "High", "Lethal" };
    public readonly int[] requiredExpPerLevel = {400,700,1500,2000,3000,4000,6000,8000,10000,15000,0};

    public readonly int[] baseChanceToFindHabitablePlanet = {-10,0,0,3,15};
    public readonly int[] chanceToFindHabitablePlanetPerExploredPlanet = {1,1,2,3,4};
    public readonly float[] expQuameMultiplier = {0.1f,0.2f,0.5f,1,2};
    public readonly float[] expNullsMultiplier = {1,1,1.5f,1.5f,3};
    public readonly float[] expTitanMultiplier = {0.3f,0.5f,1,2,5};
    public readonly float[] expEncreMultiplier = {0.3f,0.5f,1,2,5};
    public readonly float[] expMetalMultiplier = {1,1,1.5f,1.5f,3};

    public readonly int[] missionLevelEstimatedPowerValue = { 600, 1200, 2000, 4000, 8000 };

    
    public int currentTechnology=-1; // -1 means no technology
    public bool changeTechnology=false;
    public int currentInfo=0;
    public bool changeInfoHighlight=false;

    public Color[] bgPilotColors = { new Color(0.6320754f, 0.6320754f, 0.6320754f), new Color(0.253382f, 0.735849f, 0.446885f),new Color(0.6f,0.7050772f,0.8784314f),
    new Color(0.6197724f,0f,1f),new Color(0.8773585f,0.8649189f,0.2772784f)};
    public Color baseSelectedButtonColor = new Color(0.4986246f, 0.2018512f, 0.5283019f);
    public bool refreshAvailableSystems=true;

    public const int AVAILABLE_CLASSES=5;
    public  string[] CLASS_NAMES = { "A_class", "K_class","M_class","G_class","F_class" }; 
    
    public int[] AVAILABLE_VARIANTS={3,3,3,3,3};

    public List<GenerateSystem> availableSpaceSystems= new List<GenerateSystem>();
    public List<GenerateSystem> currentlyExploring;
    public List<GenerateSystem> explored;
    public int successfulExplorations;
    

    public int month = 2;
    public int year = 541;
    public float researchRate = 1.0f;
    public float increasedBaseAttackOfShips = 1.0f;
    public float increasedBaseDefenceOfShips = 1.0f;
    public float increasedBaseSpeedOfShips = 1.0f;
    public float increasedTotalAttackOfShips = 1.0f;
    public float increasedTotalDefenceOfShips = 1.0f;
    public float increasedTotalSpeedOfShips = 1.0f;
    public int unlockedSystemAlternatives = 3;
    public float pilotExpMultiplier = 1.0f;
    public float hyperspaceTravelSpeedModifier= 1.0f;
    public int startingLevelOfPilots = 1;
    public int maxLevel=11;
    public float rarityMultiplier= 1.0f;

    public int hyperdrives;


    public string getMonth(){
        if(month<10){
            return "0"+month.ToString();
        }
        return month.ToString();
    }
    public int refreshExplorationTime=36;
    private int refreshExplorationBaseTime=36;
    public void skipTime(int months){
        refreshExplorationTime-=months;
        if(refreshExplorationTime<=0){
            refreshExplorationTime=refreshExplorationBaseTime;
            refreshAvailableSystems=true;
        }
        month+=months;
        if(month>12){year+=month/12;month=month%12;}
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
    public bool updateGameStatus;
}
