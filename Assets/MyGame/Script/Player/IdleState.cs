using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    PlayerController _controller;
    PlayerAnimation _animation;
    public IdleState(PlayerController player, PlayerAnimation animation) 
    {
        _controller = player;
        _animation = animation;
    }
    public void Enter()
    {
        _animation.IdleAnimation();
        _controller.enabled = true;
    }

    public void Execute()
    {
    }

    public void Exit()
    {

    }
}
