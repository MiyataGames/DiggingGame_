using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepCondition : StatusCondition
{
    public SleepCondition(int duration) : base(STATUS_CONDITION_TYPE.SLEEP, duration) { }

    public override void ApplyEffect(Character character)
    {
        base.ApplyEffect(character);
    }
}
