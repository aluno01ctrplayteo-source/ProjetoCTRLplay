using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleButton : MonoBehaviour, IInteracted
{
    
    public PuzzleManager puzzleManager;
    public int value;

    public void Interacted()
    {
       puzzleManager.Add(value);
       Destroy(gameObject);
    }
}
