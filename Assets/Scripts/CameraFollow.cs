using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private PlayerManager target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 15f, -10f);
    [SerializeField] private Vector3 deathOffSet = new Vector3(0, -1.5f, -3f);
    [SerializeField] private float rotateX = 45f;
    [SerializeField] private float rotationSpeed = 20f;

    // Camera Shake
    [Header("Camera Shake")]
    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private float shakeMagnitude = 0.2f;
    private float shakeTimer = 0f;
    private Vector3 shakeOffset = Vector3.zero;

    private float currentYaw = 0f;

    void Start()
    {
        transform.rotation = Quaternion.Euler(rotateX, 0f, 0f);
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 basePosition;

        if (target.PlayerDeath)
        {
            currentYaw += rotationSpeed * Time.deltaTime;
            Quaternion rotation = Quaternion.Euler(0, currentYaw, 0);
            basePosition = target.transform.position + rotation * deathOffSet;
            transform.position = basePosition + shakeOffset;
            transform.LookAt(target.transform.position + Vector3.up * 1.5f);
        }
        else
        {
            basePosition = target.transform.position + offset;
            transform.position = basePosition + shakeOffset;
        }

        // Aggiorna shakeOffset se shake attivo
        if (shakeTimer > 0)
        {
            shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }
    }

    public void ShakeCamera()
    {
        shakeTimer = shakeDuration;
    }
}
