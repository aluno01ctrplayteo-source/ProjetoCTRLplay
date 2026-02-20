using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private BulletPool bulletPool;

    public float time = 2f;
    public float lifeTime = 0f;
    private float timer = 0f;


    void Counter()
    {
        timer += Time.deltaTime;
        if(timer >= time)
        {
            gameObject.SetActive(false);
            timer = 0f;
        }
    }
    void Update()
    {
        Counter();
    }
    void OnDisable()
    {
        bulletPool.ReturnBullet(gameObject);
    }
}
