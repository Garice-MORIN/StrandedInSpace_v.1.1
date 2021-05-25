using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public LayerMask mask;
    public Transform enemyPosition;
    public NavMeshAgent navMesh;
    public int damage;
    public float cooldownTime;
    Collider[] colliders;
    Transform goal;
    public bool canAttack;
    public bool slowed;
    public float baseSpeed;
    public float slowDuration;
    public float slowPower;
    void Start()
    {
        goal = GameObject.FindGameObjectWithTag("Tower").transform; //Assign AI's goal
        navMesh.destination = goal.position;
        baseSpeed = navMesh.speed;
        canAttack = true;
    }

    public void Update()
    {
        
        //Attack tower if enemy is close enough 
        if(canAttack)
        {
            StartCoroutine(TryAttack());
            canAttack = true;
        }
        
        CheckSlow();
    }

    IEnumerator TryAttack()
    {
        canAttack = false;
        colliders = Physics.OverlapSphere(enemyPosition.position, 2.0f, mask);
        foreach (var obj in colliders)
        {
            obj.GetComponent<Health>().TakeDamage(damage);
        }
        yield return new WaitForSecondsRealtime(cooldownTime);
        
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
