using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.UI;
using static SoundController;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public SoundController[] soundControllers;
    [Space]
    [Space]
    [Space]

    private SoundController currentMusic;
    private float currentMusicTime = 0f;

    [Header("Sliderlar")]
    public Slider MasterSlider;
    private float masterVolume = 1f;
    public Slider MusicSlider;
    public Slider SfxSlider;
    public Slider UISlider;

    [Header("Mute Butonlarý")]
    public bool isMasterMuted = false;
    public float lastMasterVolume;
    public Button MasterMuteButton;
    [Space]
    public bool isMusicMuted = false;
    public float lastMusicVolume;
    public Button MusicMuteButton;
    [Space]
    public bool isSfxMuted = false;
    public float lastSfxVolume;
    public Button SfxMuteButton;
    [Space]
    public bool isUIMuted = false;
    public float lastUIVolume;
    public Button UIMuteButton;


    private void Awake()        //  Starttanda Önce Çalýþan Kýsým
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  //  Baþke Sahneye Geçiþ Yapýldýðýnda Bu GameObjectin Yok Olmamasýný Saðlamak Ýçin, Þuanlýk Gereksiz Aslýnda
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (var sound in soundControllers)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.GetRandomClip();
            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.playOnAwake = false;
            sound.audioSource.loop = sound.loop;
        }
    }

    private void Start()
    {
                    // Kaydedilmiþ Ayarlarý Getiriyor
        masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        float musicVol = PlayerPrefs.GetFloat("MusicVolume");
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume");
        float uiVol = PlayerPrefs.GetFloat("UIVolume");

                    // Kaydedilmiþ Ayarlarý Ýþliyor
        MasterSlider.value = masterVolume;
        MusicSlider.value = musicVol;
        SfxSlider.value = sfxVol;
        UISlider.value = uiVol;

        OnMusicVolumeChanged(musicVol);
        OnSFXVolumeChanged(sfxVol);
        OnUIVolumeChanged(uiVol);
        OnMasterVolumeChanged(masterVolume);
    }

    public void PlayMusic(string soundName)
    {
        SoundController s = Array.Find(soundControllers, sound => sound.soundName == soundName);
        if (s == null)
        {
            Debug.Log("Ses : " + soundName + " Bulunamadý");
        }
        s.audioSource.clip = s.GetRandomClip();
        s.audioSource.Play();
    }

    public void StopMusic(string soundName)
    {
        SoundController s = Array.Find(soundControllers, sound => sound.soundName == soundName);
        if (s == null)
        {
            Debug.Log("Ses : " + soundName + " Bulunamadý");
        }
        s.audioSource.Stop();
    }

    public void SwitchMusic(string newSoundName)
    {
        SoundController newMusic = Array.Find(soundControllers, s => s.soundName == newSoundName && s.soundType == SoundType.Music);
        if (newMusic == null)
        {
            Debug.LogWarning("Yeni müzik bulunamadý: " + newSoundName);
            return;
        }

        if (currentMusic != null && currentMusic.audioSource.isPlaying)
        {
            currentMusicTime = currentMusic.audioSource.time;
            currentMusic.audioSource.Stop();
        }

        AudioClip clip = newMusic.clip.Count > 0 ? newMusic.clip[0] : null;
        if (clip == null)
        {
            Debug.LogWarning("Müzik klibi eksik: " + newSoundName);
            return;
        }

        newMusic.audioSource.clip = clip;
        newMusic.audioSource.Play();
        newMusic.audioSource.time = currentMusicTime;

        currentMusic = newMusic;
    }


    //                                          Ses Seviyesi Sliderlarý Alaný

    public void OnMasterVolumeChanged(float value)
    {
        masterVolume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayMusic("MasterSlider");
        if (value == 0)
        {
            MasterMuteButton.image.color = new Color(1, 1, 1, 1);
            isMasterMuted = true;
        }
        else
        {
            MasterMuteButton.image.color = new Color(1, 1, 1, 0);
            isMasterMuted = false;
        }

                                                    //  Ana Ses Diðer Seslerlede Baðlantýlý Olduðundan Diðerlerinide Ayarlýyor
        OnMusicVolumeChanged(MusicSlider.value);
        OnSFXVolumeChanged(SfxSlider.value);
        OnUIVolumeChanged(UISlider.value);
    }

    public void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        foreach (var sound in soundControllers)
        {
            if (sound.soundType == SoundController.SoundType.Music)
            {
                sound.audioSource.volume = value * masterVolume;
            }
        }
        PlayMusic("MusicSlider");
        if (value == 0 || isMasterMuted)
        {
            MusicMuteButton.image.color = new Color(1, 1, 1, 1);
            isMusicMuted = true;
        }
        else
        {
            MusicMuteButton.image.color = new Color(1, 1, 1, 0);
            isMusicMuted = false;
        }
    }

    public void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        foreach (var sound in soundControllers)
        {
            if (sound.soundType == SoundController.SoundType.SFX)
            {
                sound.audioSource.volume = value * masterVolume;
            }
        }
        PlayMusic("SfxSlider");
        if (value == 0 || isMasterMuted)
        {
            SfxMuteButton.image.color = new Color(1, 1, 1, 1);
            isSfxMuted = true;
        }
        else
        {
            SfxMuteButton.image.color = new Color(1, 1, 1, 0);
            isSfxMuted = false;
        }
    }

    public void OnUIVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("UIVolume", value);
        foreach (var sound in soundControllers)
        {
            if (sound.soundType == SoundController.SoundType.UI)
            {
                sound.audioSource.volume = value * masterVolume;
            }
        }
        PlayMusic("UISlider");
        if (value == 0 || isMasterMuted)
        {
            UIMuteButton.image.color = new Color(1, 1, 1, 1);
            isUIMuted = true;
        }
        else
        {
            UIMuteButton.image.color = new Color(1, 1, 1, 0);
            isUIMuted = false;
        }
    }


    //                                          Mute Butonlarý Alaný

    public void ToggleMasterMute()
    {
        if (isMasterMuted)                                          //  Mutelu Ýken Sesi Geri Açma Yeri
        {
            MasterSlider.value = lastMasterVolume;                  //  Kaydedilmiþ Son Ses Seviyesini Tekrar Veriyor
            MasterMuteButton.image.color = new Color(1, 1, 1, 0);   //  Mute Iconunu Görünmez Yapýyor
            isMasterMuted = false;                                  //  Ses Açýk
        }
        else                                                        //  Ses Açýkkan Mutelama Yeri
        {
            lastMusicVolume = MusicSlider.value;
            lastSfxVolume = SfxSlider.value;
            lastUIVolume = UISlider.value;

            lastMasterVolume = MasterSlider.value;                  //  Ses Sýfýrlanmadan Önceki Deðeri Kaydediyor
            MasterSlider.value = 0f;                                //  Sesi Kapatýyor
            MasterMuteButton.image.color = new Color(1, 1, 1, 1);   //  Mute Iconunu Aktifleþtiriyor
            isMasterMuted = true;                                   //  Ses Kapalý
        }
    }

    public void ToggleMusicMute()
    {
        if (!isMasterMuted)
        {
            if (isMusicMuted)                                           //  Mutelu Ýken Sesi Geri Açma Yeri
            {
                MusicSlider.value = lastMusicVolume;
                MusicMuteButton.image.color = new Color(1, 1, 1, 0);
                isMusicMuted = false;
            }
            else                                                        //  Ses Açýkkan Mutelama Yeri
            {
                lastMusicVolume = MusicSlider.value;
                MusicSlider.value = 0f;
                MusicMuteButton.image.color = new Color(1, 1, 1, 1);
                isMusicMuted = true;
            }
        }
    }

    public void ToggleSfxMute()
    {
        if (!isMasterMuted)
        {
            if (isSfxMuted)                                          //  Mutelu Ýken Sesi Geri Açma Yeri
            {
                SfxSlider.value = lastSfxVolume;
                SfxMuteButton.image.color = new Color(1, 1, 1, 0);
                isSfxMuted = false;
            }
            else                                                    //  Ses Açýkkan Mutelama Yeri
            {
                lastSfxVolume = SfxSlider.value;
                SfxSlider.value = 0f;
                SfxMuteButton.image.color = new Color(1, 1, 1, 1);
                isSfxMuted = true;
            }
        }
    }

    public void ToggleUIMute()
    {
        if (!isMasterMuted)
        {
            if (isUIMuted)                                          //  Mutelu Ýken Sesi Geri Açma Yeri
            {
                UISlider.value = lastUIVolume;
                UIMuteButton.image.color = new Color(1, 1, 1, 0);
                isUIMuted = false;
            }
            else                                                    //  Ses Açýkkan Mutelama Yeri
            {
                lastUIVolume = UISlider.value;
                UISlider.value = 0f;
                UIMuteButton.image.color = new Color(1, 1, 1, 1);
                isUIMuted = true;
            }
        }
    }

}
