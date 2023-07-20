using UnityEngine;

public class AiPlayer : PlayerMovement
{
    private void OnEnable()
    {
        BallPrediction.OnBallPredict += MoveToBall;
    }

    private void OnDisable()
    {
        BallPrediction.OnBallPredict -= MoveToBall;
    }

    public void Jump()
    {
        // Implement the jump behavior for AI players
        // For example, you can add a force to the Rigidbody component to make the AI player jump
        playerVelocity.y += jumpForce;
    }

    private void MoveToBall(Vector3 ballPrediction)
    {
        if(!CheckBounds(ballPrediction))
        {
            // Calculate the direction from the AI's current position to the ball prediction position
            Vector3 direction = ballPrediction - transform.position;
            direction.Normalize();

            // Move the AI towards the ball prediction position
            controller.Move(direction * speed * Time.deltaTime);

            // if ball velocity is low, jump
            /* if (ballRigidbody.velocity.magnitude < 5f)
             {
                 Jump();
             }
            */
            // if ball is above the AI, jump
            if (ballPrediction.y > transform.position.y)
             {
                 Jump();
             }
            

        }
    }
}
