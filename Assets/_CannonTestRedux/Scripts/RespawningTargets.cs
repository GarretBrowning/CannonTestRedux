using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPooling))]
public class RespawningTargets : MonoBehaviour
{
    private ObjectPooling objectPool;
    private List<GameObject> pooledObjects; // Cache of pooled objects from objectPool

    [Header("Spawn Area")]
    [SerializeField] private Vector3 spawnBoxSize = new(10, 5, 4);

    [Header("Spacing")]
    [Tooltip("Minimum distance between respawned targets to avoid overlap.")]
    [SerializeField] private float minDistanceBetweenTargets = 1.0f;
    [SerializeField] private int maxSpawnAttempts = 5;

    [Header("Debug")]
    [SerializeField] private bool debug;

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
        // Return empty if no pooled objects
        if (pooledObjects == null || pooledObjects.Count == 0)
            return new List<TargetLifecycleHandler>();

        // Debug: Print active state of all pooled objects
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (debug) Debug.Log($"[Respawn] Target {i} active: {pooledObjects[i].activeInHierarchy}");
        }

        // Check if all targets are currently disabled
        bool allDisabled = true;
        foreach (var obj in pooledObjects)
        {
            if (obj.activeInHierarchy)
            {
                allDisabled = false;
                break;
            }
        }

        if (!allDisabled)
        {
            if (debug) Debug.LogWarning("[Respawn] Not all targets are disabled, skipping respawn.");
        }

        // If all are disabled, respawn them with minimum spacing
        if (allDisabled)
        {
            List<Vector3> usedPositions = new();
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                var obj = pooledObjects[i];
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
                obj.transform.position = spawnPos;
                usedPositions.Add(spawnPos);
                obj.SetActive(true);
                if (debug) Debug.Log($"[Respawn] Target {i} set active at {spawnPos}");
            }
        }

        // Gather all active targets and initialize them
        List<TargetLifecycleHandler> activeTargets = new();
        foreach (var obj in pooledObjects)
        {
            if (obj.activeInHierarchy)
            {
                var handler = obj.GetComponent<TargetLifecycleHandler>();
                handler.Init(objectPool);
                activeTargets.Add(handler);
            }
        }
        if (debug) Debug.Log($"[Respawn] Returning {activeTargets.Count} active targets.");
        return activeTargets;
    }

    // Returns a random position within the spawn box volume centered on this object
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
        if (objectPool != null)
            objectPool.ResetPool();
    }
} 