using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

public class EnemyMovement : MonoBehaviour
{
    //Constants used to give a particular path to the enemy
    const int GG = 336;
    const int DD = 392;
    const int GD = 400;
    const int DG = 328;
    const int ALL = 472;

    public Type type;

    public LayerMask mask;
    public Transform enemyPosition;
    public NavMeshAgent navMesh;
    public bool isToGoRight;
    public int damage;
    public float cooldownTime;
    Collider[] colliders;
    GameObject target;
    Transform goal;
    public bool goToTurret;
    private bool hasPassedFirstBarricade = false;
    private bool hasPassedSecondBarricade = false;
    
    public float attackCooldown;
    public bool slowed;
    public bool canAttack;
    public float baseSpeed;
    public float slowDuration;
    void Start()
    {
        ChooseTarget(); //Assign AI's goal
        ChooseForcedPath();
        navMesh.destination = goal.position;
        baseSpeed = navMesh.speed;
        canAttack = true;
    }

    public void Update() {
        //Attack target if enemy is close enough
        if(canAttack)
        {
            StartCoroutine(TryAttack());
        }

        if(type == Type.FLYING && !(goal is null)) {
            navMesh.destination = goal.position;
        }

        CheckSlow();
    }

    IEnumerator TryAttack()
    {
        canAttack = false;
        colliders = Physics.OverlapSphere(enemyPosition.position, 2.0f, mask);
        foreach (var obj in colliders)
        {
			try {
                obj.GetComponent<Health>().TakeDamage(damage);
            }
			catch {
                continue;
			}

        }
        yield return new WaitForSeconds(cooldownTime);
        canAttack = true;
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

    GameObject FocusRandomPlayer() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        target = players[UnityEngine.Random.Range(0, players.Length)];
        return target;
	}

    public void ChooseTarget() {
        if(type == Type.FLYING) {
            target = FocusRandomPlayer();
            goal = target.transform;
        }
        else
            goal = GameObject.FindGameObjectWithTag("Checkpoint").transform;
    }

    public GameObject GetFocusedObject() {
        return target;
	}

    public void SetGoal(Transform goal, Vector3 offset = new Vector3()) {
        this.goal = goal;
        navMesh.destination = goal.position + offset;
	}

    public Transform GetGoal() {
        return goal;
	}

    public void Debug_ShowDestination(Transform destination) {
        Debug.Log($"{destination.position.x}|{destination.position.y}|{destination.position.z}");
	}

    public void SwitchCheckpoint(bool rdc) {
        if (rdc)
            hasPassedFirstBarricade = true;
        else
            hasPassedSecondBarricade = true;
	}

    public bool GetPassedCheckpoint(bool rdc) {
        return rdc ? hasPassedFirstBarricade : hasPassedSecondBarricade;
	}

    void ChooseForcedPath() {
        if(type == Type.FLYING) {
            navMesh.areaMask = ALL;
		}
        int rnd = UnityEngine.Random.Range(1, 5);
        switch(rnd) {
            case 1:
                navMesh.areaMask = GG;
                break;
            case 2:
                navMesh.areaMask = DG;
                break;
            case 3:
                navMesh.areaMask = DD;
                break;
            case 4:
                navMesh.areaMask = GD;
                break;
		}
	}

    public (bool,bool) GetPath() {
        switch(navMesh.areaMask) {
            case GG:
                return (true, true);
            case DG:
                return (false, true);
            case GD:
                return (true, false);
            case DD:
                return (false, false);
            default:
                throw new Exception("Unknown Path");
		}
	}
}
