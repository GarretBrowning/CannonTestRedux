using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 1f;
    private float currentHealth;
    
    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(float damage, GameObject source)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
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