using Fusion;
using UnityEngine;

public class ReadyCheck : NetworkBehaviour
{
    public static ReadyCheck Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    [Rpc (sources:RpcSources.StateAuthority, targets:RpcTargets.All)]
    public void Rpc_PlayerReadyChange(bool ready)
    {
        FusionManager.Instance.SetPlayerReady(ready);
    }
}
