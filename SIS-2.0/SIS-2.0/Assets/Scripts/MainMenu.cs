using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class MainMenu : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource effectSource;
    public Slider volumeSlider;
    public Slider effectSlider;
    public GameObject anchor;
    public Slider sensitivitySlider;
    public int inverted = 1;
    public Text musicLevel;
    public Text effectlevel;
    public NetworkManager networkManager;

    private void Start()
    {
        volumeSlider.value = 0.5f;
        effectSlider.value = 0.5f;
        musicLevel.text = "50 %";
        effectlevel.text = "50 %";
        PlayerPrefs.SetFloat("Music", 0.5f);
        PlayerPrefs.SetFloat("Effects", 0.5f);
        PlayerPrefs.SetInt("Inverted", 1);
        PlayerPrefs.SetFloat("Sensi", 1125);

    }

    public void OnExitButton()
    {
        Application.Quit();
    }

    public void OnStartButton()
    {
        networkManager.onlineScene = "FinalMap";
        SceneManager.LoadScene("FinalMap");
    }

    public void TestMapButton()
    {
        networkManager.onlineScene = "TestMap";
        SceneManager.LoadScene("TestMap");
    }

    public void MenuToCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void OnVolumeValueChanged()
    {
        audioSource.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("Music", volumeSlider.value);
        musicLevel.text = Mathf.FloorToInt(volumeSlider.value * 100) + " %";
    }

    public void OnEffectValueChanged()
    {
        effectSource.volume = effectSlider.value;
        PlayerPrefs.SetFloat("Effects", effectSlider.value);
        effectlevel.text = Mathf.FloorToInt(effectSlider.value * 100) + " %";
    }

    public void OnToggleChanged() //Coefficient qui permet d'inverser la souris
    {
        inverted = inverted == 1 ? -1 : 1;
        PlayerPrefs.SetInt("Inverted", inverted);
    }


    public void OnSensitivityChanged()
    {
        PlayerPrefs.SetFloat("Sensi", sensitivitySlider.value);
    }
}
