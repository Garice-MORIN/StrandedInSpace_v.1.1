using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    GameObject[] playerList;
    public Text[] texts;

    
    private void OnEnable()
    {
        playerList = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerList.Length; i++)
        {
            texts[i].color = Color.white;
            texts[i].text = $"Player 1:                     {playerList[i].GetComponent<PlayerController>().money}                             nb";
        }
    }
}
