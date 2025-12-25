using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    IEnumerator TakeHitboxDamage(HitBox hitbox);
    IEnumerator TakeDirectDamage(int damage);
    void Heal(int amount);
    IEnumerator DeathState();
    int MaxHealth { get; }
    int MinHealth { get; }
    int CurrentHealth { get; set; }
}
public interface IEnemyMarker
{

}
