using UnityEngine;

public class EscAndOptions : MonoBehaviour
{
    [Header("Managerse")]
    public Player player;
    public GameDirector gameDirector;
    public KeybindingManager KBM;

    [Header("Screens")]
    public GameObject deathScreen; 

    [Header("Menuler")]
    public MenuTypes menuType;
    public string currentOpenMenu;

    public GameObject menus;        //  T�m Men�lerin Ba�l� Oldu�u Ana Obje
    public GameObject escMenu;
    public GameObject options;
    public GameObject soundOptions;
    public GameObject keyboardOptions;


    public enum MenuTypes
    {   //   0     1      2         3                   4
            None, Esc, Options, OptionsSound, OptionsKeybinding
    }

    void Update()
    {
        if (KBM.waitingForKey || KBM.justReboundKey)    //  Tu� Atan�rken Esc �al��ma�sn Diye Kontrol
        {
            KBM.justReboundKey = false;
            return;
        }

        if (Input.GetKeyDown(KBM.GetKey("Esc")))
        {
            if (currentOpenMenu == "None")
            {
                StopGameOpenEsc();
            }
            else if (currentOpenMenu == "Options")
            {
                StopGameOpenEsc();
            }
            else if (currentOpenMenu == "OptionsSound")
            {
                OpenOptions();
            }
            else if (currentOpenMenu == "OptionsKeybinding")
            {
                OpenOptions();
            }
            else if (currentOpenMenu == "Esc")
            {
                ContinueGameCloseEsc();
            }
        }

    }

    public void StopGameOpenEsc()
    {
        menus.transform.position = player.transform.position;

        Time.timeScale = 0f;        //  Zaman Ak���n� Durduruyor

        menuType = MenuTypes.Esc;
        currentOpenMenu = menuType.ToString();

        gameDirector.GD_SwitchMusic();

        player.borderWarning.SetActive(false);  //  Uyar� Ekran� Her Hal�karda Kapat�l�yor

        escMenu.SetActive(true);    
        options.SetActive(false);
    }

    public void OpenOptions()
    {
        menuType = MenuTypes.Options;
        currentOpenMenu = menuType.ToString();

        escMenu.SetActive(false);
        options.SetActive(true);
        soundOptions.SetActive(false);
        keyboardOptions.SetActive(false);
    }

    public void SoundOptions()
    {
        menuType = MenuTypes.OptionsSound;
        currentOpenMenu = menuType.ToString();

        options.SetActive(false);
        soundOptions.SetActive(true);
    }

    public void KeyboardOptions()
    {
        menuType = MenuTypes.OptionsKeybinding;
        currentOpenMenu = menuType.ToString();

        options.SetActive(false);
        keyboardOptions.SetActive(true);
    }

    public void ContinueGameCloseEsc()
    {
        Time.timeScale = 1f;        //  Zaman�n Ak���n� Tekrar Normale �eviriyor

        menuType = MenuTypes.None;
        currentOpenMenu = menuType.ToString();

        gameDirector.GD_SwitchMusic();

        if (player.onBorder || player.fallLoopCount > 10)
        {
            player.borderWarning.SetActive(true);   //  E�er Kapanmadan �nce A��ksa Tekrar A��l�yor, De�ilse Kapal� Kal�yor
        }

        escMenu.SetActive(false);
        options.SetActive(false);

        gameDirector.cursorManager.SetActiveCursor(gameDirector.cursorManager.lst_BasicCursors[0]);     //  Esc �le Kapat�l�nca �mlec De�i�imi Ger�ekle�miyor, Buda ��z�m
    }

    public void RestartGame()
    {
        gameDirector.RestartGame();
                                        //  Kullan�ld���nda Men�leri Kapatmas� ��in
        deathScreen.SetActive(false);
        ContinueGameCloseEsc();
    }

    public void CloseGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;    //  Edit�rdede Kapatmas� ��in
#endif
    }

}
