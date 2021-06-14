using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKill : MonoBehaviour
{
    public List<Type> killedEnemies;

    private void Start()
    {
        killedEnemies = new List<Type>();
    }
}
