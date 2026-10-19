using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private void Awake()
    {
        if (_target == null)
        {
            Debug.LogError("[CameraFollow] Target transform is not assigned!");
        }
    }

    private void LateUpdate()
    {
        transform.position = _target.position;
    }
}
