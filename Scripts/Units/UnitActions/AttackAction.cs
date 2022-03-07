using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackAction : UnitAction
{
    private HKUnit targetUnit;
    UnityEngine.Events.UnityAction resetActions;

    public AttackAction(HKUnit linkedUnit, HKUnit targetUnit) : base(linkedUnit)
    {
        this.targetUnit = targetUnit;
        resetActions = ResetActions;
        targetUnit.OnDeath.AddListener(resetActions);
        LinkedUnit.transform.LookAt(targetUnit.transform);
    }

    public override void Handle()
    {
        //Vector3 unitPosition = LinkedUnit.transform.position;
        if (IsWithinRange())
        {
            if (LinkedUnit.IsMoving())
            {
                LinkedUnit.transform.LookAt(targetUnit.transform);
                LinkedUnit.StartAttacking();
            }
            else
            {
                HandleSwinging();
            }
        }
        else
        {
            CloseTheGap();
        }
    }

    private void HandleSwinging()
    {
        if (LinkedUnit.swingTimer <= 0)
        {
            LinkedUnit.transform.LookAt(targetUnit.transform);
            LinkedUnit.UseFillerSpell(targetUnit);
            LinkedUnit.ReloadSwingTimer();
        }
    }

    private void CloseTheGap()
    {
        if (!LinkedUnit.IsMoving())
        {
            Vector3 targetPosition = targetUnit.GetClosestPoint(LinkedUnit.transform.position, LinkedUnit.AttackRange);
            LinkedUnit.MoveToPosition(targetPosition);
        }
    }

    private bool IsWithinRange()
    {
        float distance = Vector3.Distance(LinkedUnit.transform.position, targetUnit.transform.position);
        float requiredDistance = LinkedUnit.AttackRange + targetUnit.GetHitboxRadius() + 0.1f;
        return distance <= requiredDistance;
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
        targetUnit.OnDeath.RemoveListener(resetActions);
    }

    public override string GetMessage()
    {
        return "is fighting";
    }
}
