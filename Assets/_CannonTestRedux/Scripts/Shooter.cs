using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Shooter is a component that handles shooting bullets.
/// </summary>
public class Shooter : MonoBehaviour
{
    [SerializeField] private ObjectPooling bulletPool;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float bulletSpeed = 10f;

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Fire();
    }

    private void Fire()
    {
        // Check if required components are assigned:
        if (bulletPool == null)
        {
            Debug.LogError("Bullet Pool is not assigned on the Shooter component");
            return;
        }
        if (bulletSpawnPoint == null)
        {
            Debug.LogError("Bullet Spawn Point is not assigned on the Shooter component");
            return;
        }

        // Get a bullet from the object pool:
        GameObject bullet = bulletPool.GetPooledObject();
        if (bullet == null) return; // No bullets available in the pool.

        // Position and activate the bullet:
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.transform.rotation = bulletSpawnPoint.rotation;
        bullet.SetActive(true);

        // Apply velocity to shoot the bullet:
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
        }

        // Track that a bullet was fired:
        BulletTracker.BulletFired();
    }
}
