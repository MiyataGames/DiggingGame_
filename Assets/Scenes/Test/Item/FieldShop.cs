using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldShop : MonoBehaviour
{
    public ShopBase shopBase;
    private List<ItemEntry> shopItems = new List<ItemEntry>(); // アイテムリスト
      private ShopManager shopManager = new ShopManager(); // ShopManager

    void Start()
    {
        InitializeShopItems();
        DisplayShopItems();
    }

// ショップの在庫データを初期化して、FieldShop インスタンスそれぞれで独自に管理できるようにする。
    private void InitializeShopItems()
    {
        foreach (var itemEntry in shopBase.ShopItems)
        {
            shopItems.Add(itemEntry);
        }
    }

    // ショップのアイテムと数量をログに表示する
    private void DisplayShopItems()
    {
        foreach (var itemEntry in shopItems)
        {
            Debug.Log($"ShopItem: {itemEntry.item.name}, quantity: {itemEntry.quantity}");
        }
    }
}
