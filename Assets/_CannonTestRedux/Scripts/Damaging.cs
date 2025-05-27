using UnityEngine;

/// <summary>
/// Component that applies damage to objects implementing IDamageable when triggered.
/// Can be configured to destroy itself on hit.
/// Supports object pooling for efficient reuse.
/// </summary>
public class Damaging : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private bool destroyOnHit = true;
    
    private void OnTriggerEnter(Collider other)
    {
        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage, gameObject);
            
            if (destroyOnHit)
            {
                ObjectPooling pool = GetComponentInParent<ObjectPooling>();
                if (pool != null)
                {
                    pool.ReturnToPool(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}