using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BarricadeSpawning : MonoBehaviour
{
    public NetworkManager GetNetworkManager() => FindObjectOfType<NetworkManager>();
    private int level = 0;
    private GameObject toSpawn;
    private GameObject barricadePrefab;
    private Vector3 position;
    private Quaternion orientation;
    public bool rotNeeded;
    void Start(){
        position = transform.position + new Vector3(0, 1f, 0);
        orientation = Quaternion.Euler(0f, rotNeeded ? 0f : 90f, 0f);
    }
    public int TryBuild(int playerMoney) {
        if(level == 0) {
            return Build(playerMoney);
        }
        return 0;
    }
    public int Build(int playerMoney)
    {
        barricadePrefab = GetNetworkManager().GetComponentInParent<SpawnTable>().GetBarricade(0);
        int priceNeeded = barricadePrefab.GetComponent<Money>().money;
        if(playerMoney >= priceNeeded) {
            level += 1;
            barricadePrefab.GetComponent<BarricadeInfo>().linkedSpawner = this.transform.gameObject;
            toSpawn = (GameObject)Instantiate(barricadePrefab, position, orientation);
            NetworkServer.Spawn(toSpawn);
            return priceNeeded;
        }
        return 0;
    }
    public void TryDestroy(){
        Destroy(toSpawn);
        level = 0;
    }
}
