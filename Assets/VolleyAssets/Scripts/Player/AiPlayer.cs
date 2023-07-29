using UnityEngine;
using UnityEngine.EventSystems;

public class AiPlayer : PlayerMovement
{
    private Vector2 aiInput;
    private Vector2 nextInput;
    private Vector2 prevInput;
    private void OnEnable()
    {
        BallPrediction.OnBallPredict += MoveToBall;
    }

    private void OnDisable()
    {
        BallPrediction.OnBallPredict -= MoveToBall;
    }

    public void SetJump()
    {
        // Implement the jump behavior for AI players
        // For example, you can add a force to the Rigidbody component to make the AI player jump
        playerVelocity.y += jumpForce;
    }

    private void MoveToBall(Vector3 ballPrediction)
    {
        if(GameStateManager.Instance.CurrentState != GameStates.GAMEPLAY)
        {
            return;
        }
        if(!CheckBounds(ballPrediction))
        {
            // Calculate the direction from the AI's current position to the ball prediction position
            moveDirection = ballPrediction - transform.position;
            moveDirection.Normalize();       
            moveDirection.y = 0f;
            // Move the AI towards the ball prediction position
            controller.Move(speed * Time.deltaTime * moveDirection);
        }
    }

    public override Vector2 GetMoveInput()
    {
        nextInput = new(moveDirection.x, moveDirection.z);        

        if (prevInput == nextInput)
        {
            aiInput = Vector2.zero;
        }
        else
        {
            aiInput = nextInput;

            if (aiInput.x != 0)
            {
                aiInput.x = Mathf.Sign(aiInput.x);
            }
            if (aiInput.y != 0)
            {
                aiInput.y = Mathf.Sign(aiInput.y);
            }
        }
        //Debug.Log(aiInput+ " = " + moveDirection);
        prevInput = nextInput;
        return aiInput;
    }

    public override bool GetJumpInput()
    {
        return isJumping;
    }

    public override float GetStrikeInput()
    {
        return isStriking ? 1f : 0f;
    }
}
