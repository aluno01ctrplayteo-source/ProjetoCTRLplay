using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class DynamicEntity : MonoBehaviour
{
    public abstract string EntityName { get; protected set; }
    public abstract int EntityID { get; }
    public abstract IEnumerator TakeDirectDamage(HitBox hitbox);
    public abstract IEnumerator TakeInternalDamage(int damage);
    public abstract void Heal(int amount);
    protected abstract IEnumerator Death();

    public event Action<HitBox> OnTakeDamage;

    public void TakeDamageEvent(HitBox hitbox)
    {
        OnTakeDamage?.Invoke(hitbox);
    }

    int MaxHealth { get; }
    int MinHealth { get; }
    int CurrentHealth { get; }
}
public interface IEnemyMarker
{

}
