using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : IState
{
    PlayerController _controller;
    PlayerAnimation _animation;
    public FallState(PlayerController player, PlayerAnimation animation)
    {
        _controller = player;
        _animation = animation;
    }
    public void Enter()
    {
        _animation.FallAnimation(true);
        Debug.Log("enter fall");
    }

    public void Execute()
    {  
        Debug.Log("exe fall");
    }

    public void Exit()
    {
        _animation.FallAnimation(false);

        Debug.Log("exit fall");
    }
}
