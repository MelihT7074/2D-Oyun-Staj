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
        Time.timeScale = 0f;        //  Zaman Ak���n� Durduruyor

        menuType = MenuTypes.Esc;
        currentOpenMenu = menuType.ToString();

        escMenu.SetActive(true);    //  Esc Men�s�n� Aktif Ediyor
    }

    public void ContinueGameCloseEsc()
    {
        Time.timeScale = 1f;        //  Zaman�n Ak���n� Tekrar Normale �eviriyor

        menuType = MenuTypes.None;
        currentOpenMenu = menuType.ToString();

        escMenu.SetActive(false);   //  Men�y� Kapat�yor

    }

    public void CloseGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;    //  Edit�rdede Kapatmas� ��in
#endif
    }

}
