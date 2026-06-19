using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private string savePath;
    private HashSet<string> destroyedBoxes = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Application.persistentDataPath + "/save.json";
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterDestroyedBox(string boxName)
    {
        if (!destroyedBoxes.Contains(boxName))
            destroyedBoxes.Add(boxName);
    }

    public void SaveGame(PlayerManager player)
    {
        SaveData data = new SaveData();

        // Salva dati del Player
        data.PlayerPosition = player.transform.position;
        data.Health = player.Health;
        data.MaxHealth = player.MaxHealth;
        data.Level = player.Level;
        data.Experience = player.Experience;
        data.PlayTime = player.PlayTime;
        data.Inventory = new List<string>(player.Inventory);
        data.SceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;


        // Salva posizione nemici
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        data.Enemies = new List<EnemyData>();
        foreach (var enemy in enemies)
        {
            BaseEnemy baseEnemy = enemy.GetComponent<BaseEnemy>();
            EnemyData enemyData = new EnemyData
            {
                Position = enemy.transform.position,
                EnemyType = enemy.name.Replace("(Clone)", "")
            };
            data.Enemies.Add(enemyData);
        }

        // Salva armi nella mappa
        var weapons = GameObject.FindGameObjectsWithTag("Weapon");
        data.WeaponsInWorld = new List<WeaponData>();
        foreach (var weapon in weapons)
        {
            if (weapon.transform.parent == null) // Arma a terra
            {
                WeaponInfo info = weapon.GetComponent<WeaponInfo>();
                WeaponData weaponData = new WeaponData
                {
                    Position = weapon.transform.position,
                    WeaponType = info.Type
                };
                data.WeaponsInWorld.Add(weaponData);
            }
        }

        // Salva armi raccolte equipaggiate
        data.WeaponsCollected = new List<string>();
        foreach (Transform child in player.transform)
        {
            if (child.CompareTag("Weapon"))
            {
                WeaponInfo info = child.GetComponent<WeaponInfo>();
                if (info != null)
                {
                    data.WeaponsCollected.Add(info.Type);
                }
            }
        }

        InGameUIManager uiManager = GameObject.FindFirstObjectByType<InGameUIManager>();
        if (uiManager != null)
        {
            data.CountdownTimer = uiManager.GetCurrentTime();
        }

        // Salva casse aperte
        data.BoxesOpened = new List<string>(destroyedBoxes);

        // Salva enemy spawner manager
        //EnemySpawner enemySpawner = GameObject.FindFirstObjectByType<EnemySpawner>();
        //data.EnemySpawner = new EnemySpawnerData();
        //data.EnemySpawner.EnemySpawnMaxRadius = enemySpawner.EnemySpawnMaxRadius;
        //data.EnemySpawner.EnemySpawnMinRadius = enemySpawner.EnemySpawnMinRadius;
        //data.EnemySpawner.Interval = enemySpawner.Interval;
        //data.EnemySpawner.EnemyEntries = enemySpawner.EnemyEntries;
        //data.EnemySpawner.Position = enemySpawner.gameObject.transform.position;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved!");
    }

    public void LoadGame(PlayerManager player)
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Save file not found!");
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // Ripristina lo stato delle casse distrutte
        destroyedBoxes = new HashSet<string>(data.BoxesOpened);

        // Carica player
        player.transform.position = data.PlayerPosition;
        player.Health = data.Health;
        player.MaxHealth = data.MaxHealth;
        player.Level = data.Level;
        player.Experience = data.Experience;
        player.PlayTime = data.PlayTime;
        player.Inventory.Clear();
        foreach (var item in data.Inventory)
        {
            player.Inventory.Add(item);
        }

        // Cancella nemici esistenti
        foreach (var e in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(e);
        }

        // Ricrea i nemici
        foreach (var enemyData in data.Enemies)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefab/Enemy/" + enemyData.EnemyType);
            GameObject newEnemy = Instantiate(prefab, enemyData.Position, Quaternion.identity);
        }

        // Cancella le armi a terra
        foreach (var w in GameObject.FindGameObjectsWithTag("Weapon"))
        {
            if (w.transform.parent == null)
                Destroy(w);
        }

        // Ricrea le armi nella mappa
        foreach (var weaponData in data.WeaponsInWorld)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefab/Weapons/" + weaponData.WeaponType);
            Instantiate(prefab, weaponData.Position, Quaternion.identity);
        }

        // Ricrea le armi raccolte come figlie del player
        foreach (string weaponType in data.WeaponsCollected)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefab/Weapons/" + weaponType);
            if (prefab != null)
            {
                GameObject weapon = Instantiate(prefab, player.transform);
                weapon.transform.localPosition = Vector3.zero;
                weapon.SetActive(true);

                // Disattiva il MeshRenderer per nascondere l�arma equipaggiata
                MeshRenderer renderer = weapon.GetComponent<MeshRenderer>();
                if (renderer != null)
                    renderer.enabled = false;
            }
        }

        // Distruggi le casse aperte
        foreach (var box in GameObject.FindGameObjectsWithTag("Box"))
        {
            if (data.BoxesOpened.Contains(box.name))
            {
                Destroy(box);
            }
        }
        // Ricrea enemy spawner manager
        GameObject enemySpawnerGO = new GameObject();
        enemySpawnerGO.transform.position = data.EnemySpawner.Position;
        EnemySpawner enemySpawner = enemySpawnerGO.AddComponent<EnemySpawner>();
        enemySpawner.EnemyEntries = data.EnemySpawner.EnemyEntries;
        enemySpawner.Player = player;
        enemySpawner.EnemySpawnMaxRadius = data.EnemySpawner.EnemySpawnMaxRadius;
        enemySpawner.EnemySpawnMinRadius = data.EnemySpawner.EnemySpawnMinRadius;
        enemySpawner.Interval = data.EnemySpawner.Interval;

        InGameUIManager uiManager = GameObject.FindFirstObjectByType<InGameUIManager>();
        if (uiManager != null)
        {
            uiManager.SetCurrentTime(data.CountdownTimer);
        }

        Debug.Log("Game loaded!");
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Salvataggio eliminato.");
        }
    }
    public string GetSavedSceneName()
    {
        if (!File.Exists(savePath)) return null;

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        return data.SceneName;
    }
}
