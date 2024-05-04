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
    string[] skills = { "Assets/TextFiles/PassiveSkills/OffensiveSkills.txt", "Assets/TextFiles/PassiveSkills/DefensiveSkills.txt", "Assets/TextFiles/PassiveSkills/MixedSkills.txt" };
    string[] reSkills = { "Assets/TextFiles/PassiveSkills/OffensiveSkillsRe.txt", "Assets/TextFiles/PassiveSkills/DefensiveSkillsRe.txt", "Assets/TextFiles/PassiveSkills/MixedSkillsRe.txt" };
    string[] exSkills = { "Assets/TextFiles/PassiveSkills/OffensiveSkillsEx.txt", "Assets/TextFiles/PassiveSkills/DefensiveSkillsEx.txt", "Assets/TextFiles/PassiveSkills/MixedSkillsEx.txt" };
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
        if (shipClass == 1) skillType = destroyer;
        if (shipClass == 2) skillType = cruiser;
        else skillType = support;
        for(int i=0;i<3;i++) // Generate 3 basic passive skills for 3 regions
        {
            
            
                region1.Add(getRandomPassiveSkill(skills[skillType[Random.Range(0, 10)]], region1));
                region2.Add(getRandomPassiveSkill(skills[skillType[Random.Range(0, 10)]], region2));
                region3.Add(getRandomPassiveSkill(skills[skillType[Random.Range(0, 10)]], region3));

            
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
                    region1.Add(getRandomPassiveSkill(exSkills[skillType[Random.Range(0, 10)]], region1));
                }
                else if (rand1 < additionalChance)
                {
                    region1.Add(getRandomPassiveSkill(reSkills[skillType[Random.Range(0, 10)]], region1));
                }
                else { }
 
                if (rand2 < baseChance)
                {
                    region2.Add(getRandomPassiveSkill(exSkills[skillType[Random.Range(0, 10)]], region2));
                }
                else if (rand2 < additionalChance)
                {
                    region2.Add(getRandomPassiveSkill(reSkills[skillType[Random.Range(0, 10)]], region2));
                }
                else { }
                if (rand3 < baseChance)
                {
                    region3.Add(getRandomPassiveSkill(exSkills[skillType[Random.Range(0, 10)]], region3));
                }
                else if (rand3 < additionalChance)
                {
                    region3.Add(getRandomPassiveSkill(reSkills[skillType[Random.Range(0, 10)]], region3));
                }
                else { }

            }
            else
            {
                if (Random.Range(0, 100) < baseChance) region1.Add(getRandomPassiveSkill(reSkills[skillType[Random.Range(0, 10)]], region1));                              
                if (Random.Range(0, 100) < baseChance) region2.Add(getRandomPassiveSkill(reSkills[skillType[Random.Range(0, 10)]], region2));
                if (Random.Range(0, 100) < baseChance) region3.Add(getRandomPassiveSkill(reSkills[skillType[Random.Range(0, 10)]], region3));
                
            }
        }


        return new SkillTree(region1, region2, region3);

    }
    public PassiveSkill getRandomPassiveSkill(string file,List<PassiveSkill> skills)
    {
        if (File.Exists(file))
        {
            string fileText = File.ReadAllText(file);
            string[] data = fileText.Split("\n");
            bool success = false;
            PassiveSkill passiveSkill=new PassiveSkill("Null");
            while (!success)
            {
                success = true;
                string randomSkill = data[Random.Range(0, data.Length)];
                passiveSkill=new PassiveSkill(randomSkill);
                foreach(PassiveSkill skill in skills)
                {
                    if (skill.getName() == passiveSkill.getName()) success = false;
                }

            }
            return passiveSkill;
        }
        else return new PassiveSkill("Null");
    }
}
public class PassiveSkill
{
    
    int id;
    string name;
    string desc;
    string baseDesc;
    string descNextLvl;
    string icon;
    int currentLevel;
    int maxLevel;
    List<string> effect;
    List<string> type;
    List<List<int>> values;
    public PassiveSkill(string data)
    {
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
                name = splitData[1];
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
    public string getName() { return name; }
    public string getBaseDesc()
    {

        desc = name+" - "+baseDesc + " " + generateSuffix(currentLevel - 1);
        return desc;
    }
    public string getNextLevelDesc()
    {
        if (currentLevel < maxLevel)
        {
            descNextLvl = "Next level: "+ baseDesc + " " + generateSuffix(currentLevel);
            return descNextLvl;
        }
        else return "Currently on max level.";
        
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