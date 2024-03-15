using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    public State currentState { get; private set; }

    public void Initialize(State startState) // Khoi tao trang thai cua doi tuong
    {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(State newState) // Tao trang thai moi cho doi tuong
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
