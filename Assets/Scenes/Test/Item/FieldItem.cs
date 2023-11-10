using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    public Item item;
    public ItemBase itemBase; // ItemBase型の公開フィールドを追加

    void Start()
    {
        // ItemBaseが設定されているかチェック
        if (itemBase != null)
        {
            // ItemBaseを基にItemインスタンスを初期化
            item = new Item(itemBase);
        }
        else
        {
            Debug.LogError("ItemBase is not set for " + gameObject.name);
        }
    }
}
