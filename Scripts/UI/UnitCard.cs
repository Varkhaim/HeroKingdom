using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitCard : MonoBehaviour
{
    private HKUnit linkedUnit;
    private UnitSelectionManager selectionManager;
    public Button button;

    public void CenterOnUnit()
    {
        selectionManager.CenterCameraOnUnit(linkedUnit);
    }

    public void Init(HKUnit unit, UnitSelectionManager selectionManager)
    {
        this.linkedUnit = unit;
        this.selectionManager = selectionManager;
        button.onClick.AddListener(CenterOnUnit);
    }
}
