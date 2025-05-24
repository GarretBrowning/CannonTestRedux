using UnityEngine;
using UnityEngine.InputSystem;

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

        GameObject bullet = bulletPool.GetPooledObject();
        if (bullet == null) return;
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.transform.rotation = bulletSpawnPoint.rotation;
        bullet.SetActive(true);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
        }
    }
}
