using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalysisCondition : StatusCondition
{
    public ParalysisCondition(int duration) : base(STATUS_CONDITION_TYPE.PARALYSIS, duration) { }

    public override void ApplyEffect(Character character)
    {
        base.ApplyEffect(character);
        
    }
}
