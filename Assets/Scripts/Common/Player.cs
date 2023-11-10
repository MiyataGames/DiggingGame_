using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Player : Character
{
    public int playerID;
    public int level;
    public int currentHP;
    public int currentSP;
    public int atk;
    public int def;
    public int agi;
    public GameObject PlayerBattleSprite { get; set; }

    public PlayerBase PlayerBase { get; set; }
    public BattlePlayerUI PlayerUI { get; set; }

    // UI
    public PlayerFieldUI playerUI;
    public BattlePlayerUI battlePlayerUI;
    // パラメータ
    public int Level { get => level; }
    public List<EnemySkill> Skills { get; set; }//スキル
    public int CurrentHp { get => currentHP; set => currentHP = value; }
    public int CurrentSp { get => currentSP; set => currentSP = value; }
    public int Atk { get => atk; set => atk = value; }
    public int Def { get => def; set => def = value; }
    public int Agi { get => agi; set => agi = value; }
    public List<Item> items;

    // レベルに応じたHPを返す
    public int currentMaxHp
    {
        get { return Mathf.FloorToInt((PlayerBase.PlayerMaxHp * Level) / 100f) + 10; }
    }

    // レベルに応じたSPを返す
    public int currentMaxSp
    {
        get { return Mathf.FloorToInt((PlayerBase.PlayerMaxSp * Level) / 100f) + 30; }
    }

    // Start is called before the first frame update
    public List<Item> Items { get => items; set => items = value; }

    public Player(PlayerBase pBase, int level)
    {
        isPlayer = true;
        PlayerBase = pBase;
        playerID = pBase.PlayerId;
        //        Debug.Log("ID" + playerID);
        // あとでレベルごとに変える
        this.level = level;
        CurrentHp = 3;
        CurrentSp = currentMaxSp;
        Atk = PlayerBase.PlayerMaxAtk;
        Def = PlayerBase.PlayerMaxDef;
        Agi = PlayerBase.PlayerMaxAgi;
        // セーブデータがあればアイテムは引継ぎなければ初期化
        Items = new List<Item>();
        Skills = new List<EnemySkill>();
        
        // 覚える技のレベル以上なら所持ペルソナのスキルをskillsに追加
        foreach (LearnableSkill learablePlayerSkill in PlayerBase.LearnablePlayerSkills)
        {
            if (Level >= learablePlayerSkill.Level)
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

    public void OverridePlayer(int level, int currentHp, int currentSp, int atk, int def, int agi)
    {
        this.level = level;
        CurrentHp = currentHp;
        CurrentSp = currentSp;
        Atk = atk;
        Def = def;
        Agi = agi;
    }

    public bool TakeHealWithItem(Item healItem)
    {
        if (CurrentHp == currentMaxHp)
        {
            return false;
        }
        HealItemBase healItemBase = healItem.ItemBase as HealItemBase;
        if (CurrentHp + healItemBase.HealPoint > currentMaxHp)
        {
            CurrentHp = currentMaxHp;
        }
        else
        {
            Debug.Log(currentMaxHp);
            CurrentHp += healItemBase.HealPoint;
        }
        //Debug.Log(currentHp);
        return true;
    }

    public bool TakeDamage(EnemySkill enemySkill, Enemy enemy)
    {
        /*
        float modifiers = Random.Range(0.85f, 1.0f) * effectiveness * critical;
        float a = (2 * enemy.Level + 10) / 250f;
        float d = a * enemySkill.skillBase.Power * ((float)enemy.MagicPower / equipEnemy.Def) + 2;
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
                float a = (2 * player.Level + 10) / 250f;
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