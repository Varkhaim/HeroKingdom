using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventReceiver : MonoBehaviour
{
    public HKUnit LinkedUnit;

    public TreasureChest LinkedChest;

    public void Step()
    {
        LinkedUnit.PlayStepSound();
    }

    public void Swing()
    {
        LinkedUnit.CastSpell();
    }

    public void Death()
    {
        LinkedUnit.DestroyUnit();
    }

    public void OpenChest()
    {
        LinkedChest.LootContent();
    }
}
