using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

// ショップに在庫の数を持たせる
/*
public class ItemEntry
{
    public ItemBase item;
    // ショップの個数今はいらないのでコメントアウト
    //public int quantity;


}*/

[CreateAssetMenu(menuName = "Base/ShopBase")]
public class ShopBase: ScriptableObject
{
    //[SerializeField] private List<ItemBase> shopItems;   
    [SerializeField] private List<Item> shopItems;
    //public List<ItemBase> AvailableItems { get { return shopItems; } }
    public List<Item> ShopItems { get { return shopItems; } }
}