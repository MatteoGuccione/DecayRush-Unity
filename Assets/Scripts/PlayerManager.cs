using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public delegate void OnGOJPickUp();
    public static OnGOJPickUp onGOJPickUp;

    public delegate void OnHealthChanged(float currentHealth, float maxHealth);
    public static event OnHealthChanged HealthChanged;

    public delegate void FinishGOJ();
    public static FinishGOJ finishGOJ;
    private CameraFollow cameraFollow;
    private float playTime;

    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int level = 1;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float experience = 0f;
    [SerializeField] private int attack = 10;
    [SerializeField] private List<string> inventory = new List<string>();
    [SerializeField] private LevelUpManager levelUpManager;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private DamageFlashUI damageFlash;

    private static readonly int IsDeath = Animator.StringToHash("IsDeath?");
    private bool isCoreMecanic = false;
    private bool playerDeath;

    public List<string> Inventory { get { return inventory; } }
    public int Health { get => health; set => health = Mathf.Clamp(value, 0, maxHealth); }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public int Level { get => level; set => level = Mathf.Max(1, value); }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = Mathf.Max(0f, value); }
    public float Experience { get => experience; set => experience = Mathf.Max(0f, value); }
    public int Attack { get => attack; set => attack = Mathf.Max(0, value); }
    public bool PlayerDeath => playerDeath;
    public float PlayTime { get => playTime; set => playTime = value; }

    void Start()
    {
        cameraFollow = Camera.main.GetComponent<CameraFollow>();

        Debug.Log("Player ready with HP: " + Health);
        HealthChanged?.Invoke(health, maxHealth);
        playerDeath = false;
        // Notifica al LoadManager che il player � pronto
        if (LoadManager.Instance != null)
        {
            LoadManager.Instance.NotifyPlayerReady();
        }
        
    }
    void Update()
    {
        playTime += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveManager.Instance.SaveGame(this);
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            SaveManager.Instance.LoadGame(this);
        }
    }


    // Sets the reference to the LevelUpManager
    public void SetLevelUpManager(LevelUpManager manager)
    {
        levelUpManager = manager;
    }

    // Adds experience and handles level up logic
    public void AddExperience(float amount)
    {
        experience += amount;
        float expToLevel = 100f + (level - 1) * 25f;

        if (experience >= expToLevel)
        {
            experience -= expToLevel;
            level++;
            HandleLevelUp();
        }
    }

    // Shows the level up panel
    private void HandleLevelUp()
    {
        if (levelUpManager != null)
            levelUpManager.ShowPanel(this);
        GameObject.FindFirstObjectByType<InGameUIManager>()?.ResetExpBar();
    }

    // Applies damage to the player
    public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        damageFlash.TriggerFlash();

        if (cameraFollow != null)
        {
            cameraFollow.ShakeCamera();
        }

        HealthChanged?.Invoke(health, maxHealth);

        if (health <= 0)
        {
            Die();
        }
    }

    // Equips a new weapon and unequips the current one
    public void EquipWeapon(string weaponType)
    {
        UnequipWeapon();

        Transform weapon = transform.Find(weaponType);
        if (weapon != null)
        {
            weapon.gameObject.SetActive(true);
            Debug.Log("Equipped weapon: " + weaponType);
        }
        else
        {
            Debug.LogWarning("Weapon not found: " + weaponType);
        }
    }

    // Deactivates all child weapons
    public void UnequipWeapon()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Weapon"))
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    // Handles player death
    private void Die()
    {
        Debug.Log("Player has died.");
        animator.SetBool(IsDeath, true);
        playerDeath = true;
        playerMovement.enabled = false;
        onGOJPickUp();

        Invoke(nameof(GoToMainMenu), 3);
        // Add respawn or game over logic here if needed
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Handles item and experience pickup on trigger enter
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickUp"))
        {
            inventory.Add(other.gameObject.name);
            Debug.Log("Picked up: " + other.gameObject.name);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Weapon") && (isCoreMecanic == false))
        {
            other.transform.parent = transform;
            other.GetComponent<SphereCollider>().enabled = false;
            other.GetComponent<Renderer>().enabled = false;
            string weaponName = other.gameObject.name;
            inventory.Add(weaponName); 
            Debug.Log("Weapon picked up: " + weaponName);

        }
        else if (other.CompareTag("Exp"))
        {
            ExpOrb orb = other.GetComponent<ExpOrb>();
            if (orb != null)
            {
                AddExperience(orb.ExperienceAmount);
                Destroy(other.gameObject);
                Debug.Log("Experience gained: " + orb.ExperienceAmount);
            }
        }
        else if (other.CompareTag("GOJ"))
        {
            onGOJPickUp();
            isCoreMecanic = true;
            Invoke("SetCoreMecanic", 10);
            other.transform.parent = transform;
            other.GetComponent<SphereCollider>().enabled = false;
            other.GetComponent<Renderer>().enabled = false;
            Debug.Log("GOJ: ");
        }
    }
    private bool SetCoreMecanic()
    {
        isCoreMecanic = !isCoreMecanic;
        finishGOJ();
        return isCoreMecanic;
    }
}
