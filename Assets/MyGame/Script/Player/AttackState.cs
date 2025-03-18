using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    PlayerController _controller;
    PlayerAnimation _animation;
    VFXManager _vFXManager;

    private float _slideDistance = 1f;
    private Vector3 _slideDirection;
    private float _slideSpeed = 10f;
    private bool _isSliding;

    private float _attackRange = 1.5f;
    private int _attackDamage = 100; 
    public AttackState(PlayerController player, PlayerAnimation animation, VFXManager vFXManager)
    {
        _controller = player;
        _animation = animation;
        _vFXManager = vFXManager;
    }
    public void Enter()
    {
        _animation.AttackAnimation();
        _vFXManager.PlayyBlade01VFX();
        
        _slideDirection = _controller.transform.forward;
        _isSliding = true;
        DealDamage();
    }

    public void Execute()
    {
        if (_isSliding)
        {
            float slideStep =  _slideSpeed * Time.deltaTime;
            Vector3 slideMovement = _slideDirection * slideStep;

            _controller.characterController.Move(slideMovement);

            _slideDistance -= slideStep ;
            if (_slideDistance <= 0f)
            {
                _isSliding = false;
            }
        }
    }

    public void Exit()
    {

    }

    private void DealDamage()
    {
        Vector3 attackPosition = _controller.transform.position + _controller.transform.forward;
        Collider[] hitColliders = Physics.OverlapSphere(attackPosition, _attackRange);

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                continue;
            }

            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Debug.Log($" hit Daamge: {collider.gameObject.name}");
                damageable.TakeDamage(_attackDamage);
            }    
        }
    }
}
