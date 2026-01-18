using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DoorTrigger : MonoBehaviour, IInteracted
{
    public Door door;
    public bool hasTranslation;
    public Vector3 translationDir = new Vector3(0,-1,0).normalized;
    public float translationDistance = 5f;
    public float translationSpeed = .2f;
    public float timeUntilComplete;
    private Vector3 _targetPos;

    private void Awake()
    {
        _targetPos = transform.position + translationDir * translationDistance;
    }
    public IEnumerator Interacted()
    {
        door.UpdateState(DoorState.Open);
        if (!hasTranslation) yield break;
        for (float t = 0; t < timeUntilComplete; t += Time.deltaTime)
        {
            Vector3 pos = Vector3.Lerp(transform.position, _targetPos, translationSpeed * Time.deltaTime);
            transform.position = pos;
            yield return null;
        }
    }
}
