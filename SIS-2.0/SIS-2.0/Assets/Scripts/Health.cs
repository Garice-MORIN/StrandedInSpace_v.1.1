﻿using UnityEngine;
using System.Collections;
using Mirror;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    public int maxHP;

    [SyncVar(hook = "OnChangeHealth")]
    public int health;

    //public RectTransform HPBar;
    //public RectTransform background;

    public bool destroyOnDeath;
    private bool dead = false;
    private NetworkStartPosition[] spawnPoints;
    private bool doDrop;
    public bool hasHealthBar;
    public EnemyKill check;
    public Text life;

    private void Start(){
        if (isLocalPlayer){
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
            maxHP = (int)(maxHP * PlayerPrefs.GetFloat("MaxHealth"));
            gameObject.GetComponent<PlayerController>().life.text = $"{maxHP} / {maxHP}";
        }
        check = FindObjectOfType<EnemyKill>();
        health = maxHP;
        if (!hasHealthBar)
            life = null;
        else
            life.text = health.ToString();

        
        if(hasHealthBar) {
            //Initialize health bars
            /*background.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxHP);
            HPBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxHP);
            HPBar.localPosition = new Vector3(maxHP / 2, 0, 0);*/
        }
        
    }

    public IEnumerator GetBurned(int burnDamage, int nbTick){
        for(int i = 0; i < nbTick; i++){
            yield return new WaitForSeconds(0.5f);
            if(!dead){
                TakeDamage(burnDamage);
            }
        }
    }

    //Give damage to entity
    public void TakeDamage(int damage){
        if(!isServer){
            return;
        }

        health -= damage;

        if(health <= 0){
            if(destroyOnDeath){
                if(tag == "Core")
                {
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    EnemyMovement[] enemies = FindObjectsOfType<EnemyMovement>();
                    foreach(var mov in enemies) {
                        mov.enabled = false;
					}
                    foreach (var player in players)
                    {
                        player.GetComponent<PlayerController>().OnEndGame(false);
                    }
                }
                else if(tag == "Enemy")
                {
                    dead = true;
                    doDrop = Random.Range(0.0f, 1.0f) < 0.6f; //Check if entity drop ammunition on death
                    check.killedEnemies.Add(gameObject.GetComponent<EnemyType>().type);
                    gameObject.GetComponent<Money>().EnemyDropMoney();
                    FindObjectOfType<EnemiesSpawner>().enemiesLeft--;
                    if (doDrop) {
                        //Give money to all players
                        //Spawn ammo crate
                        Vector3 position = gameObject.transform.position + new Vector3(0, -0.5f, 0);
                        var orientation = Quaternion.Euler(0f, 0f, 0f);
                        var toSpawn = (GameObject)Instantiate(Resources.Load("munitions") as GameObject, position, orientation);
                        NetworkServer.Spawn(toSpawn);
                    }
                }
                Destroy(gameObject);
            }
            else{
                health = maxHP;
                gameObject.GetComponent<PlayerController>().death += 1;
                RpcRespawn();
                EnemyMovement[] enemies = FindObjectsOfType<EnemyMovement>();
                for(int i = 0; i < enemies.Length; i++) {
                    if (enemies[i].type == Type.NORMAL && enemies[i].GetFocusedObject() == gameObject)
                        enemies[i].ChooseTarget();
				}
                
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
        if (hasHealthBar)
            life.text = newHealth.ToString();
        if (tag == "Player")
            gameObject.GetComponent<PlayerController>().life.text = $"{newHealth.ToString()} / {maxHP}";
            //HPBar.sizeDelta = new Vector2(newHealth, HPBar.sizeDelta.y);
        
    }
}
