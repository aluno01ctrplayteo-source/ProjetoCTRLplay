using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    IEnumerator TakeDamage(HitBox hitbox);
    IEnumerator TakeDamage(int damage);
    void Heal(int amount);
    IEnumerator Death();
    int MaxHealth { get; set; }
    int MinHealth { get; set; }
    int CurrentHealth { get; set; }
}

