using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : Item
{
    // コンストラクタ
    public HealItem(ItemBase iBase) : base(iBase) { }

    private int healPoint;// 回復量
}