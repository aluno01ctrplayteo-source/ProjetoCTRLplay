using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class FpsControll : MonoBehaviour
{
    [Header("Movimenta��o")] 
    public float moveSpeed = 5f; // Velocidade de movimento horizontal do jogador
    public Transform orientation; // Transform usado como refer�ncia de dire��o
    public Vector2 velocity; // Vetor 2D que armazena a entrada de movimento (x = lateral, y = frente/tr�s)

    [Header("Pulo & Gravidade")] 
    public float gravity = -9.81f; // Acelera��o da gravidade (valor negativo porque aponta para baixo)
    public float jumpForce = 5f; // "For�a" do pulo � aqui est� sendo usada como altura desejada na f�rmula f�sica
    public float verticalVelocity; // Velocidade vertical atual (eixo Y) do personagem
    public bool isGrounded; // Flag que indica se o jogador est� no ch�o

    [Header("Componentes")] 
    public CharacterController charController;
    public Animator playerAnimations; // Refer�ncia ao Animator
    public Controller ControllerInputs; // Inst�ncia da classe gerada pelo Input System

    [Header("Combate")]
    public int damageAmount; // Dano causado por ataque
    public float attackCooldown = 1f; // Tempo de recarga entre ataques
    public bool canAttack = true; // Flag para controlar se o jogador pode atacar
    public bool attackSlowdown = false;

    private void Awake() 
    {
        ControllerInputs = new Controller(); // Instancia o objeto de Input Actions
    } 

    private void OnEnable() 
    {
        ControllerInputs.Enable(); // Ativa o mapa de a��es para come�ar a receber entradas
        ControllerInputs.Player.Jump.performed += ctx => Jump(); // Ao disparar o evento 'Jump.performed', chama o m�todo Jump()
        ControllerInputs.Player.Move.performed += ctx => velocity = ctx.ReadValue<Vector2>(); // Ao movimentar, l� o Vector2 (input) e armazena em 'velocity'
        ControllerInputs.Player.Move.canceled += ctx => velocity = Vector2.zero; // Quando o input de movimento � cancelado (soltou a tecla/joystick), zera a velocidade
        ControllerInputs.Player.Pause.performed += ctx => GameManager.instance.GamePaused();
        ControllerInputs.Player.Inventory.performed += ctx => GameManager.instance.InventoryOpen();
        ControllerInputs.Player.Attack.performed += ctx => StartCoroutine(Attack());


    }

    private void OnDisable()
    {
        ControllerInputs.Disable(); // Desativa o mapa de a��es (interrompe leitura de inputs)

    } 
    



    void Jump()
    {
        if (isGrounded && !attackSlowdown) // S� permite pular se estiver no ch�o
        {
            // Calcula a velocidade inicial vertical necess�ria para atingir a "altura" indicada por jumpForce
            // F�rmula f�sica: v = sqrt(2 * g_abs * height) ; como gravity � negativo usamos -2f * gravity
            verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity); // atribui velocidade inicial do pulo
        } 
    }

    void Move()
    {

        Vector3 direction = orientation.forward * velocity.y + orientation.right * velocity.x;
        direction.Normalize();

        if (attackSlowdown) { direction /= 2; }

        if (direction.magnitude > 0.1f) { playerAnimations.SetFloat("movementSpeed", Mathf.Lerp(playerAnimations.GetFloat("movementSpeed"), 1, 10 * Time.deltaTime)); }
        else if (direction.magnitude < 0.1f) { playerAnimations.SetFloat("movementSpeed", Mathf.Lerp(playerAnimations.GetFloat("movementSpeed"), 0, 10 * Time.deltaTime)); }
        Vector3 move = direction * moveSpeed + Vector3.up * verticalVelocity;

        

        charController.Move(move * Time.deltaTime); // Move o CharacterController (leva em conta colis�es); multiplicado por deltaTime para ser frame-rate independent
    }
    public IEnumerator Attack()
    {
        if (!canAttack) yield break;
        attackSlowdown = true;
        canAttack = false;
        
        yield return null;
        playerAnimations.SetTrigger("isAttacking");
        AnimatorStateInfo currentAnim = playerAnimations.GetCurrentAnimatorStateInfo(0);
        while (!(currentAnim.IsName("player_attack")))
        {
            yield return null;
            currentAnim = playerAnimations.GetCurrentAnimatorStateInfo(0);
        }
        
        yield return new WaitForSeconds(currentAnim.length - 0.2f);
        attackSlowdown = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;

        //Usar Raycast para detectar inimigos na frente do jogador
        /*Collider[] hitCollider = Physics.OverlapBox(transform.position + transform.forward * 2, Vector3.one); // Cria uma caixa de colis�o na frente do jogador
        Debug.Log(hitCollider.Length); // Loga quantos colliders foram atingidos (para debug)
        List<IDamageableEnemy> damagedEnemies = new(); // Conjunto para rastrear inimigos j� danificados nesta chamada de ataque
        foreach (var collider in hitCollider) // Itera sobre todos os colliders encontrados na caixa
        {
            if (collider.gameObject == this.gameObject) continue; // Ignora o pr�prio jogador
            IDamageableEnemy enemy = collider.GetComponent<IDamageableEnemy>();
            if (enemy != null && collider.gameObject.CompareTag("Enemy") && !damagedEnemies.Contains(enemy))
            {
                enemy.TakeDamage(-damageAmount); // Aplica dano ao inimigo
                damagedEnemies.Add(enemy);
            }
        }*/
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<HitBox>() != null)
        {
           HitBox hitBox = other.transform.GetComponent<HitBox>();
           GameManager.instance.healthManager.ChangeHpValue(hitBox.damage);
        }
    }
    void Update() 
    {

        CheckGrounded();
        ApplyGravity();
        Move();

    } 

    void CheckGrounded() 
    {
        isGrounded = charController.isGrounded;
    } 

    void ApplyGravity()
    {
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Pequeno empurr�o para manter o personagem preso ao ch�o (evita ficar "flutuando" levemente)
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; // Caso contr�rio, integra a acelera��o da gravidade na velocidade vertical
        }

        
        verticalVelocity = Mathf.Clamp(verticalVelocity, -50f, 50f);
    }
}
