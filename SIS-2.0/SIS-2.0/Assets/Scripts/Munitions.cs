using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Munitions : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player")
        {
            return;
        }
        other.GetComponent<PlayerController>().munitions += Mathf.CeilToInt(Random.Range(5, 10));
        Destroy(gameObject);
    }
}
