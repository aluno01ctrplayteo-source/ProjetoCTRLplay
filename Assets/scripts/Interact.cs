using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using System;
using JetBrains.Annotations;
public class Interact : MonoBehaviour
{
    GameManager gameManager;
    Controller controllerInputs;
    public float interactDistance = 3f;

    private void Awake()
    {
        controllerInputs = new Controller(); // Instantiate the Controlle input actions
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>(); // Get reference to GameManager
    }
    private void OnEnable()
    {
        controllerInputs.Enable(); // Enable the input action map
    }
    private void OnDisable()
    {
        controllerInputs.Disable(); // Disable the input action map
    }
    void Update()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, interactDistance)) // Cast a ray from the camera forward
        {
            if (hit.transform.gameObject.GetComponent<ItemData>() != null && controllerInputs.Player.Interaction.WasPressedThisFrame() == true)
            {
                ItemData itemData = hit.transform.gameObject.GetComponent<ItemData>();
                itemData.Interacted();
            }
            if (hit.transform.gameObject.GetComponent<DoorTrigger>() != null && controllerInputs.Player.Interaction.WasPressedThisFrame() == true)
            {
                DoorTrigger doorTrigger = hit.transform.gameObject.GetComponent<DoorTrigger>();
                StartCoroutine(doorTrigger.Interacted());
            }
            if (hit.transform.gameObject.GetComponent<PuzzleButton>() != null && controllerInputs.Player.Interaction.WasPressedThisFrame() == true)
            {
                PuzzleButton button = hit.transform.gameObject.GetComponent<PuzzleButton>();
                button.Interacted();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Healer"))
        {
            Destroy(other.gameObject);
            gameManager.healthManager.ChangeHpValue(20);
        }
    }
    
}
