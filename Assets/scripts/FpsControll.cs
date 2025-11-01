using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FpsControll : MonoBehaviour
{
    [Header("Movimentação")] 
    public float moveSpeed = 5f; // Velocidade de movimento horizontal do jogador (metros por segundo)
    public Transform orientation; // Transform usado como referência de direção (normalmente a câmera ou um objeto que representa a orientação do jogador)
    public Vector2 velocity; // Vetor 2D que armazena a entrada de movimento (x = lateral, y = frente/trás)

    [Header("Pulo & Gravidade")] 
    public float gravity = -9.81f; // Aceleração da gravidade (valor negativo porque aponta para baixo)
    public float jumpForce = 5f; // "Força" do pulo — aqui está sendo usada como altura desejada na fórmula física
    public float verticalVelocity; // Velocidade vertical atual (eixo Y) do personagem
    public bool isGrounded; // Flag que indica se o jogador está no chão
    public float groundCheckDistance = 0.3f; // Distância usada para checar o chão via Raycast (não usada na versão atual do CheckGrounded, mas declarada)
    public LayerMask groundMask; // LayerMask que define quais camadas contam como "chão" para o Raycast

    [Header("Componentes")] 
    public CharacterController charController; // Referência ao componente CharacterController (usado para movimentação com colisões)
    public Animator playerAnimations; // Referência ao Animator (não usado no script atual, mas preparado)
    public Controlle ControllerInputs; // Instância da classe gerada pelo Input System (nomenclatura 'Controlle' parece ser nome customizado)

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
        ControllerInputs.Enable(); // Ativa o mapa de ações para começar a receber entradas
        ControllerInputs.Player.Jump.performed += ctx => Jump(); // Ao disparar o evento 'Jump.performed', chama o método Jump()
        ControllerInputs.Player.Move.performed += ctx => velocity = ctx.ReadValue<Vector2>(); // Ao movimentar, lê o Vector2 (input) e armazena em 'velocity'
        ControllerInputs.Player.Move.canceled += ctx => velocity = Vector2.zero; // Quando o input de movimento é cancelado (soltou a tecla/joystick), zera a velocidade
        ControllerInputs.Player.Pause.performed += ctx => GameManager.instance.IsGamePaused(); // Ao apertar Pause, chama o método no GameManager (possivelmente alterna pausa)
        ControllerInputs.Player.Inventory.performed += ctx => GameManager.instance.IsInventoryOpen(); // Ao apertar Inventory, chama o método no GameManager (possivelmente abre/fecha inventário)
        ControllerInputs.Player.Attack.performed += ctx => Attack() ; // Ao apertar Attack, chama o método Attack no playerCombat do GameManager
    } 

    private void OnDisable()
    { 
        ControllerInputs.Disable(); // Desativa o mapa de ações (interrompe leitura de inputs)
       
    } 

    void Jump() // Método que executa a lógica de pulo
    {
        if (isGrounded) // Só permite pular se estiver no chão
        {
            // Calcula a velocidade inicial vertical necessária para atingir a "altura" indicada por jumpForce
            // Fórmula física: v = sqrt(2 * g_abs * height) ; como gravity é negativo usamos -2f * gravity
            verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity); // atribui velocidade inicial do pulo
        } 
    } 

    void Move() // Método que aplica movimento horizontal e vertical através do CharacterController
    {
        // Constrói a direção de movimento combinando a orientação do mundo (forward/right) com o input (velocity.y, velocity.x)
        Vector3 direction = orientation.forward * velocity.y + orientation.right * velocity.x;
        direction.Normalize(); // Normaliza o vetor para evitar aumento de velocidade na diagonal

        // Monta o vetor final de movimento: direção horizontal multiplicada por moveSpeed + componente vertical
        Vector3 move = direction * moveSpeed + Vector3.up * verticalVelocity;
        charController.Move(move * Time.deltaTime); // Move o CharacterController (leva em conta colisões); multiplicado por deltaTime para ser frame-rate independent
    }

    public void Attack() // Método de ataque que causa dano a inimigos na frente do jogador
    {
        Collider[] hitCollider = Physics.OverlapBox(transform.position + transform.forward * 2, Vector3.one); // Cria uma caixa de colisão na frente do jogador
        Debug.Log(hitCollider.Length); // Loga quantos colliders foram atingidos (para debug)
        foreach (var collider in hitCollider) // Itera sobre todos os colliders encontrados na caixa
        {
                if (collider.gameObject == this.gameObject) continue; // Ignora o próprio jogador
            Enemy enemy = collider.GetComponent<Enemy>(); // Tenta obter o componente Enemy do objeto colidido
            if (enemy != null) // Se encontrou um componente Enemy no collider
            {
                    enemy.TakeDamage(-damageAmount); // Aplica dano ao inimigo
                                                     //problema no takedamage: está sendo chamado duas vezes
            }
        }
    }

    void Update() 
    {
        CheckGrounded(); // Verifica se o jogador está no chão e atualiza isGrounded
        ApplyGravity(); // Calcula/Aplica gravidade (atualiza verticalVelocity)
        Move(); // Executa a movimentação baseada nos valores calculados
    } 

    void CheckGrounded() // Método que verifica o chão 
    {
        // Simplesmente usa o isGrounded do CharacterController para checar se o jogador está encostando no chão
        isGrounded = charController.isGrounded;
    } 

    void ApplyGravity() // Aplica a lógica da gravidade
    {
        if (isGrounded && verticalVelocity < 0) // Se está no chão e a velocidade vertical está apontando para baixo
        {
            verticalVelocity = -2f; // Pequeno empurrão para manter o personagem preso ao chão (evita ficar "flutuando" levemente)
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; // Caso contrário, integra a aceleração da gravidade na velocidade vertical
        }

        // Limita a velocidade vertical para evitar valores extremos (ex.: queda muito rápida)
        verticalVelocity = Mathf.Clamp(verticalVelocity, -50f, 50f);
    }
}
