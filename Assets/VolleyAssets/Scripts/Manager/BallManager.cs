using Fusion;
using System.Threading.Tasks;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public GameObject ballObj;
    private int lastPoint = 0;
    private Vector3 ballServicePosition;
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

    public async void RestartBall()
    {
        await GetBall();
        if (GameModeManager.Instance.CurrentGameMode == GameModes.MultiPlayerDesktop)
        {
            NetworkObject b = ballObj.GetComponent<NetworkObject>();
            if (!b.HasStateAuthority)
                return;
        }
        ballObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        lastPoint = GameManager.Instance.LastPoint;
        ballServicePosition = new Vector3( 10 * (lastPoint == 0 ? 1: -1), 1, 0);
        Debug.Log("BallManager.RestartBall() ballServicePosition: " + ballServicePosition);
        ballObj.transform.position = ballServicePosition;        
        ballObj.GetComponent<BallController>().ServeBall(lastPoint);
    }

    public async Task<GameObject> GetBall()
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
