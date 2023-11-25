using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Base/HealItemBase")]
public class HealItemBase : ItemBase
{
    [SerializeField] private TARGET_NUM targetNum;
    [SerializeField] private int healPoint;// 回復量
    public int HealPoint { get => healPoint;}
    public TARGET_NUM TargetNum { get => targetNum;}
}