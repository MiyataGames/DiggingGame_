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

public enum GameState
{
    PLAYING,
    MENU,
    POSE
}

public enum GameMode
{
    FIELD_SCENE,
    BATTLE_SCENE,
    TOWN_SCENE,
    RESULT_SCENE,
    BOSS_SCENE,
}

public enum AreaMode
{
    Soil,
    Yougan,
    Forest,
    Science,
}

public enum Event1Scene
{
    NONE,
    EVENT1_1,
    EVENT1_2,
    EVENT1_3_4,
    EVENT1_5,
    EVENT1_6,
    EVENT1_7,
    END
}

//属性
public enum MagicType
{
    NOTHING,
    FIRE,
    ICE,
    WIND,
    THUNDER,
    DARK,
    HOLY,
    END
}