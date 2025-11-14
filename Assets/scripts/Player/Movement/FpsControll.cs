using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FpsControll : MonoBehaviour
{
    [Header("Movimenta��o")] 
    public float moveSpeed = 5f; // Velocidade de movimento horizontal do jogador (metros por segundo)
    public Transform orientation; // Transform usado como refer�ncia de dire��o (normalmente a c�mera ou um objeto que representa a orienta��o do jogador)
    public Vector2 velocity; // Vetor 2D que armazena a entrada de movimento (x = lateral, y = frente/tr�s)

    [Header("Pulo & Gravidade")] 
    public float gravity = -9.81f; // Acelera��o da gravidade (valor negativo porque aponta para baixo)
    public float jumpForce = 5f; // "For�a" do pulo � aqui est� sendo usada como altura desejada na f�rmula f�sica
    public float verticalVelocity; // Velocidade vertical atual (eixo Y) do personagem
    public bool isGrounded; // Flag que indica se o jogador est� no ch�o
    public float groundCheckDistance = 0.3f; // Dist�ncia usada para checar o ch�o via Raycast (n�o usada na vers�o atual do CheckGrounded, mas declarada)
    public LayerMask groundMask; // LayerMask que define quais camadas contam como "ch�o" para o Raycast

    [Header("Componentes")] 
    public CharacterController charController; // Refer�ncia ao componente CharacterController (usado para movimenta��o com colis�es)
    public Animator playerAnimations; // Refer�ncia ao Animator (n�o usado no script atual, mas preparado)
    public Controlle ControllerInputs; // Inst�ncia da classe gerada pelo Input System (nomenclatura 'Controlle' parece ser nome customizado)

    [Header("Combate")]
    public int damageAmount; // Dano causado por ataque
    public float attackCooldown = 1f; // Tempo de recarga entre ataques
    bool canAttack = true; // Flag para controlar se o jogador pode atacar

    private void Awake() 
    {
        ControllerInputs = new Controlle(); // Instancia o objeto de Input Actions (geralmente gerado pelo novo Input System)
    } 

    private void OnEnable() 
    {
        ControllerInputs.Enable(); // Ativa o mapa de a��es para come�ar a receber entradas
        ControllerInputs.Player.Jump.performed += ctx => Jump(); // Ao disparar o evento 'Jump.performed', chama o m�todo Jump()
        ControllerInputs.Player.Move.performed += ctx => velocity = ctx.ReadValue<Vector2>(); // Ao movimentar, l� o Vector2 (input) e armazena em 'velocity'
        ControllerInputs.Player.Move.canceled += ctx => velocity = Vector2.zero; // Quando o input de movimento � cancelado (soltou a tecla/joystick), zera a velocidade
        ControllerInputs.Player.Pause.performed += ctx => GameManager.instance.GamePaused(); // Ao apertar Pause, chama o m�todo no GameManager (possivelmente alterna pausa)
        ControllerInputs.Player.Inventory.performed += ctx => GameManager.instance.InventoryOpen(); // Ao apertar Inventory, chama o m�todo no GameManager (possivelmente abre/fecha invent�rio)
        ControllerInputs.Player.Attack.performed += ctx => Attack() ; // Ao apertar Attack, chama o m�todo Attack no playerCombat do GameManager
    } 

    private void OnDisable()
    { 
        ControllerInputs.Disable(); // Desativa o mapa de a��es (interrompe leitura de inputs)
       
    } 

    void Jump() // M�todo que executa a l�gica de pulo
    {
        if (isGrounded) // S� permite pular se estiver no ch�o
        {
            // Calcula a velocidade inicial vertical necess�ria para atingir a "altura" indicada por jumpForce
            // F�rmula f�sica: v = sqrt(2 * g_abs * height) ; como gravity � negativo usamos -2f * gravity
            verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity); // atribui velocidade inicial do pulo
        } 
    } 

    void Move() // M�todo que aplica movimento horizontal e vertical atrav�s do CharacterController
    {
        // Constr�i a dire��o de movimento combinando a orienta��o do mundo (forward/right) com o input (velocity.y, velocity.x)
        Vector3 direction = orientation.forward * velocity.y + orientation.right * velocity.x;
        direction.Normalize(); // Normaliza o vetor para evitar aumento de velocidade na diagonal

        // Monta o vetor final de movimento: dire��o horizontal multiplicada por moveSpeed + componente vertical
        Vector3 move = direction * moveSpeed + Vector3.up * verticalVelocity;
        charController.Move(move * Time.deltaTime); // Move o CharacterController (leva em conta colis�es); multiplicado por deltaTime para ser frame-rate independent
    }

    public void Attack() // M�todo de ataque que causa dano a inimigos na frente do jogador
    {
        //Usar Raycast para detectar inimigos na frente do jogador
        Collider[] hitCollider = Physics.OverlapBox(transform.position + transform.forward * 2, Vector3.one); // Cria uma caixa de colis�o na frente do jogador
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
        }
    }

    void Update() 
    {
        CheckGrounded(); // Verifica se o jogador est� no ch�o e atualiza isGrounded
        ApplyGravity(); // Calcula/Aplica gravidade (atualiza verticalVelocity)
        Move(); // Executa a movimenta��o baseada nos valores calculados
    } 

    void CheckGrounded() // M�todo que verifica o ch�o 
    {
        // Simplesmente usa o isGrounded do CharacterController para checar se o jogador est� encostando no ch�o
        isGrounded = charController.isGrounded;
    } 

    void ApplyGravity() // Aplica a l�gica da gravidade
    {
        if (isGrounded && verticalVelocity < 0) // Se est� no ch�o e a velocidade vertical est� apontando para baixo
        {
            verticalVelocity = -2f; // Pequeno empurr�o para manter o personagem preso ao ch�o (evita ficar "flutuando" levemente)
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; // Caso contr�rio, integra a acelera��o da gravidade na velocidade vertical
        }

        // Limita a velocidade vertical para evitar valores extremos (ex.: queda muito r�pida)
        verticalVelocity = Mathf.Clamp(verticalVelocity, -50f, 50f);
    }
}
