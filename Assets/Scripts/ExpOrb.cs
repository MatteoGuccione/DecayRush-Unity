using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    [SerializeField] private float experienceAmount = 10f;
    [SerializeField] private float attractionRange = 3f;
    [SerializeField] private float attractionSpeed = 5f;

    private Transform playerTransform;

    public float ExperienceAmount => experienceAmount;

    public void SetExperience(float amount)
    {
        experienceAmount = amount;
    }

    void Update()
    {
        if (playerTransform == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance <= attractionRange)
            {
                // Move the orb towards the player
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    playerTransform.position,
                    attractionSpeed * Time.deltaTime
                );
            }
        }
    }
}
