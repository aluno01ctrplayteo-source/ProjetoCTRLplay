using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public static Inventory instance; // Singleton instance
    public List<Items> inventory = new(); // List to hold inventory items
    public Transform itemContent; // Parent transform for UI item display
    public Player player; // Reference to the player to acess player stats
    public GameObject inventoryItem; // Prefab for individual inventory item UI
    public List<int> inventoryItemsID;

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // Persist inventory across scenes
        if (instance != null) // Ensure only one instance exists
        {
            Destroy(this);
        }
        instance = this; // Assign singleton instance
    }
    public void AddItem(Items item)
    {
        inventory.Add(item); // Add item to inventory list
        inventoryItemsID.Add(item.id);
    }
    public void RemoveItem(Items item)
    {
        inventory.Remove(item); // Remove item from inventory list
        inventoryItemsID.Remove(item.id);
    }
    public void ListItems()
    {
        foreach (Transform child in itemContent) // Clear existing UI elements
        {
            Destroy(child.gameObject);
        }
        foreach (Items item in inventory) // Iterate through each item in the inventory
        {
            GameObject obj = Instantiate(inventoryItem, itemContent); // Instantiate UI element for the item
            Button button = obj.GetComponent<Button>(); // Get reference to the button component
            button.onClick.AddListener(() => gameObject.GetComponent<Inventory>().UseItem(item)); // Add click listener to use the item
            TMP_Text itemName = obj.transform.Find("ItemName").GetComponent<TMP_Text>(); // Get reference to the item name text component
            Image itemIcon = obj.transform.Find("ItemImage").GetComponent<Image>(); //  Get reference to the item icon image component
            itemName.text = item.itemName; // Set the item name text
            itemIcon.sprite = item.itemIcon; // Set the item icon image
        }
    }
    public void UseItem(Items item)
    { 
        if (!inventory.Contains(item)) return;
        int itemValue = item.value;
        ItemType type = item.itemType;
        switch (type)
        {
            case ItemType.GoodConsumable:
                player.Heal(itemValue);
                break;
            case ItemType.BadConsumable:
                StartCoroutine(player.TakeInternalDamage(itemValue));
                break;
        }

            Debug.Log($"item used: {item.itemName}");
        RemoveItem(item); // Remove the item from inventory after use
        ListItems(); // Update the UI to reflect the change
    }
}

