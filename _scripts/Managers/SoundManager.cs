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

    [Header("Mute Butonlar�")]
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


    private void Awake()        //  Starttanda �nce �al��an K�s�m
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  //  Ba�ke Sahneye Ge�i� Yap�ld���nda Bu GameObjectin Yok Olmamas�n� Sa�lamak ��in, �uanl�k Gereksiz Asl�nda
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
                    // Kaydedilmi� Ayarlar� Getiriyor
        masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        float musicVol = PlayerPrefs.GetFloat("MusicVolume");
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume");
        float uiVol = PlayerPrefs.GetFloat("UIVolume");

                    // Kaydedilmi� Ayarlar� ��liyor
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
            Debug.Log("Ses : " + soundName + " Bulunamad�");
        }
        s.audioSource.clip = s.GetRandomClip();
        s.audioSource.Play();
    }

    public void StopMusic(string soundName)
    {
        SoundController s = Array.Find(soundControllers, sound => sound.soundName == soundName);
        if (s == null)
        {
            Debug.Log("Ses : " + soundName + " Bulunamad�");
        }
        s.audioSource.Stop();
    }

    public void SwitchMusic(string newSoundName)
    {
        SoundController newMusic = Array.Find(soundControllers, s => s.soundName == newSoundName && s.soundType == SoundType.Music);
        if (newMusic == null)
        {
            Debug.LogWarning("Yeni m�zik bulunamad�: " + newSoundName);
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
            Debug.LogWarning("M�zik klibi eksik: " + newSoundName);
            return;
        }

        newMusic.audioSource.clip = clip;
        newMusic.audioSource.Play();
        newMusic.audioSource.time = currentMusicTime;

        currentMusic = newMusic;
    }


    //                                          Ses Seviyesi Sliderlar� Alan�

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

                                                    //  Ana Ses Di�er Seslerlede Ba�lant�l� Oldu�undan Di�erlerinide Ayarl�yor
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


    //                                          Mute Butonlar� Alan�

    public void ToggleMasterMute()
    {
        if (isMasterMuted)                                          //  Mutelu �ken Sesi Geri A�ma Yeri
        {
            MasterSlider.value = lastMasterVolume;                  //  Kaydedilmi� Son Ses Seviyesini Tekrar Veriyor
            MasterMuteButton.image.color = new Color(1, 1, 1, 0);   //  Mute Iconunu G�r�nmez Yap�yor
            isMasterMuted = false;                                  //  Ses A��k
        }
        else                                                        //  Ses A��kkan Mutelama Yeri
        {
            lastMusicVolume = MusicSlider.value;
            lastSfxVolume = SfxSlider.value;
            lastUIVolume = UISlider.value;

            lastMasterVolume = MasterSlider.value;                  //  Ses S�f�rlanmadan �nceki De�eri Kaydediyor
            MasterSlider.value = 0f;                                //  Sesi Kapat�yor
            MasterMuteButton.image.color = new Color(1, 1, 1, 1);   //  Mute Iconunu Aktifle�tiriyor
            isMasterMuted = true;                                   //  Ses Kapal�
        }
    }

    public void ToggleMusicMute()
    {
        if (!isMasterMuted)
        {
            if (isMusicMuted)                                           //  Mutelu �ken Sesi Geri A�ma Yeri
            {
                MusicSlider.value = lastMusicVolume;
                MusicMuteButton.image.color = new Color(1, 1, 1, 0);
                isMusicMuted = false;
            }
            else                                                        //  Ses A��kkan Mutelama Yeri
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
            if (isSfxMuted)                                          //  Mutelu �ken Sesi Geri A�ma Yeri
            {
                SfxSlider.value = lastSfxVolume;
                SfxMuteButton.image.color = new Color(1, 1, 1, 0);
                isSfxMuted = false;
            }
            else                                                    //  Ses A��kkan Mutelama Yeri
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
            if (isUIMuted)                                          //  Mutelu �ken Sesi Geri A�ma Yeri
            {
                UISlider.value = lastUIVolume;
                UIMuteButton.image.color = new Color(1, 1, 1, 0);
                isUIMuted = false;
            }
            else                                                    //  Ses A��kkan Mutelama Yeri
            {
                lastUIVolume = UISlider.value;
                UISlider.value = 0f;
                UIMuteButton.image.color = new Color(1, 1, 1, 1);
                isUIMuted = true;
            }
        }
    }

}
