using UnityEngine;

public class Munitions : MonoBehaviour
{
    public Transform munitions;
    public AudioSource audioSource;
    private void OnTriggerEnter(Collider collider)
    {
        if(collider.tag != "Player")
        {
            return;
        }
        collider.GetComponent<PlayerController>().munitions += Mathf.CeilToInt(Random.Range(5, 10));
        collider.GetComponent<PlayerController>().gunSource.clip = collider.GetComponent<PlayerController>().soundArray[2];
        collider.GetComponent<PlayerController>().gunSource.Play();
        Destroy(gameObject);
    }

    private void Update()
    {
        munitions.Rotate(0f, 0.5f, 0f);
    }
}
