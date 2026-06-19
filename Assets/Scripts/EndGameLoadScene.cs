using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameLoadScene : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("MappaBosco", LoadSceneMode.Additive);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.IsValid() && scene.name == "MappaBosco" && scene.isLoaded)
        {
            EnemySpawner enemySpawner = GameObject.FindFirstObjectByType<EnemySpawner>();
            GameObject spawnerSpawnPosition = GameObject.FindGameObjectWithTag("EnemySpawner");
            if (spawnerSpawnPosition)
                enemySpawner.MoveToScene(scene, spawnerSpawnPosition.transform.position);
            else
                Debug.LogError("Couln't retrieve enemy spawner position of the game object in the MappaBosco");
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.UnloadSceneAsync("City");
        }
    }
}
