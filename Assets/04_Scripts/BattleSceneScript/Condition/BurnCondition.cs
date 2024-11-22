using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnCondition : StatusCondition
{
    public BurnCondition(int duration) : base(STATUS_CONDITION_TYPE.BURN, duration) { }

    public override void ApplyEffect(Character character)
    {
        base.ApplyEffect(character);
    }
}
