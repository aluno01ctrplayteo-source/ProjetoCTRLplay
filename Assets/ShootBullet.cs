using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBullet : MonoBehaviour
{
    public BulletPool bulletPool;
    private Controller controller;
    /*private void Start()
    {
        controller = new Controller();
    }
    void OnEnable()
    {
        controller.Enable();
        controller.Player.Attack.performed += ctx => Shoot();
    }*/
    void Shoot()
    {
        GameObject bullet = bulletPool.GetBullet();
        if(bullet != null)
        {
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            bullet.GetComponent<Rigidbody>().velocity = transform.forward * 20f;
            float t = 0f;
            while(t < 1f)
            {
                t += Time.deltaTime;
                if (t >= 1f)
                {
                    bulletPool.ReturnBullet(bullet);
                    break;
                }
            }
            
        }
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }
}
