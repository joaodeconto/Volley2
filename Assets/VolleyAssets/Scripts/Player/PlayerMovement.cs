using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] protected float speed = 5f; // Player movement speed
    [SerializeField] protected float jumpForce = 5f; // Jump force
    [SerializeField] private float strikeForce = 3f; // Jump force
    [SerializeField] private float playerGravity = -8f; // Jump force
    [SerializeField] private Vector3 playerSpawn;
    [SerializeField] private int textXaxysValue;

    private PlayerStats playerStats;
    private PlayerInput playerInput;
    protected CharacterController controller;
    private Vector2 movementInput;
    private bool isJumping;
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
            Debug.Log("Strike started");
        }
        else if (context.canceled)
        {
            Debug.Log("Strike canceled");
        }
    }
    public void Move(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Jump started");
            isJumping = true;
        }
        else if (context.canceled)
        {
            Debug.Log("Jump canceled");
            isJumping = false;
        }
    }

    private void MovePlayer()
    {
        if (playerStats.playerType == PlayerStats.PlayerType.OVR)
        {
            OVRCameraRig ovrCameraRig = GetComponentInChildren<OVRCameraRig>();
            Vector3 forwardDirection = new Vector3(1,0,0);

            Vector3 rightDirection = new Vector3(0, 0, -1);

            Vector3 moveDirection = forwardDirection * MovementInput.y + rightDirection * MovementInput.x;

            moveDirection *= speed;

            if (!controller.isGrounded)
            {
                playerVelocity.y += playerGravity * Time.deltaTime;
            }
            else
            {
                playerVelocity.y = 0f;
            }

            if (isJumping && controller.isGrounded)
            {
                playerVelocity.y += jumpForce;
                isJumping = false;
            }

            controller.Move((moveDirection * Time.deltaTime) + (playerVelocity * Time.deltaTime));
        }
        else
        {
            // The movement for non-OVR players, similar to your original implementation
            MovementInput = new Vector2(
                IsOutOfBoundsXMinus ? Mathf.Clamp(movementInput.x, 0f, 1f) : movementInput.x,
                IsOutOfBoundsZMinus ? Mathf.Clamp(movementInput.y, 0f, 1f) : movementInput.y
            );

            MovementInput = new Vector2(
                IsOutOfBoundsXPlus ? Mathf.Clamp(movementInput.x, -1f, 0f) : movementInput.x,
                IsOutOfBoundsZPlus ? Mathf.Clamp(movementInput.y, -1f, 0f) : movementInput.y
            );

            moveDirection = new Vector3(MovementInput.x, 0f, MovementInput.y);
            moveDirection = transform.TransformDirection(moveDirection);

            moveDirection *= speed;

            if (!controller.isGrounded)
            {
                playerVelocity.y += playerGravity * Time.deltaTime;
            }
            else
            {
                playerVelocity.y = 0f;
            }

            if (isJumping && controller.isGrounded)
            {
                playerVelocity.y += jumpForce;
                isJumping = false;
            }

            controller.Move((moveDirection * Time.deltaTime) + (playerVelocity * Time.deltaTime));
        }
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
}
