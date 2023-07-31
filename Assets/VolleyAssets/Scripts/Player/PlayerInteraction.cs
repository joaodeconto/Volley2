using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public struct BallStrike
{
    public Vector3 direction;
    public float hitForce;
    public int teamId;

    public BallStrike(Vector3 direction, float hitForce, int teamId)
    {
        this.direction = direction;
        this.hitForce = hitForce;
        this.teamId = teamId;
    }
}

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float hitForceUp = 2f;
    [SerializeField] private float hitForce = 6f;

    private PlayerStats playerStats;
    private PlayerMovement playerMovement;
    private bool recentTouch = false;

    private Vector3 SideDirection
    {
        get
        {
            return playerStats.PlayerTeam == 0 ? transform.right : transform.right * -1;
        }
    }

    private Vector3 hitDirection;

    public static UnityAction<BallStrike> OnBallStrike;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(recentTouch)
        {
            return;
        }
        if (collision.gameObject.CompareTag("Ball"))
        {
            Vector3 ballDirection = CalculateBallDirection();
            StrikeBall(ballDirection);
            StartCoroutine(RecentTouched());
        }
    }

    private IEnumerator RecentTouched()
    {
        recentTouch = true;
        yield return new WaitForSeconds(0.5f);
        recentTouch = false;
    }

    private Vector3 CalculateBallDirection()
    {
        if (playerMovement.IsJumping)
        {
            hitDirection = SideDirection;
        }
        else
        {
            hitDirection = SideDirection + (Vector3.up * hitForceUp);
        }

        return hitDirection.normalized;
    }

    private void StrikeBall(Vector3 direction)
    {
        OnBallStrike(new BallStrike(direction, hitForce, playerStats.PlayerTeam));
    }
}
