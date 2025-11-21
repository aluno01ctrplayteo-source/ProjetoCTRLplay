using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class PuzzleManager : MonoBehaviour
{
    PuzzleManager instance;
    public Door door;

    public List<int> order = new();
    public List<int> input = new();
    public int index = 0;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Add(int value)
    {
        input.Add(value);
        index++;
    }
    void Update()
    {
        if (index == 4) 
        {
            if (input[0] == 3 && input[1] == 1 && input[2] == 4 && input[3] == 2) StartCoroutine(door.DoorOpen());  
        }
    }
}
