using UnityEngine;

public class Explosion : MonoBehaviour
{
    private SphereCollider collider;
    private ParticleSystem explosionVFX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        explosionVFX = GetComponent<ParticleSystem>();
        collider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            collider.enabled = true;
            Destroy(gameObject, 0.5f);
            explosionVFX.Play();
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
