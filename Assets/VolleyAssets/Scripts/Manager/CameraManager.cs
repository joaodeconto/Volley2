using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform ballTransform;
    [SerializeField] private float cameraLerpSpeed = .6f;
    [SerializeField] private float maxCameraHeight = 24f;
    [SerializeField] private float cameraHeightOffset = 60f;

    private Camera mainCamera;
    private Vector3 originalCameraPosition;
    private Vector3 targetPosition;
    private Vector3 screenPos;
    private bool isBallAboveLimit = false;

    private void Start()
    {
        mainCamera = Camera.main;
        originalCameraPosition = mainCamera.transform.position;       
    }

    private void LateUpdate()
    {
        if(GameStateManager.Instance.CurrentState != GameStates.GAMEPLAY)
            return;

        if (ballTransform == null)
        {
            ballTransform = FindObjectOfType<BallController>().transform;
        }
        // Check if the ball is above the screen limit
        screenPos = mainCamera.WorldToScreenPoint(ballTransform.position);

        // a little bit of offset to make sure the ball is not at the edge of the screen
        if (screenPos.y + cameraHeightOffset > Screen.height)
        {
            if (!isBallAboveLimit)
            {
                // Move the camera up to follow the ball
                isBallAboveLimit = true;
            }
        }
        else
        {
            if (isBallAboveLimit)
            {
                // Move the camera back to its original position
                isBallAboveLimit = false;
            }
        }

        // Smoothly move the camera to the target position
        targetPosition = isBallAboveLimit ? new Vector3(originalCameraPosition.x, maxCameraHeight, originalCameraPosition.z) : originalCameraPosition;
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, cameraLerpSpeed * Time.deltaTime);
    }
}
