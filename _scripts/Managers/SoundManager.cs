using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.UI;
using static SoundController;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;    //  Eriþimi kolaylaþtýrmak için Yöntem (Özellikle Sahne Deðiþimlerinde Ama bu Projede Yok)
                                            //  static: Bellekte Sadece 1 Tane Bulundurur, Verildiði Nesneyi Çaðýrmak Ýçin Referansa* Ýhtiyacý Yok, Sýnýfýn Ýsmini Yazmak Yeterli 
    //  *: Scriptlerde Baþtaki Alanlardaki "public GameDirector gameDirector; (Editörden Atanýlýyor)" Veya Kod Ýçindeki "cursorManager = FindFirstObjectByType<CursorManager>();" Tarzý Þeyler

    public SoundController[] soundControllers;  //  Editörde SoundControllerlarýn Listeleneceði Ve Ayarlanacaðý Alan
    [Space]
        //  Kullandýðým Müzikleri Yapan Kiþi Hepsini Benzer Tonda Ve Ayný Sürede Yapmýþ, Bende 2si Arasýnda Geçiþ Yaparken Kaldýðý Yerden Devam Etmesi Ýçin Bunlarý Kullanýyorum
    private SoundController currentMusic;
    private float currentMusicTime = 0f;

    [Header("Sliderlar")]               //  Ses Seviyelerini Ayarlamak Ýçin Sliderlar
    public Slider MasterSlider;
    private float masterVolume = 1f;
    public Slider MusicSlider;
    public Slider SfxSlider;
    public Slider UISlider;

    [Header("Mute Butonlarý")]          //  Susturma Butonlarý, Ve Geri Açýldýklarýnda Sesi Eski Ayarýna Vermek Ýçin Eski Seviyeyi Kaydetme Yeri
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
                                //--    Sahne Deðiþimlerinde Bu Nesneden Sadece Birtane Olmasý Ýçin Yöntem,
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
                                //--    Ýlerde Belki Lazým Olur


        foreach (var sound in soundControllers)     //  Oluþturulan SoundControllerlarý Ayarlama
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();     //  Ses Kaynaðý Atýyor, Böylece Farklý Sesler Çakýþmadan Ayný Anda Çalabilir
            sound.audioSource.clip = sound.GetRandomClip();
            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.playOnAwake = false;
            sound.audioSource.loop = sound.loop;
        }
    }

    private void Start()
    {
                    //  Kaydedilmiþ Ayarlarý Getiriyor
                        //  PlayerPrefs : Unitynin Basit Bir Veri Kaydetme Yöntemi 
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1);         //  Eðer Daha Önceden Kaydedilen Deðer Yoksa 2. Parametreyi Ýþler
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
        float uiVol = PlayerPrefs.GetFloat("UIVolume", 0.7f);

                    //  Kaydedilmiþ Ayarlarý Görsel Olarak Slideralra Ýþliyor
        MasterSlider.value = masterVolume;
        MusicSlider.value = musicVol;
        SfxSlider.value = sfxVol;
        UISlider.value = uiVol;

                    //  Sliderlar Deðerleri Uyguluyor
        OnMusicVolumeChanged(musicVol);
        OnSFXVolumeChanged(sfxVol);
        OnUIVolumeChanged(uiVol);
        OnMasterVolumeChanged(masterVolume);
    }

    public void PlayMusic(string soundName)         //  Ses Çalma
    {
        SoundController s = Array.Find(soundControllers, sound => sound.soundName == soundName);    //  Ýsimle Eþleþen Sesi Buluyor
        if (s == null)
        {
            Debug.Log("Ses : " + soundName + " Bulunamadý");
            return;
        }
        s.audioSource.clip = s.GetRandomClip();     //  Ýçinden Sesi Çekiyor
        s.audioSource.Play();                       //  Sesi Oynatýyor
    }

    public void StopMusic(string soundName)         //  Sesi Durdurma
    {
        SoundController s = Array.Find(soundControllers, sound => sound.soundName == soundName);    //  Ýsimle Eþleþen Sesi buluyor
        if (s == null)
        {
            Debug.Log("Ses : " + soundName + " Bulunamadý");
            return;
        }
        s.audioSource.Stop();                       //  Sesi Durduruyor
    }

    public void SwitchMusic(string newSoundName)    //  Müzik Deðiþtirme
    {
        SoundController newMusic = Array.Find(soundControllers, s => s.soundName == newSoundName && s.soundType == SoundType.Music);    //  Ýsimle Eþleþen Müziði Buluyor
        if (newMusic == null)
        {
            Debug.LogWarning("Yeni müzik bulunamadý: " + newSoundName);
            return;
        }

        if (currentMusic != null && currentMusic.audioSource.isPlaying)     //  Eðer Zaten Müzik Çalýyorsa
        {
            currentMusicTime = currentMusic.audioSource.time;           //  O Anki Süresini Alýyor
            currentMusic.audioSource.Stop();                            //  Ve Müziði Durduruyor
        }

        AudioClip clip = newMusic.clip.Count > 0 ? newMusic.clip[0] : null; //  Yeni Çalacak Olan Müzikteki Ýlk Klibi Alýyor
        if (clip == null)
        {
            Debug.LogWarning("Müzik klibi eksik: " + newSoundName);
            return;
        }

        newMusic.audioSource.clip = clip;               //  Ses Kaynaðýna Yeni Müziði Veriyor
        newMusic.audioSource.Play();                    //  Oynatýyor
        newMusic.audioSource.time = currentMusicTime;   //  Eski Müziðin Kaldýðý Zaman Þuankine Ýþleniyor, Yani Kaldýðý Yerden Devam Ediyor

        currentMusic = newMusic;                        //  Güncel Müzik Not Alýnýlýyor
    }


    //                                          Ses Seviyesi Sliderlarý Alaný,  Sliderlar Hareket Ettiklerinde Çaðrýlýrlar

    public void OnMasterVolumeChanged(float value)
    {
        masterVolume = value;                                   //  Deðeri Ses Türlerinede Ýþlenmesi Ýçin Anasesin Deðerini Ekstra Kaydediyor
        PlayerPrefs.SetFloat("MasterVolume", value);                //  Deðeri Kaydediyor
        PlayMusic("MasterSlider");                          //  Ses Efekti
        if (value == 0)                 //  Ses Seviyesi Sýfýr Olursa Mute Buttonu Birnevi Aktifleþiyor
        {
            MasterMuteButton.image.color = new Color(1, 1, 1, 1);
            isMasterMuted = true;
        }
        else                            //  Eðer Mute Buttonu Varken Sliderý Oynatýrsak Mute Buttonunu Kapatýyor
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
