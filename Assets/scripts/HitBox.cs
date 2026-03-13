using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using System;

public class HitBox : MonoBehaviour
{
    public GameObject owner;
    public HitboxType type;
    public Vector3 size = new(1,1,1);
    public bool destroyOnHit;
    public bool active = false;

    public bool isDestroying;
    public GameObject destroyParticle;
    private Collider[] colliders = new Collider[10];
    public bool hitPlayer;
    public int impactForce = 5;
    public float hitCooldown = 3f;

    private GameManager _gameManager;
    private HashSet<DynamicEntity> hitEntities = new();

    public bool hitEnemy;
    public int value;
    List<GameObject> enemies;

    public event Action<HitBox> OnHit;

    private void Awake()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
    }

    private IEnumerator Check()
    {
        while (true)
        {
            if (!active) continue;
            int mask = hitEnemy && hitPlayer ? LayerMask.GetMask("Enemy", "Player") : hitPlayer ? LayerMask.GetMask("Player") : hitEnemy ? LayerMask.GetMask("Enemy") : 0;



            Physics.OverlapBoxNonAlloc(transform.position, size / 2, colliders, Quaternion.identity, mask, QueryTriggerInteraction.Ignore);
            foreach (Collider collider in colliders)
            {
                if (collider == null) continue;
                var dynamicEntity = collider.GetComponent<DynamicEntity>();
                if (dynamicEntity == null) continue;
                if (hitEntities.Contains(dynamicEntity)) continue;
                hitEntities.Add(dynamicEntity);
                dynamicEntity.TakeDamageEvent(this);
            }
            colliders = new Collider[10];
        }
    }

    public void DestroyH()
    {
        if (!destroyOnHit) return;
        if (isDestroying) return;
        isDestroying = true;
        GameObject instantiatedParticle = Instantiate(destroyParticle, transform.position, transform.rotation);
        if (destroyParticle) DestroyParticleRes(instantiatedParticle);
        Destroy(owner);
    }

    private void DestroyParticleRes(GameObject g)
    {
        float t1 = g.GetComponent<ParticleSystem>().main.duration;
        float t2 = g.GetComponent<ParticleSystem>().main.startLifetime.constant;
        float totalTime = t1 + t2;
        Destroy(g, totalTime);
    }
    public void SetActive(bool value)
    {
        active = value;
    }
    public void SetActive()
    {
        active = !active;
    }
    public enum HitboxType
    {   
        None = 0,
        Damage = 1,
        Heal = 2,
    }
}
