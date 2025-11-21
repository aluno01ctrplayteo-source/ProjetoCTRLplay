using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{

    void TakeDamage(int change);
    IEnumerator Death();
    int MaxHealth { get; set; }
    int MinHealth { get; set; }
    int CurrentHealth { get; set; }
}

