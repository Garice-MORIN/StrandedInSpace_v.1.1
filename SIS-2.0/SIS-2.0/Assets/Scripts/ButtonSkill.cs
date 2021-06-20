using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonSkill : MonoBehaviour
{
    public GameObject ShopPage;
    public SkillTree.SkillType skillOfButton;
    public Text textOfButton;
    private void Start() {
        switch (skillOfButton)
        {
            case SkillTree.SkillType.StartingMoney1:
                if(PlayerPrefs.GetFloat("StartingMoney") >= 1.1f){
                    IsBought();
                }
                break;
            case SkillTree.SkillType.StartingMoney2:
                if(PlayerPrefs.GetFloat("StartingMoney") >= 1.25f){
                    IsBought();
                }
                break;
            case SkillTree.SkillType.StartingMoney3:
                if(PlayerPrefs.GetFloat("StartingMoney") >= 1.5f){
                    IsBought();
                }
                break;
            case SkillTree.SkillType.MaxHealth1:
                if(PlayerPrefs.GetFloat("MaxHealth") >= 1.25f){
                    IsBought();
                }
                break;
            case SkillTree.SkillType.MaxHealth2:
                if(PlayerPrefs.GetFloat("MaxHealth") >= 1.5f){
                    IsBought();
                }
                break;
            case SkillTree.SkillType.MaxHealth3:
                if(PlayerPrefs.GetFloat("MaxHealth") >= 2f){
                    IsBought();
                }
                break;
            case SkillTree.SkillType.TowerDamage1:
                if(PlayerPrefs.GetFloat("TowerDamage") >= 1.1f){
                    IsBought();
                }
                break;
            case SkillTree.SkillType.TowerDamage2:
                if(PlayerPrefs.GetFloat("TowerDamage") >= 1.25f){
                    IsBought();
                }
                break;
            case SkillTree.SkillType.TowerDamage3:
                if(PlayerPrefs.GetFloat("TowerDamage") >= 1.5f){
                    IsBought();
                }
                break;
            case SkillTree.SkillType.TowerStatus1:
                if(PlayerPrefs.GetFloat("TowerStatus") >= 1.25f){
                    IsBought();
                }
                break;
            case SkillTree.SkillType.TowerStatus2:
                if(PlayerPrefs.GetFloat("TowerStatus") >= 1.5f){
                    IsBought();
                }
                break;
            case SkillTree.SkillType.TowerStatus3:
                if(PlayerPrefs.GetFloat("TowerStatus") >= 2f){
                    IsBought();
                }
                break;
            case SkillTree.SkillType.TrapDamage1:
                if(PlayerPrefs.GetFloat("TrapDamage") >= 1.25f){
                    IsBought();
                }
                break;
            case SkillTree.SkillType.TrapDamage2:
                if(PlayerPrefs.GetFloat("TrapDamage") >= 1.5f){
                    IsBought();
                }
                break;
            case SkillTree.SkillType.TrapDamage3:
                if(PlayerPrefs.GetFloat("TrapDamage") >= 2f){
                    IsBought();
                }
                break;
            case SkillTree.SkillType.TrapUses1:
                if(PlayerPrefs.GetFloat("TrapUses") >= 1.2f){
                    IsBought();
                }
                break;
            case SkillTree.SkillType.TrapUses2:
                if(PlayerPrefs.GetFloat("TrapUses") >= 1.5f){
                    IsBought();
                }
                break;
            case SkillTree.SkillType.TrapUses3:
                if(PlayerPrefs.GetFloat("TrapUses") >= 2f){
                    IsBought();
                }
                break;
        }
    }
    public void Unlock()
    {
        if(ShopPage.GetComponent<SkillTree>().UnlockSkill(skillOfButton, textOfButton.text)){
            IsBought();
        }
    }
    public void IsBought(){
        textOfButton.fontSize = 12;
        textOfButton.text = "Bought";
    }
}