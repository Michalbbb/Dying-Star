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

    public List<Pilot> pilotsInPool;
    public bool updateAether;

    public bool firstRollDone;
    GlobalVariables()
    {
        listOfResources = new List<Resource>();
        technologies = new List<Technology>();
        listOfResources.Add(new Resource(1, "Nulls","Basic currency",400000,"Nulls",50000));
        listOfResources.Add(new Resource(2, "Quame","Special mineral used for building hyper drives",0,"Quame",0));
        listOfResources.Add(new Resource(3, "Metals","Basic currency for building ships",4000,"Metals",200));
        listOfResources.Add(new Resource(4, "Titan v2","Advanced currency for building ships",1000,"Titan",50));
        listOfResources.Add(new Resource(5, "Encre","Special material for building ships",0,"Encre",0));
        recruitedPilots= new List<Pilot>();
        pilotsInPool = new List<Pilot>();
        updateAether = false;
        firstRollDone = false;
    }
    public readonly int[] pilotStats = { 600, 630, 660, 700, 750 }; // base stats for each rarity
    public readonly string[] pilotRarities = { "Ordinary", "Advanced", "Veteran", "Renowned", "Exalted" };
    
    public int currentTechnology=-1; // -1 means no technology
    public bool changeTechnology=false;
    public int currentInfo=0;
    public bool changeInfoHighlight=false;

    public Color[] bgPilotColors = { new Color(0.6320754f, 0.6320754f, 0.6320754f), new Color(0.6000801f, 0.8773585f, 0.6554767f),new Color(0.6f,0.7050772f,0.8784314f),
    new Color(0.6197724f,0f,1f),new Color(0.8773585f,0.8649189f,0.2772784f)};
    public Color baseSelectedButtonColor = new Color(0.4986246f, 0.2018512f, 0.5283019f);
    public bool refreshAvailableSystems=true;

    public const int AVAILABLE_CLASSES=2;
    public  string[] CLASS_NAMES = { "A_class", "K_class" };
    
    public int[] AVAILABLE_VARIANTS={2,3};

    public List<GenerateSystem> availableSpaceSystems= new List<GenerateSystem>();
    public GenerateSystem chosenSpaceSystem;
    

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
    public float rarityMultiplier= 1.0f;
    public float shipUpkeepMultiplier = 1.0f;

    public string getMonth(){
        if(month<10){
            return "0"+month.ToString();
        }
        return month.ToString();
    }
    public void skipTime(int months){
        month+=months;
        if(month>12){year+=month/12;month=month%12;}
        foreach(Resource res in listOfResources){
            if(res!=null)res.skipTime(months);
        }
        if(currentTechnology != -1){
            technologies[currentTechnology].skipTime(months);
            if (technologies[currentTechnology].completed) currentTechnology = -1;
        }
        updateGameStatus=true;
    }   
    public bool updateGameStatus;
}
