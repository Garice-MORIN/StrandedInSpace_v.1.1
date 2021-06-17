using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTable : MonoBehaviour
{
    private GameObject[,] towerTable;
    private GameObject[] trapTable;
    private GameObject[] barricadeTable;
    void Start() {
        towerTable = new GameObject[4,3] 
            {
            {Resources.Load("Basic11") as GameObject, Resources.Load("Basic12") as GameObject, Resources.Load("Basic13") as GameObject},
            {Resources.Load("Electric11") as GameObject, Resources.Load("Electric2") as GameObject, Resources.Load("Electric3") as GameObject},
            {Resources.Load("Fire11") as GameObject, Resources.Load("Fire2") as GameObject, Resources.Load("Fire3") as GameObject},
            {Resources.Load("Slow11") as GameObject, Resources.Load("Slow2") as GameObject, Resources.Load("Slow3") as GameObject}
        };
        trapTable = new GameObject[4] 
        {
            Resources.Load("Trap") as GameObject, 
            Resources.Load("Trap") as GameObject, 
            Resources.Load("Trap") as GameObject, 
            Resources.Load("Trap") as GameObject 
        };
        barricadeTable = new GameObject[2]
        {
            Resources.Load("Barricade") as GameObject,
            Resources.Load("Barricade") as GameObject
        };
    }
    public GameObject GetTower(int indexTower, int level){
        return towerTable[indexTower,level];
    }
    public GameObject GetTrap(int indextrap){
        return trapTable[indextrap];
    }
    public GameObject GetBarricade(int indexBarricade){
        return barricadeTable[indexBarricade];
    }
}