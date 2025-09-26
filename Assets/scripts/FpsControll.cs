using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsControll : MonoBehaviour
{
    public float gravity = -9.81f;
    public Controlle Controlle;
    public CharacterController controlle;
    public Vector2 velocity;
    public Transform orientacao;
    public bool IsGrounded;
    public float movespeed = 5f;

    private void Awake()
    {
        Controlle = new Controlle();
    }
    private void OnEnable()
    {
        Controlle.Enable();
        Controlle.Player.Jump.performed += ctx => Jump();
        Controlle.Player.Move.performed += ctx => velocity = ctx.ReadValue<Vector2>();
        Controlle.Player.Move.canceled += ctx => velocity = Vector3.zero;

    }    
    private void OnDisable() {
        Controlle.Disable();
    }
    public void Jump()
    {
    }
    public void Move()
    {
        Vector3 direcao = orientacao.forward * velocity.y + orientacao.right * velocity.x;
        controlle.Move(direcao * movespeed * Time.deltaTime);
    }

    void Start()
    {
        
    }
    void Update()
    {
        Move();
    }
    
}
