using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BaseBullet : MonoBehaviour, IPoolable
{
    private float deactivateInterval = 3f;
    private void Start()
    {
    }
    public void Free()
    {
    }

    public void New()
    {
        StartCoroutine(ReturnToPool());
    }
    private IEnumerator ReturnToPool() // Coroutine that handles the return to pool of the Bullet
    {
        yield return new WaitForSeconds(deactivateInterval); // Waits the amount of seconds given before executing the following part
        PoolManager.instance.ReturnToPool<BaseBullet>(PoolableType.Bullet, this);
    }
    public void ResetBullet()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.rotation = Quaternion.identity;
        }
        transform.rotation = Quaternion.identity;
    }
}
