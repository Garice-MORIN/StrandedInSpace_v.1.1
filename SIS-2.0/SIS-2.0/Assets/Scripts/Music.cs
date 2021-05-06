using UnityEngine;
using UnityEngine.Audio;

//DEPRECATED

public class Music : MonoBehaviour
{
    public AudioSource musicSource;
    public Variables variables;

    private void Start()
    {
        musicSource.volume = variables.musicVolume;
    }
}
