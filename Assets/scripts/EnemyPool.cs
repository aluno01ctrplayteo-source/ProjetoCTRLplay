using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool instance;
    public DynamicEntity[] enemies;
    private Dictionary<string, DynamicEntity> _enemy = new();
    public Dictionary<string, Queue<DynamicEntity>> pool = new();
    public int poolSize = 30;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        

    }
    private void Start()
    {
        _enemy = enemies.ToDictionary(enemies => enemies.EntityName, enemies => enemies);
        foreach (var enemy in enemies)
        {
            string queueKey = enemy.EntityName;
            pool.Add(queueKey, new());
            for (int i = 0; i < poolSize; i++)
            {
                var obj = Instantiate(enemy, gameObject.transform.position, Quaternion.identity);
                obj.gameObject.SetActive(false);
                pool[queueKey].Enqueue(obj);
            }
        }

    }

    public DynamicEntity GetEnemy(string enemyKey, Vector3 setPos, Quaternion setotation)
    {
        if (!(pool.ContainsKey(enemyKey)))
        {
            Debug.Log("Key has no matches");
            return null;
        }
        if (pool[enemyKey].Count > 0)
        {
            DynamicEntity enemy = pool[enemyKey].Dequeue();
            enemy.transform.position = setPos;
            enemy.transform.rotation = setotation;
            enemy.gameObject.SetActive(true);
            return enemy;
        }
        else
        {
            Debug.Log("Failed to get enemy, creating a new instance");
            var obj = Instantiate(_enemy[enemyKey], setPos, setotation);
            obj.gameObject.SetActive(true);
            return obj;
        }
    }

    public void ReturnEnemy(DynamicEntity enemy)
    {
        string enemyKey = enemy.EntityName;
        if (!(pool.ContainsKey(enemyKey))) return;
        enemy.gameObject.SetActive(false);
        pool[enemyKey].Enqueue(enemy);
    }

}
