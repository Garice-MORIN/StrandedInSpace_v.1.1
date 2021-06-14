using UnityEngine;
using Mirror;
using System;
using System.IO;
using System.Collections.Generic;

public class EnemiesSpawner : NetworkBehaviour
{
    const int maxWave = 20;
    DateTime startTime;
    DateTime endTime;
    public DateTime startWaveTwo;
    GameObject enemyPrefab;
    public LayerMask mask;
    public bool isStarted = false;
    private Door doorScript;

    public static int waveNumber = 0;
    [SyncVar(hook = "OnChangeEnemiesLeft")]
    public int enemiesLeft = 0;

    GameObject[] allSpawnPoints;
    bool canSpawnNextWave;
    Queue<string> queue = new Queue<string>();
    //[SyncVar(hook = "Endgame")]
    //bool endgame = false;

    private void Start()
    {
        startWaveTwo = new DateTime();
    }

    public void StartGame()
    {
        canSpawnNextWave = true;
        startTime = DateTime.Now;
        CreateSpawnList();
        isStarted = true;
        allSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");
        doorScript = GameObject.FindGameObjectWithTag("Door").GetComponent<Door>();
        LoadEnemies();
        waveNumber++;
    }

    //Spawn next wave if there is at least one left
    public void TrySpawningNextWave()
    {
        if(enemiesLeft == 0)
        {
			try {
                StartCoroutine("DoorAnimation");
            }
			catch (Exception e){
                Debug.Log("Exception trying to spawn next wave : " + e.ToString());
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
        if (waveNumber == 2)
            startWaveTwo = DateTime.Now;

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

    System.Collections.IEnumerator DoorAnimation() {
        doorScript.OpenDoor();
        yield return new WaitForSecondsRealtime(10);
        doorScript.CloseDoor();
        if (waveNumber < maxWave) {
            LoadEnemies();
            waveNumber++;
        }
        else {
            StartCoroutine("EndOfGame");
        }
    }

    System.Collections.IEnumerator EndOfGame() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        yield return new WaitForSecondsRealtime(10);
        foreach (var player in players)
            player.GetComponent<PlayerController>().OnEndGame(true);
    }

}
