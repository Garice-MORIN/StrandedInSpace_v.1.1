using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class TurretAI : NetworkBehaviour
{
    public enum TypeOfTower{
        Basic,
        Electric,
        Flamethrower,
        Slow
    }
    public TypeOfTower typeOfTower;
    public float range;
    public float attackDelay;
    public float cooldown;
    public int damage;
    public int targetsPerAttack;
    public float slowDuration;
    public float slowPower;
    public LayerMask mask;
    
    void Start(){
        cooldown = attackDelay;
    }
    void Update(){
        switch(typeOfTower){
            case TypeOfTower.Basic:
                AttackBasic();
                break;
            case TypeOfTower.Electric:
                AttackElectric();
                break;
            case TypeOfTower.Flamethrower:
                AttackFlamethrower();
                break;
            case TypeOfTower.Slow:
                AttackSlow();
                break;
        }
    }

    int[] GetRandomNumbers(int maxValue, int length){
        if(maxValue <= 0 || length <= 0){
            throw new ArgumentException("TurretAI.GetRandomNumbers Exception");
        }
        int[] res = new int[length];
        System.Random random = new System.Random();
        for(int i = 0; i < length; i++){
            res[i] = random.Next(maxValue);
        }
        return res;
    }
    void Shoot(GameObject enemy){
        if(enemy != null){
            enemy.GetComponent<Health>().TakeDamage(damage);
        }
    }
    GameObject BasicAim(GameObject[] enemiesLeft){
        int i = 0;
        while(i < enemiesLeft.Length){
            Ray ray = new Ray(transform.position, enemiesLeft[i].transform.position - transform.position);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, range)){
                if(hit.transform.gameObject == enemiesLeft[i]){
                    return enemiesLeft[i];
                }
            }
            i += 1;
        }
        return null;
    }
    void AttackBasic(){
        if(cooldown <= 0){
            GameObject target = BasicAim(GameObject.FindGameObjectsWithTag("Enemy"));
            for(int i = 0; i < targetsPerAttack; i++){
                Shoot(target);
            }
            cooldown = attackDelay;
        }
        else{
            cooldown -= Time.deltaTime;
        }
    }
    GameObject[] ElectricAim(GameObject[] enemiesLeft){
        int i = 0;
        int maxEnemiesAtRange = 0;
        GameObject[] enemiesPossible = new GameObject[enemiesLeft.Length];
        while(i < enemiesLeft.Length){
            Ray ray = new Ray(transform.position, enemiesLeft[i].transform.position - transform.position);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, range)){
                if(hit.transform.gameObject == enemiesLeft[i]){
                    enemiesPossible[maxEnemiesAtRange] = enemiesLeft[i];
                    maxEnemiesAtRange += 1;
                }
            }
            i += 1;
        }
        if(maxEnemiesAtRange != 0){
            int j = 0;
            GameObject[] enemiesAimed = new GameObject[targetsPerAttack];
            foreach(int enemyValue in GetRandomNumbers(maxEnemiesAtRange, targetsPerAttack)){
                enemiesAimed[j] = enemiesPossible[enemyValue];
                j += 1;
            }
            return enemiesAimed;
        }
        return new GameObject[] {null};
    }
    void AttackElectric(){
        if(cooldown <= 0){
            foreach(GameObject enemy in ElectricAim(GameObject.FindGameObjectsWithTag("Enemy"))){
                Shoot(enemy);
            }
            cooldown = attackDelay;
        }
        else{
            cooldown -= Time.deltaTime;
        }
    }
    void AttackFlamethrower(){}
    
    void Slow(GameObject enemy){
        if(enemy != null){
            enemy.GetComponent<EnemyMovement>().slowDuration = slowDuration;
            enemy.GetComponent<EnemyMovement>().slowPower = slowPower;
            enemy.GetComponent<EnemyMovement>().slowed = true;
        }
    }
    GameObject[] SlowAim(GameObject[] enemiesLeft){
        int i = 0;
        int maxEnemiesAtRange = 0;
        GameObject[] enemiesPossible = new GameObject[enemiesLeft.Length];
        while(i < enemiesLeft.Length){
            Ray ray = new Ray(transform.position, enemiesLeft[i].transform.position - transform.position);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, range)){
                if(hit.transform.gameObject == enemiesLeft[i]){
                    enemiesPossible[maxEnemiesAtRange] = enemiesLeft[i];
                    maxEnemiesAtRange += 1;
                }
            }
            i += 1;
        }
        if(maxEnemiesAtRange != 0){
            GameObject[] enemiesAimed = new GameObject[maxEnemiesAtRange];
            for(int j = 0; j < maxEnemiesAtRange; j++){
                enemiesAimed[j] = enemiesPossible[j];
            }
            return enemiesAimed;
        }
        return new GameObject[] {null};
    }
    void AttackSlow(){
        if(cooldown <= 0){
            foreach(GameObject enemy in SlowAim(GameObject.FindGameObjectsWithTag("Enemy"))){
                Slow(enemy);
            }
            cooldown = attackDelay;
        }
        else{
            cooldown -= Time.deltaTime;
        }
    }

}
