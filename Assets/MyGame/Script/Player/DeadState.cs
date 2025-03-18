using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DeadState : IState
{
    PlayerController _controller;
    PlayerAnimation _animation;

    public DeadState(PlayerController player, PlayerAnimation animation)
    {
        _controller = player;
        _animation = animation;

    }
    public void Enter()
    {
        _controller.enabled = false;
        _animation.DeadAnimation();
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
       
    }
}
