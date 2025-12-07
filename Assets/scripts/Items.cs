using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Inventory/Item")]

public class Items : ScriptableObject
{
    public Items myData;
    public int id;
    public ItemType itemType;
    public int value;
    public Sprite itemIcon;
    public string itemName;
}
public enum ItemType
{
    GoodConsumable,
    BadConsumable
}