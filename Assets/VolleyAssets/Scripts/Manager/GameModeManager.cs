using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance;
    public PlatformType CurrentPlatformType;
    public GameModes CurrentGameMode;
    public GameDifficulty CurrentGameDifficulty;
    private void Awake()
    {
        Instance = this;
    }
    public void SetPlatformType(PlatformType platformType)
    {
        CurrentPlatformType = platformType;
    }
    public void SetGameMode(GameModes gameMode)
    {
        CurrentGameMode = gameMode;
    }
    public void SetGameDifficulty(GameDifficulty gameDifficulty)
    {
        CurrentGameDifficulty = gameDifficulty;
    }
}
