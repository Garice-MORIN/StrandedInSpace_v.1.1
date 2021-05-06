using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public LayerMask mask;
    public Transform enemyPosition;
    public NavMeshAgent navMesh;
    public int damage;
    Collider[] colliders;
    Transform goal;
    public float attackCooldown;
    public bool slowed;
    public float baseSpeed;
    public float slowDuration;
    public float slowPower;
    void Start()
    {
        goal = GameObject.FindGameObjectWithTag("Tower").transform; //Assign AI's goal
        navMesh.destination = goal.position;
        baseSpeed = navMesh.speed;
    }

    public void Update()
    {
        //Attack tower if enemy is close enough 
        CheckAttack();
        CheckSlow();
    }

    //TODO: Optimize this function
    void CheckAttack(){
        if(attackCooldown <= 0){
            AttackTower();
            attackCooldown = 1;
        }
        else{
            attackCooldown -= Time.deltaTime;
        }
    }

    void AttackTower(){
        colliders = Physics.OverlapSphere(enemyPosition.position, 2.0f, mask);
        foreach(var obj in colliders){
            obj.GetComponent<Health>().TakeDamage(damage);
        }
    }

    //Check if enemy had been slowed and if it should be unslowed
    void CheckSlow(){
        if(slowed){
            navMesh.speed = baseSpeed * slowPower;
            slowed = false;
        }
        if(slowDuration <= 0){
            navMesh.speed = baseSpeed;
        }
        else{
            slowDuration -= Time.deltaTime;
        }
    }
}
