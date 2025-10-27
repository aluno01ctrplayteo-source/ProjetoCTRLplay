using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public static Inventory instance; // Singleton instance
    public List<Item> inventory = new List<Item>(); // List to hold inventory items
    public Transform itemContent; // Parent transform for UI item display
    public GameObject inventoryItem; // Prefab for individual inventory item UI

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // Persist inventory across scenes
        if (instance != null) // Ensure only one instance exists
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            Destroy(this);
        }
        instance = this; // Assign singleton instance
    }
    public void AddItem(Item item)
    {
        inventory.Add(item); // Add item to inventory list
    }
    public void RemoveItem(Item item)
    {
        inventory.Remove(item); // Remove item from inventory list
    }
    public void ListItems()
    {
        foreach (Transform child in itemContent) // Clear existing UI elements
        {
            Destroy(child.gameObject);
        }
        foreach (Item item in inventory) // Iterate through each item in the inventory
        {
            GameObject obj = Instantiate(inventoryItem, itemContent); // Instantiate UI element for the item
            TMP_Text itemName = obj.transform.Find("ItemName").GetComponent<TMP_Text>(); // Get reference to the item name text component
            Image itemIcon = obj.transform.Find("ItemImage").GetComponent<Image>(); //  Get reference to the item icon image component
            itemName.text = item.itemName; // Set the item name text
            itemIcon.sprite = item.itemIcon; // Set the item icon image
        }
    }
}

