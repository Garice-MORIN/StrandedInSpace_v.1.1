using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SkillTree : MonoBehaviour
{ 
    public List<SkillType> Tree { get; set; }
    public SkillTree(){
        Tree = new List<SkillType>{};
    }
    public enum SkillType{
        None,
        StartingMoney1,
        StartingMoney2,
        StartingMoney3,
        MaxHealth1,
        MaxHealth2,
        MaxHealth3,
        TowerDamage1,
        TowerDamage2,
        TowerDamage3,
        TowerStatus1,
        TowerStatus2,
        TowerStatus3,
        TrapDamage1,
        TrapDamage2,
        TrapDamage3,
        TrapUses1,
        TrapUses2,
        TrapUses3
    };
    public List<SkillType> GetAllUnlockedSkills(List<bool> listBool){
        List<SkillType> tree = new List<SkillType>();
        for(int i = 0; i < listBool.Count; i++)
        {
            if(listBool[i])
            {
                tree.Add((SkillType)(i+1));
            }
        }
        return tree;
    }
    public bool UnlockSkill(SkillType skillType, string text){
        int priceNeeded;
        if(Int32.TryParse(text, out priceNeeded)){
            if(CanUnlockSkill(skillType, priceNeeded)){
            Tree.Add(skillType);
            PlayerPrefs.SetInt("krux", PlayerPrefs.GetInt("krux") - priceNeeded);
            GetUpgrades(skillType);
            return true;
            }
        }
        return false;
    }

    public bool CanUnlockSkill(SkillType skillType, int priceNeeded)
    {
        return !IsUnlocked(skillType) && IsUnlocked(GetRequirements(skillType)) && PlayerPrefs.GetInt("krux") >= priceNeeded;
    }
    public bool IsUnlocked(SkillType skillType)
    {
        return skillType == SkillType.None || Tree.Contains(skillType);
    }
    public SkillType GetRequirements(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.MaxHealth2 : return SkillType.MaxHealth1;
            case SkillType.MaxHealth3 : return SkillType.MaxHealth2;
            case SkillType.StartingMoney2 : return SkillType.StartingMoney1;
            case SkillType.StartingMoney3 : return SkillType.StartingMoney2;
            case SkillType.TowerDamage2 : return SkillType.TowerDamage1;
            case SkillType.TowerDamage3 : return SkillType.TowerDamage2;
            case SkillType.TowerStatus2 : return SkillType.TowerStatus1;
            case SkillType.TowerStatus3 : return SkillType.TowerStatus2;
            case SkillType.TrapDamage2 : return SkillType.TrapDamage1;
            case SkillType.TrapDamage3 : return SkillType.TrapDamage2;
            case SkillType.TrapUses2 : return SkillType.TrapUses1;
            case SkillType.TrapUses3 : return SkillType.TrapUses2;
            default : return SkillType.None;
        }
    }
    public void GetUpgrades(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.StartingMoney1:
                PlayerPrefs.SetFloat("StartingMoney", 1.1f);
                break;
            case SkillType.StartingMoney2:
                PlayerPrefs.SetFloat("StartingMoney", 1.25f);
                break;
            case SkillType.StartingMoney3:
                PlayerPrefs.SetFloat("StartingMoney", 1.5f);
                break;
            case SkillType.MaxHealth1:
                PlayerPrefs.SetFloat("MaxHealth", 1.25f);
                break;
            case SkillType.MaxHealth2:
                PlayerPrefs.SetFloat("MaxHealth", 1.5f);
                break;
            case SkillType.MaxHealth3:
                PlayerPrefs.SetFloat("MaxHealth", 2f);
                break;
            case SkillType.TowerDamage1:
                PlayerPrefs.SetFloat("TowerDamage", 1.1f);
                break;
            case SkillType.TowerDamage2:
                PlayerPrefs.SetFloat("TowerDamage", 1.25f);
                break;
            case SkillType.TowerDamage3:
                PlayerPrefs.SetFloat("TowerDamage", 1.5f);
                break;
            case SkillType.TowerStatus1:
                PlayerPrefs.SetFloat("TowerStatus", 1.25f);
                break;
            case SkillType.TowerStatus2:
                PlayerPrefs.SetFloat("TowerStatus", 1.5f);
                break;
            case SkillType.TowerStatus3:
                PlayerPrefs.SetFloat("TowerStatus", 2f);
                break;
            case SkillType.TrapDamage1:
                PlayerPrefs.SetFloat("TrapDamage", 1.2f);
                break;
            case SkillType.TrapDamage2:
                PlayerPrefs.SetFloat("TrapDamage", 1.5f);
                break;
            case SkillType.TrapDamage3:
                PlayerPrefs.SetFloat("TrapDamage", 2f);
                break;
            case SkillType.TrapUses1:
                PlayerPrefs.SetFloat("TrapUses", 1.2f);
                break;
            case SkillType.TrapUses2:
                PlayerPrefs.SetFloat("TrapUses", 1.5f);
                break;
            case SkillType.TrapUses3:
                PlayerPrefs.SetFloat("TrapUses", 2f);
                break;
        }
    }
}
