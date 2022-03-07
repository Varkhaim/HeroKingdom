using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMessageManager : MonoBehaviour
{
    [Header("Refs")]
    public Transform messageParent;

    [Header("Prefabs")]
    public GameObject messagePrefab;

    public ItemManager itemManager;

    public void SendItemLootMessage(HKUnit looter, ItemData item)
    {
        GameObject messageGO = Instantiate(messagePrefab, messageParent);
        GameMessage gameMessage = messageGO.GetComponent<GameMessage>();
        string content = string.Format("{0} ({1}) has looted <color=#{2}>{3}</color>", looter.Name, looter.UnitClass, itemManager.GetItemColor(item.itemQuality), item.ItemName);
        gameMessage.SetMessage(content, item.ItemIcon, 10f, true);
    }
}
