using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHandler
{
    private UnitAction CurrentAction;
    private HKUnit unit;
    private UnitFinder unitFinder;
    public bool recentlyWandered = false;
    private List<UnitActionType> PossibleActions;

    public ActionHandler(HKUnit unit)
    {
        this.unit = unit;
        Start();
    }

    public void Start()
    {
        PossibleActions = new List<UnitActionType>();
        unitFinder = GameCore.GetUnitFinder();
        unitFinder.AddEntity(unit);
        RefreshPossibleActionsPool();
        if (unit.unitType != UnitType.BUILDING)
            SetWanderingAction();
    }

    public void Update()
    {
        if (CurrentAction != null)
            HandleCurrentAction();
    }

    public void SetAction(UnitActionType actionType)
    {
        switch (actionType)
        {
            case UnitActionType.WANDERING:
                SetWanderingAction();
                break;
            case UnitActionType.HUNTING:
                SetHuntingAction();
                break;
            case UnitActionType.RAIDING:
                SetRaidingAction();
                break;
            case UnitActionType.TREASURE_SEEKING:
                SetTreasureSeekingAction();
                break;
            case UnitActionType.EXPLORING:
                SetExploringAction();
                break;
            case UnitActionType.RUNNINGAWAY:
                SetRunningAwayAction();
                break;
        }
    }

    private void SetExploringAction()
    {
        if (CurrentAction != null)
            CurrentAction.Remove();

        CurrentAction = new ExploreAction(unit, GameCore.Instance.MainTerrain);
    }

    private void SetRunningAwayAction()
    {
        if (CurrentAction != null)
            CurrentAction.Remove();

        bool shelterExists = CheckRunawayAction(unitFinder.GetAllShelters());
        if (shelterExists)
        {
            HKBuilding closestShelter = unitFinder.GetClosestShelter(unit);
            CurrentAction = new RunawayAction(unit, closestShelter);
        }
        else
        {
            // BERSERK ACTION
        }
    }

    private bool CheckRunawayAction(List<HKBuilding> buildings)
    {
        if (buildings.Count == 0) return false;
        //if (buildings.Exists(x => unit.StaminaCheck(x))) return true;
        return false;
    }

    private void RefreshPossibleActionsPool()
    {
        if (unit.unitType == UnitType.BUILDING) return;
        PossibleActions.Clear();

        PossibleActions.Add(UnitActionType.WANDERING);
        PossibleActions.Add(UnitActionType.EXPLORING);

        if (unit.unitAllegiance == UnitAllegiance.ALLY && CheckRaidingAction(unitFinder.GetAllEnemyBuildings()))
            PossibleActions.Add(UnitActionType.RAIDING);
        if (unit.unitAllegiance == UnitAllegiance.ALLY && CheckHuntingAction(unitFinder.GetAllEnemyUnits()))
            PossibleActions.Add(UnitActionType.HUNTING);
    }

    private bool CheckRaidingAction(List<HKBuilding> buildings)
    {
        if (buildings.Count == 0) return false;
        if (buildings.Exists(x => unit.StaminaCheck(x))) return true;
        return false;
    }

    private bool CheckHuntingAction(List<HKUnit> units)
    {
        if (units.Count == 0) return false;
        if (units.Exists(x => unit.StaminaCheck(x))) return true;
        return false;
    }

    public void ActionCheck()
    {
        if (unit.isAttacking) return;

        if (PossibleActions.Count > 0)
        {
            int randomAction = UnityEngine.Random.Range(0, PossibleActions.Count);
            SetAction(PossibleActions[randomAction]);
        }
    }

    public bool TreasureCheck()
    {
        if (unit.isLooting || unit.isAttacking) return false;

        TreasureChest chestFound = unitFinder.FindClosestChest(unit, unit.SightDistance);
        if (!chestFound) return false;
        if (chestFound.IsLooted()) return false;
        SetLootingAction(chestFound);
        unit.isLooting = true;
        return true;
    }

    public string GetActionText()
    {
        if (CurrentAction == null)
            return "";
        return CurrentAction.GetMessage();
    }

    public void SetWanderingAction()
    {
        if (CurrentAction != null)
            CurrentAction.Remove();
        CurrentAction = new WanderingAction(unit, 3);
        recentlyWandered = true;
    }

    public void SetHuntingAction()
    {
        HKUnit unitToHunt = HuntingAction.FindUnitToHunt(unitFinder, unit);
        if (!unitToHunt)
        {
            SetWanderingAction();
            return;
        }

        if (CurrentAction != null)
            CurrentAction.Remove();

        CurrentAction = new HuntingAction(unit, unitToHunt);
    }

    public void SetTreasureSeekingAction()
    {
        TreasureChest spottedTreasure = TreasureSeekingAction.FindTresure(unitFinder, unit);
        if (!spottedTreasure)
        {
            SetWanderingAction();
            return;
        }

        if (CurrentAction != null)
            CurrentAction.Remove();

        CurrentAction = new TreasureSeekingAction(unit, spottedTreasure);
    }

    public void SetRaidingAction()
    {
        HKUnit unitToHunt = RaidingAction.FindUnitToHunt(unitFinder, unit);
        if (!unitToHunt)
        {
            SetWanderingAction();
            return;
        }

        if (CurrentAction != null)
            CurrentAction.Remove();

        CurrentAction = new RaidingAction(unit, unitToHunt);
    }

    public void SetAttackAction(HKUnit target)
    {
        if (CurrentAction != null)
            CurrentAction.Remove();
        unit.agent.ResetPath();
        CurrentAction = new AttackAction(unit, target);
    }

    public void SetLootingAction(TreasureChest chest)
    {
        if (CurrentAction != null)
            CurrentAction.Remove();
        unit.agent.ResetPath();
        CurrentAction = new LootAction(unit, chest);
    }

    public bool CheckForDanger()
    {
        HKUnit enemyFound = unitFinder.FindClosestEnemy(unit, unit.SightDistance, UnitType.ANYTHING);
        if (!enemyFound) return false;
        SetAttackAction(enemyFound);
        unit.isAttacking = true;
        return true;
    }

    private void HandleCurrentAction()
    {
        if (unit.unitType != UnitType.BUILDING)
            CurrentAction.Handle();
    }

    internal void ResetActions()
    {
        if (CurrentAction != null)
        {
            CurrentAction.Remove();
            CurrentAction = null;
        }
        ActionCheck();
    }

    internal void RemoveUnit(HKUnit toRemove)
    {
        unitFinder.RemoveEntity(toRemove);
    }
}
