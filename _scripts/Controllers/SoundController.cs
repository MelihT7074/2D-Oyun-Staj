using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]               //  SoundManagerda G�z�kmesi ��in
public class SoundController                //  Seslerin �zelliklerinin Tutuldu�u K�s�m
{
    public List<AudioClip> clip;                //  Seslerin Tutuldu�u Yer
    public string soundName;                        //  Sese Eri�mek ��in Alan
    [Range(0, 1)] public float volume = 0.3f;       //  Kendi Varsay�lan Ses Seviyesi,  Oyun ��i Ayarlan�rken Buras� Birt�k Ge�erlili�ini Yitiriyor
    [Range(0, 1)] public float pitch = 1;           //  Kendi Varsay�lan Ses Kal�nl���
    [HideInInspector]                   //  Edit�rde, Yani SoundManagerda G�z�kmemesi ��in, Sadece 1 Alt�ndakine Etki Ediyor
    public AudioSource audioSource;                 //  Ses Kayna��
    public bool loop;                               //  D�ng� Halinde �almas� ��in Se�enek
    public SoundType soundType;                     //  Sesleri Kategorilere Ay�rmak i�in, Oyun ��i Ses Seviyelerine Eri�mek ��in Bu Alan Yard�mc� Oluyor


    public enum SoundType
    {
        Music, SFX, UI
    }

    public AudioClip GetRandomClip()            //  Clipden Ses Se�mek ��in
    {
        if (clip == null || clip.Count == 0) return null;
        return clip[UnityEngine.Random.Range(0, clip.Count)];
    }

}
