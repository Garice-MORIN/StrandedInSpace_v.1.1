using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class ScoreBoard : NetworkBehaviour
{
    public RowBehaviour[] rowList;

    private void OnEnable() {
        /*var playerList = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerList.Length; i++)
        {
            Debug.Log(playerList[i]);
            PlayerController playerController = playerList[i].GetComponent<PlayerController>();
            rowList[i].WriteData(playerController._name, playerController.money, playerController.death);
        }*/
        //var _stats = GetComponentInParent<PlayerController>().GetNetworkManager().GetComponent<ListOfPlayers>().GetList();
        var players = GameObject.FindGameObjectWithTag("Check").GetComponent<ListOfPlayers>().GetList();
        int i = 0;
        foreach (var key in players) {
            PlayerController controller = key.GetComponent<PlayerController>();
            rowList[i].WriteData(controller._name, controller.money, controller.death);
            i++;
        }
    }

    /*public void CmdCreateRows() {
        var players = GameObject.FindGameObjectWithTag("Check").GetComponent<ListOfPlayers>().GetList();
        int i = 0;
        foreach (var key in players) {
            PlayerController controller = key.GetComponent<PlayerController>();
            rowList[i].WriteData(controller._name, controller.money, controller.death);
            i++;
        }
    }*/
}