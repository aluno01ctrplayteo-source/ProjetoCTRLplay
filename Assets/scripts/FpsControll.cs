using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsControll : MonoBehaviour
{
    public Controlle Controlle;
    private void Awake()
    {
        Controlle = new Controlle();
    }
    private void OnEnable()
    {
        Controlle.Enable();
        Controlle.Player.Jump.performed += ctx => Jump();

    }    
    private void OnDisable() {
        Controlle.Disable();
    }
    public void Jump()
    {
        Debug.Log("OIIII");
    }
    

    void Start()
    {
        
    }
    void Update()
    {
        }
    
}
