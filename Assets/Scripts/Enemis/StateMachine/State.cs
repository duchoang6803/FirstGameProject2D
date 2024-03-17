using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected FiniteStateMachine finiteState; 
    protected Entity entity;

    protected float startTime;

    protected string animSetBoolName;

    // Tao ham Constructor de khoi tao gia tri ban dau
    public State(Entity entity, FiniteStateMachine finiteState, string animSetBoolName)
    {
        this.entity = entity;
        this.finiteState = finiteState;
        this.animSetBoolName = animSetBoolName;
    }

    public virtual void Enter()
    {
        startTime = Time.time; // Thiet lap thoi gian khi bat dau
        entity.anim.SetBool(animSetBoolName,true);
    }

    public virtual void Exit()
    {
        entity.anim.SetBool(animSetBoolName, false);
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {

    }
}
