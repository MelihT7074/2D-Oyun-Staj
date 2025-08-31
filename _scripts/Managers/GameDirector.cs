using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [Header("Managers")]
    public Player player;
    public LevelManager levelManager;
    public CameraContorller cameraContorller;
    public EscAndOptions escAndOptions;
    public CursorManager cursorManager;
    public KeybindingManager KBM;


    private void Start()
    {
        RestartGame();
        GD_SwitchMusic();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KBM.GetKey("Restart")) && escAndOptions.currentOpenMenu != "OptionsKeybinding")    //  Tuþ Atarken Çalýþmasýn Diye
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        player.RestartPlayer();
        levelManager.RestartLevelManager();
    }

    public void GD_SwitchMusic()
    {
        if (escAndOptions.currentOpenMenu == "None" && player.isAlive)
        {
            SoundManager.Instance.SwitchMusic("GameMusic");
            print("Oyun Müziði Çalýyor");
        }
        else if (escAndOptions.currentOpenMenu != "None" && player.isAlive)
        {
            SoundManager.Instance.SwitchMusic("EscMenuMusic");
            print("Esc Menüsü Müziði Çalýyor");
        }
        else
        {
            SoundManager.Instance.SwitchMusic("DeathMusic");
            print("Ölüm Ekraný Müziði Çalýyor");
        }
    }


}
