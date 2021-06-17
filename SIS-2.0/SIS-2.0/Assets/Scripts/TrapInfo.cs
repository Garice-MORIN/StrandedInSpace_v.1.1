using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapInfo : MonoBehaviour
{
    public enum TypeOfTrap{
        AcidPool,
        Explosive,
        Spikes,
        Slow
    }
    public GameObject linkedSpawner;
    public TypeOfTrap typeOfTrap;
    public float attackDelay;
    public float cooldown;
    public int damage;
    public float slowDuration;
    public float slowPower;
    public int usesLeft;
    private bool usedThisTurn;
    
    void Start() {
        cooldown = attackDelay;
    }

    void Update() {
        switch(typeOfTrap) {
            case TypeOfTrap.AcidPool:
                AttackAcidPool();
                break;
            case TypeOfTrap.Explosive:
                AttackExplosive();
                break;
            case TypeOfTrap.Spikes:
                AttackSpikes();
                break;
            case TypeOfTrap.Slow:
                AttackSlow();
                break;
        }
    }
    IEnumerator WaitCoroutine(){
        yield return new WaitForSeconds(1);
    }
    void Attack(Vector3 hitbox, bool applySlow) {
        foreach(RaycastHit rayHit in Physics.BoxCastAll(transform.position, hitbox, transform.forward, new Quaternion(0,0,0,0) , 0.01f)) {
            if(rayHit.transform.gameObject.tag == "Enemy") {
                usedThisTurn = true;
                rayHit.transform.gameObject.GetComponent<Health>().TakeDamage(damage);
                if(applySlow) {
                    rayHit.transform.gameObject.GetComponent<EnemyMovement>().Slow(slowPower, slowDuration);
                }
            }
        }
        if(usedThisTurn) {
            usesLeft -= 1;
            CheckDestroy();
        }
    }
    void AttackAcidPool() {
        if(cooldown <= 0) {
            Attack(new Vector3(1.5f,1f,1.5f), false);
            cooldown = attackDelay;
        }
        else {
            cooldown -= Time.deltaTime;
        }
    }
    void AttackExplosive() {
        if(cooldown <= 0) {
            bool triggered = false;
            foreach(RaycastHit rayHit in Physics.BoxCastAll(transform.position, new Vector3(1.5f,1f,1.5f), transform.forward, new Quaternion(0,0,0,0) , 0.01f)) {
                if(rayHit.transform.gameObject.tag == "Enemy") {
                    triggered = true;
                }
            }
            if(triggered){
                StartCoroutine(WaitCoroutine());
                Attack(new Vector3(3f,2f,3f), false);
                cooldown = attackDelay;
            }
        }
        else {
            cooldown -= Time.deltaTime;
        }
    }
    void AttackSpikes() {
        if(cooldown <= 0) {
            Attack(new Vector3(1.5f,1f,1.5f), true);
            cooldown = attackDelay;
        }
        else {
            cooldown -= Time.deltaTime;
        }
    }
    void AttackSlow() {
        if(cooldown <= 0) {
            Attack(new Vector3(1.5f,1f,1.5f), true);
            cooldown = attackDelay;
        }
        else {
            cooldown -= Time.deltaTime;
        }
    }

    void CheckDestroy(){
        if(usesLeft == 0) {
            linkedSpawner.GetComponent<TrapSpawning>().TryDestroy();
        }
    }
}