using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class InGameUIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI WeaponStatsText;
    [SerializeField] private TextMeshProUGUI CountdownTimer;
    [SerializeField] private Image ExpBarFill;
    [SerializeField] private PlayerManager player;

    [Header("Menu")]
    [SerializeField] private GameObject EscapeMenu;
    [SerializeField] private Button FirstEscapeButton; // Per il supporto gamepad

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip menuOpenClip;
    [SerializeField] private AudioClip menuCloseClip;

    [SerializeField]private float currentTime = 300f; // esempio: 5 minuti
    private float smoothFill = 0f;
    private bool isMenuOpen = false;
    private bool statsDoubled = false;

    private void Start()
    {
        Time.timeScale = 1f;
        EscapeMenu.SetActive(false);

        if (player == null)
            player = GameObject.FindFirstObjectByType<PlayerManager>();
    }

    private void Update()
    {
        HandleTimer();
        HandleEscapeMenu();
        UpdateExpBar();
    }

    private void HandleTimer()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            currentTime = Mathf.Clamp(currentTime, 0, Mathf.Infinity);

            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            CountdownTimer.text = $"{minutes:00}:{seconds:00}";

            if (currentTime <= 0 && !statsDoubled)
            {
                DoubleEnemyStats();
                statsDoubled = true;
            }
        }
    }

    private void DoubleEnemyStats()
    {
        BaseEnemy[] allEnemies = Object.FindObjectsByType<BaseEnemy>(FindObjectsSortMode.None);
        foreach (BaseEnemy enemy in allEnemies)
        {
            enemy.DoubleStats();
        }

        Debug.Log("Tutte le statistiche dei nemici sono state raddoppiate!");
    }

    private void HandleEscapeMenu()
    {
        if (LevelUpManager.IsLevelUpActive) return; // blocca se level up aperto

        // Tasto Esc (keyboard) o Start (joystick)
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            ToggleMenu();
        }

        // Tasto B (joystick) per chiudere se il menu � aperto
        if (isMenuOpen && Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        isMenuOpen = !EscapeMenu.activeSelf;
        EscapeMenu.SetActive(isMenuOpen);
        Time.timeScale = isMenuOpen ? 0f : 1f;

        // Suono apertura/chiusura menu
        if (audioSource != null)
        {
            AudioClip clipToPlay = isMenuOpen ? menuOpenClip : menuCloseClip;
            if (clipToPlay != null)
            {
                audioSource.PlayOneShot(clipToPlay);
            }
        }

        // Se menu aperto, seleziona il primo bottone (per il gamepad)
        if (isMenuOpen && FirstEscapeButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(FirstEscapeButton.gameObject);
        }
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

    public void SetCurrentTime(float value)
    {
        currentTime = value;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneLoader.LoadScene("MainMenu");
    }

    public void SaveGame()
    {
        SaveManager.Instance.SaveGame(GameObject.FindFirstObjectByType<PlayerManager>());
    }
    private void UpdateExpBar()
    {
        if (player == null || ExpBarFill == null) return;

        float currentExp = player.Experience;
        float expToLevel = 100f + (player.Level - 1) * 25f;
        float targetFill = Mathf.Clamp01(currentExp / expToLevel);

        // Interpolazione fluida
        smoothFill = Mathf.Lerp(smoothFill, targetFill, Time.deltaTime * 5f);
        ExpBarFill.fillAmount = smoothFill;
    }
    public void ResetExpBar()
    {
        smoothFill = 0f;
    }
}
