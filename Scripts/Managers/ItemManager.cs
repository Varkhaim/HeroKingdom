using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public ItemBaseDatabase itemDatabase;
    public GameMessageManager gameMessageManager;

    public Color NormalColor;
    public Color EnchantedColor;
    public Color EmpoweredColor;
    public Color AncientColor;
    public Color LegendaryColor;

    [Header("Refs")]
    public Transform itemListParent;

    [Header("Prefabs")]
    public GameObject itemBarPrefab;

    private HKUnit inspectedUnit;

    [System.Serializable]
    public class SlotItemBarPair
    {
        public ItemSlot slot;
        public ItemBar itemBar;
    }

    public List<SlotItemBarPair> itemBars;

    public void EquipItem(Item item)
    {
        if (!inspectedUnit) return;
        inspectedUnit.inventory.TryEquipItem(item);
    }

    public void SetEquipmentUI(HKUnit unit)
    {
        foreach (var itemBar in itemBars)
        {
            if (unit.inventory.equippedGear[itemBar.slot] != null)
            {
                //itemBar.itemBar.gameObject.SetActive(true);
                itemBar.itemBar.SetItem(unit.inventory.equippedGear[itemBar.slot]);
            }
            else
            {
                //itemBar.itemBar.gameObject.SetActive(false);
                itemBar.itemBar.SetItem(null);
            }
        }
    }

    internal string GetItemColor(ItemQuality itemQuality)
    {
        Color finalColor = Color.white;

        switch (itemQuality)
        {
            case ItemQuality.NORMAL:
                finalColor = NormalColor;
                break;
            case ItemQuality.ENCHANTED:
                finalColor = EnchantedColor;
                break;
            case ItemQuality.EMPOWERED:
                finalColor = EmpoweredColor;
                break;
            case ItemQuality.ARTIFACT:
                finalColor = AncientColor;
                break;
            case ItemQuality.LEGENDARY:
                finalColor = LegendaryColor;
                break;
        }

        return ColorUtility.ToHtmlStringRGB(finalColor);
    }

    public void SetInventoryUI(HKUnit unit)
    {
        ClearOldItems();
        GenerateNewItems(unit.inventory);
        inspectedUnit = unit;
    }

    private void GenerateNewItems(Inventory inventory)
    {
        foreach (var item in inventory.carriedItems)
        {
            GameObject itemBarGameObject = Instantiate(itemBarPrefab, itemListParent);
            ItemBar itemBar = itemBarGameObject.GetComponent<ItemBar>();
            itemBar.SetItem(item);
        }
    }

    private void ClearOldItems()
    {
        foreach (Transform oldBar in itemListParent)
        {
            Destroy(oldBar.gameObject);
        }
    }

    public void SendLootMessage(HKUnit looter, ItemData item)
    {
        gameMessageManager.SendItemLootMessage(looter, item);
    }
}
