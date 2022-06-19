using System;
using UnityEngine;

public class SoundManager
{
    public AudioSource Source { get; internal set; }
    public float defaultVolume = 0.78f;

    public void Play(AudioClip clip)
    {
        Play(clip, defaultVolume);
    }

    public void Play(AudioClip clip, float vol)
    {
        if (clip == null)
            Debug.LogWarning("Sound clip not found");
        else
            Source?.PlayOneShot(clip, vol);
    }
}
