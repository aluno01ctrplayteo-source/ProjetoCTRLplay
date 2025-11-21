using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyZone : MonoBehaviour
{
    public bool enemiesDied;
    public Vector3 range = Vector3.one;
    public Door doorTrigger;
    public GameManager gameManager;
    public int enemyCount;
    private void Awake()
    {
        BoxDetection();
    }
    public void BoxDetection()
    {
       enemyCount = Physics.OverlapBox(transform.position, range, Quaternion.identity, LayerMask.GetMask("Enemy")).ToList().Count;
    }
    private void OnEnable()
    {
        gameManager.OnEnemyDeath += ZoneCleared;
    }

    private void ZoneCleared()
    {
        enemyCount--;
        if (enemyCount <= 0) StartCoroutine(doorTrigger.DoorOpen());
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, range);
    }
}
