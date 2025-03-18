using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamageable
{
    public CharacterController characterController;
    PlayerAnimation _animation;
    VFXManager _vfx;

    private IdleState _idleState;
    private MoveState _moveState;
    private FallState _fallState;
    private AttackState _attackState;
    private DeadState _deadState;

    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 3.0f;
    [SerializeField] private float _velocity;
    private Vector3 _direction;

    public int health ;
    public int maxHealth = 100;
    public Slider healthSlider;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        _animation = GetComponent<PlayerAnimation>();
        _vfx = GetComponent<VFXManager>();
    }

    void Start()
    {

        health = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        _idleState = new IdleState(this, _animation);
        _moveState = new MoveState(this, _animation, _vfx);
        _fallState = new FallState(this, _animation);
        _attackState = new AttackState(this, _animation, _vfx);
        _deadState = new DeadState(this, _animation);
        StateManager.Instance.ChangeState(new IdleState(this, _animation));
    }

    // Update is called once per frame
    void Update()
    {
        ApllyGravity();
        
        HandleState();
        
    }

    private void HandleState()
    {
        if (StateManager.Instance._currentState is AttackState)
            return;
        RotateCharacterToWardsMouse();
        if (Input.GetMouseButtonDown(0) && characterController.isGrounded)
        {
            if (!(StateManager.Instance._currentState is AttackState))
                StateManager.Instance.ChangeState(new AttackState(this, _animation, _vfx));
        }
        else if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
        {
            if (!characterController.isGrounded)
            {
                if (!(StateManager.Instance._currentState is FallState))
                    StateManager.Instance.ChangeState(_fallState);
            }
            else
            {
                if (!(StateManager.Instance._currentState is MoveState))
                    StateManager.Instance.ChangeState(_moveState);
            }
        }
        else
        {
            if (!(StateManager.Instance._currentState is IdleState))
                StateManager.Instance.ChangeState(_idleState);
        }
    }
    public void Moving(Vector3 movement)
    {
        characterController.Move(movement * Time.deltaTime * _speed);
    }
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
    public void ApllyGravity()
    {
        if (characterController.isGrounded && _velocity < 0.0f)
        {
            _velocity = -1.0f;
        }
        else
        {
            _velocity += _gravity * gravityMultiplier * Time.deltaTime;
        }

        _direction.y = _velocity;
        characterController.Move(_direction);
        
    }
    public void RotateCharacterToWardsMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 direction = mousePos - playerPos;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(-angle + 45, Vector3.up);
    }
    public void AttackEnds()
    {
        StateManager.Instance.ChangeState(_idleState);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthSlider.value = health;

        if (health < 0)
        {
            StateManager.Instance.ChangeState(_deadState);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Enemy"))
        {
            Debug.Log("Player hit the Enemy!");
        }
    }
}
