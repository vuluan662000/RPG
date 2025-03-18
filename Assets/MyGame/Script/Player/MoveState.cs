using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : IState
{
    PlayerController _controller;
    PlayerAnimation _animation;
    VFXManager _vfxManager;
    public MoveState(PlayerController player, PlayerAnimation animation, VFXManager vfx)
    {
        _controller = player;
        _animation = animation;
        _vfxManager = vfx;
    }
    public void Enter()
    {
        _vfxManager.PlayeFootStepVFX();
    }

    public void Execute()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StateManager.Instance.ChangeState(new AttackState(_controller,_animation,_vfxManager));
        }
        Vector3 _movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        _movement.Normalize();

        _animation.MoveAnimation();
        
        _controller.Moving(_movement);
    }

    public void Exit()
    {
        _vfxManager.StopFootStepVFX();
    }
}
