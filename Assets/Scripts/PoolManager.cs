using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private List<BulletEntry> bulletEntries;

    public static PoolManager instance; // Singleton instance of the PoolManager

    private readonly Dictionary<PoolableType, object> pools = new Dictionary<PoolableType, object>();

    public void Start()
    {
        foreach (BulletEntry entry in bulletEntries)
            PoolManager.instance.CreatePool(entry.BulletType, entry.Prefab, entry.InitialPoolSize, this.transform);

    }
    public void Awake() // Ensures that only one instance of PoolManager exists in the scene, if not, it destroys the new instance
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Creates a new object pool for the specified type T with the given key, prefab, initial size, and optional parent holder
    public void CreatePool<T>(PoolableType key, T prefab, int initialPoolSize, Transform parentHolder = null) where T : Component, IPoolable
    {
        if (!pools.ContainsKey(key))
        {
            ObjectPool<T> pool = new ObjectPool<T>(prefab, initialPoolSize, parentHolder);
            pools.Add(key, pool);
        }
    }

    // Retrieves an instance of type T from the pool associated with the given key
    public T Get<T>(PoolableType key) where T : Component, IPoolable
    {
        if (pools.TryGetValue(key, out object pool) && pool is ObjectPool<T> typedPool) // check if the pool exists and is of the correct type of given PoolableType
            return typedPool.Get();
        return null;
    }

    // Returns an instance of type T back to the pool associated with the given key
    public void ReturnToPool<T>(PoolableType key, T instance) where T : Component, IPoolable
    {
        if (pools.TryGetValue(key, out object pool) && pool is ObjectPool<T> typedPool)
            typedPool.ReturnToPool(instance);
    }
}