using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour, IInteracted
{
    public Itens item; // Reference to the Item scriptable object

    public void Interacted() 
    {
        Inventory inventory = GameObject.FindAnyObjectByType<Inventory>();
        if (inventory != null) // Check if the inventory instance exists
        {
            inventory.AddItem(item);
            inventory.ListItems();
            Destroy(gameObject);
        }
    }
}
