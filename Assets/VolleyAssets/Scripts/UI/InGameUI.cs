using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour, IScore
{
    [SerializeField] private TMP_Text textTeamA;
    [SerializeField] private TMP_Text textTeamB;
    [SerializeField] private TMP_Text textPoint;
    [SerializeField] private float msgTimer = 3f;

    public static InGameUI Instance;

    private int scoreTeamA = 0;
    private int scoreTeamB = 0;

    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
    }
    private void OnEnable()
    {
        BallController.OnBallHitCourt += AddPoint;
        GameManager.OnClearScore += ClearScore;
    }
    private void OnDisable()
    {
        BallController.OnBallHitCourt -= AddPoint;
        GameManager.OnClearScore -= ClearScore;
    }
    public void ShowInfoText(string msg)
    {
        textPoint.gameObject.SetActive(true);
        textPoint.text = msg;
    }

    private IEnumerator ShowInfoTimedText(int team)
    {
        textPoint.gameObject.SetActive(true);
        textPoint.text = "Point! " + (team == 0 ? "Team A":"Team B");
        yield return new WaitForSeconds(msgTimer);
        textPoint.gameObject.SetActive(false);
    }

    public void AddPoint(int team)
    {
        if (team == 0)
        {
            scoreTeamA++;
            textTeamA.text = scoreTeamA.ToString();
        }
        else
        {
            scoreTeamB++;
            textTeamB.text = scoreTeamB.ToString();
        }
        StartCoroutine(ShowInfoTimedText(team));
    }

    public void ClearScore()
    {
        scoreTeamA = scoreTeamB = 0;
        textTeamA.text = textTeamB.text = "0";        
    }

    public void Result(int team)
    {
    }
}
