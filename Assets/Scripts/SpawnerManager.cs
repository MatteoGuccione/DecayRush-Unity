using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class SpawnerManager<T> : MonoBehaviour where T : Component
{
    protected Coroutine spawnCoroutine;
    //protected List<T> prefabsToSpawn;
    protected List<PoolableType> prefabsToPool;
    protected float minRadius;
    protected float maxRadius;
    protected float spawnInterval;
    protected GameObject reference;
    public abstract void Initialize();
    protected IEnumerator Spawn() // Coroutine that handles repeated spawning of objects
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval); // Waits the amount of seconds given before executing the following part

            //if (this.prefabsToSpawn != null && this.prefabsToSpawn.Any() && this.reference != null)
            if (this.prefabsToPool != null && this.prefabsToPool.Any() && this.reference != null)
            {
                bool spawned = false;
                while (!spawned)
                {
                    //Component prefabToSpawn = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Count)];
                    PoolableType prefabTypeToPool = prefabsToPool[Random.Range(0, prefabsToPool.Count)];
                    Component pulledPrefab = PoolManager.instance.Get<BaseEnemy>(prefabTypeToPool);

                    //float radius = GetPrefabRadius(prefabToSpawn);
                    float radius = GetPrefabRadius(pulledPrefab);
                    // Gets an angle between 0 - 360 degrees in radians
                    float angle = Random.Range(0f, Mathf.PI * 2f);

                    // Gets a distance between min radius and max radius
                    float distance = Random.Range(minRadius, maxRadius);

                    // Retrieves the position calculating x and z
                    Vector3 spawnPosition = this.reference.transform.position +
                        new Vector3(
                            Mathf.Cos(angle) * distance,
                            0f,
                            Mathf.Sin(angle) * distance
                        );

                    //spawnPosition = spawnPosition + Vector3.up * GetYOffsetFromPrefab(prefabToSpawn); // Adds height offset of the Prefab
                    spawnPosition = spawnPosition + Vector3.up * GetYOffsetFromPrefab(pulledPrefab); // Adds height offset of the Prefab
                    if (IsPositionFree(spawnPosition, radius))
                    {
                        pulledPrefab.transform.position = spawnPosition;
                        spawned = true;
                    }
                }
            }
            else
                Debug.LogWarning("PrefabToSpawn or Reference is null!");
        }
    }

    protected void StartSpawning() // Starts the spawning coroutine if it is not already running
    {
        if (spawnCoroutine == null)
            spawnCoroutine = StartCoroutine(Spawn());
    }
    protected void StopSpawning() // Stops the spawning coroutine if it is running
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    // Usefull if need to check physical objects with colliders
    // Returns true if no collider is overlapping the given sphere
    private bool IsPositionFree(Vector3 position, float checkRadius) => !Physics.CheckSphere(position, checkRadius);

    private float GetPrefabRadius(Component prefabToSpawn)
    {
        float radius = .5f; // Default small value if the GameObject is empty
        Renderer renderer = prefabToSpawn.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Vector3 extents = renderer.bounds.extents; // Get half of the limit borders of the renderer (visual of the object)
            return Mathf.Max(extents.x, extents.z); // Get the longer border to define the radius
        }
        return radius;
    }
    private float GetYOffsetFromPrefab(Component prefabToSpawn)
    {
        float yOffset = .5f; // Default smallest value

        // Try to get the bounds from a Renderer limit borders first 
        Renderer renderer = prefabToSpawn.GetComponentInChildren<Renderer>();
        if (renderer != null)
            yOffset = renderer.bounds.size.y + 0.1f; // full half-height + padding
        else
        {
            // If no Renderer, try Collider
            Collider collider = prefabToSpawn.GetComponentInChildren<Collider>();
            if (collider != null)
                yOffset = collider.bounds.extents.y * 0.5f + 0.1f;
        }

        return yOffset;
    }
}