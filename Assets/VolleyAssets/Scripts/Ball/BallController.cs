using UnityEngine;
using UnityEngine.Events;

public class BallController : MonoBehaviour
{
    [SerializeField] private float gravityModifier = .5f; // Gravity modifier
    [SerializeField] private float hitForce = 10f; // Force applied to the ball when hit by a player
    public int lastToTouch = 0; // ID of the last player to touch the ball
    private Rigidbody ballRigidbody;
    private Rigidbody playerRigidbody;
    private bool isNetworked;

    // unity action to send message to gamemanager
    public static UnityAction<int> OnBallHitCourt;

    private void Start()
    {
        ballRigidbody = GetComponent<Rigidbody>();
        if(TryGetComponent(out BallNetwork networkedBall))
        {
            isNetworked = true;
        }
    }
    private void FixedUpdate()
    {
        ballRigidbody.AddForce(Physics.gravity * gravityModifier, ForceMode.Acceleration);        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (GameStateManager.Instance.CurrentState != GameStates.GAMEPLAY)
            return;

        if (collision.gameObject.CompareTag("Court"))
        {
            OnBallHitCourt(collision.GetContact(0).point.x < 0 ? 1:0);
        }

        else if (collision.gameObject.CompareTag("OutBounds"))
        {
            OnBallHitCourt(lastToTouch);
        }

        if (collision.gameObject.CompareTag("TeamA") || collision.gameObject.CompareTag("TeamB"))
        {
            playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 direction;
            if (transform.position.x < 0)
            {
                // Direction of the ball is the TEAM forward vector + a bit of upward force
                direction = Vector3.right + Vector3.up;
                //direction += playerRigidbody.velocity;

                lastToTouch = 1;
            }
            else
            {
                // Direction of the ball is the TEAM forward vector + a bit of upward force
                direction = Vector3.right * -1 + Vector3.up;
                //direction += playerRigidbody.velocity;

                lastToTouch = 0;
            }

            // Apply the hit force to the ball in the calculated direction
            ballRigidbody.AddForce(direction * hitForce, ForceMode.Impulse);
        }
    }
}