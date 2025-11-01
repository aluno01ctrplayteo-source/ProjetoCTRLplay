using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public Transform orientation;
    public bool isGrounded;

    [Header("Jump & Gravity")]
    public float verticalVelocity;     
    public float gravity = -9.81f;

    [Header("Combat system")]
    public int damage = 15;
    public int health = 100;
    public int maxHealth = 100;
    public int minHealth = 0;

    [Header("Components")]
    public static GameManager gameManager;
    public static CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private void Start()
    {
        InvokeRepeating(nameof(DealDamage), 2f, 2f);
    }

    public void DealDamage()
    {
        if (CheckHitBox("Player")) gameManager.healthManager.HpChanger(-damage);
    }

    public bool CheckHitBox(string tag)
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position + transform.forward * 1 , Vector3.one * 1f, transform.localRotation);
        foreach (var collider in hitColliders)
        {
            if (collider.gameObject == this.gameObject) continue;
            if (collider.CompareTag(tag)) return true;
        }
        return false;
    }

    public void TakeDamage(int change)
    {
        health += change;
        health = Mathf.Clamp(health, minHealth, maxHealth);
        Debug.Log($"Enemy Health: {health}");
        Death();
    }   

    public void Death()
    {
        if (health > minHealth) return;
        Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + transform.forward * 1, Vector3.one * 1f);
    }

    private void Move()
    {
        Vector3 direction = orientation.forward;
        Transform playerPos = GameObject.FindWithTag("Player").transform;
        transform.LookAt(new Vector3(playerPos.position.x, transform.position.y, playerPos.position.z));
        controller.Move(direction * moveSpeed * Time.deltaTime + new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
    }
    void Update()
    {
        isGrounded = controller.isGrounded;
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
