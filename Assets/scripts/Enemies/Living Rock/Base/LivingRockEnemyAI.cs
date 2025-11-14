using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LivingRockEnemyAI : MonoBehaviour, IDamageableEnemy, IMoveableEnemy
{
    Transform player;


    [Header("Info")]
    private string enemyName;
    private float detectionRange = 5f;
    private float followRange; // Not used currently
    int damage;
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; } = 100;
    public int MinHealth { get; set; } = 0;

    [Header("Movement")]
    float moveSpeed;
    Transform orientation;
    bool isGrounded;
    bool canMove = true;
    bool isAttacking = false;

    [Header("Gravity")]
    public float verticalVelocity;
    public float gravity = -9.81f;


    //test

    private event Action OnEnemyDeath;

    //.
    [Header("Components")]
    public Collider attackHitBox;
    public Enemies enemy;
    public NavMeshAgent agent;
    public Animator anim;   
    static GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        agent = agent == null ? GetComponent<NavMeshAgent>() : agent;
        player = GameObject.FindWithTag("Player").transform;
    }
    private void Start()
    {
        CurrentHealth = enemy.maxHealth;
        damage = enemy.damage;
        MinHealth = 0;
        MaxHealth = enemy.maxHealth;
        detectionRange = enemy.detectionRange;
        moveSpeed = enemy.speed;
        agent.speed = moveSpeed;
        enemyName = enemy.enemyName;
        anim.SetFloat("movement", 0);
    }

    private void OnEnable()
    {
        OnEnemyDeath += () => { gameManager.killCount++; Debug.Log("Killed an enemy"); };
    }

    public IEnumerator Attack()
    {
        isAttacking = true;
        canMove = false;



        yield return new WaitForSeconds(.5f);

        if (!IsAttackHitboxCollidingWith("Player")) { isAttacking = false; canMove = true; yield break; }
        anim.SetTrigger("isAttacking");
        //gameManager.healthManager.ChangeHpValue(-damage);

        yield return new WaitForSeconds(1.2f);
        anim.ResetTrigger("isAttacking");

        isAttacking = false;
        canMove = true;
        
    }
    public bool IsAttackHitboxCollidingWith(string tag)
    {
        
        
        
        
        Collider[] hitColliders = new Collider[10];
        int hitCount = Physics.OverlapBoxNonAlloc(transform.position + transform.forward * 1, new Vector3(.7f,.7f,1.5f), hitColliders, transform.rotation, LayerMask.GetMask("Default"));
        if (hitCount == 0) return false;
        
        for (int i = 0; i < hitCount; i++)
        {
            if (hitColliders[i] != null && hitColliders[i].gameObject.CompareTag(tag))
            {
                return true;
            }
        }
           
        return false;
    }
    public void TakeDamage(int change)
    {
        CurrentHealth += change;
        CurrentHealth = Mathf.Clamp(CurrentHealth, MinHealth, MaxHealth);
        Debug.Log($"Enemy Health: {CurrentHealth}");
        Death();
    }   
    public void Death()
    {
        if (CurrentHealth > MinHealth) return;
        OnEnemyDeath?.Invoke();
        Destroy(this.gameObject);
    }

    /*void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + transform.forward * 1, new Vector3(.7f, .7f, 1.5f));
    }*/

    public bool IsOnViewRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        return distanceToPlayer <= detectionRange;
    }

    public void FollowPlayer()
    {
        if (!IsOnViewRange() || !canMove) { agent.isStopped = true; anim.SetFloat("movement", 0); return; }
        agent.isStopped = false;
        agent.SetDestination(player.position);
        anim.SetFloat("movement", 1);
    }
    void Update()
    {
        //ApplyGravity();
        FollowPlayer();;
        if (IsAttackHitboxCollidingWith("Player") && !isAttacking) StartCoroutine(Attack());
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