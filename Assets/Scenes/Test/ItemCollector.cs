using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    coin,
    itemA,
    itemB
}

public class ItemCollector : MonoBehaviour
{
    public ItemType item;

    public ItemCollector(ItemType item)
    {
        this.item = item;

        switch (this.item)
        {
            case ItemType.coin:
                Debug.Log("coin����");
            case ItemType.itemA:
                Debug.Log("coin����");
            case ItemType.itemB;
                Debug.Log("coin����");
            default:
                Debug.Log("�E�Ƃ����݂��܂���");
                break;
        }
    }

}
