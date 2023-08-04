
public interface IGameState
{
    void EnterState();
    void UpdateState();
    void ExitState();

}

public enum GameStates
{
    NONE,
    SPLASHSCREEN,
    MAINMENU,
    LOBBY,
    BREAK,
    GAMEPLAY,
    PAUSED,
    GAMEOVER,
}
