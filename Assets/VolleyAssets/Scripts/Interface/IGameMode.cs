public interface IGameMode
{
    public GameModes GameMode { get; set; }
    void SetDifficulty(GameDifficulty difficulty);
    void StartGame();
}

public enum GameModes
{
    SingleDesktop,
    SingleOVR,
    MultiPlayerDesktop,
    MultiplayerOVR,
    OnlyAi
}
public enum GameDifficulty
{
    Easy,
    Medium,
    Hard
}

public enum PlatformType
{
    Desktop,
    Mobile,
    VR
}