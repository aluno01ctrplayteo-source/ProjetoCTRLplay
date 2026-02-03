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
    private List<Collider> _enemyMarker;
    public List<Collider> EnemyMarker { get { return _enemyMarker; } set { _enemyMarker = value; enemyCount = _enemyMarker.Count; } }
    
    public int enemyCount;
    private void Awake()
    {
        BoxDetection();
    }
    public void BoxDetection()
    {
        EnemyMarker = Physics.OverlapBox(transform.position, range, Quaternion.identity, LayerMask.GetMask("Enemy")).ToList();
    }
    private void OnEnable()
    {
        gameManager.OnEnemyDeath += EnemyDefeated;
    }

    private void EnemyDefeated()
    {
        enemyCount--;
        if (enemyCount == 0) doorTrigger.UpdateState(DoorState.Open);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, range);
    }
    //functions are something like programming functions?
}
