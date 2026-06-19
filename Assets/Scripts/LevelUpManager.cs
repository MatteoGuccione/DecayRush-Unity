using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LevelUpManager : MonoBehaviour
{
    private int selectedIndex = 0;
    private Button[] buttons;
    private float inputCooldown = 0.2f;
    private float lastInputTime = 0f;

    [SerializeField] private GameObject panel;
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;

    public static bool IsLevelUpActive { get; private set; }

    private PlayerManager currentPlayer;

    private List<string> availableUpgrades = new List<string>()
    {
        "HP Up",
        "Speed Up",
        "Attack Up",
        "Fire Rate Up",
        "AR Attack Up",
        "AR FireRate Up",
        "SG Attack Up",
        "SG FireRate Up",
        "Pistol Attack Up",
        "Pistol FireRate Up",
    };

    public void ShowPanel(PlayerManager player)
    {
        currentPlayer = player;
        panel.SetActive(true);
        Time.timeScale = 0f;
        IsLevelUpActive = true;

        List<string> options = GetUniqueRandomOptions(3);

        SetupButton(button1, options[0]);
        SetupButton(button2, options[1]);
        SetupButton(button3, options[2]);

        buttons = new Button[] { button1, button2, button3 };
        selectedIndex = 0;
        buttons[selectedIndex].Select(); // evidenzia il primo
    }
    void Update()
    {
        if (!panel.activeSelf) return;

        float verticalInput = Input.GetAxisRaw("Vertical");
        if (Time.unscaledTime - lastInputTime > inputCooldown)
        {
            if (verticalInput > 0.5f)
            {
                selectedIndex = (selectedIndex - 1 + buttons.Length) % buttons.Length;
                buttons[selectedIndex].Select();
                lastInputTime = Time.unscaledTime;
            }
            else if (verticalInput < -0.5f)
            {
                selectedIndex = (selectedIndex + 1) % buttons.Length;
                buttons[selectedIndex].Select();
                lastInputTime = Time.unscaledTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0)) //Press A for XBOX or press X for PS
        {
            buttons[selectedIndex].onClick.Invoke();
        }
    }


    private void SetupButton(Button button, string option)
    {
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            buttonText.text = option;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnUpgradeSelected(option));
        button.gameObject.SetActive(true);
    }

    private void OnUpgradeSelected(string upgrade)
    {
        switch (upgrade)
        {
            case "HP Up":
                IncreaseMaxHealth();
                break;
            case "Speed Up":
                IncreaseSpeed();
                break;
            case "Attack Up":
                IncreaseAttack();
                break;
            case "Fire Rate Up":
                IncreaseWeaponFireRate();
                break;
            case "AR Attack Up":
                IncreaseWeaponDamage("AR");
                break;
            case "AR FireRate Up":
                IncreaseWeaponFireRate("AR");
                break;
            case "SG Attack Up":
                IncreaseWeaponDamage("SG");
                break;
            case "SG FireRate Up":
                IncreaseWeaponFireRate("SG");
                break;
            case "Pistol Attack Up":
                IncreaseWeaponDamage("Pistol");
                break;
            case "Pistol FireRate Up":
                IncreaseWeaponFireRate("Pistol");
                break;
        }
        ClosePanel();
    }

    private void ClosePanel()
    {
        panel.SetActive(false);
        currentPlayer = null;
        Time.timeScale = 1f;
        IsLevelUpActive = false;
    }

    private List<string> GetUniqueRandomOptions(int count)
    {
        List<string> copy = new List<string>(availableUpgrades);
        List<string> chosen = new List<string>();

        for (int i = 0; i < count && copy.Count > 0; i++)
        {
            int index = Random.Range(0, copy.Count);
            chosen.Add(copy[index]);
            copy.RemoveAt(index);
        }
        return chosen;

    }

    // Increases base attack power
    private void IncreaseAttack()
    {
        WeaponInfo[] weapons = currentPlayer.GetComponentsInChildren<WeaponInfo>();
        foreach (WeaponInfo weapon in weapons)
        {
            weapon.ModifyDamage(1.1f);
        }
        Debug.Log("Attack increased to " + currentPlayer.Attack + " and weapon damage updated.");
    }
    // Increases player max HP and heals 
    private void IncreaseMaxHealth()
    {
        // Aumenta MaxHealth del 20%
        currentPlayer.MaxHealth = (int)(currentPlayer.MaxHealth * 1.2f);

        // Cura del 10% della nuova MaxHealth
        currentPlayer.Health += (int)(currentPlayer.MaxHealth * 0.1f);

        // Assicura che la salute attuale non superi la nuova MaxHealth
        if (currentPlayer.Health > currentPlayer.MaxHealth)
        {
            currentPlayer.Health = currentPlayer.MaxHealth;
        }

        Debug.Log("Max Health increased to " + currentPlayer.MaxHealth + " and healed to " + currentPlayer.Health);
    }


    // Increases generic speed stat
    private void IncreaseSpeed()
    {
        currentPlayer.MoveSpeed *= 1.3f; // Aumento del 30%
        Debug.Log("Speed increased to " + currentPlayer.MoveSpeed);
    }
    // Reduces weapon cooldowns (fire rate boost)
    private void IncreaseWeaponFireRate()
    {
        WeaponInfo[] weapons = currentPlayer.GetComponentsInChildren<WeaponInfo>();
        foreach (WeaponInfo weapon in weapons)
        {
            weapon.ModifyFireRate(0.9f); // 10% faster
        }
        Debug.Log("Weapon fire rate improved.");
    }
    private void IncreaseWeaponDamage(string weaponType)
    {
        WeaponInfo[] weapons = currentPlayer.GetComponentsInChildren<WeaponInfo>();
        foreach (WeaponInfo weapon in weapons)
        {
            if (weapon.Type == weaponType)
            {
                weapon.ModifyDamage(1.1f); // +10% danno
            }
        }
        Debug.Log($"Weapon damage increased by 10% for weapon type {weaponType}.");
    }

    private void IncreaseWeaponFireRate(string weaponType)
    {
        WeaponInfo[] weapons = currentPlayer.GetComponentsInChildren<WeaponInfo>();
        foreach (WeaponInfo weapon in weapons)
        {
            if (weapon.Type == weaponType)
            {
                weapon.ModifyFireRate(0.9f); // +10% velocità di fuoco (moltiplicatore <1)
            }
        }
        Debug.Log($"Weapon fire rate improved by 10% for weapon type {weaponType}.");
    }

}