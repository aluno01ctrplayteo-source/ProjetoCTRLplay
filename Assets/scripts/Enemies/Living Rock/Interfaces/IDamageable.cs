using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    abstract void TakeDamage(int change);
    abstract void Death();
    abstract int maxHealth { get; set; }
    abstract int currentHealth { get; set; }
}
