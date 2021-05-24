using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public Transform cameraTransform;


    // Update is called once per frame
    void Update()
    {
        cameraTransform.position = player.position + offset;
    }
}
