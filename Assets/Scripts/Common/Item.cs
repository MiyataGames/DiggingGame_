using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    MATERIAL,
    TOOL,
    HEAL_ITEM,
    WEAPON
}
[System.Serializable]
public class Item
{
    [SerializeField] private ItemBase itemBase;
    private int itemCount = 0;
    private int id;

    public Item(ItemBase iBase)
    {
        ItemBase = iBase;
        Debug.Log("アイテムの名前"+itemBase.ItemName);
        id = iBase.Id;
    }

    public ItemBase ItemBase { get => itemBase; set => itemBase = value; }
    public int ItemCount { get => itemCount; set => itemCount = value; }
    public int Id { get => id; }
}