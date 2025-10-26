using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Object", menuName = "Inventory/Item")]

public class Item : ScriptableObject
{
    public Item myData;
    public int id;
    public int value;
    public Sprite itemIcon;
    public string itemName;
    public GameObject prefab;
}
