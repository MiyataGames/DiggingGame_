using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SKILL_CATEGORY
{
    ATTACK,
    HEAL,
    STATUS,
    CONDITION,
    MOVE_ATTACK,
}

public enum SKILL_TARGET_KIND
{
    SELF,
    FOE
}

public enum TARGET_NUM
{
    SINGLE,
    ALL
}

[CreateAssetMenu]
public class SkillBase : ScriptableObject
{
    // スキルのマスターデータ

    [SerializeField] private string skillName;
    [SerializeField] private MagicType magicType;

    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private float power;
    [SerializeField] private int sp;
    [SerializeField] private int accuracy;// 命中率

    [SerializeField] private SKILL_CATEGORY skillCategory;
    [SerializeField] private SKILL_TARGET_KIND skillTargetKind;
    [SerializeField] private TARGET_NUM skillTargetNum;

    /*
        [SerializeField] private bool isAll;// 単体攻撃か
        [SerializeField] private bool isAttackSkill;// 攻撃魔法か
        [SerializeField] private bool isHeal;// 回復魔法か
        [SerializeField] private bool isRevival;// 復活魔法か
        [SerializeField] private bool isEffective;// 復活魔法か
        */

    [SerializeField] private GameObject skillRecieveEffect;
    [SerializeField] private AudioClip takeSkillSE;

    public string SkillName { get => skillName; }
    public string Description { get => description; }
    public float Power { get => power; }
    public int Sp { get => sp; }
    public int Accuracy { get => accuracy; }

    //public bool IsAll { get => isAll; }
    public GameObject SkillRecieveEffect { get => skillRecieveEffect; }

    //public bool IsHeal { get => isHeal; }
    //public bool IsRevival { get => isRevival; }
    //public bool IsAttackSkill { get => isAttackSkill; }
    public SKILL_CATEGORY SkillCategory { get => skillCategory; }

    public SKILL_TARGET_KIND SkillTargetKind { get => skillTargetKind; }
    public TARGET_NUM SkillTargetNum { get => skillTargetNum; }
    public MagicType MagicType { get => magicType; }
    public AudioClip TakeSkillSE { get => takeSkillSE;}
}