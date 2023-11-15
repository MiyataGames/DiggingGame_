using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SKILL_STATUS_KIND
{
    UP,
    DOWN
}

[CreateAssetMenu(menuName = "Base/SkillStatusBase")]
public class SkillStatusBase : SkillBase
{
    [SerializeField] SKILL_STATUS_KIND skillStatusKind;
    [SerializeField] STATUS targetStatus;

	public SKILL_STATUS_KIND SkillStatusKind { get => skillStatusKind;}
	public STATUS TargetStatus { get => targetStatus; }
}
