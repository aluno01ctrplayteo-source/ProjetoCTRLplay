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
using System.ComponentModel;
using System.Diagnostics;

[RequireComponent(typeof(Rigidbody))]
[Icon("Assets/icons/player_icon.png")]
public class Player : DynamicEntity
{
    public override string EntityName { get =>"Player"; }
    private int _entityID;
    public override int EntityID { get { return _entityID; } }

    [Header("Movimentação")]
    public float moveSpeed = 5f; // Velocidade de movimento horizontal do jogador
    public Transform orientation; // Transform usado como refer�ncia de dire��o
    public Vector2 velocity; // Vetor 2D que armazena a entrada de movimento (x = lateral, y = frente/tr�s)
    public Vector3 desired;
    public Vector3 direction;
    public bool isSprinting;
    public float sprintMultiplier = 1.3f;
    public float staminaConsumerMultiplier = 20;
    public float staminaRecoveryMultiplier = 5;

    [SerializeField]
    private float maxStamina = 100;
    public float MaxStamina { get { return maxStamina; } set { maxStamina = value; UpdateUI(); } }
    [SerializeField]
    private float minStamina = 0;
    public float MinStamina { get { return minStamina; } set { minStamina = value; UpdateUI(); } }
    [SerializeField]
    private float stamina = 100f;
    public float Stamina { get { return stamina; } set { stamina = Mathf.Clamp(value, minStamina, maxStamina); UpdateUI(); } }

    [Header("Pulo")]
    public float jumpForce = 5f; // "Força" do pulo é aqui sendo usada como altura desejada na fórmula física
    public bool isGrounded; // Flag que indica se o jogador está no chão
    public bool airBorneTransition = false; // Flag que indica que o jogador esta indo pular

    [Header("Componentes")]
    public Animator playerAnimations; // Referência ao Animator
    public Controller ControllerInputs; // Instância da classe gerada pelo Input System
    public Rigidbody body;
    public CameraManager cameraManager;
    [SerializeField]private AnimationClip[] animationClips;
    private Dictionary<string, float> animationClipLenghts; 

    [Header("UI")]
    public Slider hpBar;
    public Slider staminaBar;
    public Slider basicAttackCooldownBar;


    [Header("Combate")]
    public float attackCooldown = 1f; // Tempo de recarga entre ataques
    public bool canAttack = true; // Flag para controlar se o jogador pode atacar
    public bool isAttacking = false;
    public bool attackSlowness = false;
    public event Action OnTakeDirectDamage;
    public event Action OnTakeInternalDamage;
    public event Action OnDeath;
    public bool isDead = false;
    public float damage;
    public bool tookDamage = false;
    public bool godMode = false;
    public bool waitForAttackEnd = false;
    public bool cancelAttackWhenDamaged = false;
    [SerializeField] 
    private int maxHealth = 100;
    public int MaxHealth { get { return maxHealth; } set { maxHealth = value; UpdateUI(); } }
    [SerializeField] 
    private int minHealth;
    public int MinHealth { get { return minHealth; } set { minHealth = value; UpdateUI(); } }
    [SerializeField] 
    private int currentHealth = 100;
    public int CurrentHealth { get { return currentHealth; } set { if (!godMode) { currentHealth = Mathf.Clamp(value, MinHealth, MaxHealth); UpdateUI(); } } }
    public Effects[] currentEffects = new Effects[Enum.GetValues(typeof(Effects)).Length];

    private void Awake()
    {
        animationClipLenghts = animationClips.ToDictionary((c) => c.name, (c) => c.length);
        body.constraints = RigidbodyConstraints.FreezeRotation;
        ControllerInputs = new Controller(); // Instancia o objeto de Input Actions
        CurrentHealth = 100;
        hpBar.value = CurrentHealth;
        _entityID = gameObject.GetInstanceID();
        UpdateUI();
    }




    private void Start()
    {
        //mesh drawing test
        GameObject mesh = new("ProceduralMesh", typeof(MeshFilter)/* data */, typeof(MeshRenderer));
        
        Mesh m = new();
        mesh.GetComponent<MeshFilter>().mesh = m;
        mesh.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));

        Vector3[] vertices = new Vector3[]
        {
                           // indexes of each vertex
            new(0,0,0),   // 0
            new(0,1,5),  // 1
            new(1,1,5), // 2
            new(1,0,5), // 3
            new(0,0,5)  // 4
        };

        int[] triangles = new int[]
        {
                    // I like to think these are indexes that form triangles using the vertices above
            0,1,2, // first triangle
            0,2,3, // second triangle
            0,4,1, // third triangle
            0,3,4, // fourth triangle
        };

        m.vertices = vertices;
        m.triangles = triangles;
        m.RecalculateNormals();
        StartCoroutine(CheckGrounded());
    }

    private void OnEnable()
    {
        ControllerInputs.Enable();
        ControllerInputs.Player.Jump.performed += OnJump;
        ControllerInputs.Player.Move.performed += OnMove;
        ControllerInputs.Player.Move.canceled += OnMoveCanceled;
        ControllerInputs.Player.Pause.performed += OnPause;
        ControllerInputs.Player.Attack.performed += OnAttack;
        ControllerInputs.Player.Sprint.performed += ctx => { StartCoroutine(Sprint()); };
        ControllerInputs.Player.Sprint.canceled += ctx => { isSprinting = false; StartCoroutine(RecoverStamina()); };
        OnDeath += () => { isDead = true; ControllerInputs.Disable(); body.freezeRotation = false; };

        OnHitBoxInteraction += (ctx) => { switch (ctx.type) { case HitBox.HitboxType.Damage: StartCoroutine(TakeDirectDamage(ctx)); break; } };


        OnTakeDirectDamage += () => { 
            if (isAttacking && cancelAttackWhenDamaged)
            {
                playerAnimations.ResetTrigger("isAttacking");
                playerAnimations.SetTrigger("damaged");
                StopCoroutine(Attack());

                attackSlowness = false;
                isAttacking = false;
                canAttack = true;
            }
            StartCoroutine(cameraManager.ShakeCamera(.4f, 0.1f, false));
            StartCoroutine(ApplyEfect(Effects.Slowness, .4f));
        };
        OnTakeInternalDamage += () => { };
    }

    private void OnAttack(InputAction.CallbackContext ctx) => StartCoroutine(Attack());
    private void OnMove(InputAction.CallbackContext ctx) => velocity = ctx.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext ctx) => velocity = Vector2.zero;
    private void OnPause(InputAction.CallbackContext ctx) => GameManager.instance.ToggleMenu(ctx);
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

    public IEnumerator Sprint()
    {
        if(Stamina == minStamina || velocity == Vector2.zero) yield break;
        isSprinting = true;
        moveSpeed *= sprintMultiplier;
        while (isSprinting && Stamina != minStamina && velocity != Vector2.zero)
        {
            Stamina -= Time.deltaTime * staminaConsumerMultiplier;
            yield return null;
        }
        moveSpeed /= sprintMultiplier;
    }

    public IEnumerator RecoverStamina()
    {
        for (float t = 0; t < 2f; t += Time.deltaTime)
        {
            if(isSprinting) yield break;
            yield return null;
        }
        while (!isSprinting && Stamina != maxStamina)
        {
            Stamina += Time.deltaTime * staminaRecoveryMultiplier;
            yield return null;
        }
    }


    public void Jump()
    {
        if (isGrounded && !attackSlowness) // S� permite pular se estiver no ch�o
        {
            StartCoroutine(AirBorneTransition(.2f));
            body.AddForce(Vector3.up * jumpForce + transform.forward * 3, ForceMode.Impulse);
        } 
    }

    public void MovementHandler()
    {
        if (isDead) return;
        Vector3 right = Vector3.ProjectOnPlane(orientation.right, Vector3.up);
        Vector3 forward = Vector3.ProjectOnPlane(orientation.forward, Vector3.up);


        direction = forward * velocity.y + right * velocity.x;
        direction.Normalize();

        if (attackSlowness) { StartCoroutine(ApplyEfect(Effects.Slowness, 0f, 1)); }

        if (velocity.magnitude > 0.1f) { playerAnimations.SetFloat("movementSpeed", Mathf.Lerp(playerAnimations.GetFloat("movementSpeed"), 1, 10 * Time.deltaTime)); }
        else if (velocity.magnitude < 0.1f) { playerAnimations.SetFloat("movementSpeed", Mathf.Lerp(playerAnimations.GetFloat("movementSpeed"), 0, 10 * Time.deltaTime)); }
        Vector3 current = body.velocity;
        desired = direction * moveSpeed - current;



        ForceMode force = isGrounded ? ForceMode.VelocityChange : ForceMode.Acceleration;

        body.AddForce(new Vector3(desired.x, 0f, desired.z), force);
    }
    public IEnumerator Attack()
    {
        
        if (!canAttack) yield break;

        attackSlowness = true;
        isAttacking = true;
        canAttack = false;
        
        playerAnimations.SetTrigger("isAttacking");

        StartCoroutine(CooldownUIHandler(basicAttackCooldownBar, animationClipLenghts["player_attack"] + attackCooldown));
        // Wait for animation to start
        yield return new WaitUntil(() =>
        playerAnimations.GetCurrentAnimatorStateInfo(0).IsTag("player_attack"));
        StartCoroutine(ApplyEfect(Effects.Slowness, .5f));
            
        // Wait for animation to play
        while (playerAnimations.GetCurrentAnimatorStateInfo(0).IsTag("player_attack"))
        {

            yield return null;
        }
        yield return new WaitForSeconds(attackCooldown);

        attackSlowness = false;
        isAttacking = false;
        
        canAttack = true;
    }
    public IEnumerator ApplyEfect(Effects effect, float time, int level)
    {
        if(currentEffects[(int)effect] != 0) yield break;
        switch (effect)
        {
            case Effects.Slowness:
                moveSpeed /= 2 * level;
                currentEffects[(int)Effects.Slowness] = effect;
                for (float t = 0; t < time; t += Time.deltaTime)
                {
                    yield return null;
                }
                currentEffects[(int)Effects.Slowness] = 0;
                moveSpeed *= 2 * level; 
                break;
            default: 
                break;
        }
        yield return null;
    }

    public IEnumerator ApplyEfect(Effects effect, float time)
    {
        if (currentEffects[(int)effect] != 0) yield break;
        switch (effect)
        {
            case Effects.Slowness:
                moveSpeed /= 2;
                currentEffects[(int)Effects.Slowness] = effect;
                for (float t = 0; t < time; t += Time.deltaTime)
                {
                    yield return null;
                }
                currentEffects[(int)Effects.Slowness] = 0;
                moveSpeed *= 2;
                break;
            default:
                break;
        }
        yield return null;
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

    private void FixedUpdate()
    {
        MovementHandler();
    }

    IEnumerator CheckGrounded() 
    {
        while (true)
        {
            if (airBorneTransition) { yield return null; continue; }
            Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 1.1f, LayerMask.GetMask("Scenario"), QueryTriggerInteraction.Ignore);
            if (hitInfo.collider != null) isGrounded = hitInfo.collider.CompareTag("Ground");
            yield return null;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.down * 1.1f);
    }
    public override IEnumerator TakeDirectDamage(HitBox hitbox)
    {
        if (hitbox.type != HitBox.HitboxType.Damage) yield break;
        if (tookDamage) yield break;
        
        
        tookDamage = true;
        OnTakeDirectDamage?.Invoke();
        hitbox.SetActive(false);
        CurrentHealth -= hitbox.value;
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
    protected override IEnumerator Death()
    {
        OnDeath?.Invoke();
        yield return null;
        StartCoroutine(GameManager.instance.GameOver());
    }

    public override void Heal(int amount)
    {
        CurrentHealth += amount;
    }

    public void UpdateUI()
    {
        hpBar.maxValue = MaxHealth;
        hpBar.minValue = MinHealth;
        hpBar.value = CurrentHealth;
        staminaBar.maxValue = maxStamina;
        staminaBar.minValue = minStamina;   
        staminaBar.value = Stamina;
    }

    public override IEnumerator TakeInternalDamage(int damage)
    {
        CurrentHealth -= damage;
        yield return null;
        if (CurrentHealth <= 0)
        {
            StartCoroutine(Death());
        }
    }
}
public enum Effects
{
    None,
    Slowness,
    Stun, // to implement 
    Poison // to implement
}