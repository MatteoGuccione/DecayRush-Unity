using System;
using UnityEngine;

public class EnemyDestroyOnHit : MonoBehaviour
{
    BaseEnemy baseEnemy;

    public BaseEnemy BaseEnemy { get => baseEnemy; set => baseEnemy = value; }

    private Rigidbody rb;

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (!enabled)
    //    {
    //        return;
    //    }


    //    if (collision.gameObject.CompareTag("Bullet"))
    //    {
    //        transform.parent.parent = null;
    //        GetComponentInParent<BaseEnemy>().enabled = false;

    //        foreach (Transform t in transform)
    //        {
    //            Debug.Log(t.name);

    //            t.gameObject.SetActive(true);

    //            Rigidbody rb = t.gameObject.AddComponent<Rigidbody>();
    //            if (rb)
    //            {
    //                rb.AddExplosionForce(20, t.transform.position, 100, 0, ForceMode.Impulse);
    //            }
    //        }

    //        Destroy(transform.parent.gameObject, 3);
    //        enabled = false;
    //    }
    //}

    private void Start()
    {
        rb = gameObject.AddComponent<Rigidbody>();
    }
    public void DestoyAndExplode()
    {
        if (rb)
        {
            gameObject.SetActive(true);
            rb.AddExplosionForce(20, transform.position, 100, 0, ForceMode.Impulse);
        }
    }
}
