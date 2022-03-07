using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class UnitSelectionManager : MonoBehaviour
{
    public GameObject UnitPanel;

    public TMPro.TextMeshProUGUI UnitLevel;
    public TMPro.TextMeshProUGUI UnitName;
    public TMPro.TextMeshProUGUI UnitClass;
    public TMPro.TextMeshProUGUI UnitAction;

    public Image HealthBar;
    public Image ExpBar;
    public Image PowerBar;
    public Image Portrait;
    public GameObject LevelCircle;
    public CameraMovement cameraMovement;

    public TMPro.TextMeshProUGUI HealthBarText;
    public TMPro.TextMeshProUGUI ExpBarText;
    public TMPro.TextMeshProUGUI PowerBarText;

    [Header("Inventory Panel")]
    public TMPro.TextMeshProUGUI GoldText;

    public DecalProjector UnitSelection;

    public AudioSource audioSource;
    public AudioClip SelectionClip;

    [Header("Unit Type Panels")]
    public GameObject BuildingSubPanel;
    public GameObject UnitSubPanel;

    [Header("3D Refs")]
    public HKUnit Castle;

    [Header("Stats")]
    public List<StatSpritePair> statIcons;
    public List<StatBoxesRefs> statBoxesRefs;
    public StatBox BasicDamageBox;
    public StatBox AttackRangeBox;
    public StatBox MovementSpeedBox;
    public StatBox AttackSpeedBox;

    private HKEntity PreviousUnit;
    private HKEntity CurrentEntity;
    private ItemManager itemManager;
    List<MeshFilter> meshFilters = new List<MeshFilter>();

    [System.Serializable]
    public class StatSpritePair
    {
        public UnitStat stat;
        public Sprite icon;

        public StatSpritePair(UnitStat stat, Sprite icon)
        {
            this.stat = stat;
            this.icon = icon;
        }
    }

    [System.Serializable]
    public class StatBoxesRefs
    {
        public UnitStat stat;
        public StatBox statBox;

        public StatBoxesRefs(UnitStat stat, StatBox statBox)
        {
            this.stat = stat;
            this.statBox = statBox;
        }
    }

    public Sprite GetStatIcon(UnitStat stat)
    {
        StatSpritePair result = statIcons.Find(x => x.stat == stat);
        if (result != null)
            return result.icon;
        return null;
    }

    public void SetUpStatBoxes(HKUnit unit)
    {
        foreach (var statBoxRef in statBoxesRefs)
        {
            statBoxRef.statBox.SetStatBox(unit.GetTotalStat(statBoxRef.stat).ToString());
        }
        string basicDamageString = unit.GetBasicAttackString(); 
        BasicDamageBox.SetStatBox(basicDamageString);
        AttackRangeBox.SetStatBox(unit.AttackRange.ToString());
        MovementSpeedBox.SetStatBox(unit.MovementSpeed.ToString());
        AttackSpeedBox.SetStatBox(unit.GetAttackSpeed().ToString());
    }

    private void Awake()
    {
        itemManager = GameCore.GetItemManager();
    }

    public void SetupPanel(HKEntity unit)
    {
        CurrentEntity = unit;
        if (unit is HKUnit)
        {
            HKUnit myUnit = unit as HKUnit;
            SwitchPanels(unit.unitType);
            itemManager.SetInventoryUI(myUnit);
            itemManager.SetEquipmentUI(myUnit);
            SetUpStatBoxes(myUnit);
        }
    }

    public void RefreshInventoryUI(HKUnit unit)
    {
        if (CurrentEntity != unit) return;
        itemManager.SetInventoryUI(unit);
        itemManager.SetEquipmentUI(unit);
        SetUpStatBoxes(unit);
    }

    public void RefreshEquipmentUI(HKUnit unit)
    {
        if (CurrentEntity != unit) return;
        itemManager.SetEquipmentUI(unit); 
        SetUpStatBoxes(unit);
    }

    private void SwitchPanels(UnitType unitType)
    {
        BuildingSubPanel.SetActive(unitType == UnitType.BUILDING);
        UnitSubPanel.SetActive(unitType == UnitType.UNIT);
    }

    public void CenterCameraOnCurrentUnit()
    {
        cameraMovement.MoveToUnit(CurrentEntity.transform.position);
    }

    public void CenterCameraOnUnit(HKUnit unit)
    {
        cameraMovement.MoveToUnit(unit.transform.position);
    }

    public void CenterOnCastle()
    {
        cameraMovement.MoveToUnit(Castle.transform.position);
    }

    private void Update()
    {
        if (!CurrentEntity)
        {
            if (UnitPanel.activeSelf)
                UnitPanel.SetActive(false);
            UnitSelection.gameObject.SetActive(false);
            return;
        }
        if (!UnitPanel.activeSelf)
        {
            UnitPanel.SetActive(true);
            UnitSelection.gameObject.SetActive(true);
        }

        if (PreviousUnit != CurrentEntity)
        {
            SetupSelection();
        }
        PreviousUnit = CurrentEntity;

        SetPosition();
        UnitName.text = CurrentEntity.Name;
        switch (CurrentEntity.unitAllegiance)
        {
            case UnitAllegiance.ALLY:
                UnitName.color = Color.green;
                break;
            case UnitAllegiance.NEUTRAL:
                UnitName.color = Color.white;
                break;
            case UnitAllegiance.ENEMY:
                UnitName.color = Color.red;
                break;
        }
        UnitClass.text = CurrentEntity.UnitClass;

        if (CurrentEntity is HKBuilding)
        {
            SetupBuildingUI();
        }
        else
        if (CurrentEntity is HKUnit)
        {
            SetupUnitUI();
        }

        HealthBar.fillAmount = CurrentEntity.GetHealthPercentage();
        PowerBar.fillAmount = CurrentEntity.GetPowerPercentage();

        HealthBarText.text = CurrentEntity.GetHealthText();
        PowerBarText.text = CurrentEntity.GetPowerText();

        Portrait.sprite = CurrentEntity.Portrait;
    }

    private void SetupUnitUI()
    {
        HKUnit unit = CurrentEntity as HKUnit;

        UnitAction.text = unit.actionHandler.GetActionText();

        ExpBar.gameObject.SetActive(true);
        ExpBar.fillAmount = unit.GetExperiencePercentage();

        ExpBarText.gameObject.SetActive(true);
        ExpBarText.text = unit.GetExperienceText();

        GoldText.gameObject.SetActive(true);
        GoldText.text = unit.GetHeldGold().ToString();

        LevelCircle.SetActive(true);
        UnitLevel.text = CurrentEntity.Level.ToString();
    }

    private void SetupBuildingUI()
    {
        UnitLevel.text = CurrentEntity.Level.ToString();
        HKBuilding building = CurrentEntity as HKBuilding;
        UnitAction.text = "";
        ExpBar.gameObject.SetActive(false);
        ExpBarText.gameObject.SetActive(false);
        LevelCircle.SetActive(false);
    }

    private void SetupSelection()
    {
        audioSource.PlayOneShot(SelectionClip);
        meshFilters = CurrentEntity.GetComponentsInChildren<MeshFilter>().ToList();
    }

    private void SetPosition()
    {
        Bounds bounds = meshFilters[0].mesh.bounds;
        foreach (var mf in meshFilters)
        {
            bounds.Encapsulate(mf.mesh.bounds);
        }
        Vector3 size = bounds.size;
        size.z = 3f;
        UnitSelection.size = size;
        Vector3 position = CurrentEntity.transform.position;
        position.y = bounds.min.y;
        UnitSelection.transform.position = position;
    }

    public void DeselectUnit()
    {
        CurrentEntity = null;
    }
}
