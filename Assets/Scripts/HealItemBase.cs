using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Base/HealItemBase")]
public class HealItemBase : ItemBase
{
    [SerializeField] private bool isAll;// ‘S‘Ì‚©
    [SerializeField] private int healPoint;// ‰ñ•œ—Ê
    public bool IsAll1 { get => isAll; set => isAll = value; }
    public int HealPoint { get => healPoint; set => healPoint = value; }
}