using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.XR;

public class UIManager : MonoBehaviour
{

    [SerializeField] private PlatformType platformType;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject OVRObj;
    [SerializeField] private GameObject CameraObj;
    [SerializeField] private EventSystem eventSystem;

    private void Start()
    {
        ConfigureCanvasForPlatform(platformType);
        GameModeManager.Instance.SetPlatformType(platformType);
    }
    //TODO implement this
    private PlatformType GetPlatformType()
    {
        if (Application.isMobilePlatform)
        {
            return PlatformType.Mobile;
        }
        else if (XRSettings.enabled)
        {
            return PlatformType.VR;
        }
        else
        {
            return PlatformType.Desktop;
        }
    }
    // Input system conflicts with OVR input system
    // TODO - Fix this set all to input system
    private void ConfigureCanvasForPlatform(PlatformType platformType)
    {
        switch (platformType)
        {
            case PlatformType.Desktop:
            case PlatformType.Mobile:
                // Set Canvas render mode to Screen Space - Overlay or Screen Space - Camera

                //eventSystem.GetComponent<OVRInputModule>().enabled = false;
                CameraObj.SetActive(true);
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
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
}

