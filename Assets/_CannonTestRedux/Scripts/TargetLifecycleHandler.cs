using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class TargetLifecycleHandler : MonoBehaviour
{
    private Health health;
    private Scorable scorable;
    private ObjectPooling objectPool;

    public event Action<TargetLifecycleHandler> OnTargetDestroyed;

    private void Awake()
    {
        health = GetComponent<Health>();
        scorable = GetComponent<Scorable>();
    }

    public void Init(ObjectPooling pool)
    {
        objectPool = pool;
        health.OnDied -= HandleDeath;
        health.OnDied += HandleDeath;
    }

    private void HandleDeath(Health h)
    {
        scorable?.Score(); // raises OnScored event, which GameManager listens to
        objectPool?.ReturnToPool(gameObject); // centralized and explicit
        OnTargetDestroyed?.Invoke(this); // wave tracker can count this
    }
}
