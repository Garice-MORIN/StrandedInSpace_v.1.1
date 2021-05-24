using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class PauseMenu : NetworkBehaviour
{
    public PlayerController playerController;
    public GameObject settingsMenu;
    public GameObject commandMenu;
    public GameObject sureMenu;
    public AudioSource audioSource;
    public AudioSource effectSource;
    public Slider audioSlider;
    public Slider effectSlider;
    public Toggle azerty;
    public NetworkConnection networkConnection;
    private bool mainMenu;

    private void OnEnable()
    {
        effectSlider.value = PlayerPrefs.GetFloat("Effects");
        audioSlider.value = PlayerPrefs.GetFloat("Music");
    }

    //Revenir menu principal
    public void OnMainMenu()
    {
        mainMenu = true;
        playerController.pauseMenu.SetActive(false);
        sureMenu.SetActive(true);

    }
    

    public void AzertyIsOn()
    {
        commandMenu.transform.GetChild(4).gameObject.SetActive(false); //Tableau Qwerty pas affiché
        commandMenu.transform.GetChild(3).gameObject.SetActive(true);  //Tabbleau Azerty affiché
    }

    public void QwertyIsOn()
    {
        commandMenu.transform.GetChild(3).gameObject.SetActive(false); //Tableau Azerty pas affiché
        commandMenu.transform.GetChild(4).gameObject.SetActive(true);  //Tabbleau Qwerty affiché
    }

    public void OnQuit()
    {
        mainMenu = false;
        playerController.pauseMenu.SetActive(false);
        sureMenu.SetActive(true);
    }
    public void OnYes()
    {
        if(mainMenu)
        {
            if (isClientOnly)
            {
                playerController.GetNetworkManager().StopClient();
            }
            else
            {
                playerController.GetNetworkManager().StopHost();
                NetworkServer.DestroyPlayerForConnection(networkConnection);
                NetworkServer.Shutdown();
            }
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Application.Quit();
        }

    }

    public void OnNo()
    {
        sureMenu.SetActive(false);
        playerController.pauseMenu.SetActive(true);
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
