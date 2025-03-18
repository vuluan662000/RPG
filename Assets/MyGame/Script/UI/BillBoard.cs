using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    public Transform _camera;

    void Awake()
    {
        if (_camera == null)
        {
            _camera = Camera.main.transform;
        }
    }
    private void LateUpdate()
    {
        transform.LookAt(transform.position + _camera.forward);
    }
}
