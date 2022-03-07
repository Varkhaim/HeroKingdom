using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HuntingAction : UnitAction
{
    private HKUnit HuntedEnemy = null;
    private UnitFinder unitFinder;

    private float refreshCd = 0f;

    public HuntingAction(HKUnit linkedUnit, HKUnit huntedEnemy) : base(linkedUnit)
    {
        unitFinder = GameCore.GetUnitFinder();
        this.HuntedEnemy = huntedEnemy;
    }

    public override string GetMessage()
    {
        return "is hunting";
    }

    public override void Handle()
    {
        if (!HuntedEnemy)
        {
            HuntedEnemy = unitFinder.FindClosestEnemy(LinkedUnit, LinkedUnit.SightDistance * 4f, UnitType.UNIT);
            if (!HuntedEnemy)
            {
                LinkedUnit.actionHandler.SetWanderingAction();
            }
        }
        else
        {
            refreshCd -= Time.deltaTime;
            if (refreshCd < 0)
            {
                if (HuntedEnemy.IsDead)
                {
                    LinkedUnit.actionHandler.ResetActions();
                    return;
                }
                if (LinkedUnit.actionHandler.CheckForDanger()) return;
                Vector3 unitPosition = LinkedUnit.transform.position;
                Vector3 targetPosition = HuntedEnemy.GetClosestPoint(LinkedUnit.transform.position, LinkedUnit.AttackRange);
                float totalRange = (LinkedUnit.AttackRange);
                float currentDistance = Vector3.Distance(unitPosition, targetPosition);
                if (currentDistance > totalRange)
                {
                    LinkedUnit.MoveToPosition(targetPosition);
                }
                refreshCd = 3f;
            }
        }
    }

    public override void Init()
    {

    }

    public override void Remove()
    {

    }

    public static HKUnit FindUnitToHunt(UnitFinder unitFinder, HKUnit unit)
    {
        HKUnit foundEnemy = unitFinder.FindClosestEnemy(unit, unit.SightDistance * 4f, UnitType.UNIT);
        return foundEnemy;
    }
}
