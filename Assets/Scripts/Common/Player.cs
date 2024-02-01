using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[System.Serializable]
public class Player : Character
{
    int playerID;
    private ExpSheet expSheet;// 経験値表
    PlayerStatusBase playerStatusBase; // ステータス表
    public int Exp
    {
        get; set;
    }

    private int nextExp;// 次のレベルまでの経験値
    private int nextFullExp;// 次のレベルまでの経験値の初期値（最大値）
    public int NextExp { get => nextExp; set => nextExp = value; }

    public GameObject PlayerBattleSprite { get; set; }

    public PlayerBase PlayerBase { get; set; }
    public BattlePlayerUI PlayerUI { get; set; }

    // UI
    public PlayerFieldUI playerUI;
    public BattlePlayerUI battlePlayerUI;
    public ResultPlayerUI ResultPlayerUI;
    public List<Item> items;
    Animator playerBattleAnimator;

        // 所持金
    public int gold = 1000;
    public int Gold { get => gold; set => gold = value; }

    // レベルに応じたHPを返す
    public int CurrentMaxHp
    {
        // expSheet.sheets[0].list[level - 1].nextExp;
        get { return playerStatusBase.sheets[PlayerID].list[level].hp; }
    }

    // レベルに応じたSPを返す
    public int CurrentMaxSp
    {
        get { return playerStatusBase.sheets[PlayerID].list[level].hp; }
    }

    // レベルに応じたAtkを返す
    public int CurrentMaxAtk
    {
        get { return playerStatusBase.sheets[PlayerID].list[level].atk; }
    }

    // レベルに応じたDefを返す
    public int CurrentMaxDef
    {
        get { return playerStatusBase.sheets[PlayerID].list[level].def; }
    }

    // レベルに応じたAgiを返す
    public int CurrentMaxAgi
    {
        get { return playerStatusBase.sheets[PlayerID].list[level].agi; }
    }
    // Start is called before the first frame update
    public List<Item> Items { get => items; set => items = value; }
    public int PlayerID { get => playerID;}
    public Animator PlayerBattleAnimator { get => playerBattleAnimator; set => playerBattleAnimator = value; }

    public Player(PlayerBase pBase, int level,List<ItemBase> debugItemBase)
    {
        playerStatusBase = (PlayerStatusBase)Resources.Load("playerStatus");
        isPlayer = true;
        PlayerBase = pBase;
        characterName = PlayerBase.name;
        playerID = pBase.PlayerId;
        expSheet = (ExpSheet)Resources.Load("levelExp");
        NextExp = expSheet.sheets[0].list[level - 1].nextExp;
        this.level = level;
        currentHP = CurrentMaxHp;
        currentSP = CurrentMaxSp;
        atk = CurrentMaxAtk;
        def= CurrentMaxDef;
        agi = CurrentMaxAgi;
        Item item;
        // セーブデータがあればアイテムは引継ぎなければ初期化
        Items = new List<Item>();
        // 主人公なら
        if (playerID == 0)
        {
           //  Debug.Log(debugItemBase.Count);

            // デバッグアイテムが入っていれば
            for (int i = 0; i < debugItemBase.Count; i++)
            {
                item = new Item(debugItemBase[i]);
                item.ItemCount = 2;
                items.Add(item);
            }
            /*for (int i = 0; i < debugItemBase.Count; i++)
            {
                Debug.Log(items[i]);
            }*/

        }
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

    // 相性をみる関数
    public bool isResist(EnemySkill enemySkill)
    {
        for (int i = 0; i < this.PlayerBase.ResistanceTypes.Length; i++)
        {
            // 使われたスキルが耐性だったら
            if (enemySkill.skillBase.MagicType == this.PlayerBase.ResistanceTypes[i])
            {
                return true;
            }
        }
        return false;
    }

    public bool isEffective(EnemySkill enemySkill)
    {
        for (int i = 0; i < this.PlayerBase.WeakTypes.Length; i++)
        {
            if (enemySkill.skillBase.MagicType == this.PlayerBase.WeakTypes[i])
            {
                return true;
            }
        }
        return false;
    }

    // 体力やSPを変更する関数たち
    public bool TakeHealWithItem(Item healItem)
    {
        HealItemBase healItemBase = healItem.ItemBase as HealItemBase;

        if (healItemBase.HealType == HEAL_TYPE.HP)
        {
            if (currentHP == CurrentMaxHp)
            {
                return false;
            }

            if (currentHP + healItemBase.HealPoint > CurrentMaxHp)
            {
                currentHP = CurrentMaxHp;
            }
            else
            {
                Debug.Log(CurrentMaxHp);
                currentHP += healItemBase.HealPoint;
            }
            //Debug.Log(currentHP);
            return true;
        }
        else if (healItemBase.HealType == HEAL_TYPE.SP)
        {
            if (currentSP == currentMaxSp)
            {
                return false;
            }

            if (currentSP + healItemBase.HealPoint > CurrentMaxSp)
            {
                currentSP = CurrentMaxSp;
            }
            else
            {
                Debug.Log(CurrentMaxSp);
                currentSP += healItemBase.HealPoint;
            }
            //Debug.Log(currentHP);
            return true;
        }
        return false;
    }
 
    public override bool TakeSkillDamage(EnemySkill enemySkill, Character character)
    {
        float skillPower = enemySkill.skillBase.Power;// スキル倍率
        int damage = 0;
        // 相性
        float effectiveness = 1;
        if (isEffective(enemySkill))
        {
            effectiveness = 1.2f;
        }
        else if (isResist(enemySkill))
        {
            effectiveness = 0.7f;
        }
        // デバッグモードは敵の技が弱くて敵がめっちゃ弱い
        if (GameManager.instance.playMode == PlayMode.DEBUG)
        {
            damage = 1;
        }
        else if (GameManager.instance.playMode == PlayMode.RELEASE)
        {
            float randSeed = UnityEngine.Random.Range(0.83f, 1.17f);
            // （攻撃力/2-守備力/4）×変数(5/6~7/6)x属性効果量
            damage = (int)((character.atk / 2 - def / 4) * randSeed * skillPower * effectiveness);
            Debug.Log("属性効果量" + effectiveness);
            Debug.Log("ダメージ" + damage);
            Debug.Log("現在の体力" + currentHP);
        }

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
        if (currentHP > CurrentMaxHp)
        {
            currentHP = CurrentMaxHp;
        }
    }

    public void UseSp(EnemySkill playerSkill)
    {
        currentSP -= playerSkill.skillBase.Sp;
    }



    // expを更新して上がった経験値分のnextExpを返す
    public ExpPair GetExp(int getExp)
    {
        ExpPair expPair;
        expPair.oldLevel = level;
        List<float> currentExps = new List<float>();//レベルごとの現在の経験値のリスト
        List<float> nextExps = new List<float>();// レベルごとの次までの経験値のリスト
        nextExp = nextExp - Exp;// 次の経験値までの経験値
        Exp += getExp;
        nextExps.Add(nextExp);
        Debug.Log("次までの経験値" + nextExp+ "現在の経験値" + Exp);
        int remainExp = nextExp - getExp;// 次のレベルまでの経験値から取得した経験値を引いた1回目のあまり
        Debug.Log("残りの経験値は" + remainExp);
        float remainGetExp = getExp;
        while (remainExp <= 0)// 次のレベルまでのexpが－だったら
        {
            // レベルアップ
            level += 1;
            // レベルアップしたら体力とSP全回復する
            currentHP = CurrentMaxHp;
            currentSP = CurrentMaxSp;
            // atk,def,agiを更新
            atk = CurrentMaxAgi;
            def = CurrentMaxDef;
            agi = currentMaxAgi;
            remainGetExp -= nextExp;
            currentExps.Add(nextExp);// -だったら次までの経験値を更新する前のマックス値
            // 次までの経験値の更新
            nextExp = expSheet.sheets[0].list[level - 1].nextExp;
            nextExps.Add(nextExp);
            int deltaExp = Mathf.Abs(Exp - nextExp);// さらに余った経験値
            remainExp = nextExp - deltaExp;
        }
        Exp = (int)remainGetExp;
        currentExps.Add(remainGetExp);
        expPair.getExp = currentExps;
        expPair.nextExp = nextExps;
        expPair.newLevel = level;
        Debug.Log("経験値を得た後のレベルは" + level);
        return expPair;
    }

    public void AddItem(Item addItem)
    {
        // 同じアイテムがあるか検索
        if (items.Count > 0)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (addItem.ItemBase.Id == items[i].ItemBase.Id)
                {
                    // あったら個数を増やして破棄
                    items[i].ItemCount++;
                    return;
                }
            }
        }
        // なければ新しく追加する
        items.Add(addItem);
        items[items.Count - 1].ItemCount++;
        // idが早い順に並べる
        items.Sort((x, y) => y.Id - x.Id);
    }

    public void UseItem(Item useItem)
    {
        if (items.Find(item => item == useItem).ItemCount > 0)
        {
            // プレイヤーの持っているアイテムを探す
            items.Find(item => item == useItem).ItemCount--;
            if (items.Find(item => item == useItem).ItemCount == 0)
            {
                // アイテムを除去
                items.Remove(useItem);
            }
        }
    }
}

public struct ExpPair
{
    public int oldLevel;// 前のレベル
    public int newLevel;// 新しいレベル
    public List<float> getExp;// 得た経験値
    public List<float> nextExp;// 次の経験値までの経験値
}