using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

// ショップに在庫の数を持たせる
public class ItemEntry
{
    public ItemBase item;
    public int quantity;

    public ItemEntry(ItemBase item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

[CreateAssetMenu(menuName = "Base/ShopBase")]
public class ShopBase: ScriptableObject
{
    //[SerializeField] private List<ItemBase> shopItems;   
    [SerializeField] private List<ItemEntry> shopItems;
    //public List<ItemBase> AvailableItems { get { return shopItems; } }
    public List<ItemEntry> ShopItems { get { return shopItems; } }
}