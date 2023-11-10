using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    public Item item;
    public ItemBase itemBase; // ItemBaseを格納

    void Start()
    {
        item = new Item(itemBase);// 初期化
    }
}
