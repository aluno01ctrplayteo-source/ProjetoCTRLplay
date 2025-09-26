using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawningTest : MonoBehaviour
{
    public GameObject SelectedObj;

    public Collider SpawnArea;

    void RandomSpawn()
    {
        Vector3 ObjPosition = new Vector3(Random.Range(SpawnArea.bounds.max.x, SpawnArea.bounds.min.x), Random.Range(SpawnArea.bounds.max.y, SpawnArea.bounds.min.y), Random.Range(SpawnArea.bounds.max.z, SpawnArea.bounds.min.z));
        GameObject Clone = Instantiate(SelectedObj, ObjPosition, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        Clone.transform.localScale = Vector3.one * Random.Range(0.5f, 1.5f);
    }
    private void Start()
    {
        InvokeRepeating("RandomSpawn", 1, 1);
    }
}
