using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFinder : MonoBehaviour
{
    // ### UNITS ###
    private List<HKEntity> entities = new List<HKEntity>();
    private List<HKBuilding> enemyBuildings = new List<HKBuilding>();
    private List<HKUnit> enemyUnits = new List<HKUnit>();
    private List<HKBuilding> alliedShelters = new List<HKBuilding>();

    public void AddEntity(HKEntity entity)
    {
        entities.Add(entity);
        if (entity is HKBuilding)
        {
            HKBuilding building = entity as HKBuilding;
            if (building.unitAllegiance == UnitAllegiance.ENEMY)
                enemyBuildings.Add(building);
            if (building.unitAllegiance == UnitAllegiance.ALLY)
            {
                alliedShelters.Add(building);
            }
        }
        if (entity is HKUnit)
        {
            HKUnit unit = entity as HKUnit;
            if (unit.unitAllegiance == UnitAllegiance.ENEMY)
                enemyUnits.Add(unit);
        }
    }

    public void RemoveEntity(HKEntity entity)
    {
        entities.Remove(entity);
        if (entity is HKBuilding)
        {
            HKBuilding building = entity as HKBuilding;
            if (building.unitAllegiance == UnitAllegiance.ENEMY)
                enemyBuildings.Remove(building);
            if (building.unitAllegiance == UnitAllegiance.ALLY)
            {
                alliedShelters.Remove(building);
            }
        }
        if (entity is HKUnit)
        {
            HKUnit unit = entity as HKUnit;
            if (unit.unitAllegiance == UnitAllegiance.ENEMY)
                enemyUnits.Remove(unit);
        }
    }


    public List<HKBuilding> GetAllShelters()
    {
        return alliedShelters;
    }

    public List<HKBuilding> GetAllEnemyBuildings()
    {
        return enemyBuildings;
    }


    public List<HKUnit> GetAllEnemyUnits()
    {
        return enemyUnits;
    }
    public List<HKUnit> FindWithinRange(HKUnit origin, float range)
    {
        List<HKUnit> result = new List<HKUnit>();

        foreach (HKUnit currentUnit in entities)
        {
            float distanceSqr = (origin.transform.position - currentUnit.transform.position).sqrMagnitude;
            if (distanceSqr < range * range)
                result.Add(currentUnit);
        }

        result.Remove(origin);

        return result;
    }

    public HKUnit FindEnemy(HKUnit origin, float range)
    {
        UnitAllegiance enemyAllegiance;

        if (origin.unitAllegiance == UnitAllegiance.ALLY)
            enemyAllegiance = UnitAllegiance.ENEMY;
        else
            enemyAllegiance = UnitAllegiance.ALLY;

        List<HKUnit> hKUnits = FindWithinRange(origin, range);

        if (hKUnits.Count == 0) return null;

        HKUnit enemy = hKUnits.Find(x => x.unitAllegiance == enemyAllegiance);

        return enemy;
    }

    public HKUnit FindClosestEnemy(HKUnit origin, float range, UnitType unitType)
    {
        UnitAllegiance enemyAllegiance;

        if (origin.unitAllegiance == UnitAllegiance.ALLY)
            enemyAllegiance = UnitAllegiance.ENEMY;
        else
            enemyAllegiance = UnitAllegiance.ALLY;

        List<HKUnit> hKUnits = FindWithinRange(origin, range);

        if (hKUnits.Count == 0) return null;

        List<HKUnit> enemies = hKUnits.FindAll(x => x.unitAllegiance == enemyAllegiance);

        float dist = range;
        HKUnit enemy = null;
        foreach (var current in enemies)
        {
            if (unitType != UnitType.ANYTHING && current.unitType != unitType) continue;
            float curDist = Vector3.Distance(current.transform.position, origin.transform.position);
            if (curDist < dist)
            {
                dist = curDist;
                enemy = current;
            }
        }
        return enemy;
    }

    public HKBuilding GetClosestBuildingFromPool(List<HKBuilding> buildings, HKUnit origin)
    {
        if (buildings.Count == 0) return null;

        float dist = 200f;
        HKBuilding closest = null;
        foreach (var current in buildings)
        {
            float curDist = Vector3.Distance(current.transform.position, origin.transform.position);
            if (curDist < dist)
            {
                dist = curDist;
                closest = current;
            }
        }
        return closest;
    }

    public HKBuilding GetClosestShelter(HKUnit origin)
    {
        if (alliedShelters.Count == 0) return null;

        float dist = 200f;
        HKBuilding closest = null;
        foreach (var current in alliedShelters)
        {
            float curDist = Vector3.Distance(current.transform.position, origin.transform.position);
            if (curDist < dist)
            {
                dist = curDist;
                closest = current;
            }
        }
        return closest;
    }

    // ### TREASURE ###
    private List<TreasureChest> treasureChests = new List<TreasureChest>();

    public void AddTreasure(TreasureChest chest)
    {
        treasureChests.Add(chest);
    }

    public void RemoveTreasure(TreasureChest chest)
    {
        treasureChests.Remove(chest);
    }

    public List<TreasureChest> FindTreasureWithinRange(HKUnit origin, float range)
    {
        List<TreasureChest> result = new List<TreasureChest>();

        foreach (TreasureChest currentChest in treasureChests)
        {
            float distanceSqr = (origin.transform.position - currentChest.transform.position).sqrMagnitude;
            if (distanceSqr < range * range)
                result.Add(currentChest);
        }

        return result;
    }

    public TreasureChest FindClosestChest(HKUnit origin, float range)
    {
        List<TreasureChest> hKUnits = FindTreasureWithinRange(origin, range);

        if (hKUnits.Count == 0) return null;

        float dist = 100f;
        TreasureChest chest = null;
        foreach (var current in treasureChests)
        {
            if (current.IsLooted()) continue;
            float curDist = Vector3.Distance(current.transform.position, origin.transform.position);
            if (curDist < dist)
            {
                dist = curDist;
                chest = current;
            }
        }
        return chest;
    }
}
