using UnityEngine;
using UnityEngine.XR;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance;
    public PlatformType CurrentPlatformType;
    public GameModes CurrentGameMode;
    public GameDifficulty CurrentGameDifficulty;
    private void Awake()
    {
        Instance = this;
        SetPlatformType(GetPlatformType());
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

    private PlatformType GetPlatformType()
    {
        if (Application.isMobilePlatform)
        {
            return PlatformType.Mobile;
        }
        else if (XRSettings.enabled)
        {
            return PlatformType.VR;
        }
        else
        {
            return PlatformType.Desktop;
        }
    }
}
