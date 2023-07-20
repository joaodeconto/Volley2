using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int PlayerID;
    [SerializeField] private int playerTeam = 0; // 0 = blue, 1 = red
    [SerializeField] private MeshRenderer playerMeshRenderer;
    public int PlayerTeam { get { return playerTeam; } set { playerTeam = value; } }
    public PlayerType playerType;
    public enum PlayerType
    {
        SINGLEPLAYER,
        MULTIPLAYER,
        OVR,
        AI
    }
    // Start is called before the first frame update
    private void Start()
    {
        if (PlayerTeam == 0)
        {
            gameObject.tag = "TeamA";
            playerMeshRenderer.material.color = Color.blue;
            transform.position = new Vector3(-5f, 0f, 0f);
        }
        else
        {
            gameObject.tag = "TeamB";
            playerMeshRenderer.material.color = Color.red;
            transform.position = new Vector3(5f, 0f, 0f);
        }
    }
}
