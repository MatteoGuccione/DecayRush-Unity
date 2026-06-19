using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    [SerializeField] private Image heartFill;
    private float currentFill = 1f;
    private Coroutine currentCoroutine;

    public void UpdateHeart(float currentHealth, float maxHealth)
    {
        float targetFill = Mathf.Clamp01((float)currentHealth / maxHealth);

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(SmoothFill(targetFill));
    }
    void OnEnable()
    {
        PlayerManager.HealthChanged += UpdateHeart;
    }

    void OnDisable()
    {
        PlayerManager.HealthChanged -= UpdateHeart;
    }

    private IEnumerator SmoothFill(float targetFill)
    {
        float duration = 0.4f; // Durata della transizione
        float elapsed = 0f;
        float startFill = heartFill.fillAmount;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            heartFill.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / duration);
            yield return null;
        }

        heartFill.fillAmount = targetFill;
    }
}
