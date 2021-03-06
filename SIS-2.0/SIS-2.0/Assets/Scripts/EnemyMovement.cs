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
    const int IGNOREWALLS = 1;

    public Type type;

    public LayerMask mask;
    public Transform enemyPosition;
    public NavMeshAgent navMesh;
    public bool isToGoRight;
    public int damage;
    public float cooldownTime;
    Collider[] colliders;
    GameObject target = null;
    Transform goal;
    private bool hasPassedFirstBarricade = false;
    private bool hasPassedSecondBarricade = false;
    AudioSource explosion;
    //Animator animator;
    
    public int explosionDamage;
    public bool slowed;
    private bool canAttack;
    public float baseSpeed;
    public float slowDuration;
    void Start()
    {
        //animator = GetComponent<Animator>();
        if (type == Type.EXPLOSIVE)
            explosion = GetComponent<AudioSource>();
        ChooseForcedPath();
        ChooseTarget(); //Assign AI's goal
        navMesh.destination = goal.position;
        baseSpeed = navMesh.speed;
        canAttack = true;
    }   

    public void Update() {

        //Attack target if enemy is close enough
        if (canAttack && type != Type.EXPLOSIVE)
        {
            StartCoroutine(TryAttack());
        }
        if(type == Type.FLYING && !(goal is null)) {
            navMesh.destination = goal.position;
        }

        CheckSlow();
    }

	private void OnTriggerEnter(Collider other) {
        Debug.Log(other.tag);
		if(other.tag == "Barricade") {
            Debug.Log("Boom");
            StartCoroutine(CommitSuicide());
		}
	}

    IEnumerator CommitSuicide() {
        yield return new WaitForSecondsRealtime(1f);
        colliders = Physics.OverlapBox(transform.position, new Vector3(1.5f, 0.5f, 1.5f));
        foreach (var touchedObject in colliders) {
            if(touchedObject.tag == "Barricade") {
                touchedObject.GetComponent<BarricadeInfo>().explosionLeft -= 1;
			}
            else if(touchedObject.tag == "Turret") {
                touchedObject.GetComponent<Health>().TakeDamage(1);
			}
            else if(touchedObject.tag == "Player") {
                touchedObject.GetComponent<Health>().TakeDamage(explosionDamage);
			}
		}
        explosion.Play();
        GameObject.FindGameObjectWithTag("Spawner").GetComponent<EnemiesSpawner>().enemiesLeft -= 1;
        Destroy(gameObject);
	}

	IEnumerator TryAttack()
    {
        canAttack = false;
        colliders = Physics.OverlapSphere(enemyPosition.position, 2.0f, mask);
        foreach (var obj in colliders)
        {
			try {
                if (obj.tag == "Player" )
                    obj.GetComponent<Health>().TakeDamage(damage);
                else if(obj.tag == "Core")
                    obj.GetComponent<Health>().TakeDamage(type == Type.BOSS ? 5 : 1);
                else if (obj.tag == "Turret" && type == Type.BOSS)
                    obj.GetComponent<Health>().TakeDamage(1);
                else if (obj.tag == "Barricade")
                    obj.GetComponent<BarricadeInfo>().explosionLeft -= 2;
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
            Debug.Log(target);
            Debug_ShowDestination(target.transform);
            goal = target.transform;
        }
		else if(type == Type.EXPLOSIVE) {
            goal = GameObject.FindGameObjectWithTag("Checkpoint").transform;
            GameObject[] barricades = GameObject.FindGameObjectsWithTag("Barricade");
            foreach(var barricade in barricades) {
                if (barricade.GetComponent<BarricadeInfo>().linkedSpawner.GetComponent<BarricadeSpawning>().left == GetPath().Item1
                    && barricade.GetComponent<BarricadeInfo>().linkedSpawner.GetComponent<BarricadeSpawning>().rdc)
                    SetGoal(barricade.transform);
			}
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

    public static string Debug_ShowDestination(Transform destination) {
        if (destination is null)
            return $"Destination is null";
        return ($"{destination.position.x}|{destination.position.y}|{destination.position.z}");
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
            navMesh.areaMask = ALL + 1;
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
