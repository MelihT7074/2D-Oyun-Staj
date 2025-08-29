using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]               //  SoundManagerda Gözükmesi Ýçin
public class SoundController                //  Seslerin Özelliklerinin Tutulduðu Kýsým
{
    public List<AudioClip> clip;                //  Seslerin Tutulduðu Yer
    public string soundName;                        //  Sese Eriþmek Ýçin Alan
    [Range(0, 1)] public float volume = 0.3f;       //  Kendi Varsayýlan Ses Seviyesi,  Oyun Ýçi Ayarlanýrken Burasý Birtýk Geçerliliðini Yitiriyor
    [Range(0, 1)] public float pitch = 1;           //  Kendi Varsayýlan Ses Kalýnlýðý
    [HideInInspector]                   //  Editörde, Yani SoundManagerda Gözükmemesi Ýçin, Sadece 1 Altýndakine Etki Ediyor
    public AudioSource audioSource;                 //  Ses Kaynaðý
    public bool loop;                               //  Döngü Halinde Çalmasý Ýçin Seçenek
    public SoundType soundType;                     //  Sesleri Kategorilere Ayýrmak için, Oyun Ýçi Ses Seviyelerine Eriþmek Ýçin Bu Alan Yardýmcý Oluyor


    public enum SoundType
    {
        Music, SFX, UI
    }

    public AudioClip GetRandomClip()            //  Clipden Ses Seçmek Ýçin
    {
        if (clip == null || clip.Count == 0) return null;
        return clip[UnityEngine.Random.Range(0, clip.Count)];
    }

}
