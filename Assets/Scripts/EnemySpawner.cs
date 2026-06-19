using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class EnemyEntry
{
    public int InitialPoolSize = 100;
    public BaseEnemy Prefab;
    public PoolableType EnemyType;
}
public class EnemySpawner : SpawnerManager<BaseEnemy>
{
    [SerializeField] private List<EnemyEntry> enemyEntries; // Lista di tipi di nemici
    [SerializeField] private PlayerManager player;
    [SerializeField] private float enemySpawnMinRadius = 5f;
    [SerializeField] private float enemySpawnMaxRadius = 10f;
    [SerializeField] private float interval = 3f;
    public PlayerManager Player
    {
        get
        {
            // If there is no reference it searches for an Object tagged as 'Player'
            if (this.player == null)
            {
                GameObject gameObject = GameObject.FindWithTag("Player");
                this.player = gameObject.GetComponent<PlayerManager>();
            }
            return this.player;
        }
        set => this.player = value;
    }
    public List<EnemyEntry> EnemyEntries { get => enemyEntries; set => enemyEntries = value; }
    public float EnemySpawnMinRadius { get => enemySpawnMinRadius; set => enemySpawnMinRadius = value; }
    public float EnemySpawnMaxRadius { get => enemySpawnMaxRadius; set => enemySpawnMaxRadius = value; }
    public float Interval { get => interval; set => interval = value; }

    void Start()
    {
        Initialize();
    }
    public override void Initialize()
    {
        SetupEnemies();
        foreach (EnemyEntry enemyEntry in enemyEntries) 
            PoolManager.instance.CreatePool(enemyEntry.EnemyType, enemyEntry.Prefab, enemyEntry.InitialPoolSize, this.transform);
        base.minRadius = this.enemySpawnMinRadius;
        base.maxRadius = this.enemySpawnMaxRadius;
        base.spawnInterval = this.interval;
        base.prefabsToPool = this.enemyEntries.Select(entry => entry.EnemyType).Distinct().ToList();
        base.reference = this.Player.gameObject; // The property, NOT the value

        if (base.spawnCoroutine == null)
            StartSpawning(); // Start spawning
    }
    public void MoveToScene(Scene scene, Vector3 position)
    {
        SceneManager.MoveGameObjectToScene(this.gameObject, scene);
        this.gameObject.transform.position = position;
    }
    private void SetupEnemies()
    {
        foreach (BaseEnemy enemy in this.enemyEntries.Select(entry => entry.Prefab))
        {
            if (enemy != null && enemy.Player == null)
                enemy.Player = this.Player;
        }
    }
}