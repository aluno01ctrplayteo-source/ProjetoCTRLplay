using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class EnemyZone : MonoBehaviour
{
    public bool enemiesDied;
    public Vector3 range = Vector3.one;
    public Door doorTrigger;
    public GameManager gameManager;
    [SerializeField]private List<int> _enemyId = new();
    private void Start()
    {
        BoxDetection();
    }
    public void BoxDetection()
    {
        _enemyId.Clear();
        Collider[] enemyColliders = Physics.OverlapBox(transform.position, range, Quaternion.identity, LayerMask.GetMask("Enemy"));
        _enemyId = enemyColliders.Select(collider => collider.gameObject.GetComponent<IDynamicEntity>().EntityID).ToList();
    }
    private void OnEnable()
    {
        gameManager.OnEnemyDeath += EnemyDefeated;
    }

    private void EnemyDefeated(EnemyDeathContext context)
    {
        if (!_enemyId.Contains(context.id)) return;
        _enemyId.Remove(context.id);
        if (_enemyId.Count == 0) { enemiesDied = true; doorTrigger.UpdateState(DoorState.Open); }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, range * 2);
    }
}
