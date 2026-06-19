using UnityEngine;
using UnityEngine.UI;

public class DamageFlashUI : MonoBehaviour
{
    [SerializeField] private Image flashImage;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private float fadeSpeed = 5f;

    private Color flashColor = new Color(1f, 0f, 0f, 0.5f); // rosso semi-trasparente
    private bool isFlashing;

    void Update()
    {
        if (isFlashing)
        {
            flashImage.color = Color.Lerp(flashImage.color, flashColor, Time.deltaTime * fadeSpeed);
        }
        else
        {
            flashImage.color = Color.Lerp(flashImage.color, Color.clear, Time.deltaTime * fadeSpeed);
        }
    }

    public void TriggerFlash()
    {
        isFlashing = true;
        Invoke(nameof(StopFlash), flashDuration);
    }

    private void StopFlash()
    {
        isFlashing = false;
    }
}
