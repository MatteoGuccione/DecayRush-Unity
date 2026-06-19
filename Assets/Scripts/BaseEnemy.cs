using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour, IPoolable
{
    [SerializeField] private int hp;
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    [SerializeField] private float atkSpeed;
    [SerializeField] private int attackRange;
    [SerializeField] private float experienceDrop = 20f;
    [SerializeField] private float takePlayerTimer = 0.5f;

    [SerializeField] public PlayerManager Player;
    [SerializeField] private GameObject expOrbPrefab;
    [SerializeField] private List<EnemyDestroyOnHit> destroyOnHit;

    [SerializeField] private Animator animator;

    private float atkSpeedTimer;
    private float currentAnimation;
    private float resetTakePlayerTimer;

    private int resetHp;

    private float nextFollowSoundTime = 0f;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private CapsuleCollider capsuleCollide;
    [SerializeField] private SphereCollider sphereCollider;

    private int animationSpeed = Animator.StringToHash("Speed");

    public NavMeshAgent Agent { get => agent; set => agent = value; }

    private void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        capsuleCollide = gameObject.GetComponent<CapsuleCollider>();
        sphereCollider = gameObject.GetComponent<SphereCollider>();

        sphereCollider.isTrigger = true;
        sphereCollider.radius = attackRange;
        sphereCollider.center = new Vector3(0, 1.92f, 0.52f);

        agent.speed = speed;
        agent.stoppingDistance = attackRange;

        takePlayerTimer = resetTakePlayerTimer;
        currentAnimation = 1;
        animator = GetComponent<Animator>();

        gameObject.tag = "Enemy";

        resetHp = hp;

        if (Player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                Player = playerObj.GetComponent<PlayerManager>();
        }
    }

    void Update()
    {
        agent.destination = Player.gameObject.transform.position;
        animator.SetFloat(animationSpeed, currentAnimation);

        if (!agent.isStopped && Time.time >= nextFollowSoundTime)
        {
            EnemyAudioManager.Instance?.TryPlayFollowSound();
            nextFollowSoundTime = Time.time + Random.Range(3f, 6f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            transform.rotation = Quaternion.LookRotation(Player.transform.position - transform.position, Vector3.up);
            agent.isStopped = true;
            atkSpeedTimer -= Time.deltaTime;

            if (atkSpeedTimer <= 0)
            {
                currentAnimation = 2;
                takePlayerTimer -= Time.deltaTime;

                if (takePlayerTimer <= 0)
                {
                    Debug.Log("Attack");
                    Player.TakeDamage(damage);

                    // 🔊 Suono di attacco
                    EnemyAudioManager.Instance?.TryPlayAttackSound();

                    atkSpeedTimer = atkSpeed;
                    takePlayerTimer = resetTakePlayerTimer;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            currentAnimation = 1;
            agent.isStopped = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            OnTakeDamage(Player.Attack);
        }
        if (other.gameObject.CompareTag("Explosion"))
        {
            OnTakeDamage(9999);
        }
    }

    private void Die()
    {
        PoolManager.instance.ReturnToPool<BaseEnemy>(PoolableType.Enemy, this);
    }

    public void New()
    {
        // opzionale
    }

    public void Free()
    {
        if (expOrbPrefab != null)
        {
            GameObject orb = Instantiate(expOrbPrefab, transform.position, Quaternion.identity);
            ExpOrb expComponent = orb.GetComponent<ExpOrb>();
            if (expComponent != null)
            {
                expComponent.SetExperience(experienceDrop);
            }
        }

        currentAnimation = 3;
        agent.enabled = false;
        sphereCollider.enabled = false;
        capsuleCollide.enabled = false;

        hp = resetHp;
    }

    public void DoubleStats()
    {
        hp *= 2;
        damage *= 2;
        speed *= 2f;
        atkSpeed /= 2f;

        if (agent != null)
        {
            agent.speed = speed;
        }
    }

    private void OnTakeDamage(int dmg)
    {
        hp -= dmg;

        if (hp <= 0)
        {
            Die();
        }
    }
}
