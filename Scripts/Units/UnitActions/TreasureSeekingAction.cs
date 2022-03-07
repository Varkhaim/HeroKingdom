using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TreasureSeekingAction : UnitAction
{
    private TreasureChest SpottedChest = null;
    private UnitFinder unitFinder;

    private float refreshCd = 0f;

    public TreasureSeekingAction(HKUnit linkedUnit, TreasureChest spottedChest) : base(linkedUnit)
    {
        unitFinder = GameCore.GetUnitFinder();
        this.SpottedChest = spottedChest;
    }

    public override string GetMessage()
    {
        return "is looking for treasure";
    }

    public override void Handle()
    {
        if (!SpottedChest)
        {
            SpottedChest = unitFinder.FindClosestChest(LinkedUnit, LinkedUnit.SightDistance * 4f);
            if (!SpottedChest)
            {
                LinkedUnit.actionHandler.SetWanderingAction();
            }
        }
        else
        {
            refreshCd -= Time.deltaTime;
            if (refreshCd < 0)
            {
                if (SpottedChest.IsLooted())
                {
                    LinkedUnit.actionHandler.ResetActions();
                    return;
                }                
                if (LinkedUnit.actionHandler.CheckForDanger()) return;
                if (LinkedUnit.actionHandler.TreasureCheck()) return;
                float range = 1f;
                Vector3 unitPosition = LinkedUnit.transform.position;
                Vector3 targetPosition = SpottedChest.GetClosestPoint(LinkedUnit.transform.position, range);
                float currentDistance = Vector3.Distance(unitPosition, targetPosition);
                if (currentDistance > range && !LinkedUnit.IsMoving())
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

    public static TreasureChest FindTresure(UnitFinder unitFinder, HKUnit unit)
    {
        TreasureChest foundChest = unitFinder.FindClosestChest(unit, unit.SightDistance * 4f);
        return foundChest;
    }
}
