using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KingdomManager : MonoBehaviour
{
    public Image RecruitmentBar;
    public TMPro.TextMeshProUGUI ProgressText;
    public TMPro.TextMeshProUGUI RecruitsText;
    public Transform SpawnPoint;
    public List<Button> RecruitmentButtons;
    public List<TMPro.TextMeshProUGUI> RecruitmentNames;
    public List<TMPro.TextMeshProUGUI> RecruitmentTraits;
    public List<string> NamesPool;
    public GameObject HeroPrefab;

    [Header("Bottom Panel")]
    public Transform BottomPanel;
    public GameObject UnitCardPrefab;

    private float recruitmentProgress;
    public float BaseRecruitmentTime;
    private int AvailableRecruits;
    private int RecruitedHeroes;
    private UnitSelectionManager selectionManager;

    private List<HeroData> heroDatas = new List<HeroData>();

    public List<HKUnit> DiscoveredEnemyBuildings;
    public List<HKUnit> DiscoveredEnemyUnits;

    private void Awake()
    {
        selectionManager = GameCore.GetSelectionManager();
        DiscoveredEnemyBuildings = new List<HKUnit>();
        DiscoveredEnemyUnits = new List<HKUnit>();
    }

    private void Start()
    {
        ShuffleHeroData();
        RefreshButtons();
    }

    private void Update()
    {
        HandleRecruitmentBar();
    }


    public void AddEnemy(HKUnit unit)
    {
        if (unit.unitAllegiance != UnitAllegiance.ENEMY) return;

        if (unit.unitType == UnitType.UNIT)
            DiscoveredEnemyUnits.Add(unit);
        if (unit.unitType == UnitType.BUILDING)
            DiscoveredEnemyBuildings.Add(unit);
    }

    public void RemoveEnemy(HKUnit unit)
    {
        if (unit.unitAllegiance != UnitAllegiance.ENEMY) return;

        if (unit.unitType == UnitType.UNIT)
            DiscoveredEnemyUnits.Remove(unit);
        if (unit.unitType == UnitType.BUILDING)
            DiscoveredEnemyBuildings.Remove(unit);

    }

    public GameObject GenerateUnitCard(HKUnit unit)
    {
        GameObject unitCardGameObject = Instantiate(UnitCardPrefab, BottomPanel);
        UnitCard unitCard = unitCardGameObject.GetComponent<UnitCard>();
        unitCard.Init(unit, selectionManager);
        return unitCardGameObject;
    }

    private void RefreshUI()
    {
        float progress = recruitmentProgress / BaseRecruitmentTime;
        RecruitmentBar.fillAmount = progress;
        ProgressText.text = ((int)(progress * 100)).ToString() + "%";
        RecruitsText.text = AvailableRecruits.ToString();
    }

    private void RefreshButtons()
    {
        for (int i = 0; i < 3; i++)
        {
            bool available = (AvailableRecruits > 0);

            RecruitmentButtons[i].interactable = available;
            RecruitmentNames[i].enabled = available;
            RecruitmentTraits[i].enabled = available;

            RecruitmentNames[i].text = heroDatas[i].HeroName;
            RecruitmentTraits[i].text = heroDatas[i].GetTraits();
        }
    }

    private void HandleRecruitmentBar()
    {
        int totalHeroes = AvailableRecruits + RecruitedHeroes;
        recruitmentProgress += Time.deltaTime / (1f + totalHeroes * 0.2f);
        if (recruitmentProgress >= BaseRecruitmentTime)
        {
            recruitmentProgress = 0f;
            AvailableRecruits++;
            RefreshButtons();
        }
        RefreshUI();
    }

    public void RecruitHeroButtonClick(int index)
    {
        RecruitHero(heroDatas[index]);
    }

    private void RecruitHero(HeroData heroData)
    {
        GameObject newHero = Instantiate(HeroPrefab, SpawnPoint.position, Quaternion.identity);
        HKUnit hKUnit = newHero.GetComponent<HKUnit>();

        hKUnit.SetSpawnSickness();
        hKUnit.SetData(heroData);

        AvailableRecruits -= 1;
        RecruitedHeroes += 1;

        ShuffleHeroData();
    }

    private void ShuffleHeroData()
    {
        heroDatas.Clear();
        for (int i = 0; i < 3; i++)
        {
            heroDatas.Add(HeroData.Randomize(NamesPool));
        }
        RefreshButtons();
    }
}
