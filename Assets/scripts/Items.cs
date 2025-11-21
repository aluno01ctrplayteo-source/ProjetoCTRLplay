using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Inventory/Item")]

public class Items : ScriptableObject
{
    public Items myData;
    public int id;
    public int healingValue;
    public Sprite itemIcon;
    public string itemName;
}
