using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float verticalVelocity; 
    public bool isGrounded;
    CharacterController controller;
    public Transform orientation;
    public float moveSpeed = 3f;
    float gravity = -9.81f;
    int damage = 15;
    GameManager gameManager;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private void Start()
    {
        //InvokeRepeating("DealDamage", 2f, 2f);
    }

    /*public void DealDamage(int amount)
    {
        RaycastHit hit;
        if (Physics.CheckBox(transform.position + Vector3.forward * 3, new Vector3(0.5f, 1f, 0.5f), Quaternion.identity, LayerMask.GetMask("Player")))
        {
            gameManager.healthManager.HpChanger(-damage);
        }
    }*/

    private void Move()
    {
        Vector3 direction = orientation.forward;
        Transform playerPos = GameObject.FindWithTag("Player").transform;
        transform.LookAt(new Vector3(playerPos.position.x, transform.position.y, playerPos.position.z));
        controller.Move(direction * moveSpeed * Time.deltaTime + new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
    }
    void Update()
    {
        ApplyGravity();
        Move();
    }
    void ApplyGravity() 
    {
        if (isGrounded && verticalVelocity < 0) 
        {
            verticalVelocity = -2f; 
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; 
        }
        verticalVelocity = Mathf.Clamp(verticalVelocity, -50f, 50f);
    }

}
