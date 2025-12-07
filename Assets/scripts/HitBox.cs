using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public Collider trigger;
    public GameObject owner;
    public HitboxType type;
    public bool hitPlayer;
    public int impactForce = 5;
    private GameManager _gameManager;

    public bool hitEnemy;
    public int value;
    List<GameObject> enemies;
    private void Awake()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
    }
    private void Start()
    {
        UpdateHitBox();
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
    private void OnEnable() => _gameManager.OnEnemyCreation += UpdateHitBox;
    public void OnDisable() => _gameManager.OnEnemyCreation -= UpdateHitBox;
    public void ToggleTrigger()
    {
        trigger.enabled = !trigger.enabled;
    }
}
public enum HitboxType
{
    None = 0,
    Damage = 1,
    Heal = 2,
}