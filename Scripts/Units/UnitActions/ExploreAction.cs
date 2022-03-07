using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ExploreAction : UnitAction
{
    private float priorityCheckTimer = 1f;
    private Vector3 targetPlace = new Vector3();

    public ExploreAction(HKUnit linkedUnit, Terrain terrain) : base(linkedUnit)
    {
        float MinX = terrain.transform.position.x;
        float MaxX = terrain.transform.position.x + terrain.terrainData.size.x;
        float randX = UnityEngine.Random.Range(MinX, MaxX);

        float MinZ = terrain.transform.position.z;
        float MaxZ = terrain.transform.position.z + terrain.terrainData.size.z;
        float randZ = UnityEngine.Random.Range(MinZ, MaxZ);

        targetPlace = new Vector3(randX, 1f, randZ);
        float posY = terrain.SampleHeight(targetPlace);
        targetPlace.y = posY;

        LinkedUnit.MoveToPosition(targetPlace);
    }

    public override string GetMessage()
    {
        return "is exploring";
    }

    public override void Handle()
    {
        if (!LinkedUnit.IsMoving())
        {
            LinkedUnit.actionHandler.ActionCheck();
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

    public override void Init()
    {

    }

    public override void Remove()
    {
        LinkedUnit.actionHandler.recentlyWandered = false;
    }


}
