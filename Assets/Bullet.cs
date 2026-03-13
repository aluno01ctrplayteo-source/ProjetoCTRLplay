using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletPool bulletPool;

    public float lifeTime = 1f;
    private float timer = 0f;

    private void Awake()
    {
        bulletPool = BulletPool.instance;
    }

    void Counter()
    {
        timer += Time.deltaTime;
        if(timer >= lifeTime)
        {
            bulletPool.ReturnBullet(gameObject);
            timer = 0f;
        }
    }
    void Update()
    {
        Counter();
    }
}
