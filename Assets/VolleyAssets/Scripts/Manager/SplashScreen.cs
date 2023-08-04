using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : MonoBehaviour, IGameState
{

    [SerializeField] private float _splashScreenDuration = 3f;
    [SerializeField] private GameObject _splashScreenPanel;

    private IEnumerator SplashScreenRoutine()
    {
        _splashScreenPanel.SetActive(true);
        yield return new WaitForSeconds(_splashScreenDuration);
        _splashScreenPanel.SetActive(false);
        GameStateManager.Instance.ChangeState(GameStates.MAINMENU);
    }

    public void EnterState()
    {
        AudioManager.Instance.PlayIntroMusic();
        StartCoroutine(SplashScreenRoutine());        
    }

    public void ExitState()
    {
    }

    public void UpdateState()
    {
    }
}
