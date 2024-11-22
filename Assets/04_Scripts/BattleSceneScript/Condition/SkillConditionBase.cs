using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Base/SkillConditionBase")]
public class SkillConditionBase : SkillBase
{
    [SerializeField] STATUS_CONDITION_TYPE condition;
    [SerializeField] float conditionAttackAccuracy;
    public float ConditionAttackAccuracy { get => conditionAttackAccuracy;}
    public STATUS_CONDITION_TYPE Condition { get => condition;}
}
