using UnityEngine;

public class RandomlyRotating : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Speed of rotation in degrees per second.")]
    private float speed = 90f;

    private Vector3 rotationAxis;

    void Start()
    {
        // Set a random initial rotation:
        transform.rotation = Random.rotation;
        // Set a random rotation axis:
        rotationAxis = Random.onUnitSphere;
    }

    void Update()
    {
        // Rotate around the random axis at the specified speed:
        transform.Rotate(rotationAxis, speed * Time.deltaTime, Space.World);
    }
} 