using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemies/Enemy", order = 1)]

public class Enemies : ScriptableObject
{
    public Enemies myData;
    public string enemyName;
    public int health;
    public int damage;
    public float speed;
    public float detectionRange;
    public int maxHealth;
    public int minHealth = 0;
    public GameObject enemyPrefab;
}