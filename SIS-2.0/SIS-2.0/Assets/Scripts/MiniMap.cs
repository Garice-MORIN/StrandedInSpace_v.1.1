using UnityEngine;

public class MiniMap : MonoBehaviour
{
	// Start is called before the first frame update
	public GameObject player;

	private Vector3 newPos;

	void LateUpdate()
	{
		newPos = player.transform.position;
		newPos.y = player.transform.position.y < 1 ? -1f : 5f;
		transform.position = newPos;		
	}
}
