using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Base/HealItemBase")]
public class HealItemBase : ItemBase
{
    [SerializeField] private bool isAll;// 全体か
    [SerializeField] private int healPoint;// 回復量
    public bool IsAll { get => isAll; set => isAll = value; }
    public int HealPoint { get => healPoint; set => healPoint = value; }
}