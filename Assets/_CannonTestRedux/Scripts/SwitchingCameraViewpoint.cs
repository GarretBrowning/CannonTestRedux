using UnityEngine;
using UnityEngine.InputSystem;

public class SwitchingCameraViewpoint : MonoBehaviour
{
    [Header("Viewpoint References")]
    public Transform thirdPersonViewpoint;
    public Transform firstPersonViewpoint;
    [Header("Camera Reference")]
    public Camera targetCamera;
    [Header("Crosshairs")]
    public GameObject crosshairs;

    private bool isThirdPerson = true;
    private Vector3 initialCameraPosition;
    private Quaternion initialCameraRotation;
    private Transform initialCameraParent;

    void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    void OnEnable()
    {
        if (targetCamera != null)
        {
            // Store initial transform and parent
            initialCameraPosition = targetCamera.transform.position;
            initialCameraRotation = targetCamera.transform.rotation;
            initialCameraParent = targetCamera.transform.parent;
            SetViewpoint(thirdPersonViewpoint);
            crosshairs?.SetActive(false);
            isThirdPerson = true;
        }
    }

    void OnDisable()
    {
        if (targetCamera != null)
        {
            // Restore camera to its original state
            targetCamera.transform.SetParent(initialCameraParent);
            targetCamera.transform.position = initialCameraPosition;
            targetCamera.transform.rotation = initialCameraRotation;
        }
    }

    // Input callback for switching POV
    public void OnSwitchPOV(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ToggleViewpoint();
        }
    }

    public void ToggleViewpoint()
    {
        isThirdPerson = !isThirdPerson;
        SetViewpoint(isThirdPerson ? thirdPersonViewpoint : firstPersonViewpoint);
        crosshairs?.SetActive(!isThirdPerson); // Active in first person, inactive in third person
    }

    private void SetViewpoint(Transform viewpoint)
    {
        if (targetCamera != null && viewpoint != null)
        {
            targetCamera.transform.SetParent(viewpoint);
            targetCamera.transform.localPosition = Vector3.zero;
            targetCamera.transform.localRotation = Quaternion.identity;
        }
    }
}
