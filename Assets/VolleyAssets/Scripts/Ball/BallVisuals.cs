using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallVisuals : MonoBehaviour
{
    [SerializeField]private MeshRenderer ballRenderer;
    private TrailRenderer trailRenderer;
    
    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        GameManager.OnStateEnter += ActivateTrail;
        GameManager.OnStateExit += DeactivateTrail;
    }

    private void ActivateTrail()
    {
        trailRenderer.enabled = true;
    }
    private void DeactivateTrail()
    {
        trailRenderer.enabled = false;
    }
}
