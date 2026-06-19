using UnityEngine;

public class LoadManager : MonoBehaviour
{
    public static LoadManager Instance;

    private bool isPlayerReady = false;

    private bool hasLoaded = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void NotifyPlayerReady()
    {
        isPlayerReady = true;
        TryLoadGame();
    }

    private void TryLoadGame()
    {
        if (hasLoaded) return;

        if (isPlayerReady)
        {
            var player = GameObject.FindFirstObjectByType<PlayerManager>();
            if (player != null)
            {
                SaveManager.Instance.LoadGame(player);
                hasLoaded = true;
                Debug.Log("Save caricato dal LoadManager (solo player).");
            }
            else
            {
                Debug.LogWarning("PlayerManager non trovato dal LoadManager.");
            }
        }
    }
}
