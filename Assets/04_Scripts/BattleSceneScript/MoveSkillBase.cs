using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Direction
{
    RIGHT,
    LEFT,
    UP,
    DOWN
}

[CreateAssetMenu(menuName = "Base/MoveSkillBase")]
public class MoveSkillBase : SkillBase
{
    [SerializeField] Direction direction;
    public Direction Direction { get => direction; }
}