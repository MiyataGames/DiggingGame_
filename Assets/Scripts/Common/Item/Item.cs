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
[System.Serializable]
public class Item
{
    [SerializeField] private ItemBase itemBase;
    private int itemCount = 0;
    public int id;

    public Item(ItemBase iBase)
    {
        ItemBase = iBase;
        Debug.Log("アイテムの名前" + itemBase.ItemName);
        id = iBase.Id;
    }

    // ItemBase オブジェクトの Price プロパティにアクセスする
    public int Price
    {
        get => itemBase.Price; // 修正点
    }
    public ItemBase ItemBase { get => itemBase; set => itemBase = value; }
    public int ItemCount { get => itemCount; set => itemCount = value; }

    public int Id { get => id; }
}