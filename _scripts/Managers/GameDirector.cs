using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [Header("Managers")]
    public Player player;
    public LevelManager levelManager;
    public CameraContorller cameraContorller;
    public EscAndOptions escAndOptions;
    public CursorManager cursorManager;


    private void Start()
    {
        RestartGame();
        SwitchMusic();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        player.RestartPlayer();
        levelManager.RestartLevelManager();
    }

    public void SwitchMusic()
    {
        if (escAndOptions.currentOpenMenu == "None")
        {
            SoundManager.Instance.SwitchMusic("GameMusic");
        }
        else
        {
            SoundManager.Instance.SwitchMusic("EscMenuMusic");
        }
    }


}
