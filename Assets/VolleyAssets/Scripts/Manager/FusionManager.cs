using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FusionManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static FusionManager Instance { get; private set; }
    private NetworkRunner _runner;
    private Action<NetworkRunner, ConnectionStatus, string> _connectionCallback;
    private ConnectionStatus _status;
    private Action<NetworkRunner> _spawnWorldCallback;
    private Action<NetworkRunner, PlayerRef> _spawnPlayerCallback;
    private Action<NetworkRunner, PlayerRef> _despawnPlayerCallback;
    private Action<bool> _maxPlayersReached;
    private Action allPlayersReady;
    private int playersReady = 1;

    [SerializeField] private GameObject _rpcObj;

    public enum ConnectionStatus
    {
        Disconnected,
        Connecting,
        Failed,
        Connected,
        Loading,
        Loaded
    }

    private void Awake()
    {
        Instance = this;
    }

    public async void LaunchLobby(GameMode mode, string room, int maxPlayers,
        Action<NetworkRunner, ConnectionStatus, string> onConnect, Action<bool> onMaxPlayers, Action onAllPlayersReady)
    {
        _maxPlayersReached = onMaxPlayers;
        _connectionCallback = onConnect;
        allPlayersReady = onAllPlayersReady;
        SetConnectionStatus(ConnectionStatus.Connecting, "");

        if (_runner == null)
            _runner = gameObject.AddComponent<NetworkRunner>();

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = room,
            PlayerCount = maxPlayers,
        });

    }

    public void SetConnectionStatus(ConnectionStatus status, string message)
    {
        _status = status;
        if (_connectionCallback != null)
            _connectionCallback(_runner, status, message);
    }

    public void DisconectPlayer()
    {
        _runner.Shutdown();
    }
    public void SetPlayerReady(bool ready)
    {
        if (ready)
            playersReady = 2;
        else
            playersReady--;

        Debug.Log("Players Ready: " + playersReady);
        if (playersReady == _runner.SessionInfo.PlayerCount)
        {
            Debug.Log("All Players Ready");
            allPlayersReady();
        }
    }
    void CheckMaxPlayers(NetworkRunner runner)
    {
        if (runner.SessionInfo.PlayerCount == runner.SessionInfo.MaxPlayers)
        {
            Debug.Log("Max Players " + runner.SessionInfo.PlayerCount);
            runner.SessionInfo.IsOpen = false;
            _maxPlayersReached(true);
            allPlayersReady(); // GAMBI
        }
        else
        {
            runner.SessionInfo.IsOpen = true;
            _maxPlayersReached(false);
            Debug.Log("Not Max Players " + runner.SessionInfo.PlayerCount);
        }
    }
    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void Rpc_PlayerReadyChange(bool ready)
    {
        SetPlayerReady(ready);
    }

    #region NetworkRunner Callbacks

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server");
        if (runner.GameMode == GameMode.Shared)
        {
            SpawnManager.Instance.SpawnNetworkPlayer(runner.LocalPlayer);
            Debug.Log("SPAWNED my player Shared Mode");
            if (runner.LocalPlayer.PlayerId == 0)
                SpawnManager.Instance.SpawnNetworkBall();
        }
        SetConnectionStatus(ConnectionStatus.Connected, "");
    }
    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("Disconnected from server");
        SetConnectionStatus(ConnectionStatus.Disconnected, "");
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        request.Accept();
    }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log($"Connect failed {reason}");
        SetConnectionStatus(ConnectionStatus.Failed, reason.ToString());
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        CheckMaxPlayers(runner);
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player Left");

        //_despawnPlayerCallback(runner, player);

        CheckMaxPlayers(runner);

        SetConnectionStatus(_status, "Player Left");
        if (GameStateManager.Instance.CurrentState != GameStates.LOBBY)
            GameStateManager.Instance.ChangeState(GameStates.MAINMENU);
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("OnShutdown");
        string message = "";
        switch (shutdownReason)
        {
            /*case GameManager.ShutdownReason_GameAlreadyRunning:
                message = "Game in this room already started!";
                break;*/
            case ShutdownReason.IncompatibleConfiguration:
                message = "This room already exist in a different game mode!";
                break;
            case ShutdownReason.Ok:
                message = "User terminated network session!";
                break;
            case ShutdownReason.Error:
                message = "Unknown network error!";
                break;
            case ShutdownReason.ServerInRoom:
                message = "There is already a server/host in this room";
                break;
            case ShutdownReason.DisconnectedByPluginLogic:
                message = "The Photon server plugin terminated the network session!";
                break;
            default:
                message = shutdownReason.ToString();
                break;
        }
        SetConnectionStatus(ConnectionStatus.Disconnected, message);
        Destroy(_runner);
    }

    #endregion
}
