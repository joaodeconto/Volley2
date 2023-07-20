using UnityEngine;
using UnityEngine.Events;

public class BallPrediction : MonoBehaviour
{
    [SerializeField] private GameObject predictionObj;
    [SerializeField] private float markerYPosition = 0f;
    [SerializeField] private float scaleModifier = 0f;

    public static UnityAction<Vector3> OnBallPredict;

    private Rigidbody ballRigidbody;
    private float timeToGround;
    private float maxHeight;
    private Vector3 predictedPosition;
    private bool sendAiPrediction = false;

    private void Start()
    {
        ballRigidbody = GetComponent<Rigidbody>();
        predictionObj = Instantiate(predictionObj);
        if (GameModeManager.Instance.CurrentGameMode == GameModes.SingleDesktop
            || GameModeManager.Instance.CurrentGameMode == GameModes.OnlyAi)
        {
            sendAiPrediction =true;            
        }
    }

    private void OnDestroy()
    {
        Destroy(predictionObj);
    }

    private void Update()
    {
        PredictBallFall();
        if(sendAiPrediction)
        {
            OnBallPredict?.Invoke(predictedPosition);
        }
        if(ballRigidbody ==null)
            Destroy(this);
    }

    private void PredictBallFall()
    {
        Vector3 ballVelocity = ballRigidbody.velocity;

        // Predict the time it takes for the ball to reach the ground
        timeToGround = CalculateTimeToReachGround();
        if (timeToGround < 0f)
        {
            predictionObj.SetActive(false);
            return;
        }

        // Calculate the maximum height reached by the ball during its fall
        maxHeight = CalculateMaxHeight();

        // Calculate the position of the marker based on the maximum height
        predictedPosition = transform.position + (ballVelocity * timeToGround) - (0.5f * timeToGround * timeToGround * Physics.gravity);
        predictedPosition.y = markerYPosition;

        // Scale the prediction marker based on the distance to the ground (y = markerYPosition)
        //Debug.Log(maxHeight);
        float markerScale = Mathf.Clamp(5 - maxHeight, 1f, 5f);
        predictionObj.transform.localScale = new Vector3(markerScale * scaleModifier, markerYPosition, markerScale* scaleModifier);

        // Set the prediction marker position
        predictionObj.SetActive(true);
        predictionObj.transform.position = predictedPosition;
    }

    private float CalculateTimeToReachGround()
    {
        float verticalVelocity = ballRigidbody.velocity.y;
        float delta = (verticalVelocity * verticalVelocity) - (2f * Physics.gravity.y * transform.position.y);

        if (delta < 0f)
        {
            return -1f; // Ball won't reach the ground
        }

        float timeToGround = (-verticalVelocity - Mathf.Sqrt(delta)) / Physics.gravity.y;
        return timeToGround;
    }

    private float CalculateMaxHeight()
    {
        float verticalVelocity = ballRigidbody.velocity.y;
        float maxHeight = verticalVelocity * verticalVelocity / (2f * Mathf.Abs(Physics.gravity.y));
        return maxHeight;
    }
}
