using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOver : MonoBehaviour, IGameState, IScore
{
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private Button restartMacth;
    [SerializeField] private Button quitMacth;
    [SerializeField] private GameObject endGamePanel;


    public static UnityAction OnStateEnter;
    public static UnityAction OnStateUpdate;
    public static UnityAction OnStateExit;
    public void AddPoint(int team)
    {
    }
    public void ClearScore()
    {
    }

    public void Result(int team)
    {
        if (team == 0)
        {
            winnerText.text = "End Game, Winner " + "Team A Wins!";
        }
        else
        {
            winnerText.text = "End Game, Winner " + "Team B Wins!";
        }
    }

    public void EnterState()
    {
        endGamePanel.SetActive(true);
        restartMacth.onClick.AddListener(() => GameStateManager.Instance.ChangeState(GameStates.GAMEPLAY));
        quitMacth.onClick.AddListener(() => GameStateManager.Instance.ChangeState(GameStates.MAINMENU));
        OnStateEnter?.Invoke();
    }
    public void ExitState()
    {
        endGamePanel.SetActive(false);
        OnStateExit?.Invoke();
    }

    public void UpdateState()
    {
    }
}
