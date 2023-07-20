using Fusion;
using UnityEngine.InputSystem;

public class PlayerNetworked : NetworkBehaviour
{
    [Networked] public string PlayerName { get; set; }
    [Networked] public int TeamNumber { get; set; }

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        TeamNumber = playerStats.PlayerTeam;
    }
    public override void Spawned()
    {

        if (!HasStateAuthority)
        {
            gameObject.name = "-> RemotePlayer " + Object.Id;
            Destroy(GetComponent<PlayerInput>());
            Destroy(GetComponent<PlayerMovement>());
        }
        if(playerStats.playerType == PlayerStats.PlayerType.OVR)
        {
            Destroy(GetComponent<PlayerInput>());
            Destroy(GetComponent<PlayerMovement>());

        }
        else
        {
            gameObject.name = "--> LocalPlayer";
        }

    }
}
