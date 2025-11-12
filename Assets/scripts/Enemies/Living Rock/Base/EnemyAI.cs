using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamageable, IMoveable
{
    Transform player;


    [Header("Info")]
    readonly string enemyName;
    float detectionRange = 5f;
    int damage;
    public int currentHealth { get; set; }
    public int maxHealth { get; set; } = 100;
    public int minHealth { get; set; } = 0;

    [Header("Movement")]
    float moveSpeed;
    Transform orientation;
    bool isGrounded;
    bool canMove = false;

    [Header("Gravity")]
    public float verticalVelocity;
    public float gravity = -9.81f;


    [Header("Components")]
    public Enemies enemy;
    public NavMeshAgent agent;
    static Animator anim;
    static GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        agent = agent == null ? GetComponent<NavMeshAgent>() : agent;
        player = GameObject.FindWithTag("Player").transform;
    }
    private void Start()
    {
        currentHealth = enemy.maxHealth;
        damage = enemy.damage;
        minHealth = 0;
        maxHealth = enemy.maxHealth;
        detectionRange = enemy.detectionRange;
        moveSpeed = enemy.speed;
        agent.speed = moveSpeed;

    }
    public IEnumerator Attack()
    {
        canMove = false;
        gameManager.healthManager.HpChanger(-damage);
        yield return new WaitForSeconds(1f);
        canMove = true;

    }
    public bool CheckAttackHitBox(string tag)
    {
        if (Physics.BoxCast(transform.position + transform.forward * 1, Vector3.one, Vector3.zero, out RaycastHit hit, Quaternion.identity, 0f, 0, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject == this.gameObject) return false;
            if (hit.collider.CompareTag(tag)) return true;
        }
        return false;
    }

    public void TakeDamage(int change)
    {
        currentHealth += change;
        currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth);
        Debug.Log($"Enemy Health: {currentHealth}");
        Death();
    }   

    public void Death()
    {
        if (currentHealth > minHealth) return;
        Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + transform.forward * 1, Vector3.one);
    }


    public void Move()
    {
        agent.SetDestination(player.position);
    }
    void Update()
    {
        ApplyGravity();
        if (canMove && detectionRange < Vector3.Distance(transform.position, player.position))
        {
            Move();
        }
        else
        {
            agent.isStopped = true;
        }
        if (CheckAttackHitBox("Player"))
        {
            StartCoroutine(Attack());
        }
    }
    public void ApplyGravity() 
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