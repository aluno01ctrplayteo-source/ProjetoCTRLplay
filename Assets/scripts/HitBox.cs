using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public bool isDamageHitBox;
    public Collider trigger;
    public bool hitPlayer;
    public GameManager gameManager;

    public bool hitEnemy;
    public int damage;
    List<GameObject> enemies;
    private void Awake()
    {
        if (!isDamageHitBox) 
        {
            damage = 0;
        }
    }
    private void UpdateHitBox()
    {
        enemies = GameManager.instance.enemies;
        if (!hitPlayer)
        {
            Physics.IgnoreCollision(trigger, GameObject.FindGameObjectWithTag("Player")
                .GetComponent<Collider>(), true);

        }
        if (!hitEnemy)
        {

            foreach (GameObject enemy in enemies)
            {
                Physics.IgnoreCollision(trigger, enemy.GetComponent<Collider>(), true);
            }
        }
    }
    private void OnEnable() => gameManager.OnEnemyCreation += UpdateHitBox;
}