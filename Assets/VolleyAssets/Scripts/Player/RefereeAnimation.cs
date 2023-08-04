using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefereeAnimation : MonoBehaviour
{
    private Animator animator;
    [SerializeField] Transform lookAtReference;
    [SerializeField] private Transform ballTransform;
    void Start()
    {
        animator = GetComponent<Animator>();
        
        BallController.OnBallHitCourt += RefereeShowSideAnimation;   
        GameManager.OnStateEnter += RefereeLookAtBall;
    }

    private void OnDestroy()
    {
        BallController.OnBallHitCourt -= RefereeShowSideAnimation;
    }

    private void RefereeLookAtBall()
    {
        if (ballTransform == null)
        {
            ballTransform = BallManager.Instance.BallObject.transform;
        }
    }

    private void RefereeShowSideAnimation(int side)
    {
        if (side == 0)
        {
            animator.SetTrigger("RightWing");
        }
        else
        {
            animator.SetTrigger("LeftWing");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            animator.SetTrigger("BothWings");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(ballTransform != null)
            lookAtReference.transform.position = ballTransform.position;
        
    }
}
