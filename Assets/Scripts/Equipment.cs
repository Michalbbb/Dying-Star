using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Equipment
{
    
    List<int> values;
    List<string> types;
    List<string> attributes;

    Texture2D icon;
    Pilot attachedTo;

    string description;
    string descriptionLite;

    int comparerValue;

    
    public Equipment(string type,string quality){
        values = new List<int>();
        types = new List<string>();
        attributes = new List<string>();
        List<string> randomAttributes=new List<string>();
        List<string> descToAttributes=new List<string>();
        comparerValue=0;
        description="";
        descriptionLite="";
        icon=Resources.Load<Texture2D>($"Factory/{type}");
        float valueMultiplier=1f;
        if(quality=="ordinary"){
        description+="<color=#DBDBDB>("+quality.ToUpper()+")";
        comparerValue+=1;
        }
        if(quality=="excellent") {
            valueMultiplier=1.3f;
            description+="<color=#5CD8FF>("+quality.ToUpper()+")";
            comparerValue+=2;
        }
        if(quality=="superior") {
            valueMultiplier=1.8f;
            description+="<color=yellow>("+quality.ToUpper()+")";
            comparerValue+=3;
        }
        if(type=="forceField"){
            comparerValue+=10;
            attributes.Add("defence");
            types.Add("flat");
            attributes.Add("health");
            types.Add("flat");
            int defenceValue=(int)(Random.Range(35,66)*valueMultiplier);
            int healthValue=(int)(Random.Range(600,801)*valueMultiplier);
            values.Add(defenceValue);
            values.Add(healthValue);
            randomAttributes.AddRange(equipmentConstValues.Instance.forceFieldRandomStats);
            descToAttributes.AddRange(equipmentConstValues.Instance.forceFieldDesc);
            description+="\nFORCE FIELD</color>\n";
            description+="Base stats";
            descriptionLite+=description;
            description+="\nAdditional defence: "+defenceValue.ToString();
            description+="\nAdditional health: "+healthValue.ToString()+"\n";
            descriptionLite+="\nDefence + "+defenceValue.ToString();
            descriptionLite+="\nHealth + "+healthValue.ToString()+"\n";
        }
        else if(type=="supportModule"){
            comparerValue+=100;
            attributes.Add("speed");
            types.Add("flat");
            attributes.Add("health");
            types.Add("flat");
            int speedValue=(int)(Random.Range(6,11)*valueMultiplier);
            int healthValue=(int)(Random.Range(400,521)*valueMultiplier);
            values.Add(speedValue);
            values.Add(healthValue);
            randomAttributes.AddRange(equipmentConstValues.Instance.supportModulesRandomStats);
            descToAttributes.AddRange(equipmentConstValues.Instance.supportModulesDesc);
            description+="\nSUPPORT MODULE</color>\n";
            description+="Base stats";
            descriptionLite+=description;
            description+="\nAdditional speed: "+speedValue.ToString();
            description+="\nAdditional health: "+healthValue.ToString()+"\n";
            descriptionLite+="\nSpeed + "+speedValue.ToString();
            descriptionLite+="\nHealth + "+healthValue.ToString()+"\n";
            
        }
        else {
            comparerValue+=1000;
            attributes.Add("attack");
            types.Add("flat");
            attributes.Add("speed");
            types.Add("flat");
            int attackValue=(int)(Random.Range(78,102)*valueMultiplier);
            int speedValue=(int)(Random.Range(4,9)*valueMultiplier);
            values.Add(attackValue);
            values.Add(speedValue);
            randomAttributes.AddRange(equipmentConstValues.Instance.energyBoosterRandomStats);
            descToAttributes.AddRange(equipmentConstValues.Instance.energyBoosterDesc);
            description+="\nENERGY BOOSTER</color>\n";
            description+="Base stats";
            descriptionLite+=description;
            description+="\nAdditional attack: "+attackValue.ToString();
            description+="\nAdditional speed: "+speedValue.ToString()+"\n";
            descriptionLite+="\nAttack + "+attackValue.ToString();
            descriptionLite+="\nSpeed + "+speedValue.ToString()+"\n";
        }
        description+="\nBonus stats\n";
        descriptionLite+="Bonus stats\n";
        for(int i=0;i<2;i++){
            int randomNumber=Random.Range(0,randomAttributes.Count);
            string [] data=randomAttributes[randomNumber].Split(";");
            attributes.Add(data[0]);
            types.Add(data[1]);
            int value = (int)((Random.Range(int.Parse(data[2]),int.Parse(data[3])+1)*valueMultiplier));
            values.Add(value);
            description+=descToAttributes[randomNumber]+" "+value.ToString();
            if(data[1]!="flat")description+="%";
            description+="\n";
            string descLiteAdd="";
            if(data[1]=="percent")descLiteAdd="(total)";
            if(data[1]=="base")descLiteAdd="(base)";
            descriptionLite+=data[0]+descLiteAdd+" + "+value;
            if(data[1]!="flat")descriptionLite+="%";
            descriptionLite+="\n";
        }
    }
    public int getCompareValue(){return comparerValue;}
    public void attach(Pilot p){
        if(attachedTo!=null){
            if(attachedTo==p) return;
            attachedTo.removeEquipment(this);
        }
        attachedTo=p;

    }
    public void detach(){
        if(attachedTo==null) return;
        attachedTo.removeEquipment(this);
        attachedTo=null;
    }
    public bool isAttached(){
     if(attachedTo!=null) return true;
     return false;    
    }

    public Texture2D getIcon(){return icon;}
    public string getDescription(){
        return description;
    }
    public string getDescriptionLite(){
        string postInfo= attachedTo != null ? "<color=red>Equipped by "+attachedTo.getFullName()+"</color>" : "<color=green>Unequipped</color>";
        
        return descriptionLite+postInfo;
    }
    
    public string getStats(){
        string data="";
        for(int i=0;i<attributes.Count;i++){
            data+=attributes[i]+";";
            data+=types[i]+";";
            data+=values[i].ToString();
            if(i<attributes.Count-1)data+=';'; 
        }
        return data;
    }

}
public class equipmentConstValues
{

    public int supportModuleMinSpeed=6;
    public int supportModuleMaxSpeed=10;
    public int supportModuleMinHealth=400;
    public int supportModuleMaxHealth=520;

    public int forceFieldMinDefence=35;
    public int forceFieldMaxDefence=65;
    public int forceFieldMinHealth=600;
    public int forceFieldMaxHealth=800;
    
    public int energyBoosterMinAttack=78;
    public int energyBoosterMaxAttack=101;
    public int energyBoosterMinSpeed=4;
    public int energyBoosterMaxSpeed=8;
    public List<int> metalPrice;
    public List<int> titanPrice;
    private static equipmentConstValues instance = null;
    private static readonly object padlock = new object();
    public static equipmentConstValues Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new equipmentConstValues();
                }
                return instance;
            }
        }
    }
    public float baseExMulti=1.3f;
    public float baseSupMulti=1.8f;
    public List<string> supportModulesRandomStats;
    public List<string> supportModulesDesc;
    public List<string> forceFieldRandomStats;
    public List<string> forceFieldDesc;
    public List<string> energyBoosterRandomStats;
    public List<string> energyBoosterDesc;
    equipmentConstValues(){
        supportModulesRandomStats=new List<string>();
        forceFieldRandomStats=new List<string>();
        energyBoosterRandomStats=new List<string>();
        supportModulesDesc=new List<string>();
        forceFieldDesc=new List<string>();
        energyBoosterDesc=new List<string>();
        metalPrice=new List<int>();
        titanPrice=new List<int>();
        metalPrice.Add(1000);
        metalPrice.Add(900);
        metalPrice.Add(500);
        titanPrice.Add(0);
        titanPrice.Add(100);
        titanPrice.Add(500);

        supportModulesRandomStats.Add("healing;percent;10;15");
        supportModulesRandomStats.Add("speed;base;15;20");
        supportModulesRandomStats.Add("speed;percent;5;10");
        supportModulesRandomStats.Add("movementPoints;flat;1;1");
        supportModulesRandomStats.Add("health;flat;200;300");
        supportModulesRandomStats.Add("health;base;15;20");
        supportModulesRandomStats.Add("health;percent;5;10");

        supportModulesDesc.Add("Increases healing done by");
        supportModulesDesc.Add("Increases base speed by");
        supportModulesDesc.Add("Increases total speed by");
        supportModulesDesc.Add("Increases movement points by");
        supportModulesDesc.Add("Increases health by");
        supportModulesDesc.Add("Increases base health by");
        supportModulesDesc.Add("Increases total health by");

        forceFieldRandomStats.Add("defence;percent;7;12");
        forceFieldRandomStats.Add("speed;flat;3;6");
        forceFieldRandomStats.Add("defence;flat;40;60");
        forceFieldRandomStats.Add("defence;base;20;25");
        forceFieldRandomStats.Add("health;flat;300;400");
        forceFieldRandomStats.Add("health;base;20;25");
        forceFieldRandomStats.Add("health;percent;7;12");

        forceFieldDesc.Add("Increases total defence by");
        forceFieldDesc.Add("Increases speed by");
        forceFieldDesc.Add("Increases defence by");
        forceFieldDesc.Add("Increases base defence by");
        forceFieldDesc.Add("Increases health by");
        forceFieldDesc.Add("Increases base health by");
        forceFieldDesc.Add("Increases total health by");

        energyBoosterRandomStats.Add("attack;base;20;25");
        energyBoosterRandomStats.Add("speed;base;8;15");
        energyBoosterRandomStats.Add("attack;percent;7;12");
        energyBoosterRandomStats.Add("movementPoints;flat;1;1");
        energyBoosterRandomStats.Add("attack;flat;40;60");
        energyBoosterRandomStats.Add("critChance;percent;12;20");
        energyBoosterRandomStats.Add("critMultiplier;percent;40;60");

        energyBoosterDesc.Add("Increases base attack by");
        energyBoosterDesc.Add("Increases base speed by");
        energyBoosterDesc.Add("Increases total attack by");
        energyBoosterDesc.Add("Increases movement points by");
        energyBoosterDesc.Add("Increases attack by");
        energyBoosterDesc.Add("Increases critical strike chance by");
        energyBoosterDesc.Add("Increases critical strike multiplier by");

    }
}
