using UnityEngine;

public class EscAndOptions : MonoBehaviour
{
    [Header("Managerse")]
    public GameDirector gameDirector;

    [Header("Menuler")]
    public MenuTypes menuType;
    public string currentOpenMenu;

    public GameObject escMenu;


    public enum MenuTypes
    {   //   0     1      2
            None, Esc, Options
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentOpenMenu == "None")
            {
                StopGameOpenEsc();
            }
            else if (currentOpenMenu == "Esc")
            {
                ContinueGameCloseEsc();
            }

        }

    }

    public void StopGameOpenEsc()
    {
        Time.timeScale = 0f;        //  Zaman Akýþýný Durduruyor

        menuType = MenuTypes.Esc;
        currentOpenMenu = menuType.ToString();

        escMenu.SetActive(true);    //  Esc Menüsünü Aktif Ediyor
    }

    public void ContinueGameCloseEsc()
    {
        Time.timeScale = 1f;        //  Zamanýn Akýþýný Tekrar Normale Çeviriyor

        menuType = MenuTypes.None;
        currentOpenMenu = menuType.ToString();

        escMenu.SetActive(false);   //  Menüyü Kapatýyor

    }

    public void CloseGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;    //  Editördede Kapatmasý Ýçin
#endif
    }

}
