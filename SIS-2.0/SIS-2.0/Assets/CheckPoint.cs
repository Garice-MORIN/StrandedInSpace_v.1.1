using UnityEngine;

public class CheckPoint : MonoBehaviour
{
	public Transform coreTransform;
	public Transform checkpoint;

	private void OnTriggerEnter(Collider other) {
		if(other.tag == "Enemy" && ShouldChangeDirection(other.GetComponent<EnemyMovement>(),checkpoint)) {
			other.GetComponent<EnemyMovement>().SetGoal(coreTransform);
		}
	}

	private bool ShouldChangeDirection(EnemyMovement enemy, Transform checkpoint) {
		return (enemy.type == Type.EXPLOSIVE && enemy.GetGoal() == checkpoint) || (enemy.type != Type.EXPLOSIVE && enemy.type != Type.FLYING);
	}
}
