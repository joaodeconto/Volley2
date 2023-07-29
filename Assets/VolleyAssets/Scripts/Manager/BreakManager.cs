using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakManager : MonoBehaviour, IGameState
{
    IEnumerator BreakWait()
    {
        yield return new WaitForSeconds(3);
        GameStateManager.Instance.ChangeState(GameStates.GAMEPLAY);
    }

    public void EnterState()
    {
        StartCoroutine(BreakWait());
    }

    public void UpdateState()
    {
    }

    public void ExitState()
    {
    }
}
