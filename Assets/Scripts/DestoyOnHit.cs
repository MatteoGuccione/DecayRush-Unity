using Unity.AI.Navigation;
using UnityEngine;

public class DestoyOnHit : MonoBehaviour
{
    [SerializeField] private NavMeshSurface surface;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            // Suono distruzione
            if (EnemyAudioManager.Instance != null)
                EnemyAudioManager.Instance.PlayBoxDestroySound();

            // Registra la cassa come distrutta prima della distruzione
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.RegisterDestroyedBox(gameObject.name);
            }

            gameObject.transform.DetachChildren();
            gameObject.transform.position = new Vector3(-1000, 1000, 1000);
            //surface.UpdateNavMesh(surface.navMeshData);
            Destroy(gameObject, 2);
        }
    }
}

