using UnityEngine;

public class EnemyAudioManager : MonoBehaviour
{
    public static EnemyAudioManager Instance;

    [Header("Audio Source condiviso")]
    [SerializeField] private AudioSource audioSource;

    [Header("Suoni nemici")]
    [SerializeField] private AudioClip followSound;
    [SerializeField] private AudioClip attackSound;

    [Header("Suono box rotta")]
    [SerializeField] private AudioClip boxDestroySound;

    [Header("Delay per suono inseguimento")]
    [SerializeField] private float minDelay = 3f;
    [SerializeField] private float maxDelay = 6f;

    private float nextFollowSoundTime = 0f;
    private float nextAttackSoundTime = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TryPlayFollowSound()
    {
        if (Time.time >= nextFollowSoundTime && followSound != null)
        {
            audioSource.PlayOneShot(followSound);
            nextFollowSoundTime = Time.time + Random.Range(minDelay, maxDelay);
        }
    }

    public void TryPlayAttackSound()
    {
        if (Time.time >= nextAttackSoundTime && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
            nextAttackSoundTime = Time.time + 0.3f; 
        }
    }
    public void PlayBoxDestroySound()
    {
        if (boxDestroySound != null)
            audioSource.PlayOneShot(boxDestroySound);
    }
}
