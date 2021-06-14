using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioClip[] musicArray;
    int currentSong;

    void Start()
    {
        musicSource.volume = PlayerPrefs.GetFloat("Music");
        musicSource.clip = musicArray[currentSong];
        musicSource.Play();
    }

    private void Update()
    {
        if(!musicSource.isPlaying)
        {
            musicSource.clip = musicArray[currentSong == 2 ? 0 : currentSong++];
            musicSource.Play();
        }
    }
}
