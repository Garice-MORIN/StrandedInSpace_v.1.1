using UnityEngine;
//using UnityEditor.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using Mirror;


namespace Mirror.Discovery {
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
        public GameObject manager;
        public GameObject joinAnchor;
        public GameObject playAnchor;
        public NetworkManager networkManager;
        public InputField inputField;
        public Vector2 position = Vector2.zero;

        //Servers search
        public NetworkDiscovery networkDiscovery;

        private void Start() {
            int hasPlayed = PlayerPrefs.GetInt("hasPlayed");
            if(hasPlayed == 0){
                PlayerPrefs.SetString("name","");
                PlayerPrefs.SetInt("hasPlayed", 1);
                PlayerPrefs.SetInt("krux", 0);
                PlayerPrefs.SetFloat("StartingMoney", 1f);
                PlayerPrefs.SetFloat("MaxHealth", 1f);
                PlayerPrefs.SetFloat("TowerDamage", 1f);
                PlayerPrefs.SetFloat("TowerStatus", 1f);
                PlayerPrefs.SetFloat("TrapDamage", 1f);
                PlayerPrefs.SetFloat("TrapUses", 1f);
            }
            
            volumeSlider.value = 0.5f;
            effectSlider.value = 0.5f;
            musicLevel.text = "50 %";
            effectlevel.text = "50 %";
            //Sets default parameters
            PlayerPrefs.SetFloat("Music", 0.5f);
            PlayerPrefs.SetFloat("Effects", 0.5f);
            PlayerPrefs.SetInt("Inverted", 1);
            PlayerPrefs.SetFloat("Sensi", 1125);
            networkManager = NetworkManager.singleton;
        }
        //Quit button behaviour
        public void OnExitButton() {
            Application.Quit();
        }

        //Start button behaviour
        public void OnStartButton() {
            networkManager.onlineScene = "FinalMap";
            SceneManager.LoadScene("FinalMap");
        }

        public void StartGameAsHost() {
            networkManager.onlineScene = "FinalMap";
            networkManager.StartHost();
            networkDiscovery.AdvertiseServer();
		}

        //test map button behaviour
        public void TestMapButton() {
            networkManager.onlineScene = "TestMap";
            SceneManager.LoadScene("TestMap");
        }

        //Credit button behaviour
        public void MenuToCredits() {
            Destroy(manager);
            SceneManager.LoadScene("Credits");
        }

        //Volume slider behaviour
        public void OnVolumeValueChanged() {
            audioSource.volume = volumeSlider.value;
            PlayerPrefs.SetFloat("Music", volumeSlider.value);
            musicLevel.text = Mathf.FloorToInt(volumeSlider.value * 100) + " %";
        }

        //Effect slider behaviour
        public void OnEffectValueChanged() {
            effectSource.volume = effectSlider.value;
            PlayerPrefs.SetFloat("Effects", effectSlider.value);
            effectlevel.text = Mathf.FloorToInt(effectSlider.value * 100) + " %";
        }

        //Inverted mouse toggle behaviour
        public void OnToggleChanged() {
            inverted = inverted == 1 ? -1 : 1;
            PlayerPrefs.SetInt("Inverted", inverted);
        }

        //Sensitivity slider behaviour
        public void OnSensitivityChanged() {
            PlayerPrefs.SetFloat("Sensi", sensitivitySlider.value);
        }

        public void ChangeIpAdress() {
            networkManager.networkAddress = inputField.text;
            Debug.Log(networkManager.networkAddress);
        }

    }

}
