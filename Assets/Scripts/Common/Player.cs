using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[System.Serializable]
public class Player : Character
{
    int playerID;

    public GameObject PlayerBattleSprite { get; set; }

    public PlayerBase PlayerBase { get; set; }
    public BattlePlayerUI PlayerUI { get; set; }

    // UI
    public PlayerFieldUI playerUI;
    public BattlePlayerUI battlePlayerUI;
    public List<Item> items;

    // レベルに応じたHPを返す
    public int currentMaxHp
    {
        get { return Mathf.FloorToInt((PlayerBase.PlayerMaxHp * level) / 100f) + 10; }
    }

    // レベルに応じたSPを返す
    public int currentMaxSp
    {
        get { return Mathf.FloorToInt((PlayerBase.PlayerMaxSp * level) / 100f) + 30; }
    }

    // Start is called before the first frame update
    public List<Item> Items { get => items; set => items = value; }

    public Player(PlayerBase pBase, int level)
    {
        isPlayer = true;
        PlayerBase = pBase;
        characterName = PlayerBase.name;
        playerID = pBase.PlayerId;
        //        Debug.Log("ID" + playerID);
        // あとでレベルごとに変える
        this.level = level;
        currentHP = currentMaxHp;
        currentSP = currentMaxSp;
        currentMaxAtk = PlayerBase.PlayerMaxAtk;
        currentMaxDef = PlayerBase.PlayerMaxDef;
        currentMaxAgi = PlayerBase.PlayerMaxAgi;
        atk = PlayerBase.PlayerMaxAtk;
        def = PlayerBase.PlayerMaxDef;
        agi = PlayerBase.PlayerMaxAgi;
        // セーブデータがあればアイテムは引継ぎなければ初期化
        Items = new List<Item>();
        Skills = new List<EnemySkill>();

        // 覚える技のレベル以上なら所持ペルソナのスキルをskillsに追加
        foreach (LearnableSkill learablePlayerSkill in PlayerBase.LearnablePlayerSkills)
        {
            if (level >= learablePlayerSkill.Level)
            {
                Skills.Add(new EnemySkill(learablePlayerSkill.SkillBase));
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

    public void OverridePlayer(int level, int currentHP, int currentSP, int atk, int def, int agi)
    {
        this.level = level;
        currentHP = currentHP;
        currentSP = currentSP;
        atk = atk;
        def = def;
        agi = agi;
    }

    public bool TakeHealWithItem(Item healItem)
    {
        if (currentHP == currentMaxHp)
        {
            return false;
        }
        HealItemBase healItemBase = healItem.ItemBase as HealItemBase;
        if (currentHP + healItemBase.HealPoint > currentMaxHp)
        {
            currentHP = currentMaxHp;
        }
        else
        {
            Debug.Log(currentMaxHp);
            currentHP += healItemBase.HealPoint;
        }
        //Debug.Log(currentHP);
        return true;
    }
 
    public override bool TakeSkillDamage(EnemySkill enemySkill, Character character)
    {
        /*
        float modifiers = Random.Range(0.85f, 1.0f) * effectiveness * critical;
        float a = (2 * enemy.level + 10) / 250f;
        float d = a * enemySkill.skillBase.Power * ((float)enemy.MagicPower / equipEnemy.def) + 2;
        */
        int damage = enemySkill.skillBase.Power;

        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            return true;
        }
        return false;
    }

    public void TakeHeal(EnemySkill playerSkill, Player player)
    {
        /*
                float modifiers = Random.Range(0.85f, 1.0f);
                float a = (2 * player.level + 10) / 250f;
                float d = a * playerSkill.skillBase.Power * ((float)player.equipEnemy.MagicPower) + 2;
                // 与えるダメージの2.5倍
                int healHP = Mathf.FloorToInt(d * modifiers * 2.5f);*/
        int healHP = playerSkill.skillBase.Power;

        currentHP += healHP;
        if (currentHP > currentMaxHp)
        {
            currentHP = currentMaxHp;
        }
    }

    public void UseSp(EnemySkill playerSkill)
    {
        currentSP -= playerSkill.skillBase.Sp;
    }
}