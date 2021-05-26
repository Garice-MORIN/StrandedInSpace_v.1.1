using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    GameObject[] playerList;
    public RowBehaviour[] rowList;

    
    private void OnEnable()
    {
        playerList = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerList.Length; i++)
        {
            PlayerController playerController = playerList[i].GetComponent<PlayerController>();
            rowList[i].WriteData(playerController._name, playerController.money, playerController.death);
        }
    }
}
