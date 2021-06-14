using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTable : MonoBehaviour
{
    private GameObject[,] towerTable;
    private GameObject[] trapTable;
    
    void Start() {
        towerTable = new GameObject[4,3] 
            {
                {Resources.Load("Basic1") as GameObject, Resources.Load("Basic2") as GameObject, Resources.Load("Basic3") as GameObject},
                {Resources.Load("Electric1") as GameObject, Resources.Load("Electric2") as GameObject, Resources.Load("Electric3") as GameObject},
                {Resources.Load("Fire1") as GameObject, Resources.Load("Fire2") as GameObject, Resources.Load("Fire3") as GameObject},
                {Resources.Load("Slow1") as GameObject, Resources.Load("Slow2") as GameObject, Resources.Load("Slow3") as GameObject}
            };
        trapTable = new GameObject[4] 
            {
                Resources.Load("Trap") as GameObject, 
                Resources.Load("Trap") as GameObject, 
                Resources.Load("Trap") as GameObject, 
                Resources.Load("Trap") as GameObject 
            };
    }
    public GameObject GetTower(int indexTower, int level){
        return towerTable[indexTower,level];
    }
    public GameObject GetTrap(int indextrap){
        return trapTable[indextrap];
    }
}