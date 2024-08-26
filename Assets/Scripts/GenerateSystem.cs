using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenerateSystem 
{
    Texture2D image;
    string info;
    string systemName;
    string description;
    string threatLevel;
    int threat;

    int explorationTime;
    int baseExplorationTime;
    float currentlyExplored;

    int rewardNull;
    int rewardMetal;
    int rewardTitan;
    int rewardEncre;
    int rewardQuame;
    public int startDate=0;
    public int endDate=0;
    int estimatedPower;
    int power;
    string report;
    public bool isExplorationActive;
    public bool isExplorationOver;
    public bool isSuccess;
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
        
        systemName=name;
        float threatLevelFloat = ((GlobalVariables.Instance.year-541)/500f+0.15f)*UnityEngine.Random.Range(0,10);
        int threatLevelRoll = (int)threatLevelFloat;
        if(threatLevelRoll>4)threatLevelRoll=4;
        var gv = GlobalVariables.Instance;
        threatLevel=gv.missionDifficulty[threatLevelRoll];
        explorationTime=UnityEngine.Random.Range(60,361);
        baseExplorationTime=explorationTime;
        currentlyExplored=0;
        assignedPilots=new List<Pilot>();
        isExplorationActive=false;
        isExplorationOver=false;
        rewardNull=(int)(UnityEngine.Random.Range(20000,80001)*gv.expNullsMultiplier[threatLevelRoll]);
        rewardMetal=(int)(UnityEngine.Random.Range(800,2001)*gv.expMetalMultiplier[threatLevelRoll]);
        rewardTitan=(int)(UnityEngine.Random.Range(200,401)*gv.expTitanMultiplier[threatLevelRoll]);
        rewardEncre=(int)(UnityEngine.Random.Range(200,401)*gv.expEncreMultiplier[threatLevelRoll]);
        rewardQuame=(int)(UnityEngine.Random.Range(100,201)*gv.expQuameMultiplier[threatLevelRoll]);
        report="<align=\"center\"><color=#3399ff>[Event log]</color></align>\n";
        threat=threatLevelRoll;
        estimatedPower=GlobalVariables.Instance.missionLevelEstimatedPowerValue[threat];
        int minPower=(int)(estimatedPower/2f);
        int maxPower=(int)(estimatedPower*1.5f)+1;
        power=UnityEngine.Random.Range(minPower,maxPower);
        
        description=generateDescription();
        string color="green";
        if(threatLevelRoll==1)color="white";
        if(threatLevelRoll==2)color="yellow";
        if(threatLevelRoll==3)color="orange";
        if(threatLevelRoll==4)color="red";
        info=$"<color={color}>{name}({GlobalVariables.Instance.CLASS_NAMES[classType]})</color>";
        
    }
    public bool launchExploration(){
        if(GlobalVariables.Instance.waitingToBeAssignedToExploration.Count==0)return false;
        if(GlobalVariables.Instance.hyperdrives<GlobalVariables.Instance.waitingToBeAssignedToExploration.Count) return false;
        assignedPilots.AddRange(GlobalVariables.Instance.waitingToBeAssignedToExploration);
        string pilotsSend="";
        foreach(Pilot p in assignedPilots){
            p.bindEquipment();
            pilotsSend+="["+p.getFullName()+"]";
        }
        startDate=GlobalVariables.Instance.year*12+GlobalVariables.Instance.month;
        GlobalVariables.Instance.waitingToBeAssignedToExploration.Clear();
        GlobalVariables.Instance.hyperdrives-=assignedPilots.Count;
        isExplorationActive=true;
        GlobalVariables.Instance.currentlyExploring.Add(this);
        
        string form;
        if(assignedPilots.Count<2)form="is";
        else form="are";
        report+=GlobalVariables.Instance.getDateAsString()+$" Exploration has been launched. {pilotsSend} {form} exploring {systemName}.\n";
        float remainingTime=(baseExplorationTime-currentlyExplored)/GlobalVariables.Instance.hyperspaceTravelSpeedModifier;
        baseExplorationTime=(int)remainingTime;
        return true;
    }
    public int getRewards(int number){
        if(number==0) return rewardNull;
        if(number==1) return rewardMetal;
        if(number==2) return rewardTitan;
        if(number==3) return rewardEncre;
        if(number==4) return rewardQuame;
        return 0;
        
    }
    public string getDescription(){ return description;}
    string generateDescription(){
        string returnDes=$"[Threat level - {threatLevel}]\n";
        returnDes+=$"Solar system we call {systemName} appears to be ";
        if(threat==0){
            returnDes+="terrible environment for life as our sensor pick insignificant amount of signals from alien lifeforms.\nChance to find habitable planet is estimated to be <color=yellow>[Abyssmal]</color>";
        }
        if(threat==1){
            returnDes+="poor environment for life as our sensor don't pick many signals of alien lifeforms.\nChance to find habitable planet is estimated to be <color=yellow>[Low]</color>";
            
        }
        if(threat==2){
            returnDes+="decent environment for life as our sensor pick many signals of alien lifeforms.\nChance to find habitable planet is estimated to be <color=yellow>[Medium]</color>";

        }
        if(threat==3){
            returnDes+="great environment for life as our sensor pick high amount of signals of alien lifeforms.\n<color=orange>This exploration will be challenging.</color>\nChance to find habitable planet is estimated to be <color=yellow>[High]</color>";

        }
        if(threat==4){
            returnDes+="exceptional environment for life as our sensor pick overwhelming amount of signals from alien lifeforms.\n<color=red>This exploration may be lethal even for experienced team.DO NOT IGNORE THAT WARNING.</color>\nChance to find habitable planet is estimated to be <color=yellow>[Very high]</color>";
        }

        returnDes+=$"\n\nEstimated enemy power: {estimatedPower/2} - {estimatedPower*1.5f}.\n<color=yellow>Information: Enemy power will be revealed once exploration will end.</color>";

        return returnDes;



    }
    int totalPowerOfPilots(){
        if(assignedPilots.Count<1) return 0;
        int sum=0;
        float [] harmonyBonus= {1f,1.03f,1.05f,1.1f};
        int [] weight = {1,10,100,1000};
        int uniqueClasses=0;
        int sumOfWeight=0;
        foreach(Pilot p in assignedPilots)
        {
            sum+=p.getPower();
            sumOfWeight+=weight[p.getPilotType()];
        }
        while(sumOfWeight>0)
        {
                if(sumOfWeight%10!=0)uniqueClasses++;
            sumOfWeight/=10;
        }       

        sum=(int)(sum*harmonyBonus[uniqueClasses-1]);
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
        GlobalVariables.Instance.successfulExplorations++;
        if(GlobalVariables.Instance.successfulExplorations>=3&&!GlobalVariables.Instance.technologies[8].isUnlocked()){
            GlobalVariables.Instance.technologies[8].unlock();
            report+=GlobalVariables.Instance.getDateAsString()+"We managed to successfuly explore 3 solar systems. We can now <color=green>Update training program</color> which will result in better quality of pilots in future. Go to lab for more informations.\n";
        }
        if(!GlobalVariables.Instance.technologies[5].isUnlocked()){
            if(UnityEngine.Random.Range(0,100)<15){
                GlobalVariables.Instance.technologies[5].unlock();
                report+=GlobalVariables.Instance.getDateAsString()+"During exploration our pilots found some kind of new metal. Our scientists called it <color=green>Voll</color>. According to them we can use it to further upgrade our ships.\n";

            }
        }
        if(!GlobalVariables.Instance.technologies[4].isUnlocked()){
            if(UnityEngine.Random.Range(0,100)<15){
                GlobalVariables.Instance.technologies[4].unlock();
                report+=GlobalVariables.Instance.getDateAsString()+"This time our pilots managed to bring back great amount of alien bodies. Should we let our scientists <color=green>study them</color>?\n";

            }
        }
        if(!GlobalVariables.Instance.technologies[6].isUnlocked()&&threat>=2){
            if(UnityEngine.Random.Range(0,100)<15){
                GlobalVariables.Instance.technologies[6].unlock();
                report+=GlobalVariables.Instance.getDateAsString()+"Our pilots claims that during travel in hyperspace they found weird cube. Scientists named it <color=green>Ter</color> and they believe we can use it as extermally powerful source of energy if we give them enough time to study it.\n";

            }
        }
        if(!GlobalVariables.Instance.foundAncestorMessage){
            if(UnityEngine.Random.Range(0,100)<10){
                GlobalVariables.Instance.foundAncestorMessage=true;
                SceneManager.LoadScene(GlobalVariables.Instance.hiddenMessageScene);
            }
        }
        report+=GlobalVariables.Instance.getDateAsString()+"<color=green>[Success]</color> Exploration team has returned successfully to hangar.\nHabitable planet was not found in system.";
        var gv = GlobalVariables.Instance.listOfResources;
        gv[0].add(rewardNull);
        gv[1].add(rewardQuame);
        gv[2].add(rewardMetal);
        gv[3].add(rewardTitan);
        gv[4].add(rewardEncre);
        GlobalVariables.Instance.hyperdrives+=assignedPilots.Count;
        foreach(Pilot p in assignedPilots){p.addExp(power);GlobalVariables.Instance.recruitedPilots.Add(p);p.unbindEquipment();}
        assignedPilots.Clear();
        isSuccess=true;
        report+=$"\nWe gained from exploration {rewardNull} [Nulls], {rewardMetal} [Metals], {rewardTitan} [Titan v2], {rewardEncre} [Encre], {rewardQuame} [Quame] ";
        info+="\n<color=green>[SUCCESS]</color>";
    }
    void missionFailure(){
        string names= "";
        foreach(Pilot p in assignedPilots){names+="["+p.getFullName()+"] ";}
        report+=GlobalVariables.Instance.getDateAsString()+$"<color=red>[FAILURE]</color> Our sensors can no longer detect signal from exploration team. Exploration is marked as ended. We lost those pilots (including ship and equipment): {names}\n";
        report+=$"We lost {assignedPilots.Count} hyperdrives.";
        GlobalVariables.Instance.checkIfGameIsOver();
        info+="\n<color=red>[FAILURE]</color>";
        isSuccess=false;
    }
    public int getExplorationTime(){
        float remainingTime=(explorationTime-currentlyExplored)/GlobalVariables.Instance.hyperspaceTravelSpeedModifier;
        return (int)remainingTime;
    }
    public int getBaseExplorationTime(){
        
        return baseExplorationTime;
    }
    public void skipTime(int months){
        if(!isExplorationActive) return;
        currentlyExplored+=months*GlobalVariables.Instance.hyperspaceTravelSpeedModifier;
        foreach(Pilot p in assignedPilots){p.addExp(months);}
        if(currentlyExplored>=explorationTime){
            isExplorationActive=false;
            isExplorationOver=true;
            resolveBattle();
        }

    }
    public bool isOver(){
        return isExplorationOver;
    }
    public string getReport(){
         return report;
    }
    void resolveBattle(){
        endDate=GlobalVariables.Instance.year*12+GlobalVariables.Instance.month;
        report+=GlobalVariables.Instance.getDateAsString()+$"Enemy power has been confirmed to be <color=yellow>{power}</color>\n";
        GlobalVariables.Instance.currentlyExploring.Remove(this);
        GlobalVariables.Instance.explored.Add(this);
        if(power>totalPowerOfPilots()){
            missionFailure();
        }
        else missionSuccess();


    }
    public Texture2D getImage(){
        return image;
    }
    public string getInfo(){
        if(isExplorationActive) return info+"\n[Exploring]";
        return info;
    }
   
}
public class SystemComparer : IComparer<GenerateSystem>
{
    public int Compare(GenerateSystem x, GenerateSystem y)
    {
        if(x.isExplorationOver&&!y.isExplorationOver) return 1;
        if(x.isExplorationOver&&y.isExplorationOver) return y.endDate-x.endDate; 
        if(x.isExplorationActive&&y.isExplorationActive) return x.getExplorationTime()-y.getExplorationTime();
        
        return -1;
    }
}
