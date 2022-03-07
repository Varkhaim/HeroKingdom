using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemBaseDatabase", menuName = "Hero Kingdom/ItemBaseDatabase", order = 1)]
public class ItemBaseDatabase : ScriptableObject
{
    [System.Serializable]
    public class ItemBase
    {
        public string BaseName;
        public Sprite ItemIcon;
        public ItemSlot ItemSlot;
    }

    public List<ItemBase> itemBases;

    public ItemBase GetRandomBase()
    {
        int random = Random.Range(0, itemBases.Count);
        return itemBases[random];
    }
}
