using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsControll : MonoBehaviour
{
    public float gravity = -9.81f;
    public Controlle ControllerInputs;
    public CharacterController charController;
    public Vector2 velocity;
    public Transform orientation;
    public float movespeed = 5f;
    public float verticalVelocity;
    public bool isGrounded;
    float distanceToGround = .1f;
    public Animator playerAnimations;

    private void Awake()
    {
        ControllerInputs = new Controlle();
    }
    private void OnEnable()
    {
        ControllerInputs.Enable();
        ControllerInputs.Player.Jump.performed += ctx => Jump();
        ControllerInputs.Player.Move.performed += ctx => velocity = ctx.ReadValue<Vector2>();
        ControllerInputs.Player.Move.canceled += ctx => velocity = Vector2.zero;
        

    }    
    private void OnDisable() {
        ControllerInputs.Disable();
    }
    public void Jump()
    {
        if (isGrounded)
        verticalVelocity = 5f;
    }
    public void Move()
    {
        Vector3 direcao = orientation.forward * velocity.y + orientation.right * velocity.x;
        Vector3 fall = new Vector3(0, verticalVelocity, 0);
        charController.Move(direcao * movespeed * Time.deltaTime + fall * Time.deltaTime);
    }
    
    void Start()
    {
    }
    void Update()
    {
        Move();
        CheckGrounded();
        Mathf.Clamp(verticalVelocity, -20f, 20f);
        Gravity();
    }
    bool CheckGrounded()
    {

        isGrounded = Physics.Raycast(orientation.transform.position, Vector3.down, distanceToGround);
        return isGrounded;

    }
    void Gravity()
    {
        
        if (isGrounded)
        {
            verticalVelocity = -1f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
            
        }
        
    }


    
}
