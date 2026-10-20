using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Musci : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip startMusicClip;
    [SerializeField] private AudioClip musicClip;

    private void Start()
    {
        musicSource.loop = false;
        musicSource.PlayOneShot(startMusicClip);
    }

    private void Update()
    {
        if (!musicSource.isPlaying){
            musicSource.PlayOneShot(musicClip);
        }
    }
}