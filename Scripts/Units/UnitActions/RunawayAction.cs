using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunawayAction : UnitAction
{
    private HKBuilding shelter;

    public RunawayAction(HKUnit linkedUnit, HKBuilding shelter) : base(linkedUnit)
    {
        this.shelter = shelter;
        LinkedUnit.transform.LookAt(shelter.transform);
    }

    public override void Handle()
    {
        //Vector3 unitPosition = LinkedUnit.transform.position;
        if (IsWithinRange())
        {
            if (LinkedUnit.IsMoving())
            {
                LinkedUnit.transform.LookAt(shelter.transform);
            }
            else
            {
                LinkedUnit.EnterBuilding(shelter);
            }
        }
        else
        {
            CloseTheGap();
        }
    }

    private void CloseTheGap()
    {
        if (!LinkedUnit.IsMoving())
        {
            Vector3 targetPosition = shelter.GetClosestPoint(LinkedUnit.transform.position, LinkedUnit.AttackRange);
            LinkedUnit.MoveToPosition(targetPosition);
        }
    }

    private bool IsWithinRange()
    {
        float distance = Vector3.Distance(LinkedUnit.transform.position, shelter.transform.position);
        float requiredDistance = LinkedUnit.AttackRange + shelter.GetHitboxRadius() + 0.1f;
        return distance <= requiredDistance;
    }

    public override void Init()
    {

    }

    public override void Remove()
    {

    }

    public override string GetMessage()
    {
        if (LinkedUnit.IsVanish)
            return "is resting";
        else
            return "is running away";
    }
}
