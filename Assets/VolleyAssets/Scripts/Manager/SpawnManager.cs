using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnManager : MonoBehaviour
{
    [Header("Player Prefabs")]
    [SerializeField] private GameObject playerPrefabLocal;
    [SerializeField] private GameObject playerPrefabNetwork;
    [SerializeField] private GameObject AiPrefabLocal;
    [SerializeField] private GameObject OVRPlayer;
    [Header("Ball Prefabs")]
    [SerializeField] private GameObject ballLocalPrefab;
    [SerializeField] private NetworkObject ballNetworkedPrefab;

    public static SpawnManager Instance;

    private GameObject ballSpawned;
    private NetworkRunner runner;

    private void Awake()
    {
        Instance = this;
        
    }

    // Start is called before the first frame update
    public void SpawnNetworkPlayer(PlayerRef playerRef)
    {
        runner = GetComponent<NetworkRunner>();
        runner.Spawn(playerPrefabNetwork, Vector3.zero, Quaternion.identity, playerRef, InitNetworkState);

        void InitNetworkState(NetworkRunner runner, NetworkObject networkObject)
        {
            PlayerStats player = networkObject.gameObject.GetComponent<PlayerStats>();
            player.PlayerTeam = (int)playerRef.PlayerId % 2;
            Debug.Log($"Initializing player {player.PlayerID}");

            if(GameModeManager.Instance.CurrentPlatformType == PlatformType.VR)
                if (playerRef.PlayerId == runner.LocalPlayer.PlayerId)
                {
                    player.transform.parent = OVRPlayer.transform;
                    player.transform.localPosition = Vector3.zero;
                }
        }        
    }
    public void SpawnLocalPlayer()
    {
        if (GameModeManager.Instance.CurrentGameMode == GameModes.SingleDesktop)
        {
            GameObject playerA = Instantiate(playerPrefabLocal, new Vector3(-5, .5f, 0), Quaternion.identity);
            playerA.GetComponent<PlayerStats>().PlayerTeam = 0;

        }
        else if(GameModeManager.Instance.CurrentGameMode == GameModes.SingleOVR)
        {
            GameObject playerA = OVRPlayer;
            playerA.GetComponent<PlayerStats>().PlayerTeam = 0;
            playerA.transform.position = new Vector3(-5, .5f, 0);
        }
        
        GameObject playerB = Instantiate(AiPrefabLocal, new Vector3(5, .5f, 0), Quaternion.identity);
        playerB.GetComponent<PlayerStats>().PlayerTeam = 1;
        Debug.Log("Initializing players A and B");
    }

    public void SpawnOnlyAi()
    {
        PlayerStats playerA = Instantiate(AiPrefabLocal, new Vector3(-5, .5f, 0), Quaternion.identity).GetComponent<PlayerStats>();
        playerA.PlayerTeam = 0;
        playerA.playerType = PlayerStats.PlayerType.AI;
        PlayerStats playerB = Instantiate(AiPrefabLocal, new Vector3(5, .5f, 0), Quaternion.identity).GetComponent<PlayerStats>();
        playerA.playerType = PlayerStats.PlayerType.AI;
        playerB.PlayerTeam = 1;
        Debug.Log("Initializing players OnlyAi");
    }

    public GameObject SpawnNetworkBall()
    {
        if(TryGetComponent<BallNetwork>(out BallNetwork ball))
        {
            return ball.gameObject;
        }
        runner = GetComponent<NetworkRunner>();
        runner.Spawn(ballNetworkedPrefab, Vector3.zero, Quaternion.identity, null, InitNetworkState);

        void InitNetworkState(NetworkRunner runner, NetworkObject networkObject)
        {
            ballSpawned = networkObject.gameObject;
            BallController ball = networkObject.gameObject.GetComponent<BallController>();
            ballSpawned.name = "FirstBall";
            Debug.Log($"Initializing ball");
        }

        return ballSpawned;
    }

    public GameObject SpawnLocalBall()
    {
        ballSpawned = Instantiate(ballLocalPrefab, Vector3.zero, Quaternion.identity);
        ballSpawned.name = "LocalBall";
        return ballSpawned;
    }
}
