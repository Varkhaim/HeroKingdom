using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingUIManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject UnitBar;
    public GameObject CombatTextPrefab;
    public GameObject GoldTextPrefab;

    [Header("Refs")]
    public Transform UnitBarsParent;
    public Transform CombatTextParent;

    [Header("Params")]
    public Color GoldColor;
    public AudioClip GoldLootClip;

    public UnitBar GenerateUnitBar()
    {
        GameObject generatedUnitBar = Instantiate(UnitBar, UnitBarsParent);
        return generatedUnitBar.GetComponent<UnitBar>();
    }

    public void SpawnCombatText(string value, Vector3 position, bool isAlly)
    {
        //Vector3 screenPos = Camera.main.WorldToScreenPoint(position);
        CombatText cbText = Instantiate(CombatTextPrefab, position, Quaternion.identity, CombatTextParent).GetComponent<CombatText>();

        cbText.SetText(value, isAlly);
    }

    internal void SpawnGoldText(string value, Vector3 position)
    {
        CombatText cbText = Instantiate(GoldTextPrefab, position, Quaternion.identity, CombatTextParent).GetComponent<CombatText>();

        cbText.SetText(value, GoldColor);
    }
}
