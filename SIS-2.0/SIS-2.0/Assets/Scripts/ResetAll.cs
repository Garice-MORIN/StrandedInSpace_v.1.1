using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAll : MonoBehaviour
{
    public void ResetPlayerPrefs(){
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString("name","");
        PlayerPrefs.SetInt("hasPlayed", 1);
        PlayerPrefs.SetInt("krux", 0);
        PlayerPrefs.SetFloat("StartingMoney", 1f);
        PlayerPrefs.SetFloat("MaxHealth", 1f);
        PlayerPrefs.SetFloat("TowerDamage", 1f);
        PlayerPrefs.SetFloat("TowerStatus", 1f);
        PlayerPrefs.SetFloat("TrapDamage", 1f);
        PlayerPrefs.SetFloat("TrapUses", 1f);
        PlayerPrefs.SetFloat("Music", 0.5f);
        PlayerPrefs.SetFloat("Effects", 0.5f);
        PlayerPrefs.SetInt("Inverted", 1);
        PlayerPrefs.SetFloat("Sensi", 1125);
    }
}
