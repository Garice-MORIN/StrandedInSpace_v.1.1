using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateName : MonoBehaviour
{
    public Text nameText;
    public Text placeHolder;
    private void Start() {
        if(PlayerPrefs.GetString("name") != ""){
            placeHolder.text = PlayerPrefs.GetString("name");
        }
    }
    
    public void ChangeName(){
        PlayerPrefs.SetString("name", nameText.text);
    }
}
