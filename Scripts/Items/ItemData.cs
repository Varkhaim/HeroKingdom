using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    public Sprite ItemIcon;
    public string ItemName;
    public ItemSlot itemSlot;
    public ItemQuality itemQuality;

    public Dictionary<UnitStat, int> unitStats;

    public static ItemData GetRandom(ItemBaseDatabase itemDatabase)
    {
        ItemData newItem = new ItemData();

        string namePrefix = "";
        string baseName = "";
        string nameSuffix = "";

        ItemBaseDatabase.ItemBase itemBase = itemDatabase.GetRandomBase();
        baseName = itemBase.BaseName;
        newItem.itemSlot = itemBase.ItemSlot;
        newItem.ItemIcon = itemBase.ItemIcon;

        int qualityCount = System.Enum.GetNames(typeof(ItemQuality)).Length;
        newItem.itemQuality = (ItemQuality)Random.Range(0, qualityCount);

        switch (newItem.itemQuality)
        {
            case ItemQuality.NORMAL:
                namePrefix = "";
                break;
            case ItemQuality.ENCHANTED:
                namePrefix = "Enchanted ";
                break;
            case ItemQuality.EMPOWERED:
                namePrefix = "Empowered ";
                break;
            case ItemQuality.ARTIFACT:
                namePrefix = "Ancient ";
                break;
            case ItemQuality.LEGENDARY:
                namePrefix = "Legendary ";
                break;
        }

        int statCount = System.Enum.GetNames(typeof(UnitStat)).Length;
        UnitStat chosenStat = (UnitStat)Random.Range(0, statCount);

        switch (chosenStat)
        {
            case UnitStat.STRENGTH:
                {
                    nameSuffix = " of the Wolf";
                }
                break;
            case UnitStat.DEXTERITY:
                {
                    nameSuffix = " of the Eagle";
                }
                break;
            case UnitStat.VITALITY:
                {
                    nameSuffix = " of the Bear";
                }
                break;
            case UnitStat.INTELLIGENCE:
                {
                    nameSuffix = " of the Dolphin";
                }
                break;
            case UnitStat.KNOWLEDGE:
                {
                    nameSuffix = " of the Owl";
                }
                break;
            case UnitStat.POWER:
                {
                    nameSuffix = " of the Wizard";
                }
                break;
        }

        newItem.unitStats = new Dictionary<UnitStat, int>();
        newItem.unitStats.Add(chosenStat, ((int)newItem.itemQuality) + 1);

        newItem.ItemName = string.Format("{0}{1}{2}", namePrefix, baseName, nameSuffix);

        return newItem;
    }

    public string GetSlotText()
    {
        switch (itemSlot)
        {
            case ItemSlot.HEAD:
                return "Helmet";
            case ItemSlot.ARMOR:
                return "Armor";
            case ItemSlot.WEAPON:
                return "Weapon";
            case ItemSlot.OFFHAND:
                return "Offhand";
        }
        return "???";
    }
}
