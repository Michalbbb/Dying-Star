using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using UnityEngine;

public class GeneratePassiveSkill 
{
    private static GeneratePassiveSkill instance = null;
    private static readonly object padlock = new object();
    public static GeneratePassiveSkill Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new GeneratePassiveSkill();
                }
                return instance;
            }
        }
    }
    // 0 - offensive , 1 - defensive , 2 - mixed
    string[] skills = { Path.Combine(Application.streamingAssetsPath,"TextFiles/PassiveSkills/OffensiveSkills.txt"),Path.Combine(Application.streamingAssetsPath,"TextFiles/PassiveSkills/DefensiveSkills.txt"),Path.Combine(Application.streamingAssetsPath,"TextFiles/PassiveSkills/MixedSkills.txt") };
    string[] reSkills = { Path.Combine(Application.streamingAssetsPath,"TextFiles/PassiveSkills/OffensiveSkillsRe.txt"),Path.Combine(Application.streamingAssetsPath,"TextFiles/PassiveSkills/DefensiveSkillsRe.txt"),Path.Combine(Application.streamingAssetsPath,"TextFiles/PassiveSkills/MixedSkillsRe.txt") };
    string[] exSkills = { Path.Combine(Application.streamingAssetsPath,"TextFiles/PassiveSkills/OffensiveSkillsEx.txt"), Path.Combine(Application.streamingAssetsPath,"TextFiles/PassiveSkills/OffensiveSkillsEx.txt"),Path.Combine(Application.streamingAssetsPath,"TextFiles/PassiveSkills/OffensiveSkillsEx.txt") };
    readonly int[] skirmisher = { 0, 0, 0, 0, 2, 2, 2, 2, 1, 1 };
    readonly int[] destroyer = { 0, 0, 0, 0, 0, 0, 2, 2, 1, 1 };
    readonly int[] cruiser = { 1, 1, 1, 1, 1, 1, 2, 2, 2, 0 };
    readonly int[] support = { 2, 2, 2, 2, 2, 2, 1, 1, 0, 0 };
    public SkillTree makeSkillTree(int shipClass,int pilotRarity)
    {
        List<PassiveSkill> region1=new List<PassiveSkill>();
        List<PassiveSkill> region2=new List<PassiveSkill>();
        List<PassiveSkill> region3=new List<PassiveSkill>();
        int[] skillType;
        if (shipClass == 0) skillType = skirmisher;
        else if (shipClass == 1) skillType = destroyer;
        else if (shipClass == 2) skillType = cruiser;
        else skillType = support;
        
        for(int i=0;i<3;i++) // Generate 3 basic passive skills for 3 regions
        {
            
            
                region1.Add(getRandomPassiveSkill(skills[skillType[Random.Range(0, 10)]], region1,default,0+i)); 
                region2.Add(getRandomPassiveSkill(skills[skillType[Random.Range(0, 10)]], region2,default,3+i)); 
                region3.Add(getRandomPassiveSkill(skills[skillType[Random.Range(0, 10)]], region3,default,6+i)); 
                

            
        }
        if (pilotRarity >= 3) // For renowned and exalted rarity pilots try to generate additional passve skill in each region
        {
            int baseChance = 20;
            if (pilotRarity == 4)
            {
                int additionalChance = 66;
                int rand1 = Random.Range(0, 100);
                int rand2 = Random.Range(0, 100);
                int rand3 = Random.Range(0, 100);
                if (rand1 < baseChance)
                {
                    region1.Add(getRandomPassiveSkill(exSkills[skillType[Random.Range(0, 10)]], region1,"exalted",100));
                }
                else if (rand1 < additionalChance)
                {
                    region1.Add(getRandomPassiveSkill(reSkills[skillType[Random.Range(0, 10)]], region1,"renowned",100));
                }
                else { }
 
                if (rand2 < baseChance)
                {
                    region2.Add(getRandomPassiveSkill(exSkills[skillType[Random.Range(0, 10)]], region2,"exalted",200));
                }
                else if (rand2 < additionalChance)
                {
                    region2.Add(getRandomPassiveSkill(reSkills[skillType[Random.Range(0, 10)]], region2,"renowned",200));
                }
                else { }
                if (rand3 < baseChance)
                {
                    region3.Add(getRandomPassiveSkill(exSkills[skillType[Random.Range(0, 10)]], region3,"exalted",300));
                }
                else if (rand3 < additionalChance)
                {
                    region3.Add(getRandomPassiveSkill(reSkills[skillType[Random.Range(0, 10)]], region3,"renowned",300));
                }
                else { }

            }
            else
            {
                if (Random.Range(0, 100) < baseChance) region1.Add(getRandomPassiveSkill(reSkills[skillType[Random.Range(0, 10)]], region1,"renowned",100));                              
                if (Random.Range(0, 100) < baseChance) region2.Add(getRandomPassiveSkill(reSkills[skillType[Random.Range(0, 10)]], region2,"renowned",200));
                if (Random.Range(0, 100) < baseChance) region3.Add(getRandomPassiveSkill(reSkills[skillType[Random.Range(0, 10)]], region3,"renowned",300));
                
            }
        }

        int startingSkillPoints=GlobalVariables.Instance.startingLevelOfPilots-1;
        if(pilotRarity>=3)startingSkillPoints+=pilotRarity-2;
        return new SkillTree(region1, region2, region3,startingSkillPoints);

    }
    public PassiveSkill getRandomPassiveSkill(string file,List<PassiveSkill> skills,string rarity="normal",int id=1)
    {
        if (File.Exists(file))
        {
            string fileText = File.ReadAllText(file);
            string[] data = fileText.Split("\n");
            bool success = false;
            PassiveSkill passiveSkill=new PassiveSkill(id,"Null",rarity);
            while (!success)
            {
                success = true;
                string randomSkill = data[Random.Range(0, data.Length)];
                passiveSkill=new PassiveSkill(id,randomSkill,rarity);
                foreach(PassiveSkill skill in skills)
                {
                    if (skill.getName() == passiveSkill.getName()) success = false;
                }

            }
            return passiveSkill;
        }
        else return new PassiveSkill(id,"Null",rarity);
    }
}
public class PassiveSkill
{
    
    int id;
    string name;
    string desc;
    string baseDesc;
    string icon;
    int currentLevel;
    int maxLevel;
    List<string> effect;
    List<string> type;
    List<List<int>> values;
    string rarity;
    public PassiveSkill(int id,string data,string rarity)
    {
        this.id=id;
        effect= new List<string>();
        type= new List<string>();
        values= new List<List<int>>();
        maxLevel = 0;
        currentLevel = 0;
        if (data == "Null" || data == null)
        {
            name = "Error while loading passive skill";
            desc = "...";
        }
        else
        {
            try
            {
                //id;name;description;iconType;numberOfEffects;effectType;valueType;levels;value;
                string[] splitData = data.Split(';');
                id = int.Parse(splitData[0]);
                if(rarity=="exalted") name = "<color=yellow>"+splitData[1]+"</color>";
                else if(rarity=="renowned") name = "<color=purple>"+splitData[1]+"</color>";
                else name = "<color=grey>"+splitData[1]+"</color>";
                baseDesc = splitData[2];
                icon = splitData[3];
                int current = 5;
                for (int i = 0; i < int.Parse(splitData[4]); i++)
                {
                    effect.Add(splitData[current]);
                    current++;
                    type.Add(splitData[current]);
                    current++;
                    if (maxLevel == 0) maxLevel = int.Parse(splitData[current]);
                    List<int> valueDependingOnLevel = new List<int>();
                    for (int j = 0; j < maxLevel; j++)
                    {
                        current++;
                        valueDependingOnLevel.Add(int.Parse(splitData[current]));
                    }
                    values.Add(valueDependingOnLevel);
                    current++;
                }
            }
            catch
            {
                Debug.Log("Found error in this data: " + data);
            }

        }
    }
    public int getId(){return id;}
    public string getStats(){
        if(currentLevel<=0) return "";
        else{
            string data="";
            for(int i=0;i<effect.Count;i++){
            data+=effect[i]+";";
            data+=type[i]+";";
            if(i<effect.Count-1)data+=values[i][currentLevel-1].ToString()+";";
            else data+=values[i][currentLevel-1].ToString();
            }
            return data;
            
        }
    }
    public bool levelUp(){
        if(currentLevel<maxLevel){ 
            currentLevel++;
            return true;
        }
        return false;
    }
    public Texture2D returnIcon(){
        Texture2D texture = Resources.Load<Texture2D>($"Icons/{icon}");
        if (texture != null) return texture;
        else
        {
            Debug.Log("Texture at: " + $"Icons/{icon}"+" returned as null");
            return Resources.Load<Texture2D>("ph");

        }
    }
    public string getLevel(){
        return currentLevel!=maxLevel ? currentLevel+"/"+maxLevel : currentLevel+"/"+maxLevel+"(max)";
    }
    public string getName() { return name; }
    public string getProperDesc()
    {
        desc = name+"\n\nCurrent level: "+baseDesc + " " + generateSuffix(currentLevel - 1);
        if (currentLevel < maxLevel)
        {
            desc += "\n\nNext level: "+ baseDesc + " " + generateSuffix(currentLevel);
            
        }
        else  desc+="(max level achieved.)";
        return desc;
        
    }
    string generateSuffix(int level)
    {
        string suffix = "";
        if (level >= 0 )
        {
            int typeIterator = 0;
            foreach (List<int> list in values)
            {
                if (suffix != "") suffix += "/";
                suffix += list[level];
                if (type[typeIterator++] != "flat") suffix += "%";

            }
        }
        else
        {
            int typeIterator = 0;
            foreach (List<int> list in values)
            {
                if (suffix != "") suffix += "/";
                suffix += 0;
                if (type[typeIterator++] != "flat") suffix += "%";

            }
        }
        return suffix;
    }

}