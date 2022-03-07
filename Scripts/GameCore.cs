using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class GameCore : MonoBehaviour
{
    private static GameCore _instance;

    public static GameCore Instance { get { return _instance; } }

    public FloatingUIManager floatingUIManager;
    public UnitFinder unitFinder;
    public SpellDatabase spellDatabase;
    public UnitSelectionManager selectManager;
    public Transform MissileParent;
    public KingdomManager kingdomManager;
    public ItemManager itemManager;
    public TooltipManager tooltipManager;
    public Terrain MainTerrain;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public static KingdomManager GetKingdomManager()
    {
        return Instance.kingdomManager;
    }

    public static ItemManager GetItemManager()
    {
        return Instance.itemManager;
    }

    public static TooltipManager GetTooltipManager()
    {
        return Instance.tooltipManager;
    }

    public static FloatingUIManager GetFloatingUIManager()
    {
        return Instance.floatingUIManager;
    }

    public static Transform GetMissileParent()
    {
        return Instance.MissileParent;
    }

    public static UnitFinder GetUnitFinder()
    {
        return Instance.unitFinder;
    }

    internal static SpellDatabase GetSpellDatabase()
    {
        return Instance.spellDatabase;
    }

    internal static UnitSelectionManager GetSelectionManager()
    {
        return Instance.selectManager;
    }
}
