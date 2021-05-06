using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class PauseMenu : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject settingsMenu;
    public GameObject commandMenu;
    public AudioSource audioSource;
    public AudioSource effectSource;
    public Slider audioSlider;
    public Slider effectSlider;


    //Ouvrir Settings
    public void OnSettingsMenu()
    {
        playerController.pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }
    //Quitter Settings
    public void OnSettingsBack()
    {
        playerController.pauseMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    //Ouvrir commands dans settings
    public void OnCommand()
    {
        settingsMenu.SetActive(false);
        commandMenu.SetActive(true);
    }

    //Fermer commande
    public void OnCommandBack()
    {
        commandMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void AzertyIsOn()
    { 
        commandMenu.transform.GetChild(4).gameObject.SetActive(false); //Tableau Qwerty pas affiché
        commandMenu.transform.GetChild(3).gameObject.SetActive(true);  //Tabbleau Azerty affiché
       // qwerty.isOn = false;
    }

    public void QwertyIsOn()
    {
        commandMenu.transform.GetChild(3).gameObject.SetActive(false); //Tableau Azerty pas affiché
        commandMenu.transform.GetChild(4).gameObject.SetActive(true);  //Tabbleau Qwerty affiché
        //azerty.isOn = false;
    }

    public void OnVolumeValueChanged()
    {
        audioSource.volume = audioSlider.value;
        PlayerPrefs.SetFloat("Music", audioSlider.value);
    }

    public void OnEffectValueChanged()
    {
        effectSource.volume = effectSlider.value;
        PlayerPrefs.SetFloat("Effects", audioSlider.value);
    }
}
