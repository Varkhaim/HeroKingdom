using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TreasureChest : Interactable
{
    private int goldAmount = 0;
    public Animator animator;
    public GameObject content;
    private HKUnit looter;
    private UnitFinder unitFinder;
    private AudioSource audioSource;
    public AudioClip openClip;
    private Item ContainedItem;
    private ItemManager itemManager;

    private void Awake()
    {
        if (!animator)
            animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        itemManager = GameCore.GetItemManager();
    }

    private void Start()
    {
        unitFinder = GameCore.GetUnitFinder();
        goldAmount = Random.Range(0, 200);
        unitFinder.AddTreasure(this);
        ContainedItem = new Item(ItemData.GetRandom(itemManager.itemDatabase));
    }

    public override void Interact(HKUnit unit)
    {
        if (isBeingInteracted || usedUp || IsAnimatorPlaying()) return;

        isBeingInteracted = true;
        looter = unit;
        unit.isLooting = false;
        audioSource.PlayOneShot(openClip);
        animator.SetTrigger("OPEN");
    }

    private void Update()
    {
        if (isBeingInteracted && !usedUp && !IsAnimatorPlaying())
        {
            isBeingInteracted = false;
            usedUp = true;
        }
    }

    public bool IsLooted()
    {
        return usedUp;
    }

    public void LootContent()
    {
        looter.AwardGold(goldAmount);
        goldAmount = 0;
        content.SetActive(false);
        if (ContainedItem != null)
        {
            looter.inventory.PlaceInInventory(ContainedItem);
            itemManager.SendLootMessage(looter, ContainedItem.itemData);
            ContainedItem = null;
        }
        unitFinder.RemoveTreasure(this);
        looter.actionHandler.ActionCheck();
    }

    private bool IsAnimatorPlaying()
    {
        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return animatorStateInfo.length > animatorStateInfo.normalizedTime;
    }

    public Vector3 GetClosestPoint(Vector3 pos, float attackRange)
    {
        float unitRadius = 1f;
        Vector3 offset = (pos - transform.position).normalized * (unitRadius * 1f + attackRange);
        Vector3 finalPosition = transform.position + offset;
        if (NavMesh.SamplePosition(finalPosition, out NavMeshHit hit, 10f, 0))
            finalPosition = hit.position;
        return finalPosition;
    }
}
