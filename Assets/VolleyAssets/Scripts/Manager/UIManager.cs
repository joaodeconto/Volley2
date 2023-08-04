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
    [SerializeField] private Button pauseButton;
    [SerializeField] private EventSystem eventSystem;

    public static UnityAction OnJumpPressed;

    private void Start()
    {
        //GameModeManager.Instance.SetPlatformType(platformType);
        pauseButton.onClick.AddListener(SetPause);
        ConfigureCanvasForPlatform(GameModeManager.Instance.CurrentPlatformType);
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
        }
    }

    private void ActivateJoystick()
    {
        //Debug.Log("Activating Joystick");
        mobileJoystick.SetActive(true);
    }
    private void DeactivateJoystick()
    {
        //Debug.Log("Deactivating Joystick");
        mobileJoystick.SetActive(false);
    }

    private void SetPause()
    {
        if(GameStateManager.Instance.CurrentState != GameStates.PAUSED)
            GameStateManager.Instance.ChangeState(GameStates.PAUSED);
        else
            GameStateManager.Instance.ChangeState(GameStateManager.Instance.PreviousState);
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

