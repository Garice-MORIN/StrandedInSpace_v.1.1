using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TurretSpawning : NetworkBehaviour
{
    public NetworkManager GetNetworkManager() => FindObjectOfType<NetworkManager>();
    private int level = 0;
    public int towerType;
    private GameObject toSpawn;
    private GameObject towerPrefab;
    private Vector3 position;
    private Quaternion orientation;
    void Start(){
        position = transform.position;
        orientation = Quaternion.Euler(0f, 0f, 0f);
    }

    public int TryBuild(int playerMoney, int towerToBuild){
        if(level == 0){
            return Build(playerMoney, towerToBuild);
        }
        else if (level < 3){
            return Upgrade(playerMoney);
        }
        return 0;
    }

    public int Build(int playerMoney, int towerToBuild) {
        towerPrefab = GetNetworkManager().GetComponentInParent<SpawnTable>().GetTower(towerToBuild,0);
        int priceNeeded = towerPrefab.GetComponent<Money>().money;
        if(playerMoney >= priceNeeded) {
            towerType = towerToBuild;
            towerPrefab.GetComponent<TurretInfo>().linkedSpawner = this.transform.gameObject;
            toSpawn = (GameObject)Instantiate(towerPrefab, position, orientation);
            NetworkServer.Spawn(toSpawn);
            UpdateEnemiesGoal(true);
            level += 1;
            return priceNeeded;
        }
        return 0;
    }
    public int Upgrade(int playerMoney) {
        int priceNeeded = toSpawn.GetComponent<Money>().money;
        if(playerMoney >= priceNeeded) {
            Destroy(toSpawn);
            towerPrefab = GetNetworkManager().GetComponentInParent<SpawnTable>().GetTower(towerType,level);
            towerPrefab.GetComponent<TurretInfo>().linkedSpawner = this.transform.gameObject;
            toSpawn = (GameObject)Instantiate(towerPrefab, position, orientation);
            NetworkServer.Spawn(toSpawn);
            level += 1;
            return priceNeeded;
        }
        return 0;
    }

    public int TryDestroy() {
        if(level != 0) {
            Destroy(toSpawn);
            UpdateEnemiesGoal(false, toSpawn);
            level = 0;
            return toSpawn.GetComponent<Money>().money;
        }
        return 0;
    }

    void UpdateEnemiesGoal(bool add, GameObject turretTarget = null) {
        EnemyMovement[] enemies = FindObjectsOfType<EnemyMovement>();
        if(add) {
            for (int i = 0; i < enemies.Length; i++) {
                if (enemies[i].type == Type.FLYING && !enemies[i].goToTurret) {
                    enemies[i].ChooseTarget(true);
                }
            }
        }
		else {
            for (int i = 0; i < enemies.Length; i++) {
                if (enemies[i].type == Type.FLYING && enemies[i].GetFocusedObject() == turretTarget) {
                    enemies[i].ChooseTarget(true);
                }
            }
        }

        
	}
}
