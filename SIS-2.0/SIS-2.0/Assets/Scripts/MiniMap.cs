using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
	// Start is called before the first frame update
	public GameObject player;

	private Vector3 newPos;
	private Vector3 offset;
	private float height;

	void Start()
	{
		//Récupération de la position locale de départ
		offset = transform.position - player.transform.position;
		//Récupération de la position y de départ du personnage (puisque l'offset sera ajoutée par la suite)
		height = player.transform.position.y;
	}

	void LateUpdate()
	{
		//Récupération de la position du personnage avec la position y initiale (donc sans le saut)
		newPos = new Vector3(player.transform.position.x, height, player.transform.position.z);
		//Affectation
		transform.position = newPos + offset;
	}
}
