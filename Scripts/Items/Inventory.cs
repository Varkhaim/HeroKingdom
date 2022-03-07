using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory
{
    public List<Item> carriedItems;
    public Dictionary<ItemSlot, Item> equippedGear;
    public UnityEvent OnInventoryUpdate;
    public UnityEvent OnEquipmentUpdate;

    public void InitInventory()
    {
        carriedItems = new List<Item>();
        OnInventoryUpdate = new UnityEvent();
        OnEquipmentUpdate = new UnityEvent();
        equippedGear = new Dictionary<ItemSlot, Item>();
        equippedGear.Add(ItemSlot.HEAD, null);
        equippedGear.Add(ItemSlot.ARMOR, null);
        equippedGear.Add(ItemSlot.WEAPON, null);
        equippedGear.Add(ItemSlot.OFFHAND, null);
    }

    public void AddItem(Item newItem)
    {
        carriedItems.Add(newItem);
        OnInventoryUpdate.Invoke();
    }

    public void EquipItem(Item newItem, ItemSlot slot)
    {
        equippedGear[slot] = newItem;
        carriedItems.Remove(newItem);
        OnInventoryUpdate.Invoke();
        OnEquipmentUpdate.Invoke();
    }

    public void UnequipItem(ItemSlot slot)
    {
        Item unequiped = equippedGear[slot];
        if (unequiped == null) return;
        PlaceInInventory(unequiped);
        equippedGear[slot] = null;
        OnInventoryUpdate.Invoke();
        OnEquipmentUpdate.Invoke();
    }

    public bool PlaceInInventory(Item item)
    {
        carriedItems.Add(item);
        OnInventoryUpdate.Invoke();
        return true;
    }

    public void TryEquipItem(Item toEquip)
    {
        ItemSlot slotToEquip = toEquip.itemData.itemSlot;
        if (equippedGear[slotToEquip] == null)
            EquipItem(toEquip, slotToEquip);
        else
            SwapItem(toEquip, slotToEquip);
    }

    public void SwapItem(Item toEquip, ItemSlot slotToEquip)
    {
        UnequipItem(slotToEquip);
        EquipItem(toEquip, slotToEquip);
    }
}
