using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ObjectPooling is a component that enables an object to manage a pool of objects.
/// It provides methods to get and return objects from the pool.
/// </summary>
public class ObjectPooling : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The prefab object to spawn and pool.")]
    private GameObject objectPrefab;

    [SerializeField]
    [Tooltip("The amount of objects to spawn and pool.")]
    private int poolSize;

    private List<GameObject> pooledObjects;
    public List<GameObject> PooledObjects => pooledObjects;

    void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(objectPrefab, transform);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    /// <summary>
    /// Returns an inactive object from the pool.
    /// </summary>
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void ResetPool()
    {
        if (pooledObjects == null) return;
        foreach (var obj in pooledObjects)
        {
            obj.SetActive(false);
        }
    }
}
