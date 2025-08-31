using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeybindingManager : MonoBehaviour
{
    [System.Serializable]
    public class KeybindUI      //  Tuş Özellikleri
    {
        public string actionName;               // İşlevin İsmi
        public TextMeshProUGUI keyText;         // UIda Gözükecek Yazı
        public Button rebindButton;             // Değiştir Butonu
        [HideInInspector] public KeyCode key;   // Atanan Tuş
    }

    public List<KeybindUI> keybinds;

    public Button btn_SetDefaults;

    public bool waitingForKey = false;
    public bool justReboundKey = false;     //  Tuş Atamalarında Esc Çalışmasın Diye Ekstra Kontrol,
                        //   GameDirectorda Keybinding Menüsü Açıksa, Playerdada Oyun Durduğunda Direkt Tüm Girdileri Kapatarak Bu Sorun Çözülüyor

    private KeybindUI currentRebind;


    void Start()
    {
        foreach (var kb in keybinds)    //  Kaydedilmiş Tuşları Yükleme
        {
            if (PlayerPrefs.HasKey(kb.actionName))  //  Önceden Tuş Ataması Kaydedilmişse Onları Getirme
            {
                kb.key = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(kb.actionName));
            }
            else
            {
                kb.key = GetDefaultKey(kb.actionName);  //  Önceden Tuş Ataması Olmadıysa Varsayılanları Yükleme
            }

            kb.keyText.text = kb.actionName + ": " + kb.key.ToString();     //  UIdaki Yazıyı Ayarlama

            kb.rebindButton.onClick.AddListener(() => StartRebind(kb));     //  Butona Tıklandığında Yeniden Atama Sağlanması İçin İşlev Verme
        }
    }

    void Update()
    {
        if (waitingForKey && Input.anyKeyDown)
        {
            foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(k))
                {
                    currentRebind.key = k;
                    currentRebind.keyText.text = currentRebind.actionName + ": " + k.ToString();    //  UIdaki Texti Güncelliyor
                    PlayerPrefs.SetString(currentRebind.actionName, k.ToString());                  //  Tuş Atamasını Kaydetme
                    waitingForKey = false;
                    justReboundKey = true;
                    break;
                }
            }
        }
    }

    void StartRebind(KeybindUI keybind)     //  Tuş Atama İşlemi
    {
        currentRebind = keybind;
        waitingForKey = true;
        currentRebind.keyText.text = keybind.actionName + ": ...";
    }

    public KeyCode GetKey(string actionName)    // Atanan Tuşu Kullanmak İçin
    {
        foreach (var kb in keybinds)
        {
            if (kb.actionName == actionName)
                return kb.key;
        }
        return KeyCode.None;
    }

    KeyCode GetDefaultKey(string actionName)    //  Varsayılan Tuşlar
    {
        switch (actionName)
        {
            case "LookUp": return KeyCode.W;
            case "LookDown": return KeyCode.S;
            case "LookUp_Sc": return KeyCode.UpArrow;
            case "LookDown_Sc": return KeyCode.DownArrow;
            case "MoveLeft": return KeyCode.A;
            case "MoveRight": return KeyCode.D;
            case "MoveLeft_Sc": return KeyCode.LeftArrow;
            case "MoveRight_Sc": return KeyCode.RightArrow;
            case "Jump": return KeyCode.Space;
            case "Esc": return KeyCode.Escape;
            case "Restart": return KeyCode.R;
        }
        return KeyCode.None;
    }

    public void ResetAllKeys()      //  Tüm Tuşları Varsayılanla Değiştiriyor
    {
        foreach (var kb in keybinds)
        {
            kb.key = GetDefaultKey(kb.actionName);
            kb.keyText.text = kb.actionName + ": " + kb.key;
            PlayerPrefs.DeleteKey(kb.actionName);               //  Eski Atamaların Kayıtlarını Siliyor
        }
    }
}