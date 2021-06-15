using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public Type type;

    public LayerMask mask;
    public Transform enemyPosition;
    public NavMeshAgent navMesh;
    public int damage;
    public float cooldownTime;
    Collider[] colliders;
    Transform goal;

    public float attackCooldown;
    public bool slowed;
    public bool canAttack;
    public float baseSpeed;
    public float slowDuration;
    void Start()
    {
        goal = ChooseTarget(); //Assign AI's goal
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
        yield return new WaitForSeconds(cooldownTime);

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

    Transform FocusRandomPlayer() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        return players[Random.Range(0,players.Length)].transform;
	}

    Transform ChooseTarget() {
        switch(type) {
            case Type.HEAVY:
                return GameObject.FindGameObjectWithTag("Core").transform;
            case Type.FLYING:
                GameObject[] turretsPos = GameObject.FindGameObjectsWithTag("Turret");
                if (turretsPos.Length == 0)
                    return GameObject.FindGameObjectWithTag("Core").transform;
                else
                    return turretsPos[Random.Range(0, turretsPos.Length)].transform;
            case Type.NORMAL:
                int i = Random.Range(0, 2);
                if (i == 0) {
                    return GameObject.FindGameObjectWithTag("Core").transform;
                }
				else {
                    return FocusRandomPlayer();
				}
            default: //Type.BOSS
                return GameObject.FindGameObjectWithTag("Core").transform;
        }
    }
}
