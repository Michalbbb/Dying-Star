using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GenerateSystem 
{
    Texture2D image;
    string info;
    string systemName;
    string description;
    string threatLevel;
    int threat;

    int explorationTime;
    float currentlyExplored;

    int rewardNull;
    int rewardMetal;
    int rewardTitan;
    int rewardEncre;
    int rewardQuame;

    int estimatedPower;
    int power;
    string report;
    public bool isExplorationActive;
    List<Pilot> assignedPilots;

    public GenerateSystem(){
        int classType= UnityEngine.Random.Range(0,GlobalVariables.AVAILABLE_CLASSES);
        int variant=UnityEngine.Random.Range(1,GlobalVariables.Instance.AVAILABLE_VARIANTS[classType]+1);
        image=Resources.Load<Texture2D>($"Space systems/{GlobalVariables.Instance.CLASS_NAMES[classType]}{variant}");
        int firstLetter=UnityEngine.Random.Range(65,91);
        int secondLetter=UnityEngine.Random.Range(65,91);
        int number=UnityEngine.Random.Range(0,100);
        string name="";
        name+=(char)firstLetter;
        name+=(char)secondLetter;
        name+='-';
        name+=number;
        info=$"{name}({GlobalVariables.Instance.CLASS_NAMES[classType]})";
        systemName=name;
        float threatLevelFloat = ((GlobalVariables.Instance.year-541)/500f+0.15f)*UnityEngine.Random.Range(0,10);
        int threatLevelRoll = (int)threatLevelFloat;
        if(threatLevelRoll>4)threatLevelRoll=4;
        var gv = GlobalVariables.Instance;
        threatLevel=gv.missionDifficulty[threatLevelRoll];
        explorationTime=UnityEngine.Random.Range(60,361);
        currentlyExplored=0;
        assignedPilots=new List<Pilot>();
        isExplorationActive=false;
        rewardNull=(int)(UnityEngine.Random.Range(20000,80001)*gv.expNullsMultiplier[threatLevelRoll]);
        rewardMetal=(int)(UnityEngine.Random.Range(800,2001)*gv.expMetalMultiplier[threatLevelRoll]);
        rewardTitan=(int)(UnityEngine.Random.Range(200,401)*gv.expTitanMultiplier[threatLevelRoll]);
        rewardEncre=(int)(UnityEngine.Random.Range(200,401)*gv.expEncreMultiplier[threatLevelRoll]);
        rewardQuame=(int)(UnityEngine.Random.Range(100,201)*gv.expQuameMultiplier[threatLevelRoll]);
        report="";
        threat=threatLevelRoll;
        estimatedPower=GlobalVariables.Instance.missionLevelEstimatedPowerValue[threat];
        int minPower=estimatedPower/2;
        int maxPower=(int)(estimatedPower*1.5f)+1;
        power=UnityEngine.Random.Range(minPower,maxPower);
        description=generateDescription();
    }
    public bool launchExploration(){
        if(GlobalVariables.Instance.waitingToBeAssignedToExploration.Count==0)return false;
        if(GlobalVariables.Instance.hyperdrives<GlobalVariables.Instance.waitingToBeAssignedToExploration.Count) return false;
        assignedPilots.AddRange(GlobalVariables.Instance.waitingToBeAssignedToExploration);
        foreach(Pilot p in assignedPilots){
            p.bindEquipment();
        }
        GlobalVariables.Instance.waitingToBeAssignedToExploration.Clear();
        GlobalVariables.Instance.hyperdrives-=assignedPilots.Count;
        isExplorationActive=true;
        GlobalVariables.Instance.currentlyExploring.Add(this);
        return true;
    }
    public string getDescription(){ return description;}
    string generateDescription(){
        string returnDes=$"[Threat level - {threatLevel}]\n";
        returnDes+=$"Solar system we call {systemName} appears to be ";
        if(threat==0){
            returnDes+="terrible environment for lifeforms as our sensor pick insignificant amount of signals from alien lifeforms.\nChance to find habitable planet is estimated to be <color=yellow>Very low</color>";
        }
        if(threat==1){
            returnDes+="poor environment for lifeforms as our sensor don't pick many signals of alien lifeforms.\nChance to find habitable planet is estimated to be <color=yellow>Low</color>";
            
        }
        if(threat==2){
            returnDes+="decent environment for lifeforms as our sensor pick many signals of alien lifeforms.\nChance to find habitable planet is estimated to be <color=yellow>Medium</color>";

        }
        if(threat==3){
            returnDes+="great environment for lifeforms as our sensor pick high amount of signals of alien lifeforms.\n<color=orange>This exploration will be challenging.</color>\nChance to find habitable planet is estimated to be <color=yellow>High</color>";

        }
        if(threat==4){
            returnDes+="exceptional environment for lifeforms as our sensor pick overwhelming amount of signals from alien lifeforms.\n<color=red>This exploration may be lethal even for experienced team. YOU HAVE BEEN WARNED.</color>\nChance to find habitable planet is estimated to be <color=yellow>Very high</color>";
        }
        returnDes+=$"\n\nEstimated enemy power: {estimatedPower}.\n<color=yellow>Warning: estimated enemy power will be acurate only to some extent.</color>";
        returnDes+="\n<color=green>According to our theory actual power should be between half and  one and half estimated amount.</color>";

        return returnDes;



    }
    int totalPowerOfPilots(){
        if(assignedPilots.Count<1) return 0;
        int sum=0;
        foreach(Pilot p in assignedPilots){
            sum+=p.getPower();
        }

        return sum;
    }
    void missionSuccess(){
        int chanceToFindPlanet=GlobalVariables.Instance.baseChanceToFindHabitablePlanet[threat]+GlobalVariables.Instance.chanceToFindHabitablePlanetPerExploredPlanet[threat]*GlobalVariables.Instance.successfulExplorations;
        if(chanceToFindPlanet>0){
           if( UnityEngine.Random.Range(0,100) < chanceToFindPlanet)
           {
            GlobalVariables.Instance.victoryScreen();
            return;
           }    
        }
        report+="<color=green>[Success]</color> Exploration team has returned successfully to hangar.\nHabitable planet was not found in system.";
        var gv = GlobalVariables.Instance.listOfResources;
        gv[0].add(rewardNull);
        gv[1].add(rewardQuame);
        gv[2].add(rewardMetal);
        gv[3].add(rewardTitan);
        gv[4].add(rewardEncre);
        GlobalVariables.Instance.successfulExplorations++;
        GlobalVariables.Instance.hyperdrives+=assignedPilots.Count;
        foreach(Pilot p in assignedPilots){p.addExp((int)(power*GlobalVariables.Instance.pilotExpMultiplier));GlobalVariables.Instance.recruitedPilots.Add(p);p.unbindEquipment();}
        assignedPilots.Clear();
        report+=$"\nWe gained from exploration {rewardNull} [Nulls] (by selling useless items), {rewardMetal} [Metals], {rewardTitan} [Titan v2], {rewardEncre} [Encre], {rewardQuame} [Quame] ";
        
    }
    void missionFailure(){
        string names= "";
        foreach(Pilot p in assignedPilots){names+="["+p.getFullName()+"] ";}
        report+=$"<color=red>[FAILURE]</color> Our sensors can no longer detect signal from exploration team. Exploration is marked as ended. We lost those pilots (including ship and equipment): {names}\n";
        report+=$"We lost {assignedPilots.Count} hyperdrives.";
        GlobalVariables.Instance.checkIfGameIsOver();
    }
    public int getExplorationTime(){
        float remainingTime=(explorationTime-currentlyExplored)/GlobalVariables.Instance.hyperspaceTravelSpeedModifier;
        return (int)remainingTime;
    }
    public void skipTime(int months){
        if(!isExplorationActive) return;
        currentlyExplored+=months*GlobalVariables.Instance.hyperspaceTravelSpeedModifier;
        foreach(Pilot p in assignedPilots){p.addExp(months);}
        if(currentlyExplored>=explorationTime){
            isExplorationActive=false;
            resolveBattle();
        }

    }
    public string getReport(){
        if(report!="") return report;
        return "Report is not ready yet.";
    }
    void resolveBattle(){
        Debug.Log(power + " "+totalPowerOfPilots());
        if(power>totalPowerOfPilots()){
            missionFailure();
        }
        else missionSuccess();
        GlobalVariables.Instance.currentlyExploring.Remove(this);
        GlobalVariables.Instance.explored.Add(this);

        Debug.Log(getReport());

    }
    public Texture2D getImage(){
        return image;
    }
    public string getInfo(){
        if(isExplorationActive) return info+"\n[Exploring]";
        return info;
    }
   
}
