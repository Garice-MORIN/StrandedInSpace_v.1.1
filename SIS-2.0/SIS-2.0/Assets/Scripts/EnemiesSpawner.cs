using UnityEngine;
using Mirror;
using System;
using System.IO;
using System.Collections.Generic;

public class EnemiesSpawner : NetworkBehaviour
{
    GameObject enemyPrefab;
    public LayerMask mask;

    [SyncVar(hook = "OnChangeEnemiesLeft")]
    public int enemiesLeft = 0;

    GameObject[] allSpawnPoints;
    string path;
    Queue<string> queue = new Queue<string>();

    public override void OnStartServer()
    {
        ChoosePath();
        CreateSpawnList();

        allSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");
        LoadEnemies();
        
    }

    //Locate spawn file
    public void ChoosePath()
    {
        if(Application.isPlaying)
        {
            path = "StandAlone/Spawns.txt";
        }
        else
        {
            path = "Assets/Scripts/Spawns.txt";
        }
    }

    //Spawn next wave if there is at least one left
    public void TrySpawningNextWave()
    {
        if(enemiesLeft == 0)
        {
            try
            {
                LoadEnemies();
            }
            catch (Exception)
            {
                Debug.Log("The entity you're trying to spawn does not exist");
                return;
            }
            
        }
    }

    public void LoadEnemies()
    {
        int i = 0;

        foreach (var enemy in queue.Dequeue().Split(','))
        {
            enemyPrefab = Resources.Load(enemy) as GameObject;     //Charge le modèle correspondant au type d'ennemi
            
            var position = allSpawnPoints[i].transform.position;
            var orientation = Quaternion.Euler(0f, (float)UnityEngine.Random.Range(0, 360), 0f);
            var toSpawn = (GameObject)Instantiate(enemyPrefab, position, orientation);

            NetworkServer.Spawn(toSpawn);
            i = (i+1)%4;
            enemiesLeft++;
        }
        //enemiesLeft = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    void CreateSpawnList()
    {
        StreamReader sr = new StreamReader(path);
        string s;
        while ((s = sr.ReadLine()) != null)
        {
            queue.Enqueue(s);
        }
        sr.Close();
    }


    void OnChangeEnemiesLeft(int oldEnemiesleft, int newEnemiesLeft)
    {
        if(newEnemiesLeft != 0)
        {
            return;
        }
        TrySpawningNextWave();
    }

}
