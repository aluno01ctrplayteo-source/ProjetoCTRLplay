using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour, IInteracted
{
    public List<Items> items; // Reference to the Item scriptable object
    public int goldAmount;

    public void Interacted() 
    {
        Inventory inventory = GameObject.FindAnyObjectByType<Inventory>();
        if (inventory != null && items.Count != 0)
        {
            foreach (var i in items)
            {
                inventory.AddItem(i);
            }
            inventory.ListItems();
        }
        if (!(goldAmount <= 0))
        {
            GameManager.instance.ChangeCurrencyAmount(goldAmount);
        }
        Destroy(gameObject);
    }

    public void OnDrop()
    {
        // Optional: Logic for when the item is dropped from the inventory
    }
}
