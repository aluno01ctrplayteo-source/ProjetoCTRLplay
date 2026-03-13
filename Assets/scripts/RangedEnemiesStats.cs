using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangedEnemy", menuName = "ScriptableObjects/Enemies/RangedEnemy", order = 1)]

public class RangedEnemiesStats : ScriptableObject
{
    public RangedEnemiesStats myData;
    public DynamicEntity script;
    public string enemyName;
    public float attackRange;
    public int damage;
    public float attackSpeed;
    public float detectionRange;
    public int maxHealth;
    public int minHealth = 0;
}