using UnityEngine;
using UnityEngine.Audio;

public class Music : MonoBehaviour
{
    public AudioSource musicSource;
    public Variables variables;

    private void Start()
    {
        musicSource.volume = variables.musicVolume;
    }
}
