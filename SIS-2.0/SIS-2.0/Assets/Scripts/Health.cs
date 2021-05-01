using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Health : NetworkBehaviour
{
    public int maxHP;// = 100;

    [SyncVar(hook = "OnChangeHealth")]
    public int health;// = maxHP; 

    public RectTransform HPBar;
    public RectTransform background;

    public bool destroyOnDeath;

    private NetworkStartPosition[] spawnPoints;

    private void Start()
    {
        health = maxHP;
        if(isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
        background.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxHP);
        HPBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxHP);
        HPBar.localPosition = new Vector3(maxHP/2, 0, 0);
    }

    public void TakeDamage(int damage)
    {   
        if(!isServer)
        {
            return;
        }
        health -= damage;

        if(health <= 0){
            if(destroyOnDeath)
            {
                Destroy(gameObject);
                FindObjectOfType<EnemiesSpawner>().enemiesLeft--;
            }
            else
            {
                health = maxHP;
                RpcRespawn();
            }
        }

    }
    
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

    public void OnChangeHealth(int oldHealth, int newHealth)
    {
        HPBar.sizeDelta = new Vector2(newHealth, HPBar.sizeDelta.y);  //Actualise la taille de la barre de vie 
    }

}
