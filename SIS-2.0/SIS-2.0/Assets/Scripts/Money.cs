using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Money : MonoBehaviour
{
        public int money;
        //Money is a script added to all entity with a link to money
        //      Players -> Money each player have
        //      Enemy -> Money each enemy drop on their death (see Health script for attribution)
        //      Turret -> Money needed to upgrade to the next level (on destroy give back a % of the price to the next level)

        public void EnemyDropMoney(){
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach(GameObject player in players){
                        player.GetComponent<Money>().money += money / players.Length;
                }
        }
}