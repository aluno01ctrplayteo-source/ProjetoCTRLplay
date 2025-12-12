
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace StandardEnemyAIBehaviour
{
    public class StandardMeleeEnemy : MonoBehaviour , IDamageable, IEnemyMarker
    {
        internal Transform player;

        [Header("Info")]
        public bool randomizeStatsOnStart = false;
        protected string _enemyName;
        protected float _attackRange;
        protected float _defaultDetectionRange;
        protected float _detectionRange;
        protected int _damage;
        public GameObject dropPrefab;
        public int _maxHealth;
        
        public int MaxHealth { get { return _maxHealth; } set { _maxHealth = value; } }
        protected int _minHealth;
        public int MinHealth { get { return _minHealth; } set { _minHealth = value; } }
        public int _currentHealth;
        public int CurrentHealth { get { return _currentHealth; } set { _currentHealth = Mathf.Clamp(value, MinHealth, MaxHealth); } }

        [Header("Movement")]
        protected float moveSpeed;
        public bool canMove = true;
        public bool isAttacking = false;
        public bool isDead = false;

        
        
        protected event Action OnTakeDamage;
        protected event Action OnPlayerDetection;
        protected event Action OnPlayerOutOfDetectionRange;


        protected void PlayerDetectionEvent() => OnPlayerDetection?.Invoke();
        protected void PlayerOutOfDetectionRangeEvent() => OnPlayerOutOfDetectionRange?.Invoke();

        protected void TakeDamageEvent() => OnTakeDamage?.Invoke();





        [Header("Components")]
        public MeleeEnemiesStats stats;
        public NavMeshAgent agent;
        public Animator anim;
        internal GameManager gameManager;

        private void Awake()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            agent = agent == null ? GetComponent<NavMeshAgent>() : agent;
            player = GameObject.FindWithTag("Player").transform;
        }

        private void Start()
        {
            gameManager.RaiseEnemyCreationEvent();
            _damage = stats.damage;
            MinHealth = stats.minHealth;
            MaxHealth = stats.maxHealth;
            CurrentHealth = MaxHealth;
            _attackRange = stats.attackRange;
            _defaultDetectionRange = stats.detectionRange;
            _detectionRange = _defaultDetectionRange;
            moveSpeed = stats.speed;
            agent.speed = moveSpeed;
            _enemyName = stats.enemyName;
            if (randomizeStatsOnStart)
            {
                System.Random rnd = new();
                int healthVariance = rnd.Next(-10, 11);
                int damageVariance = rnd.Next(-10, 3);
                MaxHealth += healthVariance;
                _damage += damageVariance;
            }
            anim.SetFloat("movement", 0);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.GetComponent<HitBox>() != null)
            {
                HitBox hitBox = other.GetComponent<HitBox>();
                StartCoroutine(TakeHitboxDamage(hitBox));
            }
        }


        public virtual IEnumerator TakeHitboxDamage(HitBox hitbox)
        {
            throw new NotImplementedException(); // to implement basic logic
        }

        public virtual IEnumerator Attack()
        {
            throw new NotImplementedException(); // to implement basic logic
        }

        public virtual IEnumerator TakeDirectDamage(int damage)
        {
            throw new NotImplementedException(); // to implement basic logic
        }

        public virtual void Heal(int amount)
        {
            throw new NotImplementedException(); // to implement basic logic
        }

        public virtual IEnumerator Death()
        {
            yield return null; // to implement basic logic
        }
        protected enum EnemyState //Unused for now
        {
            Idle,
            Chasing,
            Attacking,
            Dead
        }
    }
    public class StandardRangedEnemy : MonoBehaviour, IDamageable, IEnemyMarker //Incomplete for now
    {
        internal Transform player;

        [Header("Info")]
        public GameObject dropPrefab;
        public Transform projectileOrigin;
        public GameObject projectile;

        public List<GameObject> projectileAmount;
        protected EnemyState _enemyState = EnemyState.Idle;
        protected EnemyState State { get{ return _enemyState; } set { _enemyState = value; OnStateUpdate?.Invoke(); } }
        public bool isDead = false;
        public bool isAttacking = false;
        public bool isTargetOnViewRange = false;
        public int MaxHealth { get { return stats.maxHealth; } }
        public int MinHealth { get { return stats.minHealth; } }
        protected int _currentHealth = 100;
        public int CurrentHealth { get { return _currentHealth; } set { _currentHealth = Mathf.Clamp(value, MinHealth, MaxHealth); } }

        protected event Action OnTakeDamage;
        protected event Action OnPlayerDetection;
        protected event Action OnPlayerOutOfDetectionRange;
        protected event Action OnStateUpdate;



        [Header("Components")]
        public Animator anim;
        public RangedEnemiesStats stats;
        internal GameManager gameManager;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.GetComponent<HitBox>() != null)
            {
                HitBox hitBox = other.GetComponent<HitBox>();
                StartCoroutine(TakeHitboxDamage(hitBox));
            }
        }

        private void Awake()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            player = GameObject.FindWithTag("Player").transform;

        }

        private void OnEnable()
        {
            OnPlayerDetection += () => { isTargetOnViewRange = true; };
            OnPlayerOutOfDetectionRange += () => { isTargetOnViewRange = false; };
        }



        protected void PlayerDetectionEvent() => OnPlayerDetection?.Invoke();
        protected void PlayerOutOfDetectionRangeEvent() => OnPlayerOutOfDetectionRange?.Invoke();
        protected void TakeDamageEvent() => OnTakeDamage?.Invoke();

        public virtual IEnumerator TakeHitboxDamage(HitBox hitbox)
        {
            throw new NotImplementedException();
        }

        protected virtual IEnumerator ProjectileBehaviour(GameObject pr)
        {
            while (pr != null)
            {
                pr.transform.Translate(pr.transform.forward * 5 * Time.deltaTime, Space.World);
                yield return null;
            }   
        }

        public virtual void UpdateState()
        {
            switch (State)
            {
                case EnemyState.Idle:
                    break;
                case EnemyState.Attacking:
                    break;
                case EnemyState.Dead:
                    break;
            }
        }
        public virtual IEnumerator TakeDirectDamage(int damage)
        {
            throw new NotImplementedException();
        }

        public virtual void Heal(int amount)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerator Death()
        {
            yield return null;
        }
        public enum EnemyState
        {
            Idle,
            Attacking,
            Dead,
        }
    }
}