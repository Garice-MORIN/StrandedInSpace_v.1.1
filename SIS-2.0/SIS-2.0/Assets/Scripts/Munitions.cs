using UnityEngine;
using Mirror;
public class Munitions : MonoBehaviour
{
    public Transform munitions;
    public AudioSource audioSource;
    public NetworkManager GetNetworkManager() => FindObjectOfType<NetworkManager>();
    //Pick up function
    private void OnTriggerEnter(Collider collider)
    {
        if(collider.tag != "Player")
        {
            return;
        }
        collider.GetComponent<PlayerController>().munitions += Mathf.CeilToInt(Random.Range(50, 100));
        collider.GetComponent<PlayerController>().gunSource.clip = GetNetworkManager().GetComponentInParent<SpawnTable>().GetGunSound(2);
        collider.GetComponent<PlayerController>().gunSource.Play();
        Destroy(gameObject);
    }

    private void Update()
    {
        munitions.Rotate(0f, 0.5f, 0f);
    }
}
