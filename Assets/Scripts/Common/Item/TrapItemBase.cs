using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Base/TrapItemBase")]
public class TrapItemBase : ItemBase
{
    public Sprite itemImageSprite;
    [SerializeField] private int basicDamage;// ダメージ比率
    [SerializeField] STATUS_CONDITION_TYPE condition;
    public int BasicDamage { get => basicDamage; }
    public STATUS_CONDITION_TYPE Condition { get => condition; }
}