using UnityEngine;

public class CameraBis : MonoBehaviour
{

    float sensitivity;

    int inverted;

    public Transform playerBody;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;    //Lock le curseur au milieu de l'écran au moment de la connexion
        inverted = PlayerPrefs.GetInt("Inverted");
        sensitivity = PlayerPrefs.GetFloat("Sensi");
    }

    public void UpdateCamera()
    {
        float vertRotate = Input.GetAxis("Mouse X") * sensitivity * inverted * Time.deltaTime ;
        float horRotate = Input.GetAxis("Mouse Y") * sensitivity * inverted * Time.deltaTime;

        xRotation -= horRotate;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); //Bloque la rotation à 90° vers le bas et le haut

        playerBody.Rotate(0f, vertRotate * inverted, 0f); //Tourne la capsule
        transform.localRotation = Quaternion.Euler(xRotation,0f,0f);  //Tourne la caméra 
    }
}