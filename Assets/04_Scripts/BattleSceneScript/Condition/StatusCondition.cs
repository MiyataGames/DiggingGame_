using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STATUS_CONDITION_TYPE
{
    NONE,

    POISON,
    PARALYSIS,
    BURN,
    FREEZE,
    SLEEP,
    // 他の状態異常も追加可能
}

public class StatusCondition
{
    public STATUS_CONDITION_TYPE type;
    // 持続ターン
    public int duration;

    // コンストラクタ
    public StatusCondition(STATUS_CONDITION_TYPE type, int duration)
    {
        this.type = type;
        this.duration = duration;
        Debug.Log(type + ":" + duration + "ターン");
    }

    // 毎ターンの効果を適用するメソッド
    public virtual void ApplyEffect(Character character)
    {
        // 効果の実装
    }

    // 状態異常の更新（ターン経過）
    public void UpdateCondition(Character character)
    {
        duration--;
        if (duration > 0)
        {
            ApplyEffect(character);
        }
    }
}
