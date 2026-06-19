using UnityEngine;

public class DestroyBulletOnCollision : MonoBehaviour
{   
    private void OnCollisionEnter(Collision other)
    {
        if(this.GetComponent<BaseBullet>() is BaseBullet bullet && bullet)
            PoolManager.instance.ReturnToPool<BaseBullet>(PoolableType.Bullet, bullet);
        //Destroy(gameObject);
    }

}
