using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private Transform armCannonPivot;
    private float currentCannonPitch = 0f;
    private const float INITIAL_CANNON_OFFSET = 90f;

    public void OnRotate(InputAction.CallbackContext context)
    {
        Vector2 rotationInput = context.ReadValue<Vector2>();
        RotatePlayer(rotationInput.x);
        RotateArmCannon(rotationInput.y);
    }

    private void RotatePlayer(float rotationX)
    {
        if (rotationX != 0)
        {
            float yawDelta = rotationX * rotationSpeed * Time.deltaTime;
            transform.Rotate(0f, yawDelta, 0f);
        }
    }

    private void RotateArmCannon(float rotationY)
    {
        if (armCannonPivot == null || rotationY == 0) return;

        float pitchDelta = rotationY * rotationSpeed * Time.deltaTime;
        currentCannonPitch = Mathf.Clamp(currentCannonPitch - pitchDelta, -90f, 90f);
        armCannonPivot.localRotation = Quaternion.Euler(INITIAL_CANNON_OFFSET + currentCannonPitch, 0f, 0f);
    }
}
