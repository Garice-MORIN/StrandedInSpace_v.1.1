using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTable : MonoBehaviour
{
    private GameObject[,] towerTable;
    
    void Start() {
        towerTable = new GameObject[4,3] 
            {
                {Resources.Load("Basic1") as GameObject, Resources.Load("Basic2") as GameObject, Resources.Load("Basic3") as GameObject},
                {Resources.Load("Electric1") as GameObject, Resources.Load("Electric2") as GameObject, Resources.Load("Electric3") as GameObject},
                {Resources.Load("Fire1") as GameObject, Resources.Load("Fire2") as GameObject, Resources.Load("Fire3") as GameObject},
                {Resources.Load("Slow1") as GameObject, Resources.Load("Slow2") as GameObject, Resources.Load("Slow3") as GameObject}
            }; 
    }
    public GameObject GetTower(int indexTower, int level){
        return towerTable[indexTower,level];
    }
}