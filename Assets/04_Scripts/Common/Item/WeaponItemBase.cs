using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Base/WeaponItemBase")]
public class WeaponItemBase : ItemBase
{
    [SerializeField] private float damage; // ダメージ倍率
    [SerializeField] private TARGET_NUM targetNum;

    // 状態異常
    [SerializeField]
    public float Damage { get => damage; }
    public TARGET_NUM TargetNum { get => targetNum; }

}