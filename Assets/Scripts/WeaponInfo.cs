using System;
using UnityEngine;

[Serializable]
public class BulletEntry
{
    public int InitialPoolSize = 100;
    public BaseBullet Prefab;
    public PoolableType BulletType;
}

public class WeaponInfo : MonoBehaviour
{
    [SerializeField] private string weaponType;
    [SerializeField] private float timeToShoot;
    [SerializeField] private int weaponDamage = 10;
    [SerializeField] private int maxDamage = 20;
    [SerializeField] private float minTimeToShoot = 0.3f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private bool GOJWeapon;

    private float timeRemaning;
    private float GOJTimer = 10f;
    private float resetGOJTimer = 10f;
    private Vector3 bulletSpawnPositionFallBack = Vector3.up;

    // Properties per uso esterno
    public string Type => weaponType;
    public int CurrentDamage => weaponDamage;
    public int MaxDamage => maxDamage;
    public float CurrentFireRate => timeToShoot;
    public float MinTimeToShoot => minTimeToShoot;

    void Start()
    {
        timeRemaning = 0;

        if (gameObject.CompareTag("Weapon"))
        {
            PlayerManager.onGOJPickUp += FireRateZero;
            PlayerManager.finishGOJ += NormalFireRate;
        }
    }

    void Update()
    {   
        // Solo se la weapon � stata raccolta e il fire rate non � disabilitato
        if (transform.parent == null || timeRemaning == float.MaxValue || transform.parent.tag != "Player")
            return;

        timeRemaning += Time.deltaTime;
        if (timeRemaning >= timeToShoot)
        {
            timeRemaning = 0;

            switch (weaponType)
            {
                case "SG": // Shotgun
                    for (int i = -2; i <= 2; i++)
                    {
                        BaseBullet SGbullet = PoolManager.instance.Get<BaseBullet>(PoolableType.Bullet);
                        SGbullet.ResetBullet();
                        SGbullet.transform.position = transform.parent.position + this.bulletSpawnPositionFallBack;
                        SGbullet.transform.rotation = Quaternion.identity;
                        float spreadAngle = i * 5f;
                        Quaternion spreadRotation = Quaternion.Euler(0, spreadAngle, 0) * transform.parent.rotation;
                        //GameObject SGbullet = Instantiate(bulletPrefab, transform.parent.position, Quaternion.identity);
                        SGbullet.GetComponent<MoveBullet>().Configure(15, weaponDamage);
                        SGbullet.transform.up = spreadRotation * Vector3.forward;
                    }
                    break;

                case "AR":
                    FireSingleBullet(25);
                    break;

                case "Pistol":
                    FireSingleBullet(20);
                    break;

                case "MG":
                    FireSingleBullet(20);
                    break;
            }
        }

        // Gestione GOJWeapon autodistruzione
        if (!GOJWeapon) return;

        GOJTimer -= Time.deltaTime;
        if (GOJTimer <= 0)
        {
            GOJTimer = resetGOJTimer;
            Destroy(gameObject);
        }


    }

    // Metodo riutilizzabile per armi singolo proiettile
    private void FireSingleBullet(float speed)
    {
        Vector3 flatDirection = new Vector3(transform.parent.forward.x, 0f, transform.parent.forward.z).normalized;
        BaseBullet bullet = PoolManager.instance.Get<BaseBullet>(PoolableType.Bullet);
        bullet.ResetBullet();
        bullet.transform.position = transform.parent.position + this.bulletSpawnPositionFallBack;
        bullet.transform.rotation = Quaternion.identity;
        //GameObject bullet = Instantiate(bulletPrefab, transform.parent.position + new Vector3(0,1), Quaternion.identity);
        bullet.GetComponent<MoveBullet>().Configure(speed, weaponDamage);
        bullet.transform.up = flatDirection;
    }

    // Modificatori
    public void ModifyDamage(float multiplier)
    {
        weaponDamage = Mathf.CeilToInt(weaponDamage * multiplier);
        if (weaponDamage > maxDamage)
        {
            weaponDamage = maxDamage;
        }
    }

    public void ModifyFireRate(float multiplier)
    {
        timeToShoot *= multiplier;
        if (timeToShoot < minTimeToShoot)
        {
            timeToShoot = minTimeToShoot;
        }

        if (timeRemaning != float.MaxValue)
        {
            timeRemaning = Mathf.Min(timeRemaning, timeToShoot);
        }
    }

    // Eventi GOJ
    private void FireRateZero()
    {
        timeRemaning = float.MaxValue;
    }

    private void NormalFireRate()
    {
        timeRemaning = 0;
    }

    private void OnDestroy()
    {
        if (gameObject.CompareTag("Weapon"))
        {
            PlayerManager.onGOJPickUp -= FireRateZero;
            PlayerManager.finishGOJ -= NormalFireRate;
        }
    }
}
