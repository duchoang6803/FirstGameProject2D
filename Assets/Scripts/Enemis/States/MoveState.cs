using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    protected D_MoveState stateData;

    protected bool isDetectingWall;
    protected bool isDetectingGround;
    public MoveState(Entity entity, FiniteStateMachine finiteState, string animSetBoolName, D_MoveState stateData) : base(entity, finiteState, animSetBoolName)
    {
        this.stateData = stateData;
    }

    public override void Enter()
    {
        base.Enter();
        entity.SetVelocity(stateData.movementSpeed);
        isDetectingWall = entity.CheckWall();
        isDetectingGround = entity.CheckGround();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        entity.SetVelocity(stateData.movementSpeed);
        isDetectingWall = entity.CheckWall();
        isDetectingGround = entity.CheckGround();

    }

}
