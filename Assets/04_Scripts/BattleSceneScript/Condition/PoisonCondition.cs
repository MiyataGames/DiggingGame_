using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonCondition : StatusCondition
{
    public PoisonCondition(int duration) : base(STATUS_CONDITION_TYPE.POISON, duration) { }

    public override void ApplyEffect(Character character)
    {
        base.ApplyEffect(character);
        int damage = Mathf.FloorToInt(character.currentMaxHp * 0.05f);
        character.TakeConditionDamage(damage);
        
    }
}
