using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDynamicEntity
{
    string EntityName { get; }
    int EntityID { get; }
    public virtual IEnumerator TakeDirectDamage(HitBox hitbox)
    {
        yield return null;
    }
    public virtual IEnumerator TakeInternalDamage(int damage)
    {
        yield return null;
    }
    public virtual void Heal(int amount)
    {
        return;
    }
    protected virtual IEnumerator Death()
    {
        yield return null;
    }
    int MaxHealth { get; }
    int MinHealth { get; }
    int CurrentHealth { get; }
}
public interface IEnemyMarker
{

}
