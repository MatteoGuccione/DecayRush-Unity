using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    [SerializeField] private Vector3 playerPosition;
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private int level;
    [SerializeField] private float experience;
    [SerializeField] private List<string> inventory;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float playTime;

    [SerializeField] private List<EnemyData> enemies;
    [SerializeField] private List<WeaponData> weaponsInWorld;
    [SerializeField] private List<string> weaponsCollected;
    [SerializeField] private List<string> boxesOpened;
    [SerializeField] private float countdownTimer;
    [SerializeField] private string sceneName;

    [SerializeField] private EnemySpawnerData enemySpawner;

    // Per accedere a questi dall'esterno (es. SaveManager), crea delle property:
    public Vector3 PlayerPosition { get => playerPosition; set => playerPosition = value; }
    public int Health { get => health; set => health = value; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public int Level { get => level; set => level = value; }
    public float Experience { get => experience; set => experience = value; }
    public List<string> Inventory { get => inventory; set => inventory = value; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

    public List<EnemyData> Enemies { get => enemies; set => enemies = value; }
    public List<WeaponData> WeaponsInWorld { get => weaponsInWorld; set => weaponsInWorld = value; }
    public float PlayTime { get => playTime; set => playTime = value; }
    public List<string> WeaponsCollected { get => weaponsCollected; set => weaponsCollected = value; }
    public List<string> BoxesOpened { get => boxesOpened; set => boxesOpened = value; }
    public float CountdownTimer { get => countdownTimer; set => countdownTimer = value; }
    public string SceneName { get => sceneName; set => sceneName = value; }
    public EnemySpawnerData EnemySpawner { get => enemySpawner; set => enemySpawner = value; }
}


[System.Serializable]
public class EnemyData
{
    [SerializeField] private Vector3 position;
    [SerializeField] private string enemyType;

    public Vector3 Position { get => position; set => position = value; }
    public string EnemyType { get => enemyType; set => enemyType = value; }
}

[System.Serializable]
public class WeaponData
{
    [SerializeField] private Vector3 position;
    [SerializeField] private string weaponType;

    public Vector3 Position { get => position; set => position = value; }
    public string WeaponType { get => weaponType; set => weaponType = value; }
}

[System.Serializable]
public class EnemySpawnerData
{
    [SerializeField] private Vector3 position;
    [SerializeField] private List<EnemyEntry> enemyEntries; // Lista di tipi di nemici
    [SerializeField] private PlayerManager player;
    [SerializeField] private float enemySpawnMinRadius;
    [SerializeField] private float enemySpawnMaxRadius;
    [SerializeField] private float interval;

    public Vector3 Position { get => position; set => position = value; }
    public PlayerManager Player { get => player; set => player = value; }
    public List<EnemyEntry> EnemyEntries { get => enemyEntries; set => enemyEntries = value; }
    public float EnemySpawnMinRadius { get => enemySpawnMinRadius; set => enemySpawnMinRadius = value; }
    public float EnemySpawnMaxRadius { get => enemySpawnMaxRadius; set => enemySpawnMaxRadius = value; }
    public float Interval { get => interval; set => interval = value; }
}
    
