using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;
using System.Linq;
[RequireComponent(typeof(Rigidbody))]
[Icon("Assets/icons/player_icon.png")]
public class Player : MonoBehaviour, IDamageable
{
    public static Player instance;

    [Header("Movimentação")]
    public float moveSpeed = 5f; // Velocidade de movimento horizontal do jogador
    public Transform orientation; // Transform usado como refer�ncia de dire��o
    public Vector2 velocity; // Vetor 2D que armazena a entrada de movimento (x = lateral, y = frente/tr�s)
    public Vector3 desired;
    public Vector3 direction;


    [Header("Pulo")]
    public float jumpForce = 5f; // "Força" do pulo é aqui sendo usada como altura desejada na fórmula física
    public bool isGrounded; // Flag que indica se o jogador está no chão
    public bool airBorneTransition = false;

    [Header("Componentes")]
    public Animator playerAnimations; // Referência ao Animator
    public Controller ControllerInputs; // Instância da classe gerada pelo Input System
    public Rigidbody body;
    public CameraManager cameraManager;
    [SerializeField]private AnimationClip[] animationClips;
    private Dictionary<string, float> animationClipLenghts; 

    [Header("UI")]
    public Slider hpBar;
    public Slider basicAttackCooldownBar;


    [Header("Combate")]
    public float attackCooldown = 1f; // Tempo de recarga entre ataques
    public bool canAttack = true; // Flag para controlar se o jogador pode atacar
    public bool isAttacking = false;
    public bool slowdown = false;
    public event Action OnTakeDamage;
    public event Action OnDeath;
    public bool isDead = false;
    private int _maxHealth = 100;
    public bool tookDamage = false;
    public bool cancelAttack = false;
    public bool godMode = false;
    public bool waitForAttackEnd = false;
    public bool cancelAttackWhenDamaged = false;
    public int MaxHealth { get { return _maxHealth; } set { _maxHealth = value; } }
    private int _minHealth;
    public int MinHealth { get { return _minHealth; } set { _minHealth = value; } }
    private int _currentHealth = 100;
    public int CurrentHealth { get { return _currentHealth; } set { if (!godMode) { _currentHealth = Mathf.Clamp(value, MinHealth, MaxHealth); UpdateUI(); } } }

    private void Awake()
    {
        animationClipLenghts = animationClips.ToDictionary((c) => c.name, (c) => c.length);
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
        body.constraints = RigidbodyConstraints.FreezeRotation;
        ControllerInputs = new Controller(); // Instancia o objeto de Input Actions
        CurrentHealth = 100;
        hpBar.value = CurrentHealth;
        UpdateUI();
    }




    private void Start()
    {
        StartCoroutine(CheckGrounded());
    }

    private void OnEnable()
    {
        ControllerInputs.Enable();
        ControllerInputs.Player.Jump.performed += OnJump;
        ControllerInputs.Player.Move.performed += OnMove;
        ControllerInputs.Player.Move.canceled += OnMoveCanceled;
        ControllerInputs.Player.Pause.performed += OnPause;
        ControllerInputs.Player.Inventory.performed += OnInventory;
        ControllerInputs.Player.Attack.performed += OnAttack;
        OnDeath += () => { isDead = true; ControllerInputs.Disable(); body.freezeRotation = false; };

        OnTakeDamage += () => { if (isAttacking) cancelAttack = true; };
    }

    private void OnAttack(InputAction.CallbackContext ctx) => StartCoroutine(Attack());
    private void OnMove(InputAction.CallbackContext ctx) => velocity = ctx.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext ctx) => velocity = Vector2.zero;
    private void OnPause(InputAction.CallbackContext ctx) => GameManager.instance.ToggleMenu();
    private void OnInventory(InputAction.CallbackContext ctx) => GameManager.instance.ToggleInventory();
    private void OnJump(InputAction.CallbackContext ctx) => Jump();



    private IEnumerator AirBorneTransition(float timeToTransitate = .1f)
    {
        if(airBorneTransition) yield break;
        airBorneTransition = true;
        isGrounded = false;
        yield return new WaitForSeconds(timeToTransitate);
        airBorneTransition = false;
    }

    private void OnDisable()
    {
        ControllerInputs.Disable(); // Desativa o mapa de acoes (interrompe leitura de inputs)

    }




    public void Jump()
    {
        if (isGrounded && !slowdown) // S� permite pular se estiver no ch�o
        {
            StartCoroutine(AirBorneTransition(.2f));
            body.AddForce(Vector3.up * jumpForce + transform.forward * 3, ForceMode.Impulse);
        } 
    }

    public void Move()
    {
        if (isDead) return;
        Vector3 right = Vector3.ProjectOnPlane(orientation.right, Vector3.up);
        Vector3 forward = Vector3.ProjectOnPlane(orientation.forward, Vector3.up);


        direction = forward * velocity.y + right * velocity.x;
        direction.Normalize();

        if (slowdown) { direction /= 2; }

        if (velocity.magnitude > 0.1f) { playerAnimations.SetFloat("movementSpeed", Mathf.Lerp(playerAnimations.GetFloat("movementSpeed"), 1, 10 * Time.deltaTime)); }
        else if (velocity.magnitude < 0.1f) { playerAnimations.SetFloat("movementSpeed", Mathf.Lerp(playerAnimations.GetFloat("movementSpeed"), 0, 10 * Time.deltaTime)); }
        Vector3 current = body.velocity;
        desired = direction * moveSpeed - current;

        ForceMode forceMode = isGrounded ? ForceMode.VelocityChange : ForceMode.Impulse;

        body.AddForce(new Vector3(desired.x, 0f, desired.z), forceMode);
    }
    public IEnumerator Attack()
    {
        if (!canAttack) yield break;

        slowdown = true;
        isAttacking = true;
        canAttack = false;
        
        playerAnimations.SetTrigger("isAttacking");
        StartCoroutine(CooldownUIHandler(basicAttackCooldownBar, animationClipLenghts["player_attack"] + attackCooldown));
        // Wait for animation to start
        yield return new WaitUntil(() =>
            playerAnimations.GetCurrentAnimatorStateInfo(0).IsTag("player_attack"));

        // Wait for animation to play
        while (playerAnimations.GetCurrentAnimatorStateInfo(0).IsTag("player_attack"))
        {
            if (cancelAttackWhenDamaged && cancelAttack)
            {
                cancelAttack = false;
                playerAnimations.ResetTrigger("isAttacking");
                playerAnimations.SetTrigger("damaged");

                slowdown = false;
                isAttacking = false;
                canAttack = true;
                yield break;
            }

            yield return null;
        }
        yield return StartCoroutine(CooldownHandler(attackCooldown));

        slowdown = false;
        isAttacking = false;
        
        canAttack = true;
    }

    private IEnumerator CooldownUIHandler(Slider ui, float time)
    {
        ui.value = 0f;
        ui.maxValue = time;
        for (float t = 0; t <= time;)
        {
            ui.value = t;
            t += Time.deltaTime;
            yield return null;
        }
        ui.value = ui.maxValue;
    }
    private IEnumerator CooldownHandler(float time)
    {
        for (float t = 0; t < time;)
        {
            t += Time.deltaTime;
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;
        if (other.transform.GetComponent<HitBox>() != null)
        {
            HitBox hb = other.transform.GetComponent<HitBox>();
            hb.ToggleTrigger();
            StartCoroutine(hb.Destroy());
            StartCoroutine(TakeHitboxDamage(hb));
            
        }
        if (other.CompareTag("Healer"))
        {
            Heal(25);
            Destroy(other.gameObject);
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    IEnumerator CheckGrounded() 
    {
        while (true)
        {
            if (airBorneTransition) { yield return null; continue; }
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, LayerMask.GetMask("Scenario"), QueryTriggerInteraction.Ignore);
            yield return null;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.down * 1.1f);
    }
    public IEnumerator TakeHitboxDamage(HitBox hitbox)
    {
        if (hitbox.type != HitboxType.Damage) yield break;
        if (tookDamage) yield break;
        
        
        tookDamage = true;
        OnTakeDamage?.Invoke();
        hitbox.ToggleTrigger();
        CurrentHealth -= hitbox.value;
        StartCoroutine(cameraManager.ShakeCamera(.4f, 0.1f, false));
        if (hitbox.impactForce != 0)
        {
            StartCoroutine(AirBorneTransition(.3f));
            Vector3 v = body.velocity;
            v.y = 0;
            body.velocity = v;
            body.AddForce(((Vector3.up / 4) + hitbox.owner.transform.forward) * hitbox.impactForce, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(0.2f);
        tookDamage = false;
        if (CurrentHealth > 0.1f) yield break;
        StartCoroutine(Death());
    }
    public IEnumerator Death()
    {
        OnDeath?.Invoke();
        yield return null;
        StartCoroutine(GameManager.instance.GameOver());
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
    }

    public void UpdateUI()
    {
        hpBar.value = CurrentHealth;
    }

    public IEnumerator TakeDirectDamage(int damage)
    {
        CurrentHealth -= damage;
        yield return null;
        if (CurrentHealth <= 0)
        {
            StartCoroutine(Death());
        }
    }
}
