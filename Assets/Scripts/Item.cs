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

public class Item
{
    [SerializeField] private ItemBase itemBase;
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
     // アイテムの数を増やす
    public void IncreaseItemCount(int amount)
    {
        itemCount += amount;
    }

    // アイテムの数を減らす（負にならないようにチェック）
    public void DecreaseItemCount(int amount)
    {
        itemCount = Mathf.Max(itemCount - amount, 0);
    }
}