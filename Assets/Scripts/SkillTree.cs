using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree 
{
    List<PassiveSkill> region1;
    List<PassiveSkill> region2;
    List<PassiveSkill> region3;
    int passiveSkills=0;
   public SkillTree(List<PassiveSkill> reg1, List<PassiveSkill> reg2, List<PassiveSkill> reg3, int availableSkillPoints)
    {
        region1 = reg1;
        region2 = reg2;
        region3 = reg3;
        passiveSkills=availableSkillPoints;
        
    }
    public void addSkillPoints(int howMany){
        passiveSkills+=howMany;
    }
    public int getSkillPoints(){
        return passiveSkills;
    }
    public void levelUp(PassiveSkill skillToLevelUp){
        if(passiveSkills<=0)return ;
        foreach(PassiveSkill skill in getTree()){
            if(skillToLevelUp.getId() == skill.getId()){
                if(skill.levelUp())passiveSkills--;
                break;
            }
        }
    }
    public List<PassiveSkill> getTree()
    {
        List<PassiveSkill> tree=new List<PassiveSkill>();
        tree.AddRange(region1);
        tree.AddRange(region2);
        tree.AddRange(region3);
        return tree;
    }
    public List<PassiveSkill> getRegion1(){return region1;}
    public List<PassiveSkill> getRegion2(){return region2;}
    public List<PassiveSkill> getRegion3(){return region3;}
}
