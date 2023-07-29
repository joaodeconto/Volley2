using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using UnityEngine.XR;

public class UIManager : MonoBehaviour
{

    [SerializeField] private PlatformType platformType;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject OVRObj;
    [SerializeField] private GameObject cameraObj;
    [SerializeField] private GameObject mobileJoystick;
    [SerializeField] private Button jumpButtonMobile;
    [SerializeField] private EventSystem eventSystem;

    public static UnityAction OnJumpPressed;

    private void Start()
    {
        platformType = GameModeManager.Instance.CurrentPlatformType;
        ConfigureCanvasForPlatform(platformType);
        DeactivateJoystick();
    }
    private void ConfigureCanvasForPlatform(PlatformType platformType)
    {
        switch (platformType)
        {
            case PlatformType.Desktop:
                cameraObj.SetActive(true);
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                break;
            case PlatformType.Mobile:
                cameraObj.SetActive(true);
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                GameManager.OnStateEnter += ActivateJoystick;
                GameOver.OnStateEnter += DeactivateJoystick;
                break;
            case PlatformType.VR:
                // Set Canvas render mode to World Space
                OVRObj.SetActive(true);
                canvas.renderMode = RenderMode.WorldSpace;
                eventSystem.GetComponent<InputSystemUIInputModule>().enabled = false;
                // Adjust scale for VR
                canvas.transform.localScale = Vector3.one * 0.001f;
                canvas.transform.position = new Vector3(-1f, .7f, -.5f);
                canvas.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                // Add OVR Raycaster component if not present
                if (!canvas.GetComponent<OVRPhysicsRaycaster>())
                {
                    canvas.gameObject.AddComponent<OVRRaycaster>();
                }
                break;
        }
    }

    private void ActivateJoystick()
    {
        if(mobileJoystick.activeSelf == true) 
            return;
        mobileJoystick.SetActive(true);
    }
    private void DeactivateJoystick()
    {
        mobileJoystick.SetActive(false);
        jumpButtonMobile.onClick.RemoveAllListeners();
    }
    private void OnDestroy()
    {
        if(platformType == PlatformType.Mobile)
        {
            GameManager.OnStateEnter -= ActivateJoystick;
            GameManager.OnStateExit -= DeactivateJoystick;
        }
    }
}

