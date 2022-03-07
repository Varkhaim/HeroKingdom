using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class ActionPool
{
    //[System.Serializable]
    //public class ActionChance
    //{
    //    [SerializeField] public UnitActionType unitAction;
    //    [SerializeField] public int frequency;

    //    private int rangeMin = 0;
    //    private int rangeMax = 0;

    //    public ActionChance(UnitActionType unitAction, int frequency)
    //    {
    //        this.unitAction = unitAction;
    //        this.frequency = frequency;
    //    }

    //    public bool IsWithinRange(int val)
    //    {
    //        return (val < rangeMax && val >= rangeMin);
    //    }

    //    public void SetRange(int min, int max)
    //    {
    //        rangeMin = min;
    //        rangeMax = max;
    //    }
    //}

    //private int total = 0;
    //public List<ActionChance> actionChances;

    //private int noWanderTotal = 0;
    //private List<ActionChance> noWanderActionChances = new List<ActionChance>();


    //public void Init()
    //{
    //    total = 0;
    //    noWanderTotal = 0;

    //    foreach (var ac in actionChances)
    //    {
    //        if (ac.unitAction == UnitActionType.WANDERING) continue;
    //        ActionChance item = new ActionChance(ac.unitAction, ac.frequency);
    //        item.SetRange(noWanderTotal, noWanderTotal + ac.frequency);
    //        noWanderActionChances.Add(item);
    //        noWanderTotal += ac.frequency;
    //    }

    //    actionChances.Add(new ActionChance(UnitActionType.WANDERING, 30));
    //    foreach (var ac in actionChances)
    //    {
    //        ac.SetRange(total, total + ac.frequency);
    //        total += ac.frequency;
    //    }
    //}

    //public UnitActionType GetRandomAction()
    //{
    //    int random = Random.Range(0, total);

    //    ActionChance actionChance;

    //    actionChance = actionChances.First(x => x.IsWithinRange(random));

    //    return actionChance.unitAction;
    //}

    //public UnitActionType GetMeaningfulAction()
    //{
    //    if (noWanderActionChances.Count == 0) return UnitActionType.WANDERING;

    //    int random = Random.Range(0, noWanderTotal);

    //    ActionChance actionChance;

    //    actionChance = noWanderActionChances.First(x => x.IsWithinRange(random));

    //    return actionChance.unitAction;
    //}
}
