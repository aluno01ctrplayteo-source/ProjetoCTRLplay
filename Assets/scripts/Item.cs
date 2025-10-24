using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour , IInteracted
{
    public void Interacted()
    {
        Debug.Log("Item interacted with");
    }
}
