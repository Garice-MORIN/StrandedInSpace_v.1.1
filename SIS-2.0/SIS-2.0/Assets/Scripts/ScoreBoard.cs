using UnityEngine;
using System.Collections.Generic;

public class ScoreBoard : MonoBehaviour
{
    public RowBehaviour[] rowList;

	private void OnEnable()
    {
        /*playerList = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerList.Length; i++)
        {
            PlayerController playerController = playerList[i].GetComponent<PlayerController>();
            rowList[i].WriteData(playerController._name, playerController.money, playerController.death);
            Debug.Log(playerController._name);
        }*/
        //var _stats = GetComponentInParent<PlayerController>().GetNetworkManager().GetComponent<ListOfPlayers>().GetList();
        var _stats = GameObject.FindGameObjectWithTag("Check").GetComponent<ListOfPlayers>().GetList();
        Debug.Log(_stats.Count);
        int i = 0;
        foreach(var key in _stats) {
            rowList[i].WriteData(key.Key, key.Value.Item1, key.Value.Item2);
		}
    }
}