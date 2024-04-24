using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public sealed class GlobalVariables {
private static GlobalVariables instance = null;
    private static readonly object padlock = new object();
    public List<Resource> listOfResources;

    GlobalVariables()
    {
        listOfResources = new List<Resource>();
        listOfResources.Add(new Resource(1, "Nulls","Basic currency",400000,"Nulls",50000));
        listOfResources.Add(new Resource(2, "Quame","Special mineral used for building hyper drives",0,"Quame",0));
        listOfResources.Add(new Resource(3, "Metals","Basic currency for building ships",4000,"Metals",200));
        listOfResources.Add(new Resource(4, "Titan v2","Advanced currency for building ships",1000,"Titan",50));
        listOfResources.Add(new Resource(5, "Encre","Special material for building ships",0,"Encre",0));
        
    }

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
    public Technology currentTech=null;
    public int currentTechnology=0; // 0 means no technology
    public bool changeTechnology=false;
    public int currentInfo=0;
    public bool changeInfoHighlight=false;

    public bool refreshAvailableSystems=true;

    public const int AVAILABLE_CLASSES=2;
    public  string[] CLASS_NAMES = { "A_class", "K_class" };
    
    public int[] AVAILABLE_VARIANTS={2,3};

    public List<GenerateSystem> availableSpaceSystems= new List<GenerateSystem>();
    public GenerateSystem chosenSpaceSystem;
    public int unlockedSystemAlternatives=3;

    public int month = 2;
    public int year = 541;

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
        if(currentTech!=null){
            currentTech.skipTime(months);
            if(currentTech.completed) currentTech=null;
        }
        updateGameStatus=true;
    }   
    public bool updateGameStatus;
}
