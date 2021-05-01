using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//DEPRECATED
public class Bullet : MonoBehaviour
{
    void OnCollisionEnter(Collision collision){
        //Item hit by bullet
        var hit = collision.gameObject;
        var HP = hit.GetComponent<Health>();

        if(HP != null){
            //Take damage
            HP.TakeDamage(50);
        }
        Destroy(gameObject);
    }
}
