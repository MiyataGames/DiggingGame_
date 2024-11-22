using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HEAL_TYPE
{
    HP,
    SP
}
[CreateAssetMenu(menuName = "Base/HealItemBase")]
public class HealItemBase : ItemBase
{
    [SerializeField] HEAL_TYPE healType;
    [SerializeField] private TARGET_NUM targetNum;
    [SerializeField] private int healPoint;// 回復量
    public int HealPoint { get => healPoint;}
    public TARGET_NUM TargetNum { get => targetNum;}
    public HEAL_TYPE HealType { get => healType; }
}