using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update(){
        transform.LookAt(Camera.main.transform); //Oriente le billboard dans la direction de la camera afin d'assurer sa visibilité
    }
}
