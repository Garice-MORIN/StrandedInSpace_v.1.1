using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Health : NetworkBehaviour
{
    public int maxHP;

    [SyncVar(hook = "OnChangeHealth")]
    public int health;

    public RectTransform HPBar;
    public RectTransform background;

    public bool destroyOnDeath;

    private NetworkStartPosition[] spawnPoints;
    private bool doDrop;

    private void Start(){
        health = maxHP;
        if(isLocalPlayer){
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
        //Initialize health bars
        background.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxHP);
        HPBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxHP);
        HPBar.localPosition = new Vector3(maxHP/2, 0, 0);
    }

    //Give damage to entity
    public void TakeDamage(int damage){   
        if(!isServer){
            return;
        }
        health -= damage;

        if(health <= 0){
            doDrop = gameObject.tag == "Enemy" ? true : false; //Check if entity drop ammunition on death
            //TODO: Add a probability to drop ammunations
            if(destroyOnDeath){
                if(doDrop){
                    //Give money to all players
                    gameObject.GetComponent<Money>().EnemyDropMoney();
                    //Spawn ammo crate
                    Vector3 position = gameObject.transform.position + new Vector3(0,-0.5f,0);
                    var orientation = Quaternion.Euler(0f, 0f, 0f);
                    var toSpawn = (GameObject)Instantiate(Resources.Load("munitions") as GameObject, position, orientation);
                    NetworkServer.Spawn(toSpawn);
                }
                Destroy(gameObject);
                FindObjectOfType<EnemiesSpawner>().enemiesLeft--;
            }
            else{
                health = maxHP;
                RpcRespawn();
            }
        }
    }
    
    //Respawn player
    [ClientRpc]
    public void RpcRespawn()
    {
        if(isLocalPlayer)
        {
            Vector3 spawnPoint = new Vector3(0f, 1f, 0f);
            if(spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }
            transform.position = spawnPoint;
        }
    }

    //Update health bar when damage is received
    public void OnChangeHealth(int oldHealth, int newHealth)
    {
        HPBar.sizeDelta = new Vector2(newHealth, HPBar.sizeDelta.y);
    }
}
