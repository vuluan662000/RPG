using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    private static StateManager _instance;
    public static StateManager Instance { get => _instance; }

    [SerializeField] public IState _currentState;

    private void Awake()
    {
        StateManager._instance = this;
    }
    public void ChangeState(IState state)
    {
        if (_currentState !=null && state.GetType() == _currentState.GetType()) return;

        if (_currentState != null)
        {
            _currentState.Exit();
        }
        _currentState = state;
        _currentState.Enter();
        
    }
    private void Update()
    {
        ExecuteCurrentState();
    }
    public void ExecuteCurrentState()
    {
        if (_currentState != null)
        {
            _currentState.Execute();
        }
    }
}

