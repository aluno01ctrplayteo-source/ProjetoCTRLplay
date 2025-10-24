using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using System;
public class Interact : MonoBehaviour
{
    GameManager gameManager;
    Controlle controllerInputs;
    public Items item;
    private void Start()
    {
        controllerInputs = new Controlle();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Healer"))
        {
            Destroy(other.gameObject);
            gameManager.healthManager.HpChanger(20);
        }
        if (other.CompareTag("Item"))
        {
            Item item = other.GetComponent<Item>();
            if (Input.GetKeyDown(KeyCode.E) && item != null)
            {
                item.Interacted();
            }

        }


    }
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
        {
            if (hit.transform.gameObject.GetComponent<Door>() != null && Input.GetKeyDown(KeyCode.E))
            {
                Door door = hit.transform.gameObject.GetComponent<Door>();
                door.Interacted();
            }
            if (hit.transform.gameObject.GetComponent<ItemData>() != null && Input.GetKeyDown(KeyCode.E))
            {
                item = hit.transform.gameObject.GetComponent<ItemData>().myData;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            gameManager.currency++;
            gameManager.display.text = $"Coins -> {gameManager.currency}";
        }
    }
}
