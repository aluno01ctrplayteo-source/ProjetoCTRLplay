using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public Controlle controls;
    public GameObject playerbody;
    public Vector2 cameraInput;
    public GameObject PlayerBody;
    public GameObject Pivot;
    public float sensitivity = 100f;
    public float xRotation = 0f;

    public void Awake()
    {
        controls = new Controlle();
    }
    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Look.performed += ctx => cameraInput = ctx.ReadValue<Vector2>(); //Whenever the Look event is performed, store Look value inside ctx and assign ctx value to cameraInput
        controls.Player.Look.canceled += ctx => cameraInput = Vector2.zero;
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CameraMove();
    }
    public void CameraMove()
    {
        Vector2 Look = cameraInput * sensitivity * Time.deltaTime;
        xRotation -= Look.y;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Returns A number between -90 and 90

        Pivot.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerbody.transform.Rotate(Vector3.up, Look.x);
    }
}
