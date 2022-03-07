using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public Image itemIcon;
    public TMPro.TextMeshProUGUI itemSlotText;
    public TMPro.TextMeshProUGUI itemName;

    internal void Hide()
    {
        gameObject.SetActive(false);
    }

    [System.Serializable]
    public class UnitStatBoxPair
    {
        public StatBox statBox;
        public UnitStat unitStat;
    }

    public List<UnitStatBoxPair> pairs;

    public void ShowTooltip(Item item, string nameColor)
    {
        gameObject.SetActive(true);
        itemIcon.sprite = item.itemData.ItemIcon;
        itemName.text = string.Format("<color=#{0}>{1}</color>", nameColor, item.itemData.ItemName);
        foreach (var pair in pairs)
        {
            if (!item.itemData.unitStats.ContainsKey(pair.unitStat))
            {
                pair.statBox.gameObject.SetActive(false);
                continue;
            }
            pair.statBox.gameObject.SetActive(item.itemData.unitStats[pair.unitStat] > 0);
            pair.statBox.SetStatBox(string.Format("+{0}", item.itemData.unitStats[pair.unitStat]));
        }
        itemSlotText.text = item.itemData.GetSlotText();
    }
}
