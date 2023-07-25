using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, ICharacterInput
{
    [SerializeField] protected float speed = 5f; // Player movement speed
    [SerializeField] protected float jumpForce = 5f; // Jump force
    [SerializeField] private float strikeForce = 3f; // Jump force
    [SerializeField] private float playerGravity = -8f; // Jump force
    [SerializeField] private Vector3 playerSpawn;
    [SerializeField] private int textXaxysValue;
    [SerializeField] private float forcedYdown = -.1f;

    private PlayerStats playerStats;
    private PlayerInput playerInput;
    protected CharacterController controller;
    private Vector2 movementInput;
    private bool isJumping;
    private bool isLanded;
    private bool isStriking;
    private bool isJumpRequested;
    private Vector3 moveDirection;

    protected Vector3 playerVelocity;

    public int TeamPosition => playerStats.PlayerTeam == 0 ? (int)CourtBounds.x * -1 : (int)CourtBounds.x;

    private Vector3 CourtBounds => new(5f, 0f, 5);

    private Vector2 MovementInput
    {
        get => movementInput;
        set => movementInput = value;
    }

    private bool IsOutOfBoundsXPlus => transform.position.x > CourtBounds.x + TeamPosition - (transform.localScale.x / 2);
    private bool IsOutOfBoundsXMinus => transform.position.x < -CourtBounds.x + TeamPosition + (transform.localScale.x / 2);
    private bool IsOutOfBoundsZPlus => transform.position.z > CourtBounds.z - (transform.localScale.z / 2);
    private bool IsOutOfBoundsZMinus => transform.position.z < -CourtBounds.z + (transform.localScale.z / 2);


    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        GameManager.OnStateEnter += OnEnterGameplay;
        GameManager.OnStateExit += OnExitGameplay;
    }

    private void OnDestroy()
    {
        GameManager.OnStateEnter -= OnEnterGameplay;
        GameManager.OnStateExit -= OnExitGameplay;
    }

    private void OnEnterGameplay()
    {

        if (playerStats.playerType == PlayerStats.PlayerType.AI)
            return;
        else
        {
            playerInput.enabled = true;
            playerInput.actions.FindActionMap("Player1").Enable();
            playerInput.actions["Move"].performed += Move;
            playerInput.actions["Jump"].performed += Jump;
        }
    }

    private void OnExitGameplay()
    {
        if (playerStats.playerType == PlayerStats.PlayerType.AI)
            return;
        else
        {
            playerInput.enabled = false;
            playerInput.actions.FindActionMap("Player1").Disable();
            playerInput.actions["Move"].performed -= Move;
            playerInput.actions["Jump"].performed -= Jump;
        }

    }
    private void Update()
    {
        if (playerStats.playerType == PlayerStats.PlayerType.AI)
            return;
        MovePlayer();
    }

    protected void Strike(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isStriking = true;
            Debug.Log("Strike started");
        }
        else if (context.canceled)
        {
            isStriking = false;
            Debug.Log("Strike canceled");
        }
    }
    public void Move(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();

        movementInput = new Vector2(
                IsOutOfBoundsXMinus ? Mathf.Clamp(movementInput.x, 0f, 1f) : movementInput.x,
                IsOutOfBoundsZMinus ? Mathf.Clamp(movementInput.y, 0f, 1f) : movementInput.y
            );

        movementInput = new Vector2(
            IsOutOfBoundsXPlus ? Mathf.Clamp(movementInput.x, -1f, 0f) : movementInput.x,
            IsOutOfBoundsZPlus ? Mathf.Clamp(movementInput.y, -1f, 0f) : movementInput.y
        );
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Jump started");
            isJumpRequested = true; // Set the jump request when the jump is performed
            isLanded = false; // Reset isLanded when jump is requested
        }
        else if (context.canceled)
        {
            Debug.Log("Jump canceled");
            // No need to set isJumping to false here
        }
    }

    private void MovePlayer()
    {
        moveDirection = new Vector3(MovementInput.x, forcedYdown, MovementInput.y);
        moveDirection = transform.TransformDirection(moveDirection);

        moveDirection *= speed;
        if (!controller.isGrounded)
        {
            playerVelocity.y += playerGravity * Time.deltaTime;
            isLanded = false; // Character is not landed if not grounded
        }
        else
        {
            playerVelocity.y = 0f;
            if (!isLanded)
            {
                if (isJumpRequested) // Check if a jump request was made
                {
                    playerVelocity.y += jumpForce;
                    isJumpRequested = false; // Reset the jump request
                    isJumping = true; // Set isJumping to true when the character jumps
                }
                else
                {
                    isJumping = false; // Set isJumping to false only when the character lands
                    isLanded = true; // Set isLanded to true when the character lands
                }
            }
        }

        controller.Move((moveDirection * Time.deltaTime) + (playerVelocity * Time.deltaTime));
    }


    protected bool CheckBounds(Vector3 nextPos)
    {
        bool isOutOfBounds = false;

        if (nextPos.x > CourtBounds.x + TeamPosition - (transform.localScale.x / 2))
        {
            isOutOfBounds = true;
        }
        else if (nextPos.x < -CourtBounds.x + TeamPosition + (transform.localScale.x / 2))
        {
            isOutOfBounds = true;
        }

        if (nextPos.z > CourtBounds.z - (transform.localScale.z / 2))
        {
            isOutOfBounds = true;
        }
        else if (nextPos.z < -CourtBounds.z + (transform.localScale.z / 2))
        {
            isOutOfBounds = true;
        }

        return isOutOfBounds;
    }

    public Vector2 GetMoveInput()
    {
        return MovementInput;
    }

    public bool GetJumpInput()
    {
        return isJumping;
    }

    public float GetStrikeInput()
    {
        return isStriking ? 1f : 0f;
    }
}
