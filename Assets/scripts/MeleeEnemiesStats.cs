using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemies/Enemy", order = 1)]

public class MeleeEnemiesStats : ScriptableObject
{
    public MeleeEnemiesStats myData;
    public DynamicEntity script;
    public string enemyName;
    public float attackTriggerRangeMultiplier;
    public int damage;
    public float speed;
    public float detectionRange;
    public int maxHealth;
    public int minHealth = 0;
}