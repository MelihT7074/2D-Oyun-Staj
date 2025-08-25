using UnityEngine;

public class EscAndOptions : MonoBehaviour
{
    [Header("Managerse")]
    public Player player;
    public GameDirector gameDirector;

    [Header("Menuler")]
    public MenuTypes menuType;
    public string currentOpenMenu;

    public GameObject menus;        //  T�m Men�lerin Ba�l� Oldu�u Ana Obje
    public GameObject escMenu;
    public GameObject options;
    public GameObject soundOptions;
    public GameObject keyboardOptions;


    public enum MenuTypes
    {   //   0     1      2         3
            None, Esc, Options, OptionsX
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentOpenMenu == "None")
            {
                StopGameOpenEsc();
            }
            else if (currentOpenMenu == "Options")
            {
                StopGameOpenEsc();
            }
            else if (currentOpenMenu == "OptionsX")
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

        gameDirector.SwitchMusic();

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
        menuType = MenuTypes.OptionsX;
        currentOpenMenu = menuType.ToString();

        options.SetActive(false);
        soundOptions.SetActive(true);
    }

    public void KeyboardOptions()
    {
        menuType = MenuTypes.OptionsX;
        currentOpenMenu = menuType.ToString();

        options.SetActive(false);
        keyboardOptions.SetActive(true);
    }

    public void ContinueGameCloseEsc()
    {
        Time.timeScale = 1f;        //  Zaman�n Ak���n� Tekrar Normale �eviriyor

        menuType = MenuTypes.None;
        currentOpenMenu = menuType.ToString();

        gameDirector.SwitchMusic();

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

        ContinueGameCloseEsc();         //  �uanl�k B�ye Kals�n
    }

    public void CloseGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;    //  Edit�rdede Kapatmas� ��in
#endif
    }

}
