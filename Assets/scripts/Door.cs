using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen;
    private Vector3 _initPos;
    public Vector3 openDirection = new Vector3(-1,0,0).normalized;
    public float rotationAmount = 25;
    public float timeUntilComplete = 10;
    CameraManager _cameraManager;
    public float openSpeed = .2f;
    public float distance = 1.0f;
    private Vector3 _targetpos;
    private void Awake()
    {
        _initPos = transform.position;
        _targetpos = transform.position + openDirection * distance;
        _cameraManager = FindObjectOfType<CameraManager>();
    }



    public IEnumerator DoorOpen()
    {
        if (isOpen) yield break;
        isOpen = true;
        _cameraManager.StartCoroutine(_cameraManager.ShakeCamera(timeUntilComplete, 0.1f, false));
        for (float t = 0; t < timeUntilComplete; t += Time.deltaTime) 
        {
            Vector3 pos = Vector3.Lerp(transform.position, _targetpos, openSpeed * Time.deltaTime);
            transform.position = pos;
            transform.Rotate(Vector3.up, rotationAmount * Time.deltaTime);
            yield return null;
        }
        
    }
}
