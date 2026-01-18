
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
        public GameObject enemyModel;
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
        public MeleeEnemiesStats info;
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
            _damage = info.damage;
            MinHealth = info.minHealth;
            MaxHealth = info.maxHealth;
            CurrentHealth = MaxHealth;
            _attackRange = info.attackTriggerRangeMultiplier;
            _defaultDetectionRange = info.detectionRange;
            _detectionRange = _defaultDetectionRange;
            moveSpeed = info.speed;
            agent.speed = moveSpeed;
            _enemyName = info.enemyName;
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

        public virtual IEnumerator ProcDamageAnim()
        { // mainly used to create a visual clue of damage without an actual anim
            //default : shake
            Vector3 startPos = enemyModel.transform.localPosition;
            for (float t = 0f; t < .2f; t += Time.deltaTime)
            {
                yield return new WaitUntil(() => { return !(gameManager.isPaused); });
                float percentComplete = t / .2f;
                float damp = 1 - percentComplete;
                Vector3 pos = startPos;
                pos += UnityEngine.Random.insideUnitSphere * .4f * damp;
                enemyModel.transform.localPosition = pos;
                yield return null;
            }
            enemyModel.transform.localPosition = startPos;
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

        public virtual IEnumerator DeathState()
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
        public GameObject enemyModel;
        public float projectileLifeTime = 20f;
        public List<GameObject> projectileAmount;
        protected Coroutine currentStateRoutine;
        protected EnemyState _enemyState = EnemyState.Idle;
        public EnemyState State { get{ return _enemyState; } set { if (_enemyState == value) return ; _enemyState = value; OnStateUpdate?.Invoke(); } }
        public int MaxHealth { get { return stats.maxHealth; } }
        public int MinHealth { get { return stats.minHealth; } }
        protected int _currentHealth = 100;
        public int CurrentHealth { get { return _currentHealth; } set { _currentHealth = Mathf.Clamp(value, MinHealth, MaxHealth); } }

        protected event Action OnTakeDamage;
        protected event Action OnStateUpdate;
        protected event Func<Transform> OnProjectileShot;
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
        protected void TakeDamageEvent() => OnTakeDamage?.Invoke();
        protected Transform ProjectileShotEvent() => OnProjectileShot?.Invoke();

        public virtual IEnumerator TakeHitboxDamage(HitBox hitbox)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerator ProcDamageAnim()
        { // mainly used to create a visual clue of damage without an actual anim
            //default : shake
            Vector3 startPos = enemyModel.transform.localPosition;
            for (float t = 0f; t < .2f; t += Time.deltaTime)
            {
                yield return new WaitUntil(() => { return !(gameManager.isPaused); });
                float percentComplete = t / .2f;
                float damp = 1 - percentComplete;
                Vector3 pos = startPos;
                pos += UnityEngine.Random.insideUnitSphere * .4f * damp;
                enemyModel.transform.localPosition = pos;
                yield return null;
            }
            enemyModel.transform.localPosition = startPos;
        }
        protected virtual IEnumerator ProjectileBehaviour(GameObject pr, HitBox prhb)
        {
            float lifeTime = projectileLifeTime;
            while (pr != null && lifeTime > 0)
            {
                pr.transform.Translate(pr.transform.forward * 5 * Time.deltaTime, Space.World);
                if ( Physics.CheckBox(pr.transform.position, prhb.trigger.bounds.extents, pr.transform.rotation, LayerMask.GetMask("Scenario"), QueryTriggerInteraction.Ignore))
                {
                    StartCoroutine(prhb.DestroyH());
                    yield break;
                }
                lifeTime -= Time.deltaTime;
                yield return null;
            }

            StartCoroutine(prhb.DestroyH());
        }

        public virtual void ChangeState(EnemyState newState)
        {
            if (currentStateRoutine != null) StopCoroutine(currentStateRoutine);

            State = newState;

            switch (State)
            {
                case EnemyState.Idle:
                    break;
                case EnemyState.Attacking:
                    break;
                case EnemyState.Dead:
                    StartCoroutine(DeathState());
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

        public virtual IEnumerator DeathState()
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