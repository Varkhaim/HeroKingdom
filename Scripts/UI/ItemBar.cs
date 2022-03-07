using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMPro.TextMeshProUGUI itemName;
    public Image itemIcon;
    public Item item;
    public Button button;

    private void Start()
    {
        button.onClick.AddListener(UseItem);
    }

    private void UseItem()
    {
        GameCore.GetItemManager().EquipItem(item); 
        GameCore.GetTooltipManager().HideTooltip();
    }

    public void SetItem(Item item)
    {
        if (item == null)
        {
            this.item = null;
            itemIcon.sprite = null;
            itemIcon.color = Color.clear;
            itemName.text = "";
            button.interactable = false;
            return;
        }
        this.item = item;
        itemIcon.sprite = item.itemData.ItemIcon;
        itemIcon.color = Color.white;
        button.interactable = true;
        string colorCode = GameCore.GetItemManager().GetItemColor(item.itemData.itemQuality);
        itemName.text = string.Format("<color=#{0}>{1}</color>", colorCode, item.itemData.ItemName);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null) return;
        GameCore.GetTooltipManager().ShowTooltip(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameCore.GetTooltipManager().HideTooltip();
    }
}
