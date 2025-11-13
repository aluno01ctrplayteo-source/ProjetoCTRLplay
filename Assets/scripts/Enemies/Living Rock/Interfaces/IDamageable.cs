using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageableEnemy
{

    void TakeDamage(int change);
    void Death();
    int MaxHealth { get; set; }
    int MinHealth { get; set; }
    int CurrentHealth { get; set; }
}
