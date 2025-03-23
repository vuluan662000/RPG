using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AttackState : IState
{
    PlayerController _controller;
    PlayerAnimation _animation;
    VFXManager _vFXManager;
    PlayerStats _playerStats;

    private float _slideDistance = 1f;
    private Vector3 _slideDirection;
    private float _slideSpeed = 10f;
    private bool _isSliding;

    public AttackState(PlayerController player, PlayerAnimation animation, VFXManager vFXManager, PlayerStats playerStats)
    {
        _controller = player;
        _animation = animation;
        _vFXManager = vFXManager;
        _playerStats = playerStats;
    }
    public void Enter()
    {
        DealDamage();
        _vFXManager.PlayyBlade01VFX();
        _slideDirection = _controller.transform.forward;
        _isSliding = true;
        _animation.AttackAnimation();
        
        //string currentClipName = _animation._animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        //float attackAnimationDuration = _animation._animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        //Debug.Log($"{currentClipName} {attackAnimationDuration}");
        //if (currentClipName != "LittleAdventurerAndie_ATTACK_03" && attackAnimationDuration > 0.3f && attackAnimationDuration < 0.5f)
        //{
        //    StateManager.Instance.ChangeState(_controller._attackState);
        //}
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
        Collider[] hitColliders = Physics.OverlapSphere(attackPosition,_playerStats._attackRange);

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                continue;
            }

            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                //Debug.Log($" hit Daamge: {collider.gameObject.name}");
                damageable.TakeDamage(_playerStats._attackDamage);
            }    
        }
    }
}
