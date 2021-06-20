using UnityEngine;
using Mirror;
public class CheckForPlayer : NetworkBehaviour
{
	public int nbPlayer;
	public EnemiesSpawner enemiesSpawner;

	private void OnTriggerEnter(Collider other) {
		nbPlayer++;
		if(nbPlayer == NetworkServer.connections.Count && enemiesSpawner.enemiesLeft != 0) {
			enemiesSpawner.EndOfGame(false);
		}
	}

	private void OnTriggerExit(Collider other) {
		nbPlayer--;
		if (nbPlayer == 0 && !enemiesSpawner.isStarted)
			enemiesSpawner.StartGame();
		else if(nbPlayer == 0 && enemiesSpawner.isStarted)
			enemiesSpawner.SpawnEnemies();
	}
}
