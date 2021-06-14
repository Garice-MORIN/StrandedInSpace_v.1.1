﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TrapSpawning : MonoBehaviour
{
    public NetworkManager GetNetworkManager() => FindObjectOfType<NetworkManager>();
    private int level = 0;
    public int trapType;
    private GameObject toSpawn;
    private GameObject trapPrefab;
    private Vector3 position;
    private Quaternion orientation;

    void Start(){
        position = transform.position + new Vector3(0, 0.1f, 0);
        orientation = Quaternion.Euler(0f, 0f, 0f);
    }
    public int TryBuild(int playerMoney, int trapToBuild) {
        if(level == 0) {
            return Build(playerMoney, trapToBuild);
        }
        return 0;
    }
    public int Build(int playerMoney, int trapToBuild) {
        trapPrefab = GetNetworkManager().GetComponentInParent<SpawnTable>().GetTrap(trapToBuild);
        int priceNeeded = trapPrefab.GetComponent<Money>().money;
        if(playerMoney >= priceNeeded) {
            trapType = trapToBuild;
            level += 1;
            trapPrefab.GetComponent<TrapInfo>().linkedSpawner = this.transform.gameObject;
            toSpawn = (GameObject)Instantiate(trapPrefab, position, orientation);
            NetworkServer.Spawn(toSpawn);
            return priceNeeded;
        }
        return 0;
    }

    public int TryDestroy() {
        if(level != 0) {
            Destroy(toSpawn);
            level = 0;
        }
        return 0;
    }
}