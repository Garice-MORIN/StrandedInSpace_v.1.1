using UnityEngine;
using Mirror;
public class CheckForPlayer : NetworkBehaviour
{
	public int nbPlayer;
	public EnemiesSpawner enemiesSpawner;

	private void OnTriggerEnter(Collider other) {
		nbPlayer++;
	}

	private void OnTriggerExit(Collider other) {
		nbPlayer--;
		if (nbPlayer == 0 && !enemiesSpawner.isStarted)
			enemiesSpawner.StartGame();
		else if(nbPlayer == 0 && enemiesSpawner.isStarted)
			enemiesSpawner.SpawnEnemies();
	}
}
