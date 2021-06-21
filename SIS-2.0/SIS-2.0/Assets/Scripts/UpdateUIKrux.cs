using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUIKrux : MonoBehaviour
{
    public Text MyKrux;
    private void Update() {
        MyKrux.text = $"Krux : {PlayerPrefs.GetInt("krux")}";
    }
}
