using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Base/TrapItemBase")]
public class TrapItemBase : ItemBase
{
    public Sprite itemImageSprite;
    [SerializeField] private int damageRatio;// ダメージ比率
    [SerializeField] STATUS_CONDITION_TYPE condition;
    [SerializeField] GameObject receivedEffect;
    public int DamageRatio { get => damageRatio; }
    public STATUS_CONDITION_TYPE Condition { get => condition; }
    public GameObject ReceivedEffect { get => receivedEffect; }
}