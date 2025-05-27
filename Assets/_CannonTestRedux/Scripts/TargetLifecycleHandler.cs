using System;
using UnityEngine;

/// <summary>
/// TargetLifecycleHandler is a component that handles the lifecycle of targets.
/// </summary>
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

    /// <summary>
    /// Initializes the target with a reference to its object pool and sets up death event handling.
    /// Ensures clean event subscription by unsubscribing before resubscribing to avoid duplicates.
    /// </summary>
    /// <param name="pool">The object pool this target belongs to</param>
    public void Init(ObjectPooling pool)
    {
        objectPool = pool;
        health.OnDied -= HandleDeath;
        health.OnDied += HandleDeath;
    }

    /// <summary>
    /// Handles the death of a target by:
    /// - Scoring points via the Scorable component, which raises the OnScored event for GameManager
    /// - Returning the target GameObject to its object pool
    /// - Invoking OnTargetDestroyed event which GameManager listens to for tracking remaining targets
    /// </summary>
    /// <param name="h">The Health component that triggered the death event</param>
    private void HandleDeath(Health h)
    {
        scorable?.Score();
        objectPool?.ReturnToPool(gameObject);
        OnTargetDestroyed?.Invoke(this);
    }
}
