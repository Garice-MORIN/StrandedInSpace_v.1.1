using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ButtonBehaviour : MonoBehaviour
{
	NetworkManager networkManager;
	public string adress;

	public void OnButtonClicked() {
		networkManager = FindObjectOfType<NetworkManager>();
		networkManager.networkAddress = adress;
		networkManager.StartClient();
	}
}
