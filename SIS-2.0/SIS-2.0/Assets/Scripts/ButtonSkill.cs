using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonSkill : MonoBehaviour
{
    public GameObject ShopPage;
    public SkillTree.SkillType skillOfButton;
    public Text textOfButton;
    public void Unlock()
    {
        if(ShopPage.GetComponent<SkillTree>().UnlockSkill(skillOfButton, textOfButton.text)){
            textOfButton.fontSize = 12;
            textOfButton.text = "Bought";
        }
    }
}