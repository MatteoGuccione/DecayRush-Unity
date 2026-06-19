using UnityEngine;

public class MoveBullet : MonoBehaviour
{
    private float speed = 1f;
    private int damage = 10;
    public void Configure(float speed, int damage)
    {
        this.speed = speed;
        this.damage = damage;
    }

    public void Update()
    {
        transform.Translate(Vector3.up * (Time.deltaTime * speed), Space.Self);
    }

    public int GetDamage()
    {
        return damage;
    }
    public void StartUpdating()
    {
    }
}
