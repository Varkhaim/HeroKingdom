using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TooltipManager : MonoBehaviour
{
    public ItemTooltip tooltip;
    public ItemManager itemManager;

    public void ShowTooltip(Item item)
    {
        tooltip.ShowTooltip(item, itemManager.GetItemColor(item.itemData.itemQuality));
        //tooltip.transform.position = Mouse.current.position.ReadValue();
    }

    public void HideTooltip()
    {
        tooltip.Hide();
    }
}
