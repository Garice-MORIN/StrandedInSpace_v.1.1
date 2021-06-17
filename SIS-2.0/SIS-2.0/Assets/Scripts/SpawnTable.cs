using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTable : MonoBehaviour
{
    private GameObject[,] towerTable;
    private GameObject[] trapTable;
    private GameObject[] barricadeTable;
    private AudioClip[] gunSoundTable;
    void Start() {
        towerTable = new GameObject[4,3] 
        {
            {Resources.Load("Basic11") as GameObject, Resources.Load("Basic12") as GameObject, Resources.Load("Basic13") as GameObject},
            {Resources.Load("Electric11") as GameObject, Resources.Load("Electric12") as GameObject, Resources.Load("Electric13") as GameObject},
            {Resources.Load("Fire11") as GameObject, Resources.Load("Fire12") as GameObject, Resources.Load("Fire13") as GameObject},
            {Resources.Load("Slow11") as GameObject, Resources.Load("Slow12") as GameObject, Resources.Load("Slow13") as GameObject}
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
        gunSoundTable = new AudioClip[4] 
        {
            Resources.Load("EmptyGun") as AudioClip,
            Resources.Load("GunFire") as AudioClip,
            Resources.Load("Pick up") as AudioClip,
            Resources.Load("Reload") as AudioClip
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
    public AudioClip GetGunSound(int indexSound){
        return gunSoundTable[indexSound];
    }
}