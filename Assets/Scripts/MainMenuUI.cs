using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [Header("Pulsante iniziale selezionato (per gamepad)")]
    [SerializeField] private Button firstSelectedButton;

    [Header("Pulsante Carica Partita")]
    [SerializeField] private Button loadButton;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private float sceneChangeDelay = 0.3f;

    void Start()
    {
        if (firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
        }
        else
        {
            Debug.LogWarning("firstSelectedButton non assegnato nel MainMenuUI.");
        }

        if (loadButton != null)
        {
            string savePath = Application.persistentDataPath + "/save.json";
            loadButton.interactable = File.Exists(savePath);
        }
        else
        {
            Debug.LogWarning("loadButton non assegnato nel MainMenuUI.");
        }
    }

    public void OnStartButton()
    {
        PlayClickAndLoadScene("City", deleteSave: true);
    }

    public void OnLoadButton()
    {
        string savedScene = SaveManager.Instance.GetSavedSceneName();
        if (!string.IsNullOrEmpty(savedScene))
        {
            PlayClickAndLoadScene(savedScene, deleteSave: false);
        }
        else
        {
            Debug.LogWarning("Nessuna scena salvata trovata. Carico 'City' come fallback.");
            PlayClickAndLoadScene("City", deleteSave: false);
        }
    }

    public void OnQuitButton()
    {
        StartCoroutine(PlayClickAndQuit());
    }

    private void PlayClickAndLoadScene(string sceneName, bool deleteSave)
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        StartCoroutine(DelayedSceneLoad(sceneName, deleteSave));
    }

    private IEnumerator DelayedSceneLoad(string sceneName, bool deleteSave)
    {
        yield return new WaitForSeconds(sceneChangeDelay);

        if (deleteSave)
        {
            SaveManager.Instance.DeleteSave();
        }

        SceneLoader.LoadScene(sceneName);
    }

    private IEnumerator PlayClickAndQuit()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        yield return new WaitForSeconds(sceneChangeDelay);

        Application.Quit();
    }
}
