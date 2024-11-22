using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Base/PlayerBase")]
public class PlayerBase : ScriptableObject
{
    [SerializeField] private int playerId;
    [SerializeField] private string playerName;
    [SerializeField]
    private MagicType[] weakTypes;
    [SerializeField]
    private MagicType[] resistanceTypes;
    [TextArea(1, 2)]
    [SerializeField] private string playerDiscription;
    [SerializeField] private Sprite playerFaceIcon;
    [SerializeField] private Sprite playerOverView;

    [SerializeField] GameObject playerBattleSceneSprite;// 待機用のスプライト

    [SerializeField] private int playerMaxLv;
    [SerializeField] private int playerMaxHp;
    [SerializeField] private int playerMaxSp;
    [SerializeField] private int playerMaxAtk;// 攻撃力
    [SerializeField] private int playerMaxDef;// 防御力
    [SerializeField] private int playerMaxAgi;// 素早さ

    // 覚えるスキル
    [SerializeField] private List<LearnableSkill> learnablePlayerSkills;

    public int PlayerId { get => playerId; }
    public string PlayerName { get => playerName; }
    public Sprite PlayerFaceIcon { get => playerFaceIcon; }

    public GameObject PlayerBattleSceneSprite { get => playerBattleSceneSprite; }
    public int PlayerMaxLv { get => playerMaxLv; }
    public int PlayerMaxHp { get => playerMaxHp; }
    public int PlayerMaxSp { get => playerMaxSp; }
    public int PlayerMaxAtk { get => playerMaxAtk; }
    public int PlayerMaxDef { get => playerMaxDef; }
    public int PlayerMaxAgi { get => playerMaxAgi; }
    public List<LearnableSkill> LearnablePlayerSkills { get => learnablePlayerSkills; }
    public string PlayerDiscription { get => playerDiscription;}
    public Sprite PlayerOverView { get => playerOverView; }
    public MagicType[] WeakTypes { get => weakTypes; }
    public MagicType[] ResistanceTypes { get => resistanceTypes;  }
}