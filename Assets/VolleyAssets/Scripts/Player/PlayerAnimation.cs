using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float speedMultiplier = 1f;

    private ICharacterInput characterInput;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        characterInput = GetComponent<ICharacterInput>();
    }

    private void Update()
    {
        if (animator != null)
        {

            // Get movement input from character input
            Vector2 moveInput = characterInput.GetMoveInput();
            bool jumpingInput = characterInput.GetJumpInput();
            float strikingInput = characterInput.GetStrikeInput();
            float speed = moveInput.magnitude;

            // Set animator parameters based on input values
            animator.SetBool("Jump", jumpingInput);
            animator.SetFloat("Left", moveInput.y);
            animator.SetFloat("Right", moveInput.y);
            animator.SetFloat("Backward", moveInput.x);
            animator.SetFloat("Forward", moveInput.x);
        }
    }
}

