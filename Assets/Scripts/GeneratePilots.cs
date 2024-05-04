using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GeneratePilots : MonoBehaviour
{
    //// 0 - Skirmisher, 1 - Destroyer, 2 - Cruiser, 3 - Support
    readonly int[] critChances = { 5, 15, 0, 0 };
    readonly int[] critMutlipliers = { 30, 50, 0, 0 };
    readonly float[] attackDistribution = { 0.21f,0.4f,0.1f,0.23f};
    readonly float[] defenceDistribution = { 0.18f,0.15f,0.35f,0.23f};
    readonly float[] speedDistribution = { 0.4f,0.3f,0.2f,0.3f};
    readonly float[] healthDistribution = { 0.21f,0.15f,0.35f,0.24f };
    readonly int baseExaltedRarityChance = 2;
    readonly int baseRenownedRarityChance = 7;
    readonly int baseVeteranRarityChance = 25;
    readonly int baseAdvancedRarityChance = 50;
    readonly string[] names = { "James", "Mohammed", "Santiago", "Liu", "Ravi", "Ahmed", "Antonio", "Hiroshi", "Youssef", "Vladimir", "Sophia", "Aisha", "Olga", "Yuna", "Fatima", "Mia", "Ananya", "Sakura", "Layla", "Wei" };
    readonly string[] surnames = { "Smith", "Garcia", "Kim", "Müller", "Singh", "González", "Yamamoto", "Silva", "Lopez", "Patel", "Chen", "Novak", "Ali", "Jansen", "Ramos", "Kaur", "Lee", "Hernandez", "Ferreira", "Nguyen" };
    readonly float critChanceCost=4f;
    readonly float critMultiplierCost=1.34f;
    readonly float attackCost=1f;
    readonly float defenceCost=1f;
    readonly float healthCost=0.125f;
    readonly float speedCost=10f;
    float baseY = Screen.height / 2.5f;
    float pilotHeight = Screen.height / 4;

    [SerializeField] Canvas canvas;
    void Start()
    {

        if (!GlobalVariables.Instance.firstRollDone)
        {
            GlobalVariables.Instance.firstRollDone = true;
            GlobalVariables.Instance.pilotsInPool = generatePilots(3);

        }

        foreach(Pilot p in GlobalVariables.Instance.recruitedPilots)
        {
            Debug.Log(p.getAllInfo());
        }
        float y = baseY;
        foreach (Pilot p in GlobalVariables.Instance.pilotsInPool)
        {
            p.generatePortrait(canvas, 0, y);
            y -= (pilotHeight*1.8f);
        }
       
    }
    private void Update()
    {
        if (GlobalVariables.Instance.updateAether)
        {
            GlobalVariables.Instance.updateAether = false;
            refreshList();
        }        
    }
    private void refreshList()
    {
        
        int childCount = canvas.transform.childCount;

        
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = canvas.transform.GetChild(i);
            if (child.name == "Pilot") Destroy(child.gameObject);
        }
        float y = baseY;
        foreach (Pilot p in GlobalVariables.Instance.pilotsInPool)
        {
            p.generatePortrait(canvas, 0, y);
            y -= (pilotHeight * 1.8f);
        }

    }
    private List<Pilot> generatePilots(int howMany,bool isEncreDraft=false)
    {
        bool encreDraft = isEncreDraft;
        List<Pilot> pilots=new List<Pilot>();
        for (int i = 0; i < howMany; i++)
        {
            int shipType = Random.Range(0, 4);
            int pilotRarity;
            int roll = Random.Range(0, 100);
            if (isEncreDraft)
            {
                isEncreDraft = false;
                float sum = baseExaltedRarityChance * GlobalVariables.Instance.rarityMultiplier + baseRenownedRarityChance * GlobalVariables.Instance.rarityMultiplier;
                float exalted = (baseExaltedRarityChance * GlobalVariables.Instance.rarityMultiplier / sum)*100;
                if (roll < exalted)
                {
                    pilotRarity = 4;
                }
                else pilotRarity = 3;
            }
            else
            {
                if (roll < baseExaltedRarityChance * GlobalVariables.Instance.rarityMultiplier) { pilotRarity = 4; }
                else if (roll < baseRenownedRarityChance * GlobalVariables.Instance.rarityMultiplier) { pilotRarity = 3; }
                else if (roll < baseVeteranRarityChance * GlobalVariables.Instance.rarityMultiplier) { pilotRarity = 2; }
                else if (roll < baseAdvancedRarityChance * GlobalVariables.Instance.rarityMultiplier) { pilotRarity = 1; }
                else pilotRarity = 0;
            }
            int statPoints = GlobalVariables.Instance.pilotStats[pilotRarity];
            float critChance = critChances[shipType];
            float critMultiplier = critMutlipliers[shipType];
            statPoints -= (int)(critChance * critChanceCost);
            statPoints -= (int)(critMultiplier * critMultiplierCost);
            float attackVariation = Random.Range(-0.03f, 0.03f);
            float defenceVariation = attackVariation * -1;
            float speedVariation = Random.Range(-0.03f, 0.03f);
            float healthVariation = speedVariation * -1;
            float attack = (attackDistribution[shipType] + attackVariation) * statPoints / attackCost;
            float defence = (defenceDistribution[shipType] + defenceVariation) * statPoints / defenceCost;
            float speed = (speedDistribution[shipType] + speedVariation) * statPoints / speedCost;
            float health = (healthDistribution[shipType] + healthVariation) * statPoints / healthCost;
            string name = names[Random.Range(0, 20)];
            string surname = surnames[Random.Range(0, 20)];

            pilots.Add(new Pilot(shipType, pilotRarity, critChance, critMultiplier, attack, defence, speed, health, name, surname));
        }
        return pilots;
    }
    public void basicDraft()
    {
        GlobalVariables.Instance.pilotsInPool = generatePilots(3);
        refreshList();

    }
    public void encreDraft()
    {
        GlobalVariables.Instance.pilotsInPool = generatePilots(3,true);
        refreshList();

    }


}
