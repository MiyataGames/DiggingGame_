using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldShop : MonoBehaviour
{
    public ShopBase shopBase;
    private List<ItemEntry> shopItems = new List<ItemEntry>(); // アイテムリスト

    void Start()
    {
        InitializeShopItems();
        DisplayShopItems();
    }

    private void InitializeShopItems()
    {
        foreach (var itemEntry in shopBase.AvailableItems)
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
