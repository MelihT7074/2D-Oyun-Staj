using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SoundController
{
    public List<AudioClip> clip;
    public string soundName;
    [Range(0, 1)] public float volume = 0.3f;
    [Range(0, 1)] public float pitch = 1;
    [HideInInspector]
    public AudioSource audioSource;
    public bool loop;
    public SoundType soundType;


    public enum SoundType
    {
        Music, SFX, UI
    }

    public AudioClip GetRandomClip()
    {
        if (clip == null || clip.Count == 0) return null;
        return clip[UnityEngine.Random.Range(0, clip.Count)];
    }

}
