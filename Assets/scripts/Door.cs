using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteracted
{
    // Start is called before the first frame update
    public bool isOpen = false;
    void Start()
    {
        
    }
    public void Interacted()
    {
        isOpen = !isOpen;
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void DoorRotate()
    {
        if (isOpen)
        {
            transform.Rotate(0, 90, 0);
        }
        else
        {
            transform.Rotate(0, -90, 0);
        }
    }
}
