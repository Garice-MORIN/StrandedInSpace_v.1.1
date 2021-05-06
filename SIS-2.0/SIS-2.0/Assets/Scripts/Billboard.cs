using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update(){
        transform.LookAt(Camera.main.transform); //Rotate billboard in diretion of the camera to ensure its visibility
    }
}
