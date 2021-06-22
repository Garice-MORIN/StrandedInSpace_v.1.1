using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ListOfPlayers : NetworkBehaviour
{
    private SyncList<GameObject> players = new SyncList<GameObject>();
	
	public void AddPlayer(GameObject player) {
		if (!players.Contains(player)) {
			players.Add(player);
		}
	}

	public SyncList<GameObject> GetList() {
		return players;
	}
}
