using UnityEngine;
using System;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 1f;
    private float currentHealth;

    public event Action<Health> OnDied;

    private void OnEnable() => currentHealth = maxHealth;

    public void TakeDamage(float damage, GameObject source)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            OnDied?.Invoke(this);
        }
    }
}
