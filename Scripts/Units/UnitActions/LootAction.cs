using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LootAction : UnitAction
{
    private TreasureChest targetChest;

    public LootAction(HKUnit linkedUnit, TreasureChest targetChest) : base(linkedUnit)
    {
        this.targetChest = targetChest;
        LinkedUnit.transform.LookAt(targetChest.transform);
    }

    public override void Handle()
    {
        Vector3 unitPosition = LinkedUnit.transform.position;
        Vector3 targetPosition = targetChest.GetClosestPoint(LinkedUnit.transform.position, LinkedUnit.AttackRange);

        float currentDistance = Vector3.Distance(unitPosition, targetPosition);

        if (currentDistance > 0.1f)
        {
            if (!LinkedUnit.IsMoving())
            {
                LinkedUnit.MoveToPosition(targetPosition);
            }
        }
        else
        {
            LinkedUnit.transform.LookAt(targetChest.transform);
            if (!LinkedUnit.IsMoving() && !targetChest.IsLooted())
            {
                targetChest.Interact(LinkedUnit);
            }
        }
    }

    private void ResetActions()
    {
        if (!LinkedUnit) return;

        if (!LinkedUnit.actionHandler.CheckForDanger())
        {
            LinkedUnit.StopAttacking();
            LinkedUnit.actionHandler.SetWanderingAction();
        }
    }

    public override void Init()
    {

    }

    public override void Remove()
    {

    }

    public override string GetMessage()
    {
        return "is looting";
    }
}
