using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour, IInteracted
{
    public Items item;

    public void Interacted()
    {
        Destroy(this.gameObject);
    }
}
