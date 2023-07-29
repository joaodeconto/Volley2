using Fusion;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public GameObject ballObj;

    private void OnEnable()
    {
        // Register event listeners when the script is enabled
        GameManager.OnStateEnter += RestartBall;
    }

    private void OnDisable()
    {
        // Unregister event listeners when the script is disabled
        GameManager.OnStateEnter -= RestartBall;
    }

    public void RestartBall()
    {
        GetBall();
        if (GameModeManager.Instance.CurrentGameMode == GameModes.MultiPlayerDesktop)
        {
            NetworkObject b = ballObj.GetComponent<NetworkObject>();
            if (!b.HasStateAuthority)
                return;
        }
        ballObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Vector3 ballRandomPosition = new Vector3(Random.Range(-10f, 10f), 7, Random.Range(-5f, 5f));
        ballObj.transform.position = ballRandomPosition;
    }

    public GameObject GetBall()
    {
        if (ballObj != null)
            return ballObj;

        if (GameModeManager.Instance.CurrentGameMode == GameModes.MultiPlayerDesktop)
        {
            BallNetwork networkBall = FindObjectOfType<BallNetwork>();
            if (networkBall != null)
            {
                ballObj = networkBall.gameObject;
                return ballObj;
            }
            else
            {
                ballObj = SpawnManager.Instance.SpawnNetworkBall();
                return ballObj;
            }
        }
        else
        {
            if (ballObj == null)
            {
                ballObj = SpawnManager.Instance.SpawnLocalBall();
            }
            return ballObj;
        }
    }
}
