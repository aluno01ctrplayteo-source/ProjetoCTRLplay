
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class LivingRockEnemyAI : MonoBehaviour, IEnemy
{
    Transform player;

    [Header("Info")]
    private string enemyName;
    private float defaultDetectionRange = 5f;
    private float detectionRange;
    int damage;
    public GameObject dropPrefab;
    
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; } = 100;
    public int MinHealth { get; set; } = 0;

    [Header("Movement")]
    float moveSpeed;
    readonly Transform orientation;
    bool isGrounded; //unused
    bool canMove = true;
    bool isAttacking = false;
    public bool isDead = false;

    [Header("Gravity")]
    public float verticalVelocity;
    public float gravity = -9.81f;


    //test

    
    private event Action OnPlayerDetection;
    private event Action OnPlayerOutOfDetectionRange;

    //.

    [Header("Components")]
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
        gameManager.RaiseEnemyCreationEvent();
        CurrentHealth = enemy.maxHealth;

        damage = enemy.damage;
        MinHealth = 0;
        MaxHealth = enemy.maxHealth;
        defaultDetectionRange = enemy.detectionRange;
        detectionRange = defaultDetectionRange;
        moveSpeed = enemy.speed;
        agent.speed = moveSpeed;
        enemyName = enemy.enemyName;
        anim.SetFloat("movement", 0);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + transform.forward * 1, new Vector3(.7f, .7f, .7f));
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<HitBox>() != null)
        {
            HitBox hitBox = other.GetComponent<HitBox>();
            if (!(hitBox.hitEnemy)) return;
            TakeDamage(hitBox.damage);
            
        }
    }
    private void OnEnable()
    {
        gameManager.OnEnemyDeath += () => { gameManager.killCount++; Debug.Log("Killed an enemy"); };
        OnPlayerDetection += () => { detectionRange = defaultDetectionRange * 1.1f; };
        OnPlayerOutOfDetectionRange += () => { detectionRange = defaultDetectionRange; };
    }

    public IEnumerator Attack()
    {
        isAttacking = true;
        canMove = false;

        yield return new WaitForSeconds(1f);

        if (!IsAttackHitboxCollidingWith("Player", "Player")) { isAttacking = false; canMove = true; yield break; }
        anim.SetTrigger("isAttacking");
        AnimatorStateInfo currentAnim = anim.GetCurrentAnimatorStateInfo(0);
        while (!(currentAnim.IsName("RockRig_Attack")))
        {
            yield return null;
            currentAnim = anim.GetCurrentAnimatorStateInfo(0);
        }
        anim.ResetTrigger("isAttacking");
        yield return new WaitForSeconds(currentAnim.length);
        yield return new WaitForSeconds(.1f);
        

        isAttacking = false;
        canMove = true;

    }
    public bool IsAttackHitboxCollidingWith(string tag, string layer)
    {
        Collider[] hitColliders = new Collider[10];
        int hitCount = Physics.OverlapBoxNonAlloc(transform.position + transform.forward * 1, new Vector3(.7f,.7f,1f), hitColliders, transform.rotation, LayerMask.GetMask(layer));
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
        if (isDead) return;
        CurrentHealth += change;
        CurrentHealth = Mathf.Clamp(CurrentHealth, MinHealth, MaxHealth);
        //anim.SetTrigger("hasBeenDamaged"); // to implement
        Debug.Log($"Enemy Health: {CurrentHealth}");
        StartCoroutine(Death());
    }   
    public IEnumerator Death()
    {
        if (CurrentHealth > MinHealth) yield break;
        isDead = true;
        gameManager.RaiseEnemyDeathEvent();
        anim.SetTrigger("isDying");

        yield return new WaitForSeconds(3f);

        StartCoroutine(DeathDisappear());
        Instantiate(dropPrefab, transform.position, Quaternion.identity);

    }
    IEnumerator DeathDisappear()
    {
        while (true) 
        {
            if (transform.localScale.magnitude <= 0.05f) { Destroy(gameObject); }

            transform.localScale += Vector3.one * -.005f;
            yield return null;
            
        }
    }
    public bool IsOnViewRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        return distanceToPlayer <= detectionRange;
    }

    public void FollowPlayer()
    {
        
        if (!IsOnViewRange() || !canMove || isDead) { agent.isStopped = true; anim.SetFloat("movement", Mathf.Lerp(anim.GetFloat("movement"), 0, 10f * Time.deltaTime)); ; OnPlayerOutOfDetectionRange.Invoke() ; return; }
        OnPlayerDetection.Invoke();
        agent.isStopped = false;
        agent.SetDestination(player.position);
        anim.SetFloat("movement", Mathf.Lerp(anim.GetFloat("movement"), 1, 10f * Time.deltaTime));
    }
    void Update()
    {
        //ApplyGravity();
        if (isDead)
        {
            canMove = false;
            isAttacking = false;
        }
        FollowPlayer();
        if (IsAttackHitboxCollidingWith("Player", "Player") && !isAttacking) StartCoroutine(Attack());

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