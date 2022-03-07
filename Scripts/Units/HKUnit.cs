using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class HKUnit : HKEntity
{
    [Header("Animation Triggers")]
    public string MOVE_TRIGGER;
    public string IDLE_TRIGGER;
    public string DEATH_TRIGGER;
    public string ATTACK_TRIGGER;

    [Header("Unit Info")]
    public int MovementSpeed = 10;

    [Header("Combat Stats")]
    public float AttackSpeed = 2.0f;
    public float AttackRange = 2.0f;
    public float Stamina = 100f;
    private float currentStamina = 100f;
    public Vector2 BaseAttackDamage;


    private Vector2 TotalAttackDamage;

    [Header("Distances")]
    public float minimumDistance = 1.0f;
    public float WanderDistance;
    public float WanderTime;
    public float SightDistance = 10.0f;

    [Header("Refs")]
    public NavMeshAgent agent;
    public Animator animator;
    public AudioSource audioSource;
    public Collider UnitCollider;
    public Transform MissileSpawnPoint;
    public Transform TargetObject;

    [Header("Audio")]
    public AudioClip StepClip;
    public AudioClip SwingClip;
    public AudioClip OnDeathClip;

    [Header("Attack")]
    public int FillerSpellID;
    public float swingTimer = 0.0f;

    [Header("Actions")]
    public UnitActionType StartingActionType = UnitActionType.WANDERING;

    private float experience = 0f;
    private float requiredExperience = 50f;

    private bool isMoving = false;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool isLooting = false;

    private float alphaLevel = 1f;
    private bool isFadingOut = false;

    private float spawnSicknessTimer = 0f;
    private bool isFadingIn = false;

    private GameObject unitCard;
    public ActionHandler actionHandler;

    private HKUnit CurrentTarget;
    public Inventory inventory;

    /// Stats
    private Dictionary<UnitStat, int> BaseUnitStats;
    private Dictionary<UnitStat, int> UnitStatBoosts;

    //private UnityEvent OnStatsChange = new UnityEvent();

    protected override void InitEntity()
    {
        if (!agent)
            agent = GetComponent<NavMeshAgent>();
        InitStats();
        InitActionHandling();
        ResetSwingTimer();
        GenerateUnitCard();
        RefillHealth();
        SetupMissileSpawnPoint();
        InitRendererList();
        InitStartingGold();
        InitInventory();
        SetUpStartingAction();
        SetupEvents();
    }

    internal void EnterBuilding(HKBuilding shelter)
    {
        shelter.AddGuest(this);
        Vanish(10f);
    }

    private void SetupEvents()
    {
        OnNewAttacker.AddListener(ReactToNewTarget);
        OnDeath.AddListener(ReactToDeath);
    }

    public string GetBasicAttackString()
    {
        return string.Format("{0}-{1}", TotalAttackDamage.x, TotalAttackDamage.y);
    }

    public float GetTotalStat(UnitStat stat)
    {
        return BaseUnitStats[stat] + UnitStatBoosts[stat];
    }

    private float GetCoeff(UnitStat stat, float coeff)
    {
        return 1f + (GetTotalStat(stat) * coeff);
    }

    public float GetAttackSpeed()
    {
        float attackspeed = AttackSpeed / GetCoeff(UnitStat.DEXTERITY, 0.1f);
        return Mathf.Round(attackspeed * 100f) / 100f;
    }

    private void InitStats()
    {
        BaseUnitStats = new Dictionary<UnitStat, int>();
        UnitStatBoosts = new Dictionary<UnitStat, int>();

        int statsAmount = System.Enum.GetNames(typeof(UnitStat)).Length;
        for (int i = 0; i < statsAmount; i++)
        {
            BaseUnitStats.Add((UnitStat)i, 0);
            UnitStatBoosts.Add((UnitStat)i, 0);
        }
        RecalculateBasicAttackDamage();
    }

    private void RecalculateBasicAttackDamage()
    {
        float minimumDamage = BaseAttackDamage.x * (1f + (GetTotalStat(UnitStat.STRENGTH) * 0.1f));
        float maximumDamage = BaseAttackDamage.y * (1f + (GetTotalStat(UnitStat.STRENGTH) * 0.1f));
        TotalAttackDamage = new Vector2(Mathf.Round(minimumDamage), Mathf.Round(maximumDamage));
    }

    internal void SetData(HeroData heroData)
    {
        Name = heroData.HeroName;
        UnitClass = heroData.GetTraits();
    }

    public bool StaminaCheck(HKEntity target)
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);
        return distance < 200f;
    }

    private void InitInventory()
    {
        inventory = new Inventory();
        inventory.InitInventory();
        inventory.OnInventoryUpdate.AddListener(() => selectManager.RefreshInventoryUI(this));
        inventory.OnEquipmentUpdate.AddListener(RefreshStats);
    }

    private void RefreshStats()
    {
        int statsAmount = System.Enum.GetNames(typeof(UnitStat)).Length;
        for (int i = 0; i < statsAmount; i++)
        {
            UnitStat currentUnitStat = (UnitStat)i;
            UnitStatBoosts[currentUnitStat] = 0;
        }
        foreach (var equipped in inventory.equippedGear)
        {
            if (equipped.Value == null) continue;
            ApplyItemStats(equipped.Value.itemData);
        }
        RecalculateBasicAttackDamage();
        RecalculateMaxHealth();
        selectManager.RefreshEquipmentUI(this);
    }

    protected override void RecalculateMaxHealth()
    {
        MaxHealth = Mathf.Floor((BaseMaxHealth + (HealthPerLevel * Level)) * GetCoeff(UnitStat.VITALITY, 0.1f) * GetCoeff(UnitStat.STRENGTH, 0.05f));
    }

    private void ApplyItemStats(ItemData item)
    {
        foreach (var statBoost in item.unitStats)
        {
            UnitStatBoosts[statBoost.Key] += statBoost.Value;
        }
    }

    private void SetupMissileSpawnPoint()
    {
        if (!MissileSpawnPoint)
            MissileSpawnPoint = transform;
    }

    private void InitRendererList()
    {
        foreach (var mr in renderers)
        {
            materials.Add(mr.material);
        }
    }

    private void SetUpStartingAction()
    {
        actionHandler.ActionCheck();
        //actionHandler.SetAction(StartingActionType);
    }

    private void InitStartingGold()
    {
        heldGold = UnityEngine.Random.Range(10, 30);
    }

    private void GenerateUnitCard()
    {
        if (unitAllegiance == UnitAllegiance.ALLY && unitType == UnitType.UNIT)
            unitCard = kingdomManager.GenerateUnitCard(this);
    }

    private void ResetSwingTimer()
    {
        swingTimer = 0f;
    }

    private void InitActionHandling()
    {
        actionHandler = new ActionHandler(this);
    }

    public void SetSpawnSickness()
    {
        isFadingIn = true;
        spawnSicknessTimer = 0f;
    }

    private void HandleSpawnSickness()
    {
        if (!isFadingIn) return;
        spawnSicknessTimer += Time.deltaTime;
        materials.ForEach(x => x.SetFloat("_Opacity", spawnSicknessTimer / 3f));
        if (spawnSicknessTimer >= 3f)
            CancelSpawnSickness();
    }

    private void CancelSpawnSickness()
    {
        materials.ForEach(x => x.SetFloat("_Opacity", 1f));
        unitBar.gameObject.SetActive(true);
        isFadingIn = false;
    }

    public int GetHeldGold()
    {
        return heldGold;
    }

    internal void ReloadSwingTimer()
    {
        swingTimer = AttackSpeed / GetCoeff(UnitStat.DEXTERITY, 0.1f);
    }

    internal float GetExperiencePercentage()
    {
        return experience / requiredExperience;
    }


    internal void StopAttacking()
    {
        isAttacking = false;
    }

    public override float GetHitboxRadius()
    {
        if (agent)
            return agent.radius;
        return 0f;
    }

    private void HandleUnitBarPosition()
    {
        Bounds bounds = meshFilters[0].mesh.bounds;
        foreach (var mf in meshFilters)
        {
            bounds.Encapsulate(mf.mesh.bounds);
        }
        Vector3 position = transform.position;
        position.y = bounds.min.y;

        unitBar.UpdatePosition(position);
    }

    private void CheckTargeting()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.GetInstanceID() != gameObject.GetInstanceID()) return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                selectManager.SetupPanel(this);
            }
        }
    }

    protected override void HandleDeath()
    {
        deathTimer -= Time.deltaTime;
        if (deathTimer < 0)
        {
            if (!isFadingOut)
            {
                Debug.Log("Fading Started");
                isFadingOut = true;
                renderers.ForEach(x => x.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off);
                List<Collider> colliders = new List<Collider>();
                colliders = GetComponentsInChildren<Collider>().ToList();
                colliders.ForEach(x => x.isTrigger = true);
            }
            HandleFadeOut();
        }
    }

    private void HandleFadeOut()
    {
        if (!isFadingOut) return;
        alphaLevel -= Time.deltaTime / 3f;
        materials.ForEach(x => x.SetFloat("_Opacity", alphaLevel));
        if (alphaLevel <= 0f)
            DestroyUnit();
    }

    private void Movement()
    {
        float remainingDistance = GetRemainingDistance();
        if (remainingDistance < 0.1f)
        {
            agent.isStopped = true;
            isMoving = false;
            animator.SetTrigger(IDLE_TRIGGER);
        }
        //if (!isAttacking)
        //    CheckForDanger();
    }

    private float GetRemainingDistance()
    {
        return Vector3.Distance(transform.position, agent.destination);
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public void PlayStepSound()
    {
        audioSource.PlayOneShot(StepClip);
    }

    private void ReactToNewTarget(HKUnit source)
    {
        if (!CurrentTarget || !attackers.Contains(CurrentTarget))
        {
            ChangeTarget(source);
        }
    }

    private void ReactToDeath()
    {
        actionHandler.RemoveUnit(this);
        if (agent)
            agent.radius = 0f;
        if (animator)
            animator.SetTrigger(DEATH_TRIGGER);
        actionHandler.ResetActions();
        audioSource.PlayOneShot(OnDeathClip);

        int goldPerAttacker = heldGold / attackers.Count;
        int rest = heldGold % attackers.Count;
        foreach (var attacker in attackers)
        {
            int goldToReceive = goldPerAttacker;
            if (rest > 0)
            {
                goldToReceive += 1;
                rest -= 1;
            }

            attacker.AwardGold(goldToReceive);
            attacker.AwardExperience(Level * 25f);
        }
    }

    public void AwardGold(int goldToReceive)
    {
        heldGold += goldToReceive;
        SpawnGoldText(goldToReceive, this);
        audioSource.PlayOneShot(floatingUIManager.GoldLootClip);
    }

    private void SpawnGoldText(int val, HKUnit receiver)
    {
        Vector3 position = receiver.CalculateUnitTop(false);
        floatingUIManager.SpawnGoldText((val).ToString(), position);
    }

    private void AwardExperience(float exp)
    {
        experience += exp;

        unitBar.SetExpBar(experience / requiredExperience);

        while (experience >= requiredExperience)
        {
            GainLevel();
        }
    }

    private void GainLevel()
    {
        experience -= requiredExperience;
        requiredExperience = Mathf.Round(requiredExperience * 1.2f);
        Level += 1;
        unitBar.SetText(string.Format("{0} {1}", Level, Name));
    }

    public void DestroyUnit()
    {
        if (unitCard)
            Destroy(unitCard);
        kingdomManager.RemoveEnemy(this);
        Destroy(gameObject);
    }

    public float GetPriority()
    {
        float averageDamage = (BaseAttackDamage.x + BaseAttackDamage.y) / 2f;
        float myDPS = CurrentHealth / (averageDamage / AttackSpeed);

        return myDPS;
    }

    private HKUnit FindHighestPrio(List<HKUnit> enemies)
    {
        float highest = 0.0f;
        HKUnit result = null;
        foreach (var enemy in enemies)
        {
            float currentPrio = enemy.GetPriority();
            if (currentPrio > highest)
            {
                highest = currentPrio;
                result = enemy;
            }
        }
        return result;
    }

    public float GetBasicAttackDamage()
    {
        return UnityEngine.Random.Range(TotalAttackDamage.x, TotalAttackDamage.y);
    }

    /// COMMANDS ///

    public void MoveToPosition(Vector3 destination)
    {
        if (IsDead || IsVanish) return;
        animator.SetTrigger(MOVE_TRIGGER);
        agent.isStopped = false;
        //if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 10f, 0))
        //{
        //    agent.destination = hit.position;
        //}
        //else
        agent.destination = destination;
        isMoving = true;
    }

    public void MoveToUnit(HKUnit destination)
    {
        Vector3 direction = (transform.position - destination.transform.position).normalized;

        float distance = destination.GetHitboxRadius() + GetHitboxRadius();

        Vector3 finalPosition = destination.transform.position + direction * distance;
        if (NavMesh.SamplePosition(finalPosition, out NavMeshHit hit, 10f, 0))
            finalPosition = hit.position;
        MoveToPosition(finalPosition);
    }

    public void StartAttacking()
    {
        if (IsDead || IsVanish) return;
        isAttacking = true;
        isMoving = false;
        agent.isStopped = true;
        //animator.SetTrigger(IDLE_TRIGGER);
    }

    public void UseFillerSpell(HKUnit targetUnit)
    {
        if (IsDead || IsVanish) return;
        CurrentTarget = targetUnit;
        animator.SetTrigger(ATTACK_TRIGGER);
    }

    public void CastSpell()
    {
        if (IsDead || IsVanish) return;
        audioSource.PlayOneShot(SwingClip);
        if (!CurrentTarget) return;
        if (FillerSpellID != -1)
        {
            SpellInfo spellInfo = new SpellInfo
            {
                Caster = this,
                Target = CurrentTarget,
                MissileSpawnPoint = MissileSpawnPoint.position
            };
            SpellEffect spellEffect = spellDatabase.GetSpellEffect(FillerSpellID, spellInfo);
            spellEffect.Execute();
        }
        //CurrentTarget.TakeDamage(GetDamage(), this);
        //CurrentTarget = null;
    }

    private void ChangeTarget(HKUnit newTarget)
    {
        if (CurrentTarget == null)
            Debug.Log(string.Format("[{0}] changed target to [{1}]", Name, newTarget.Name));
        else
            Debug.Log(string.Format("[{0}] changed target from [{1}] to [{2}]", Name, CurrentTarget.Name, newTarget.Name));
        actionHandler.SetAttackAction(newTarget);
    }

    protected override void UpdateAlive()
    {
        HandleSpawnSickness();
        if (isFadingIn) return;
        CheckTargeting();
        if (agent && TargetObject)
            TargetObject.position = new Vector3(agent.destination.x, 0.5f, agent.destination.z);
        actionHandler.Update();
        HandleUnitBarPosition();
        HandleRegeneration();

        if (swingTimer > 0f)
        {
            swingTimer -= Time.deltaTime;
            unitBar.SetSwingTimer(1f - (swingTimer / AttackSpeed));
        }
        if (isMoving)
            Movement();
    }

    public string GetExperienceText()
    {
        return string.Format("{0}/{1}", (int)experience, (int)requiredExperience);
    }

    private void HandleRegeneration()
    {
        if (currentTickTimer > 0)
        {
            currentTickTimer -= Time.deltaTime;
        }
        else
        {
            currentTickTimer = TimeBetweenRegenTicks / GetCoeff(UnitStat.VITALITY, 0.1f);
            RegenerationTick();
        }
    }

    private void RegenerationTick()
    {
        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + HealthPerTick);
    }
}
