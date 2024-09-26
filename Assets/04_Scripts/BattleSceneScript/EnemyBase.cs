using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyBase : ScriptableObject
{
    [SerializeField] private int enemyID;

    [SerializeField] private string enemyName;
    [SerializeField]
    private MagicType[] weakTypes;
    [SerializeField]
    private MagicType[] resistanceTypes;
    [SerializeField] private int level;

    [TextArea]
    [SerializeField] private string description;

    // モデル
    [SerializeField] private GameObject enemyPrefab;

    // ステータス
    [SerializeField] private int maxHp;
    [SerializeField] private int atk;// 力
    [SerializeField] private int magicPower;// 魔
    [SerializeField] private int def;// 耐
    [SerializeField] private int agi;// 速
    [SerializeField] private int luck; // 運

    // 覚える技一覧
    [SerializeField] private List<LearnableSkill> learnableEnemySkills;

    // ドロップアイテム あとで割合にする
    [SerializeField] List<ItemBase> dropItemBase;
    // ドロップゴールド
    [SerializeField] int maxDropGold;
    [SerializeField] int minDropGold;

    public int MaxHp { get => maxHp; }
    public int Agi { get => agi; }
    public int Atk { get => atk; }
    public int Def { get => def; }
    public int MagicPower { get => magicPower; }
    public int Luck { get => luck; }
    public List<LearnableSkill> LearableEnemySkills { get => learnableEnemySkills; }
    public string EnemyName { get => enemyName; }
    public string Description { get => description; }
    public GameObject EnemyPrefab { get => enemyPrefab; }
    public int EnemyID { get => enemyID; set => enemyID = value; }
    public int Level { get => level; set => level = value; }
    public int MaxDropGold { get => maxDropGold; set => maxDropGold = value; }
    public int MinDropGold { get => minDropGold; set => minDropGold = value; }
    public List<ItemBase> DropItemBase { get => dropItemBase; set => dropItemBase = value; }
    public MagicType[] WeakTypes { get => weakTypes; }
    public MagicType[] ResistanceTypes { get => resistanceTypes; }
}

// 覚える技：どのレベルで何を覚えるのか
[Serializable]
public class LearnableSkill
{
    [SerializeField] private SkillBase skillBase;
    [SerializeField] private int level;

    public SkillBase SkillBase { get => skillBase; }
    public int Level { get => level; }
}