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

            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            bullet.GetComponent<Rigidbody>().velocity = transform.forward * 20f;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }
}
