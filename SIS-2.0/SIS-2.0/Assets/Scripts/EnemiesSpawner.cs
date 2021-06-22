using UnityEngine;
using Mirror;
using System;
using System.IO;
using System.Collections.Generic;

public class EnemiesSpawner : NetworkBehaviour
{
    const int maxWave = 3;
    DateTime startTime;
    DateTime endTime;
    public DateTime startWaveTwo;
    GameObject enemyPrefab;
    public LayerMask mask;
    public bool isStarted = false;
    private Door doorScript;
    public GameObject check;
    public CheckForPlayer checkForPlayer;
    private StopWatch stopWatchCheck;

    public static int waveNumber = 0;
    [SyncVar(hook = "OnChangeEnemiesLeft")]
    public int enemiesLeft = 0;

    GameObject[] allSpawnPoints;
    [SyncVar(hook = "OnStateChanged")]
    public bool openDoor = false;
    Queue<string> queue = new Queue<string>();

    private void Start()
    {
        startWaveTwo = new DateTime();
    }

    public void StartGame()
    {
        CreateSpawnList();
        stopWatchCheck = check.GetComponent<StopWatch>();
        isStarted = true;
        allSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");
        doorScript = GameObject.FindGameObjectWithTag("Door").GetComponent<Door>();
        LoadEnemies();
        waveNumber++;
        doorScript.CloseDoor();
    }

    public void LoadEnemies()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players) {
            var playerController = player.GetComponent<PlayerController>();
            playerController.watch.StartWatch();
            if(waveNumber == 3) {
                Debug.Log(stopWatchCheck.GetElapsedTime());
                playerController.SetRoundThree(stopWatchCheck.GetElapsedTime());
			}
        }
        if (waveNumber < 3)
            stopWatchCheck.StartWatch();

        StartCoroutine(GenerateEnemies());
    }

    System.Collections.IEnumerator GenerateEnemies() {
        int i = 0;
        foreach (var enemy in queue.Dequeue().Split(',')) {
            enemyPrefab = Resources.Load(enemy) as GameObject;     //Load corresponding enemy model

            var position = allSpawnPoints[i].transform.position;
            var orientation = Quaternion.Euler(0f, (float)UnityEngine.Random.Range(0, 360), 0f);
            var toSpawn = (GameObject)Instantiate(enemyPrefab, position, orientation);

            NetworkServer.Spawn(toSpawn);
            i = (i + 1) % 4; //Enable rotation of spawn point
            enemiesLeft++;
            yield return new WaitForSecondsRealtime(1f);
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

    public void SpawnEnemies() {
        if (waveNumber < maxWave) {
            waveNumber++;
            LoadEnemies();
            openDoor = false;
        }
        else {
            EndOfGame();
        }
    }

    //Update number of ennemies left on the map when one is killed
    void OnChangeEnemiesLeft(int oldEnemiesleft, int newEnemiesLeft)
    {
        if(newEnemiesLeft != 0)
        {
            return;
        }
        foreach (var watch in FindObjectsOfType<StopWatch>())
            watch.PauseWatch();
        if(checkForPlayer.nbPlayer == 0) {
            StartCoroutine("WaitToSpawn");
		}
        else {
            openDoor = true;
        }
    }

    void OnStateChanged(bool oldState, bool newState) {
        if (newState)
            doorScript.OpenDoor();
        else
            doorScript.CloseDoor();
	}

    System.Collections.IEnumerator WaitToSpawn() {
        yield return new WaitForSecondsRealtime(40);
        SpawnEnemies();
    }

    public void EndOfGame(bool victory = true) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
            player.GetComponent<PlayerController>().OnEndGame(victory);
    }

}
