using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderingAction : UnitAction
{
    private Vector3 NewLocation;
    private float DestChangeTime = 3.0f;
    private float WanderRange = 1.0f;
    private float MoveTimer = 0.0f;
    private int wanderCounter = 0;
    private int actionCheckBreakpoint;

    private float priorityCheckTimer = 1f;

    public WanderingAction(HKUnit linkedUnit, int breakpoint) : base(linkedUnit)
    {
        this.DestChangeTime = linkedUnit.WanderTime;
        this.WanderRange = linkedUnit.WanderDistance;
        this.actionCheckBreakpoint = breakpoint;
    }

    public override string GetMessage()
    {
        return "is wandering";
    }

    public override void Handle()
    {
        if (!LinkedUnit.IsMoving())
        {
            if (MoveTimer > 0f)
            {
                HandleTimer();
            }
            else
            {
                SpotReachAction();
            }
        }
        HandlePriorityCheck();
    }

    private void HandlePriorityCheck()
    {
        if (priorityCheckTimer > 0f)
        {
            priorityCheckTimer -= Time.deltaTime;
        }
        else
        {
            if (!LinkedUnit.actionHandler.CheckForDanger())
                LinkedUnit.actionHandler.TreasureCheck();
            priorityCheckTimer = 1f;
        }
    }

    private void SpotReachAction()
    {
        wanderCounter++;

        if (wanderCounter >= actionCheckBreakpoint)
        {
            wanderCounter = 0;
            LinkedUnit.actionHandler.ActionCheck();
        }

        MoveTimer = DestChangeTime;
        SetNewMovePosition();
    }

    private void SetNewMovePosition()
    {
        Vector2 randomVector = Random.insideUnitCircle * WanderRange;
        NewLocation = LinkedUnit.transform.position + new Vector3(randomVector.x, 0, randomVector.y);
        if (NavMesh.SamplePosition(NewLocation, out NavMeshHit hit, 10f, 0))
            NewLocation = hit.position;
        LinkedUnit.MoveToPosition(NewLocation);
    }

    private void HandleTimer()
    {
        MoveTimer -= Time.deltaTime;
    }

    public override void Init()
    {

    }

    public override void Remove()
    {
        LinkedUnit.actionHandler.recentlyWandered = false;
    }


}
