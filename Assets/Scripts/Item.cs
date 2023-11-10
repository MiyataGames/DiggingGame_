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

// アイテム
public class Item
{
    private ItemBase itemBase;
    private int itemCount = 0;
    private int id;

    public Item(ItemBase iBase)
    {
        ItemBase = iBase;
        id = iBase.Id;
    }

    public ItemBase ItemBase { get => itemBase; set => itemBase = value; }
    public int ItemCount { get => itemCount; set => itemCount = value; }
    public int Id { get => id; }
}