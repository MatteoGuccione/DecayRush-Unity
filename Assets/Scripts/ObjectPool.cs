using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool<T> where T : Component, IPoolable
{
    private readonly Queue<T> pool;
    private readonly T prefab;
    private readonly Transform parentHolder;

    public ObjectPool(T prefab, int initialPoolSize, Transform parentHolder = null)
    {
        this.prefab = prefab;
        this.parentHolder = parentHolder;
        pool = new Queue<T>();

        for (int i = 0; i < initialPoolSize; i++) // Instantiate N(initialPoolSize) prefabs and add them to the pool
        {
            // For some reason, when instancing prefabs, if the given transform is owned by another object, the instantciated object will be defined in parent-child hierarchy
            // Example :    - Container
            //              └── Instanciated Object(Clone)
            T instance = GameObject.Instantiate(prefab, parentHolder);
            instance.gameObject.SetActive(false);
            pool.Enqueue(instance);
        }
    }

    public T Get() // Retrieves an instance from the pool, or instantiates a new one if the pool is empty
    {
        T instance = pool.Count > 0 ? pool.Dequeue() : GameObject.Instantiate(prefab, parentHolder);
        instance.gameObject.SetActive(true);
        instance.New();
        return instance;
    }

    public void ReturnToPool(T instance) // Returns an instance to the pool, deactivating it and calling Free() method
    {
        instance.Free();
        instance.gameObject.SetActive(false);
        pool.Enqueue(instance);
    }
}
