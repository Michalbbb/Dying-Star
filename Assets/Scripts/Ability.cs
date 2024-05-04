using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Ability 
{
    int id;
    string name;
    string desc;
    int baseCooldown;
    int Cooldown;
    List<string> effect;
    List<string> target;
    List<int> value;
    List<int> duration;
    List<int> scaling;
    List<int> rows;
    List<int> cols;
    int range;
    string icon;



    public Ability(string data)
    {
        effect = new List<string>();
        target = new List<string>();
        value = new List<int>();
        duration = new List<int>();
        scaling = new List<int>();
        rows = new List<int>();
        cols = new List<int>();
        if (data == "Null" ||data == null)
        {
            name = "Error while loading ability";
            desc = "...";
        }
        else
        {
            try
            {
                string[] splitData = data.Split(';');
                id = int.Parse(splitData[0]);
                name = splitData[1];
                desc = splitData[2];
                baseCooldown = int.Parse(splitData[3]);
                Cooldown = 0;
                int current = 5;
                for (int i = 0; i < int.Parse(splitData[4]); i++)
                {
                    effect.Add(splitData[current]);
                    current++;
                    target.Add(splitData[current]);
                    current++;
                    value.Add(int.Parse(splitData[current]));
                    current++;
                    duration.Add(int.Parse(splitData[current]));
                    current++;
                    scaling.Add(int.Parse(splitData[current]));
                    current++;
                    rows.Add(int.Parse(splitData[current]));
                    current++;
                    cols.Add(int.Parse(splitData[current]));
                    current++;
                }
                range = int.Parse(splitData[current]);
                current++;
                icon = splitData[current].TrimEnd();
            }
            catch { Debug.Log(data); }
        }
    }
    public string getInfo()
    {
        string info = "<b>" + name + "</b>" + "\n" + desc;
        if (baseCooldown != 0) info += "\nCooldown: " + baseCooldown + " turns.";
        if(range!=99)info += "\n" + "Attack Range: " + range;
        return info;
    }
    public void use() // TODO 
    {

    }
    public string getName() { return name; }
    public Texture2D getIcon()
    {
        Texture2D texture = Resources.Load<Texture2D>($"Icons/{icon}");
        if (texture != null) return texture;
        else
        {
            Debug.Log("Texture at: " + $"Icons/{icon}"+" returned as null");
            return Resources.Load<Texture2D>("ph");

        }
    }
   
}

public class GenerateAbility // Singleton responsible for generating abilities for pilots
{
    private static GenerateAbility instance = null;
    private static readonly object padlock = new object();
    public static GenerateAbility Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new GenerateAbility();
                }
                return instance;
            }
        }
    }
    string [] type = { "Assets/TextFiles/SkirmisherAbilities.txt", "Assets/TextFiles/DestroyerAbilities.txt", "Assets/TextFiles/DefenderAbilities.txt", 
        "Assets/TextFiles/SupportAbilities.txt","Assets/TextFiles/HealingAbilities.txt"}; // 0 - Skirmisher, 1 - Destroyer, 2 - Defender, 3 - Support, 4 - Healing 
    int[] skirmisherChances = { 2, 3, 1, 1, 1, 0, 0, 0, 0, 0};
    int[] destroyerChances = { 2, 3, 1, 1, 1, 1, 1, 0, 0, 0};
    int[] cruiserChances = { 1, 3, 3, 3, 2, 2, 2, 2, 2, 2};
    int[] supportChances = { 0, 2, 2, 4, 4, 3, 3, 3, 3, 3};
    string baseAbility = "1;Attack;Deals damage equal to 100% of attack power single enemy;0;1;damage;enemy;0;0;100;1;1;4;Attack";
    public List<Ability> generateTwo(int pilotClass)
    {
        List<Ability> twoAbilites = new List<Ability>();

        int successes = 0;
        while (successes < 2)
        {
            Ability abi=getAbility(pilotClass);
            if(twoAbilites.Count == 0)
            {
                twoAbilites.Add(abi);
                successes++;
            }
            else
            {
                if (twoAbilites[twoAbilites.Count - 1].getName() != abi.getName())
                {
                    twoAbilites.Add(abi);
                    successes++;
                }
            }
        }
        List<Ability> abilities = new List<Ability>();
        abilities.Add(new Ability(baseAbility));
        abilities.AddRange(twoAbilites);
        if (abilities.Count != 3) Debug.Log("Something went terribly wrong");
        return abilities;
    }

    public Ability getAbility(int pilotClass)
    {
        Ability ability;
        int outcome;
        if(pilotClass == 0) { outcome = skirmisherChances[Random.Range(0, 10)]; }
        else if(pilotClass==1) { outcome = destroyerChances[Random.Range(0, 10)]; }
        else if(pilotClass==2) { outcome = cruiserChances[Random.Range(0, 10)]; }
        else { outcome = supportChances[Random.Range(0, 10)]; }
        if (File.Exists(type[outcome]))
        {
            // Read all text from the file
            string fileText = File.ReadAllText(type[outcome]);
            string[] data = fileText.Split("\n");
            int abilityToPick = Random.Range(0,data.Length);
            ability = new Ability(data[abilityToPick]);
        }
        else
        {
            ability = new Ability("Null");
        }

        return ability;
    }

}
