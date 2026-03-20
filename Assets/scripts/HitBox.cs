using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.PlayerLoop;

public class HitBox : MonoBehaviour
{
    public GameObject owner;
    public HitboxType type;
    public Vector3 size = new(1, 1, 1);
    public Vector3 pos = new(0, 0, 0);
    public bool destroyOnHit;
    public bool active = false;
    public bool foundInLastHit;

    public bool isDestroying;
    public GameObject destroyParticle;
    public Collider[] colliders = new Collider[10];
    public bool hitPlayer;
    public int impactForce = 5;
    public int hitCooldown = 3;
    public GeometryType geometryType;
    private GameManager _gameManager;
    private TempSet<DynamicEntity> hitEntities;
    public bool hitEnemy;
    public int value = 25;

    private void Awake()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
        hitEntities = new(hitCooldown);
    }

    private void Start()
    {
        StartCoroutine(Check());
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.matrix = Matrix4x4.TRS(transform.position + transform.rotation * pos, transform.rotation, Vector3.one);
        switch (geometryType) { 
            case GeometryType.Box:
                Gizmos.DrawWireCube(Vector3.zero, size);
                break;
            case GeometryType.Sphere:
                Gizmos.DrawWireSphere(Vector3.zero, Mathf.Max(size.x, size.y, size.z)*.5f);
                break;
        }
    }

    private void OnEnable()
    {
    }

    private IEnumerator Check()
    {
        while (true)
        {
            if (!active)
            {
                yield return null;
                continue;
            }
            int mask = hitEnemy && hitPlayer ? LayerMask.GetMask("Enemy", "Player") : hitPlayer ? LayerMask.GetMask("Player") : hitEnemy ? LayerMask.GetMask("Enemy") : 0;

            int count = 0;

            switch (geometryType)
            {
                case GeometryType.Box:
                    count = Physics.OverlapBoxNonAlloc(transform.position + transform.rotation * pos, size / 2, colliders, transform.rotation, mask, QueryTriggerInteraction.Ignore);
                    break;
                case GeometryType.Sphere:
                    count = Physics.OverlapSphereNonAlloc(transform.position + transform.rotation * pos, Mathf.Max(size.x,size.y,size.z) *.5f, colliders, mask, QueryTriggerInteraction.Ignore);
                    break;
            }
            
            for (int i = 0; i < count; i++)
            {
                var collider = colliders[i];
                if (collider == null) continue;
                var dynamicEntity = collider.GetComponent<DynamicEntity>();
                if (dynamicEntity == null) continue;
                if (hitEntities.Contains(dynamicEntity)) continue;
                hitEntities.Add(dynamicEntity);
                dynamicEntity.HitBoxInteractionEvent(this);
            }
            foundInLastHit = count > 0;
            if(foundInLastHit && destroyOnHit) DestroyH();
            yield return null;
        }
    }

    public void DestroyH()
    {
        if (!destroyOnHit) return;
        if (isDestroying) return;
        isDestroying = true;
        if (destroyParticle) { GameObject instantiatedParticle = Instantiate(destroyParticle, transform.position, transform.rotation); DestroyParticleRes(instantiatedParticle); }
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

    public enum GeometryType
    {
        Box,
        Sphere
    }
}


public class TempSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable
{
    private T[] _items = new T[0];
    public T this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }
    public TempSet(int time) 
    {
        stime = time;
    }

    public int stime = 0;

    public int _count = 0;
    public int Count { get => _count; }



    public bool IsReadOnly => false;

    public void Add(T item)
    {
        if (_items.Contains(item)) { return; }

        if (_count >= _items.Length)
        {
            var oldItems = _items;
            _items = _count == 0 ? new T[4] : new T[_count * 2];
            Array.Copy(oldItems, _items, _count);
        }
        _items[_count] = item;
        _count++;
        _ = Temp(item);
    }

    private async Task Temp(T item)
    {
        await Task.Delay((int)(stime * 1000));
        int index = Array.IndexOf(_items, item, 0, _count) ;
        if (index == -1) return;
        
        Array.Copy(_items, index + 1, _items, index, _count - index - 1);
        _count--;
        _items[_count ] = default;
    }



    public void Clear()
    {
        _items = new T[0];
        _count = 0;
    }

    public bool Contains(T item)
    {
        for (int i = 0; i <_count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(_items[i], item))
            {
                return true;
            }
        }
        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator<T>(this);
    }

    public bool Remove(T item)
    {
        _count--;
        return true;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public class Enumerator<TE> : IEnumerator, IEnumerator<T>
    {
        public Enumerator(TempSet<T> set)
        {
            var _snapshot = new T[set.Count];
            Array.Copy(set._items, _snapshot, set.Count);
            _item = _snapshot;
            _count = _item.Length;
            _index = -1;

        }
        public object Current
        {
            get
            {
                return _item[_index];
            }
        }
        private int _index;
        private T[] _item;
        private int _count;

        T IEnumerator<T>.Current => _item[_index];

        public void Dispose()
        {
            
        }

        public bool MoveNext()
        {
            _index++;
            return _index < _count;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}