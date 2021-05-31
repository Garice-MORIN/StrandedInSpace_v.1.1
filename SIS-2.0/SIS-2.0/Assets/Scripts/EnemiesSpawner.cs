using UnityEngine;
using Mirror;
using System;
using System.IO;
using System.Collections.Generic;

public class EnemiesSpawner : NetworkBehaviour
{
    GameObject enemyPrefab;
    public LayerMask mask;
    public bool isStarted = false;

    public static int waveNumber = 0;
    [SyncVar(hook = "OnChangeEnemiesLeft")]
    public int enemiesLeft = 0;

    GameObject[] allSpawnPoints;
    Queue<string> queue = new Queue<string>();
    //[SyncVar(hook = "Endgame")]
    //bool transition = false;

    public void StartGame()
    {
        CreateSpawnList();
        isStarted = true;
        allSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");
        LoadEnemies();
        waveNumber++;
    }

    //Spawn next wave if there is at least one left
    public void TrySpawningNextWave()
    {
        if(enemiesLeft == 0)
        {
            try
            {
                LoadEnemies();
                waveNumber++;
            }
            catch (InvalidOperationException)
            {
                StartCoroutine("Sleep");
            }
            catch (Exception)
            {
                Debug.Log("The entity you're trying to spawn does not exist");
                return;
            }
            
        }
    }

    public void Endgame(bool _old, bool _new)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject host = null;
        foreach (var player in players)
        {
            if (!player.GetComponent<PlayerController>()._isServer)
            {
                player.GetComponent<PlayerController>().GetNetworkManager().offlineScene = "WinScene";
                player.GetComponent<PlayerController>().OnEndGame(true);
            }
            else
                host = player;
        }
        host.GetComponent<PlayerController>().OnEndGame(true);
    }

    public void LoadEnemies()
    {
        int i = 0;

        foreach (var enemy in queue.Dequeue().Split(','))
        {
            enemyPrefab = Resources.Load(enemy) as GameObject;     //Load corresponding enemy model
            
            var position = allSpawnPoints[i].transform.position;
            var orientation = Quaternion.Euler(0f, (float)UnityEngine.Random.Range(0, 360), 0f);
            var toSpawn = (GameObject)Instantiate(enemyPrefab, position, orientation);

            NetworkServer.Spawn(toSpawn);
            i = (i+1)%4; //Enable rotation of spawn point
            enemiesLeft++;
        }
    }

    //Get all waves from a .txt file
    void CreateSpawnList()
    {
        StreamReader sr = new StreamReader(Application.dataPath + "/Spawns.txt"); 
        string s;
        while ((s = sr.ReadLine()) != null)
        {
            queue.Enqueue(s);
        }
        sr.Close();
    }

    //Update number of ennemies left on the map when one is killed
    void OnChangeEnemiesLeft(int oldEnemiesleft, int newEnemiesLeft)
    {
        if(newEnemiesLeft != 0)
        {
            return;
        }
        TrySpawningNextWave();
    }

    System.Collections.IEnumerator Sleep()
    {
        yield return new WaitForSecondsRealtime(1);
        //transition = true;
    }

}

/* SmallEnemy,HeavyEnemy,HeavyEnemy,NormalEnemy,SmallEnemy,NormalEnemy,SmallEnemy
NormalEnemy,SmallEnemy,HeavyEnemy,NormalEnemy*/