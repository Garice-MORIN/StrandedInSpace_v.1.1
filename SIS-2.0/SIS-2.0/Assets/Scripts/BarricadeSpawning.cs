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
    public bool rdc;
    public bool left;
    void Start(){
        position = transform.position + new Vector3(0, 0, -1.5f);
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
            barricadePrefab.GetComponent<BarricadeInfo>().linkedSpawner = transform.gameObject;
            toSpawn = (GameObject)Instantiate(barricadePrefab, position, orientation);
            NetworkServer.Spawn(toSpawn);

            foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
                EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
                if (ShouldChangeDirection(enemyMovement,rdc)) {
                    if (rdc)
                        enemyMovement.SetGoal(transform);
                    else
                        enemyMovement.SetGoal(transform, left ? new Vector3(0.5f, 0, 0) : new Vector3(-0.5f, 0,0));
                }
            }
            return priceNeeded;
        }
        return 0;
    }
    public void TryDestroy(){
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
            EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
            if (enemyMovement.GetGoal() == transform) {
                if (rdc)
                    enemyMovement.SetGoal(GameObject.FindGameObjectWithTag("Checkpoint").transform);
                else
                    enemyMovement.SetGoal(GameObject.FindGameObjectWithTag("Core").transform);
            }
        }
        Destroy(toSpawn);
        level = 0;
    }

    private bool ShouldChangeDirection(EnemyMovement enemy, bool rdc) {
        (bool, bool) path = enemy.GetPath();
        bool isExplosive = enemy.type == Type.EXPLOSIVE;
        if(rdc)
            return isExplosive && !enemy.GetPassedCheckpoint(true) && path.Item1 == left;
        else
            return isExplosive && !enemy.GetPassedCheckpoint(false) && path.Item2 == left;

	}
}
