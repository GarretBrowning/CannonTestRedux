using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RespawningTargets is a component that manages the respawning of targets.
/// It ensures targets are spawned in a specified area with minimum spacing.
/// </summary>
[RequireComponent(typeof(ObjectPooling))]
public class RespawningTargets : MonoBehaviour
{
    private ObjectPooling objectPool; // Target object pool.
    private List<GameObject> pooledObjects; // Cache of pooled objects (targets) from objectPool.

    [Header("Spawn Area")]
    [SerializeField] private Vector3 spawnBoxSize = new(10, 5, 4);

    [Header("Spacing")]
    [Tooltip("Minimum distance between respawned targets to avoid overlap.")]
    [SerializeField] private float minDistanceBetweenTargets = 1.0f;

    [SerializeField]
    [Tooltip("Maximum number of attempts to find a valid spawn position for each target.")]
    private int maxSpawnAttempts = 5;

    private void Start()
    {
        objectPool = GetComponent<ObjectPooling>();
        pooledObjects = objectPool.PooledObjects;
    }

    /// <summary>
    /// Respawns all targets if all are currently disabled, ensuring minimum spacing between them.
    /// Returns a list of active TargetLifecycleHandler components.
    /// </summary>
    public List<TargetLifecycleHandler> Respawn()
    {
        if (pooledObjects == null || pooledObjects.Count == 0)
            return new List<TargetLifecycleHandler>();

        // Check if all targets are currently disabled:
        bool allDisabled = true;
        foreach (var target in pooledObjects)
        {
            if (target.activeInHierarchy)
            {
                allDisabled = false;
                break;
            }
        }

        // If all targets are disabled, respawn them with minimum spacing:
        if (allDisabled)
        {
            List<Vector3> usedPositions = new();
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                var target = pooledObjects[i];
                // Try to find a valid spawn position with enough distance from others
                Vector3 spawnPos = Vector3.zero;
                for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
                {
                    spawnPos = GetRandomSpawnPosition();
                    bool tooClose = false;
                    foreach (var pos in usedPositions)
                    {
                        if (Vector3.Distance(spawnPos, pos) < minDistanceBetweenTargets)
                        {
                            tooClose = true;
                            break;
                        }
                    }
                    if (!tooClose) break; // Found a valid position
                }
                target.transform.position = spawnPos;
                usedPositions.Add(spawnPos);
                target.SetActive(true);
            }
        }

        // Gather all active targets and initialize them:
        List<TargetLifecycleHandler> activeTargets = new();
        foreach (var target in pooledObjects)
        {
            if (target.activeInHierarchy)
            {
                var handler = target.GetComponent<TargetLifecycleHandler>();
                handler.Init(objectPool);
                activeTargets.Add(handler);
            }
        }
        return activeTargets;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 halfExtents = spawnBoxSize * 0.5f;
        return transform.position + new Vector3(
            Random.Range(-halfExtents.x, halfExtents.x),
            Random.Range(-halfExtents.y, halfExtents.y),
            Random.Range(-halfExtents.z, halfExtents.z)
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnBoxSize);
    }

    public void ResetTargets()
    {
        objectPool?.ResetPool();
    }
} 