
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;

namespace StandardEnemyAIBehaviour
{
    public class StandardMeleeEnemy : DynamicEntity, IEnemyMarker
    {
        internal Transform player;

        [Header("Info")]
        public bool randomizeStatsOnStart = false;
        private string _enemyName;
        public override string EntityName { get { return _enemyName; } protected set { _enemyName = value; } }
        protected float _attackRange;
        protected float _defaultDetectionRange;
        protected float _detectionRange;
        private int _entityID;
        public override int EntityID { get { return _entityID; } }
        protected int _damage;
        public GameObject enemyModel;
        public GameObject dropPrefab;

        private int _maxHealth;
        public int MaxHealth { get { return _maxHealth; } protected set { _maxHealth = value; } }
        private int _minHealth;
        public int MinHealth { get { return _minHealth; } protected set { _minHealth = value; } }
        private int _currentHealth;
        public int CurrentHealth { get { return _currentHealth; } protected set { _currentHealth = Mathf.Clamp(value, MinHealth, MaxHealth); } }

        [Header("Movement")]
        protected float moveSpeed;
        public bool canMove = true;
        public bool isAttacking = false;
        public bool isDead = false;



        protected event Action OnPlayerDetection;
        protected event Action OnPlayerOutOfDetectionRange;


        protected void PlayerDetectionEvent() => OnPlayerDetection?.Invoke();
        protected void PlayerOutOfDetectionRangeEvent() => OnPlayerOutOfDetectionRange?.Invoke();





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
            _entityID = gameObject.GetInstanceID();
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + Vector3.up * 5, 0.2f);
        }


        public override IEnumerator TakeDirectDamage(HitBox hitbox)
        {
            throw new NotImplementedException(); // to implement basic logic
        }

        protected virtual IEnumerator ProcDamageAnim()
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

        protected virtual IEnumerator Attack()
        {
            throw new NotImplementedException(); // to implement basic logic
        }

        public override IEnumerator TakeInternalDamage(int damage)
        {
            throw new NotImplementedException(); // to implement basic logic
        }

        public override void Heal(int amount)
        {
            throw new NotImplementedException(); // to implement basic logic
        }

        protected override IEnumerator Death()
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
    public class StandardRangedEnemy : DynamicEntity, IEnemyMarker //Incomplete for now
    {
        internal Transform player;

        [Header("Info")]
        public GameObject dropPrefab;   
        public Transform projectileOrigin;
        public GameObject projectile;
        public GameObject enemyModel;
        protected int _damage;
        public float projectileLifeTime = 20f;
        public List<GameObject> projectileAmount;
        private string _enemyName;
        public override string EntityName { get { return _enemyName; } protected set { _enemyName = value; } }
        private int _entityID;
        public override int EntityID { get { return _entityID; } }
        protected Coroutine currentStateRoutine;
        protected EnemyState _enemyState = EnemyState.Idle;
        public EnemyState State { get{ return _enemyState; } set { if (_enemyState == value) return ; _enemyState = value; OnStateUpdate?.Invoke(); } }
        private int _maxHealth;
        public int MaxHealth { get { return _maxHealth; } protected set { _maxHealth = value; } }
        private int _minHealth;
        public int MinHealth { get { return _minHealth; } protected set { _minHealth = value; } }
        private int _currentHealth = 100;
        public int CurrentHealth { get { return _currentHealth; } protected set { _currentHealth = Mathf.Clamp(value, MinHealth, MaxHealth); } }

        protected event Action OnStateUpdate;
        protected event Func<Transform> OnProjectileShot;
        [Header("Components")]
        public Animator anim;
        public RangedEnemiesStats stats;
        internal GameManager gameManager;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + Vector3.up * 5, 0.2f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.GetComponent<HitBox>() != null)
            {
                HitBox hitBox = other.GetComponent<HitBox>();
                StartCoroutine(TakeDirectDamage(hitBox));
            }
        }

        private void Awake()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            player = GameObject.FindWithTag("Player").transform;
            _enemyName = stats.enemyName;
            _damage = stats.damage;
            _minHealth = stats.minHealth;
            _maxHealth = stats.maxHealth;
            _entityID = gameObject.GetInstanceID();
        }
        protected Transform ProjectileShotEvent() => OnProjectileShot?.Invoke();

        public override IEnumerator TakeDirectDamage(HitBox hitbox)
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
                if ( Physics.CheckBox(pr.transform.position, prhb.size / 2, pr.transform.rotation, LayerMask.GetMask("Scenario"), QueryTriggerInteraction.Ignore))
                {
                    prhb.DestroyH();
                    yield break;
                }
                lifeTime -= Time.deltaTime;
                yield return null;
            }

            prhb.DestroyH();
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
                    StartCoroutine(Death());
                    break;
            }
        }
        public override IEnumerator TakeInternalDamage(int damage)
        {
            throw new NotImplementedException();
        }

        public override void Heal(int amount)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerator Death()
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
