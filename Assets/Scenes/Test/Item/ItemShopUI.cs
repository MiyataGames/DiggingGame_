using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ItemShopUI : MonoBehaviour
{
    public ShopBase shopBase; // ItemShop スクリプタブルオブジェクトの参照

    // ボタンが押されたときに呼ばれるメソッド
    public void DisplayItems()
    {
        foreach (var itemEntry in shopBase.AvailableItems)
        {
            Debug.Log($"アイテム名: {itemEntry.item.name}, 数量: {itemEntry.quantity}");
        }
    }
}