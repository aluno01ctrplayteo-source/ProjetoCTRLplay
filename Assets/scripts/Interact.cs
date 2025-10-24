using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
public class Interact : MonoBehaviour
{
    GameManager gameManager;
    Controlle controllerInputs;

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
