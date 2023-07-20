
public interface IGameState
{
    void EnterState();
    void UpdateState();
    void ExitState();

}

public enum GameStates
{
    NONE,
    MAINMENU,
    LOBBY,
    BREAK,
    GAMEPLAY,
    PAUSED,
    GAMEOVER,
}
