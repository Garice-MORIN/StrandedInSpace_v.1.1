using UnityEngine;

public class CheckpointBehaviour : MonoBehaviour
{
    public bool rdc;

	private void OnTriggerEnter(Collider other) {
		if(other.tag == "Enemy" && other.GetComponent<EnemyMovement>().type == Type.EXPLOSIVE) {
			other.GetComponent<EnemyMovement>().SwitchCheckpoint(rdc);
		}
	}
}