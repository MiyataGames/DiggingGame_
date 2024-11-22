using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeCondition : StatusCondition
{
    public FreezeCondition(int duration) : base(STATUS_CONDITION_TYPE.FREEZE, duration) { }

    public override void ApplyEffect(Character character)
    {
        base.ApplyEffect(character);
    }
}
