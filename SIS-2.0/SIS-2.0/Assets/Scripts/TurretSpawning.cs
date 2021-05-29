using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TurretSpawning : NetworkBehaviour
{
    public bool upgrade = false;
    public bool created = false;
    public bool destroy = false;
    private int i = 0; 
    private GameObject toSpawn;
    private GameObject towerPrefab;
    private GameObject towerPrefab1;
    private GameObject towerPrefab2;
    private GameObject towerPrefab3;
    private Vector3 position;
    private Quaternion orientation;
    void Start(){
        towerPrefab1 = Resources.Load("Basic1") as GameObject;
        towerPrefab2 = Resources.Load("Basic2") as GameObject;
        towerPrefab3 = Resources.Load("Basic3") as GameObject;
        position = transform.position + new Vector3(0, 2f, 0);
        orientation = Quaternion.Euler(0f, 0f, 0f);
    }
    void Update(){    
        
    }
    GameObject DefTowerPrefab(int i){
        switch(i){
            case 0:
                return towerPrefab1;
            case 1:
                return towerPrefab2;
            case 2:
                return towerPrefab3;
            default:
                throw new ArgumentException("The level of the tower mumst be between 0 and 2");
        }
    }
    public int TryUpgrade(int playerMoney){
        upgrade = true;
        int priceNeeded = i == 0 ? GetComponent<Money>().money : toSpawn.GetComponent<Money>().money;
        if(upgrade && i < 3 && playerMoney >= priceNeeded){ 
            if(i != 0){
                Destroy(toSpawn);
            }
            towerPrefab = DefTowerPrefab(i); 
            toSpawn = (GameObject)Instantiate(towerPrefab, position, orientation);
            NetworkServer.Spawn(toSpawn);
            created = true;
            destroy = false;
            upgrade = false;
            i += 1;
            return playerMoney - priceNeeded;
        }
        return playerMoney;
    }
    public int TryDestroy(int playerMoney){
        destroy = true;
        if(destroy && created){
            Destroy(toSpawn);
            created = false;
            upgrade = false;
            i = 0;
            return playerMoney += toSpawn.GetComponent<Money>().money;
        }
        return playerMoney;
    }
}
