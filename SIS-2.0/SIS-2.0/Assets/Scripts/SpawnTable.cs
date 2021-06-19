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
            {Resources.Load("Basic1Tower") as GameObject, Resources.Load("Basic2Tower") as GameObject, Resources.Load("Basic3Tower") as GameObject},
            {Resources.Load("Electric1Tower") as GameObject, Resources.Load("Electric2Tower") as GameObject, Resources.Load("Electric3Tower") as GameObject},
            {Resources.Load("Fire1Tower") as GameObject, Resources.Load("Fire2Tower") as GameObject, Resources.Load("Fire3Tower") as GameObject},
            {Resources.Load("Slow1Tower") as GameObject, Resources.Load("Slow2Tower") as GameObject, Resources.Load("Slow3Tower") as GameObject}
        };
        trapTable = new GameObject[4] 
        {
            Resources.Load("AcidPoolTrap") as GameObject, 
            Resources.Load("ExplosiveTrap") as GameObject, 
            Resources.Load("SpikesTrap") as GameObject, 
            Resources.Load("SlowTrap") as GameObject 
        };
        barricadeTable = new GameObject[2]
        {
            Resources.Load("Barricade") as GameObject,
            Resources.Load("Barricade1") as GameObject
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