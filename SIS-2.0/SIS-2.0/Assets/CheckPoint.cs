using UnityEngine;

public class CheckPoint : MonoBehaviour
{
	public Transform coreTransform;
	public Transform checkpoint;

	private void OnTriggerEnter(Collider other) {
		
		if(other.tag == "Enemy" && ShouldChangeDirection(other.GetComponent<EnemyMovement>(),checkpoint)) {
			if (other.GetComponent<EnemyMovement>().type == Type.EXPLOSIVE) {
				EnemyMovement tmp = other.GetComponent<EnemyMovement>();
				tmp.SetGoal(coreTransform);
				GameObject[] barricades = GameObject.FindGameObjectsWithTag("Barricade");
				foreach (var barricade in barricades) {
					if (barricade.GetComponent<BarricadeInfo>().linkedSpawner.GetComponent<BarricadeSpawning>().left == tmp.GetPath().Item2
						&& !barricade.GetComponent<BarricadeInfo>().linkedSpawner.GetComponent<BarricadeSpawning>().rdc)
						tmp.SetGoal(barricade.transform, barricade.GetComponent<BarricadeInfo>().linkedSpawner.GetComponent<BarricadeSpawning>().left ? new Vector3(0.5f, 0, 0) : new Vector3(-0.5f, 0, 0));
				}
			}
			else
				other.GetComponent<EnemyMovement>().SetGoal(coreTransform);
		}
	}

	private bool ShouldChangeDirection(EnemyMovement enemy, Transform checkpoint) {
		return (enemy.type == Type.EXPLOSIVE && enemy.GetGoal() == checkpoint) || (enemy.type != Type.EXPLOSIVE && enemy.type != Type.FLYING);
	}
}
