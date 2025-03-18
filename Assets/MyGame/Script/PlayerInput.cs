using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private static PlayerInput _instance;
    public static PlayerInput Instance { get => _instance;}

    public float horizontalInput;
    public float verticalInput;

    private void Awake()
    {
        _instance = this;

    }
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }
    private void OnDisable()
    {
        horizontalInput = 0;
        verticalInput = 0;
    }

}
