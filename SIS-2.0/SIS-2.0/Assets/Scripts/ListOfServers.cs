using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListOfServers : MonoBehaviour
{
	public GameObject button;
	List<GameObject> buttonList = new List<GameObject>();

	public GameObject CreateButton(string text, string adress) {
		button.GetComponentInChildren<Text>().text = text;
		button.GetComponent<ButtonBehaviour>().adress = adress;
		buttonList.Add(button);
		var newButton = Instantiate(button);
		newButton.transform.SetParent(transform);
		return newButton;
	}


	public void DestroyAllButtons() {
		foreach (Transform button in transform) {
			GameObject.Destroy(button.gameObject);
		}
		buttonList.Clear();
	}

}
