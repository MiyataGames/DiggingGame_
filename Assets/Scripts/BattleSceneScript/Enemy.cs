using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy : Character
{
    [SerializeField] private EnemyBase enemyBase;

    // ベースとなるデータ
    public EnemyBase EnemyBase { get => enemyBase; }

    public int Level { get => level; }
    public GameObject EnemyPrefab { get; set; }
    public Animator EnemyAnimator { get; set; }

    public BattleEnemyUI EnemyUI { get; set; }
    public int positionIndex;

    // レベルに応じたHPを返す
    public int currentMaxHp
    {
        get { return Mathf.FloorToInt((EnemyBase.MaxHp * level) / 100f) + 10; }
    }


    // コンストラクタ:生成時の初期設定
    public Enemy(EnemyBase eBase, int eLevel)
    {
        //しょきか
        isPlayer = false;
        enemyBase = eBase;
        characterName = enemyBase.name;
        level = eLevel;
        currentHP = currentMaxHp;
        currentMaxAtk = EnemyBase.Atk;
        currentMaxDef = enemyBase.Def;
        currentMaxAgi = EnemyBase.Agi;
        atk = currentMaxAtk;
        def = currentMaxDef;
        agi = currentMaxAgi;
        //		agi = eBase.Agi;
        Skills = new List<EnemySkill>();
        // 覚える技のレベル以上ならslillsに追加
        foreach (LearnableSkill learableSkill in eBase.LearableEnemySkills)
        {
            if (Level >= learableSkill.Level)
            {
                Skills.Add(new EnemySkill(learableSkill.SkillBase));
            }
            // 8つ以上はだめ
            if (Skills.Count >= 8)
            {
                break;
            }
        }
    }

    /// <summary>
    /// レベルに応じた初期値を設定する関数
    /// </summary>
    public override void InitStatusValue(int level)
    {

    }

    /*
        public bool isEffective(EnemySkill playerSkill)
        {
            for (int i = 0; i < this.EnemyBase.WeakTypes.Length; i++)
            {
                if (playerSkill.skillBase.MagicType == this.EnemyBase.WeakTypes[i])
                {
                    return true;
                }
            }
            return false;
        }

        public bool isResist(EnemySkill playerSkill)
        {
            for (int i = 0; i < this.EnemyBase.ResistanceTypes.Length; i++)
            {
                // 使われたスキルが耐性だったら
                if (playerSkill.skillBase.MagicType == this.EnemyBase.ResistanceTypes[i])
                {
                    return true;
                }
            }
            return false;
        }

        public bool isInvalid(EnemySkill playerSkill)
        {
            for (int i = 0; i < this.EnemyBase.InvalidTypes.Length; i++)
            {
                if (playerSkill.skillBase.MagicType == this.EnemyBase.InvalidTypes[i])
                {
                    return true;
                }
            }
            return false;
        }
        */

    private float critical = 1;

    public bool isCritical()
    {
        if (Random.value * 100 < 6.25)
        {
            critical = 2f;
            return true;
        }
        return false;
    }

    public override bool TakeSkillDamage(EnemySkill playerSkill, Character player)
    {

        // クリティカル
        // 相性
        float effectiveness = 1;// 効果量
        /*
        if (isEffective(playerSkill))
        {
            effectiveness = 1.5f;
        }
        else if (isResist(playerSkill))
        {
            effectiveness = 0.5f;
        }
        else if (isInvalid(playerSkill))
        {
            effectiveness = 0;
        }*/
        float modifiers = Random.Range(0.85f, 1.0f) * effectiveness * critical;
        float a = (2 * player.level + 10) / 250f;
        float d = a * playerSkill.skillBase.Power * ((float)player.atk / def) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            return true;
        }

        return false;
    }

    // damage
    public override bool TakeItemDamage(int basicDamage, Character turnCharacter, Character damagedCharacter)
    {
        // 合計ダメージ = (基本ダメージ + 攻撃力修正 + レベル修正) × クリティカルヒット倍率 - 敵の防御力
        int damage = basicDamage; // + turnCharacter.atk + turnCharacter.level;
        currentHP -= damage;
        Debug.Log("ダメージ" + damage + "げんざいのHP は" + currentHP);
        if (currentHP <= 0)
        {
            currentHP = 0;
            return true;
        }

        return false;
    }
    public void TakeHeal(EnemySkill enemSkill)
    {
        /*
        float modifiers = Random.Range(0.85f, 1.0f);
        float a = (2 * Level + 10) / 250f;
        float d = a * enemSkill.skillBase.Power * (MagicPower) + 2;
        int healHP = Mathf.FloorToInt(d * modifiers * 2.5f);

        Hp += healHP;
        if (Hp > MaxHp)
        {
            Hp = MaxHp;
        }
        */
    }

}