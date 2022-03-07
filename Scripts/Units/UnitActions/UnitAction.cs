using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitAction
{
    public HKUnit LinkedUnit;

    protected UnitAction(HKUnit linkedUnit)
    {
        LinkedUnit = linkedUnit;
    }

    public abstract void Init();

    public abstract void Handle();

    public abstract void Remove();

    public abstract string GetMessage();
}
