using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ConditionID
{
    None,
    Poison,    // 毒
    Burn,      // 火傷
    Sleep,     // 睡眠
    Paralysis, // 麻痺
    Freeze,    // こおり
    Confusion, // 混乱
}

public enum STATUS
{
    // 攻撃力
    ATTACK,
    // 防御力
    DEFENSE,
    // スピード
    SPEED,
    // 命中率
    ACCURACY,
    // 回避率
    EVASION
}