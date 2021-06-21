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
    private float upgradeDamageKept;
    private float upgradeStatusKept;
    public float upgradeDamageSent;
    public float upgradeStatusSent;
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
            upgradeDamageKept = upgradeDamageSent;
            upgradeStatusKept = upgradeStatusSent;
            towerPrefab.GetComponent<TurretInfo>().damage = (int)(towerPrefab.GetComponent<TurretInfo>().damage * upgradeDamageKept);
            towerPrefab.GetComponent<TurretInfo>().statusDuration = (int)(towerPrefab.GetComponent<TurretInfo>().statusDuration * upgradeStatusKept);
            towerPrefab.GetComponent<TurretInfo>().linkedSpawner = this.transform.gameObject;
            toSpawn = (GameObject)Instantiate(towerPrefab, position, orientation);
            NetworkServer.Spawn(toSpawn);
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
            towerPrefab.GetComponent<TurretInfo>().damage = (int)(towerPrefab.GetComponent<TurretInfo>().damage * upgradeDamageKept);
            towerPrefab.GetComponent<TurretInfo>().statusDuration = (int)(towerPrefab.GetComponent<TurretInfo>().statusDuration * upgradeStatusKept);
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
            level = 0;
            return toSpawn.GetComponent<Money>().money;
        }
        return 0;
    }

}
