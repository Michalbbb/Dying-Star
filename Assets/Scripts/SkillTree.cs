using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree 
{
    List<PassiveSkill> region1;
    List<PassiveSkill> region2;
    List<PassiveSkill> region3;
   public SkillTree(List<PassiveSkill> reg1, List<PassiveSkill> reg2, List<PassiveSkill> reg3)
    {
        region1 = reg1;
        region2 = reg2;
        region3 = reg3;
    }
    public void getBasicInfo()
    {
        foreach(PassiveSkill skill in region1)
        {
            Debug.Log(skill.getBaseDesc());
            Debug.Log(skill.getNextLevelDesc());
        }
        foreach (PassiveSkill skill in region2)
        {
            Debug.Log(skill.getBaseDesc());
            Debug.Log(skill.getNextLevelDesc());
        }
        foreach (PassiveSkill skill in region3)
        {
            Debug.Log(skill.getBaseDesc());
            Debug.Log(skill.getNextLevelDesc());
        }
    }
}
