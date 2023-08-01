using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class BallController : MonoBehaviour
{
    [SerializeField] private float gravityModifier = 0.5f; // Gravity modifier
    
    private AudioSource audioSource;
    public int LastToTouch { get; private set; } // ID of the last player to touch the ball

    // Unity event to send message to GameManager
    public static UnityAction<int> OnBallHitCourt;

    private Rigidbody ballRigidbody;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        ballRigidbody = GetComponent<Rigidbody>();
        PlayerInteraction.OnBallStrike += HitBall;
    }

    private void OnDestroy()
    {
        PlayerInteraction.OnBallStrike -= HitBall;
    }

    private void Start()
    {
        if (TryGetComponent(out BallNetwork networkedBall))
        {
            // If you have a BallNetwork component for networking, set isNetworked to true
        }
    }

    private void FixedUpdate()
    {
        ballRigidbody.AddForce(Physics.gravity * gravityModifier, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (!collision.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySound(AudioManager.AudioType.SoundEffect, Random.Range(0,2), audioSource);
        }

        if (GameStateManager.Instance.CurrentState != GameStates.GAMEPLAY)
            return;

        if (collision.gameObject.CompareTag("Court"))
        {
            int side = collision.GetContact(0).point.x < 0 ? 1 : 0;
            OnBallHitCourt?.Invoke(side);
        }

        else if (collision.gameObject.CompareTag("OutBounds"))
        {
            
            OnBallHitCourt?.Invoke(LastToTouch == 0 ? 1:0);
        }

    }

    public void HitBall(BallStrike strikeData)
    {
        // Apply the hit force to the ball in the provided direction
        ballRigidbody.AddForce(strikeData.direction * strikeData.hitForce, ForceMode.Impulse);

        // Set the last player to touch the ball
        LastToTouch = strikeData.teamId;
        AudioManager.Instance.PlaySound(AudioManager.AudioType.SoundEffect, 2, audioSource);
    }
    public void ServeBall(int team)
    {
        int side = team == 0 ? -1 : 1;
        ballRigidbody.AddForce((Vector3.up + Vector3.right *side) * 8 , ForceMode.Impulse);
        LastToTouch = team;
    }
}
