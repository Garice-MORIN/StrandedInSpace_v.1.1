using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BarricadeInfo : NetworkBehaviour
{
    public GameObject linkedSpawner;
    [SyncVar(hook = "OnLivesChanged")] public int explosionLeft;

    void OnLivesChanged(int _old, int _new) {
        if(_new == 0) {
            linkedSpawner.GetComponent<BarricadeSpawning>().TryDestroy();
        }
    }
}
