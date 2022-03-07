using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public abstract class HKEntity : MonoBehaviour
{
    [Header("General")]
    public string Name;
    public string UnitClass;
    public int Level = 1;
    public Sprite Portrait;
    public UnitType unitType;
    public UnitAllegiance unitAllegiance;
    public float DeathTime = 10.0f;
    public List<GameObject> VanishHiders;

    [Header("Health")]
    public float BaseMaxHealth = 100f;
    public float HealthPerLevel = 10f;
    public float MaxHealth = 100f;

    [Header("Regeneration")]
    public float TimeBetweenRegenTicks = 5f;
    public float HealthPerTick = 3f;

    [Header("Flags")]
    public bool IsDead = false;
    public bool IsVanish = false;

    // PROTECTED
    protected float currentTickTimer = 0f;
    protected float CurrentHealth = 100f;
    protected float deathTimer = 0.0f;
    protected int heldGold = 0;
    protected float vanishTimer = 0f;
    protected UnitBar unitBar;
    protected FloatingUIManager floatingUIManager;
    protected List<HKUnit> attackers = new List<HKUnit>();
    protected SpellDatabase spellDatabase;
    protected UnitSelectionManager selectManager;
    protected KingdomManager kingdomManager;
    protected List<MeshFilter> meshFilters = new List<MeshFilter>();
    protected List<Material> materials = new List<Material>();
    protected List<Renderer> renderers = new List<Renderer>();

    // ABSTRACTS
    protected abstract void RecalculateMaxHealth();
    protected abstract void UpdateAlive();
    protected abstract void HandleDeath();
    protected abstract void InitEntity();
    public abstract float GetHitboxRadius();

    /// EVENTS
    protected UnityEvent OnDamageTaken = new UnityEvent();
    protected UnityEvent OnInventoryChange = new UnityEvent();
    protected HKEntityEvent OnNewAttacker = new HKEntityEvent();
    [HideInInspector] public UnityEvent OnDeath = new UnityEvent();

    private void Start()
    {
        SetUpReferences();
        InitEntity();
        RecalculateMaxHealth();
        GenerateUnitBar();
    }

    private void SetUpReferences()
    {
        floatingUIManager = GameCore.GetFloatingUIManager();
        spellDatabase = GameCore.GetSpellDatabase();
        selectManager = GameCore.GetSelectionManager();
        kingdomManager = GameCore.GetKingdomManager();
        renderers = GetComponentsInChildren<Renderer>().ToList();
        meshFilters = GetComponentsInChildren<MeshFilter>().ToList();
    }

    private void GenerateUnitBar()
    {
        unitBar = floatingUIManager.GenerateUnitBar();
        unitBar.SetText(string.Format("{0} {1}", Level, Name));
        //unitBar.SetExpBar(experience / requiredExperience);
        unitBar.gameObject.SetActive(false);
        OnDamageTaken.AddListener(() => unitBar.SetHealthLevel(CurrentHealth / MaxHealth));
    }

    private void HandleVanish()
    {
        if (!IsVanish) return;

            vanishTimer -= Time.deltaTime;
            if (vanishTimer <= 0)
            {
                IsVanish = false;
            }
    }

    private void Update()
    {
        HandleVanish();
        if (!IsDead && !IsVanish)
        {
            UpdateAlive();
        }
        else
        {
            HandleDeath();
        }
    }

    public float GetPowerPercentage()
    {
        return 0f;
    }

    public float GetHealthPercentage()
    {
        return CurrentHealth / MaxHealth;
    }

    public string GetHealthText()
    {
        return string.Format("{0}/{1}", (int)CurrentHealth, (int)MaxHealth);
    }

    public string GetPowerText()
    {
        return string.Format("{0}/{1}", 0, 100);
    }

    public void TakeDamage(float val, HKUnit source)
    {
        if (IsDead || IsVanish) return;
        CurrentHealth -= val;
        SpawnCombatText(val); 

        if (!attackers.Contains(source))
        {
            AddAttacker(source);
            OnNewAttacker.Invoke(source);
        }

        CheckHealth();
        OnDamageTaken.Invoke();
    }

    private void SpawnCombatText(float val)
    {
        Vector3 position = CalculateUnitTop(true);
        floatingUIManager.SpawnCombatText(((int)val).ToString(), position, unitAllegiance == UnitAllegiance.ALLY);
    }

    public Vector3 CalculateUnitTop(bool randomize)
    {
        Bounds bounds = meshFilters[0].mesh.bounds;
        foreach (var mf in meshFilters)
        {
            bounds.Encapsulate(mf.mesh.bounds);
        }
        Vector3 position = transform.position;
        position.y = bounds.max.y;
        if (randomize)
        {
            position.x += UnityEngine.Random.Range(-2f, 2f);
            position.z += UnityEngine.Random.Range(-2f, 2f);
        }
        return position;
    }

    public void Vanish(float timeVanished)
    {
        IsVanish = true;
        vanishTimer = timeVanished;
        HideEntity();
    }

    public void Reappear()
    {
        IsVanish = false;
        vanishTimer = 0f;
        ShowEntity();
    }

    protected void HideEntity()
    {
        VanishHiders.ForEach(x => x.SetActive(false));
    }
    protected void ShowEntity()
    {
        VanishHiders.ForEach(x => x.SetActive(true));
    }

    protected void AddAttacker(HKUnit source)
    {
        source.OnDeath.AddListener(() => attackers.Remove(source));
        attackers.Add(source);
    }

    private void KillUnit()
    {
        if (IsDead) return;

        Destroy(unitBar.gameObject);
        IsDead = true;
        deathTimer = DeathTime;

        OnDeath.Invoke();
        //Destroy(gameObject);
    }

    private void CheckHealth()
    {
        if (CurrentHealth <= 0)
            KillUnit();
        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;
    }
    protected void RefillHealth()
    {
        CurrentHealth = MaxHealth;
    }

    public Vector3 GetClosestPoint(Vector3 pos, float attackRange)
    {
        float unitRadius = GetHitboxRadius();
        Vector3 offset = (pos - transform.position).normalized * (unitRadius + attackRange);
        Vector3 finalPosition = transform.position + offset;
        if (NavMesh.SamplePosition(finalPosition, out NavMeshHit hit, 10f, 0))
            finalPosition = hit.position;
        return finalPosition;
    }
}
