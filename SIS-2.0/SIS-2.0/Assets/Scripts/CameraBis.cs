using UnityEngine;

public class CameraBis : MonoBehaviour
{

    float sensitivity;

    int inverted;

    public Transform playerBody;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;    //Lock cursor in the center of the screen on connection
        inverted = PlayerPrefs.GetInt("Inverted"); 
        sensitivity = PlayerPrefs.GetFloat("Sensi");
    }

    public void UpdateCamera()
    {
        float vertRotate = Input.GetAxis("Mouse X") * sensitivity * inverted * Time.deltaTime ;
        float horRotate = Input.GetAxis("Mouse Y") * sensitivity * inverted * Time.deltaTime;

        xRotation -= horRotate;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); //Lock up and down rotation to 90° in each direction

        playerBody.Rotate(0f, vertRotate * inverted, 0f); //Rotate capsule
        transform.localRotation = Quaternion.Euler(xRotation,0f,0f);  //Rotate camera 
    }
}