using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
	public Transform coreTransform;
	private void OnTriggerEnter(Collider other) {
		if(other.tag == "Enemy" && other.GetComponent<EnemyMovement>().type != Type.FLYING) {
			other.GetComponent<EnemyMovement>().SetGoal(coreTransform);
		}
	}
}
