using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

public class UIManager : MonoBehaviour
{

    [SerializeField] private PlatformType platformType;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject OVRObj;
    [SerializeField] private GameObject CameraObj;

    private void Start()
    {
        ConfigureCanvasForPlatform(platformType);
        GameModeManager.Instance.SetPlatformType(platformType);
    }

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

    private void ConfigureCanvasForPlatform(PlatformType platformType)
    {
        switch (platformType)
        {
            case PlatformType.Desktop:
                // Set Canvas render mode to Screen Space - Overlay or Screen Space - Camera
                CameraObj.SetActive(true);
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                break;
            case PlatformType.Mobile:
                // Set Canvas render mode to Screen Space - Overlay or Screen Space - Camera
                CameraObj.SetActive(true);
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                break;
            case PlatformType.VR:
                // Set Canvas render mode to World Space
                OVRObj.SetActive(true);
                canvas.renderMode = RenderMode.WorldSpace;
                // Adjust scale for VR
                canvas.transform.localScale = Vector3.one * 0.001f;
                canvas.transform.position = new Vector3(-5f, .7f, -.5f);
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

