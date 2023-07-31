using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerMovement : MonoBehaviour, ICharacterInput
{
    [SerializeField] protected float speed = 5f; 
    [SerializeField] protected float jumpForce = 5f;
    [SerializeField] private float playerGravity = -8f;
    [SerializeField] private Vector3 playerSpawn;
    [SerializeField] private float forcedYdown = -.1f;

    public bool IsJumping => isJumping;

    private PlayerStats playerStats;
    private PlayerInput playerInput;
    protected CharacterController controller;
    protected Vector2 movementInput;
    protected bool isJumping;
    protected bool isLanded;
    protected bool isStriking;
    protected bool isJumpRequested;
    protected Vector3 moveDirection;

    protected Vector3 playerVelocity;

    public int TeamPosition => playerStats.PlayerTeam == 0 ? (int)CourtBounds.x * -1 : (int)CourtBounds.x;

    private Vector3 CourtBounds => new(5f, 0f, 5);

    private Vector2 MovementInput
    {
        get => movementInput;
        set => movementInput = value;
    }
    private float NetPosition => playerStats.PlayerTeam == 0 ? -1 : 1;

    private bool IsOutOfBoundsXPlus => transform.position.x > CourtBounds.x + TeamPosition + NetPosition; // - (transform.localScale.x / 2);
    private bool IsOutOfBoundsXMinus => transform.position.x < -CourtBounds.x + TeamPosition + NetPosition; // + (transform.localScale.x / 2);
    private bool IsOutOfBoundsZPlus => transform.position.z > CourtBounds.z + (transform.localScale.z);
    private bool IsOutOfBoundsZMinus => transform.position.z < -CourtBounds.z - (transform.localScale.z);


    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        if (playerStats.playerType == PlayerStats.PlayerType.AI)
            return;
        else
        {
            playerInput.uiInputModule = FindObjectOfType<InputSystemUIInputModule>();            
        }
    }
    private void OnEnable()
    {
        GameOver.OnStateEnter += DeactivateInput;
        GameManager.OnStateEnter += ActivateInput;
    }

    private void OnDestroy()
    {
        GameOver.OnStateEnter -= DeactivateInput;
        GameManager.OnStateEnter -= ActivateInput;
    }

    private void ActivateInput()
    {
        Debug.Log("ActivateInput");
        playerInput.actions["Move"].performed += Move;
        playerInput.actions["Jump"].performed += Jump;
    }
    private void DeactivateInput()
    {
        Debug.Log("DeactivateInput");
        playerInput.actions["Move"].performed -= Move;
        playerInput.actions["Jump"].performed -= Jump;
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
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isJumping) return; 
            isJumpRequested = true;
            isLanded = false;
        }
        else if (context.canceled)
        {
            //Debug.Log("Jump canceled");
            // No need to set isJumping to false here
        }
    }
    //Mobile AI

    protected void MovePlayer()
    {
        movementInput = new Vector2(
               IsOutOfBoundsXMinus ? Mathf.Clamp(movementInput.x, 0f, 1f) : movementInput.x,
               IsOutOfBoundsZMinus ? Mathf.Clamp(movementInput.y, 0f, 1f) : movementInput.y
           );

        movementInput = new Vector2(
            IsOutOfBoundsXPlus ? Mathf.Clamp(movementInput.x, -1f, 0f) : movementInput.x,
            IsOutOfBoundsZPlus ? Mathf.Clamp(movementInput.y, -1f, 0f) : movementInput.y
        );

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

        if (nextPos.x > CourtBounds.x + TeamPosition + NetPosition)
        {
            isOutOfBounds = true;
        }
        else if (nextPos.x < -CourtBounds.x + TeamPosition + NetPosition)
        {
            isOutOfBounds = true;
        }

        if (nextPos.z > CourtBounds.z + (transform.localScale.z / 2))
        {
            isOutOfBounds = true;
        }
        else if (nextPos.z < -CourtBounds.z - (transform.localScale.z / 2))
        {
            isOutOfBounds = true;
        }

        return isOutOfBounds;
    }

    public virtual Vector2 GetMoveInput()
    {
        //invert animation if player is on the right side
        if (TeamPosition == 1)
            movementInput.x *= -1;

        return MovementInput;
    }

    public virtual bool GetJumpInput()
    {
        return isJumping;
    }

    public virtual float GetStrikeInput()
    {
        return isStriking ? 1f : 0f;
    }
}
