using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.UI;
using static SoundController;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;    //  Eri�imi kolayla�t�rmak i�in Y�ntem (�zellikle Sahne De�i�imlerinde Ama bu Projede Yok)
                                            //  static: Bellekte Sadece 1 Tane Bulundurur, Verildi�i Nesneyi �a��rmak ��in Referansa* �htiyac� Yok, S�n�f�n �smini Yazmak Yeterli 
    //  *: Scriptlerde Ba�taki Alanlardaki "public GameDirector gameDirector; (Edit�rden Atan�l�yor)" Veya Kod ��indeki "cursorManager = FindFirstObjectByType<CursorManager>();" Tarz� �eyler

    public SoundController[] soundControllers;  //  Edit�rde SoundControllerlar�n Listelenece�i Ve Ayarlanaca�� Alan
    [Space]
        //  Kulland���m M�zikleri Yapan Ki�i Hepsini Benzer Tonda Ve Ayn� S�rede Yapm��, Bende 2si Aras�nda Ge�i� Yaparken Kald��� Yerden Devam Etmesi ��in Bunlar� Kullan�yorum
    private SoundController currentMusic;
    private float currentMusicTime = 0f;

    [Header("Sliderlar")]               //  Ses Seviyelerini Ayarlamak ��in Sliderlar
    public Slider MasterSlider;
    private float masterVolume = 1f;
    public Slider MusicSlider;
    public Slider SfxSlider;
    public Slider UISlider;

    [Header("Mute Butonlar�")]          //  Susturma Butonlar�, Ve Geri A��ld�klar�nda Sesi Eski Ayar�na Vermek ��in Eski Seviyeyi Kaydetme Yeri
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
                                //--    Sahne De�i�imlerinde Bu Nesneden Sadece Birtane Olmas� ��in Y�ntem,
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
                                //--    �lerde Belki Laz�m Olur


        foreach (var sound in soundControllers)     //  Olu�turulan SoundControllerlar� Ayarlama
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();     //  Ses Kayna�� At�yor, B�ylece Farkl� Sesler �ak��madan Ayn� Anda �alabilir
            sound.audioSource.clip = sound.GetRandomClip();
            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.playOnAwake = false;
            sound.audioSource.loop = sound.loop;
        }
    }

    private void Start()
    {
                    //  Kaydedilmi� Ayarlar� Getiriyor
                        //  PlayerPrefs : Unitynin Basit Bir Veri Kaydetme Y�ntemi 
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1);         //  E�er Daha �nceden Kaydedilen De�er Yoksa 2. Parametreyi ��ler
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
        float uiVol = PlayerPrefs.GetFloat("UIVolume", 0.7f);

                    //  Kaydedilmi� Ayarlar� G�rsel Olarak Slideralra ��liyor
        MasterSlider.value = masterVolume;
        MusicSlider.value = musicVol;
        SfxSlider.value = sfxVol;
        UISlider.value = uiVol;

                    //  Sliderlar De�erleri Uyguluyor
        OnMusicVolumeChanged(musicVol);
        OnSFXVolumeChanged(sfxVol);
        OnUIVolumeChanged(uiVol);
        OnMasterVolumeChanged(masterVolume);
    }

    public void PlayMusic(string soundName)         //  Ses �alma
    {
        SoundController s = Array.Find(soundControllers, sound => sound.soundName == soundName);    //  �simle E�le�en Sesi Buluyor
        if (s == null)
        {
            Debug.Log("Ses : " + soundName + " Bulunamad�");
            return;
        }
        s.audioSource.clip = s.GetRandomClip();     //  ��inden Sesi �ekiyor
        s.audioSource.Play();                       //  Sesi Oynat�yor
    }

    public void StopMusic(string soundName)         //  Sesi Durdurma
    {
        SoundController s = Array.Find(soundControllers, sound => sound.soundName == soundName);    //  �simle E�le�en Sesi buluyor
        if (s == null)
        {
            Debug.Log("Ses : " + soundName + " Bulunamad�");
            return;
        }
        s.audioSource.Stop();                       //  Sesi Durduruyor
    }

    public void SwitchMusic(string newSoundName)    //  M�zik De�i�tirme
    {
        SoundController newMusic = Array.Find(soundControllers, s => s.soundName == newSoundName && s.soundType == SoundType.Music);    //  �simle E�le�en M�zi�i Buluyor
        if (newMusic == null)
        {
            Debug.LogWarning("Yeni m�zik bulunamad�: " + newSoundName);
            return;
        }

        if (currentMusic != null && currentMusic.audioSource.isPlaying)     //  E�er Zaten M�zik �al�yorsa
        {
            currentMusicTime = currentMusic.audioSource.time;           //  O Anki S�resini Al�yor
            currentMusic.audioSource.Stop();                            //  Ve M�zi�i Durduruyor
        }

        AudioClip clip = newMusic.clip.Count > 0 ? newMusic.clip[0] : null; //  Yeni �alacak Olan M�zikteki �lk Klibi Al�yor
        if (clip == null)
        {
            Debug.LogWarning("M�zik klibi eksik: " + newSoundName);
            return;
        }

        newMusic.audioSource.clip = clip;               //  Ses Kayna��na Yeni M�zi�i Veriyor
        newMusic.audioSource.Play();                    //  Oynat�yor
        newMusic.audioSource.time = currentMusicTime;   //  Eski M�zi�in Kald��� Zaman �uankine ��leniyor, Yani Kald��� Yerden Devam Ediyor

        currentMusic = newMusic;                        //  G�ncel M�zik Not Al�n�l�yor
    }


    //                                          Ses Seviyesi Sliderlar� Alan�,  Sliderlar Hareket Ettiklerinde �a�r�l�rlar

    public void OnMasterVolumeChanged(float value)
    {
        masterVolume = value;                                   //  De�eri Ses T�rlerinede ��lenmesi ��in Anasesin De�erini Ekstra Kaydediyor
        PlayerPrefs.SetFloat("MasterVolume", value);                //  De�eri Kaydediyor
        PlayMusic("MasterSlider");                          //  Ses Efekti
        if (value == 0)                 //  Ses Seviyesi S�f�r Olursa Mute Buttonu Birnevi Aktifle�iyor
        {
            MasterMuteButton.image.color = new Color(1, 1, 1, 1);
            isMasterMuted = true;
        }
        else                            //  E�er Mute Buttonu Varken Slider� Oynat�rsak Mute Buttonunu Kapat�yor
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
