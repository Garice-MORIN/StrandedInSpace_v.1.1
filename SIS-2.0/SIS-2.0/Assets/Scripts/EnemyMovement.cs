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
<<<<<<< Updated upstream
    public float attackCooldown;
    public bool slowed;
=======
    public bool canAttack;
>>>>>>> Stashed changes
    public float baseSpeed;
    public float slowDuration;
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

    public void Slow(float slowPower1, float slowDuration1){
        if(navMesh.speed == 0){
            return;
        }
        if(navMesh.speed < baseSpeed * slowPower1){
            slowDuration = slowDuration < slowDuration1 ? slowDuration1 : slowDuration;
            return;
        }
        navMesh.speed = baseSpeed * slowPower1;

    }
    //Check if enemy had been slowed and if it should be unslowed
    void CheckSlow(){
        if(slowDuration <= 0){
            navMesh.speed = baseSpeed;
        }
        else{
            slowDuration -= Time.deltaTime;
        }
    }
}
