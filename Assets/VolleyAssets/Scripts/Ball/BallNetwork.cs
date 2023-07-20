using Fusion;
using System.Net;
using UnityEngine;
using UnityEngine.Events;

public class BallNetwork : NetworkBehaviour
{
    protected Rigidbody BallRigidbody { get; set; }
    protected Vector3 CollisionReference { get; set; }
    protected AudioSource AudioSource { get; set; }


    public static UnityAction<int> OnNetworkScore;
    public override void Spawned()
    {
        BallController.OnBallHitCourt += RpcAddPoint;

        BallRigidbody = GetComponent<Rigidbody>();
        AudioSource = GetComponent<AudioSource>();
    }
    [Rpc(sources:RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RpcAddPoint(int team)
    {
        if(!Object.HasStateAuthority)
        {
            BallController.OnBallHitCourt(team);
        }        
    }

    public override void FixedUpdateNetwork()
    {
      
    }


    protected async virtual void AskAuthority()
    {
        if (!Object.HasStateAuthority)
        {
            Object.AssignInputAuthority(Runner.LocalPlayer);
            await Object.WaitForStateAuthority();
        }
    }
    public void SetDespawn()
    {
        Runner.Despawn(this.GetComponent<NetworkObject>());
    }
}
