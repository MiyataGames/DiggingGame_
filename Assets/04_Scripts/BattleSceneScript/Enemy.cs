using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy : Character
{
    [SerializeField] private EnemyBase enemyBase;

    // ベースとなるデータ
    public EnemyBase EnemyBase { get => enemyBase; }
    private string enemyBattleName;// 敵のバトル上での名前
    int dropGold;// ドロップするお金
    List<Item> dropItems = new List<Item>();// ドロップするアイテム
    bool counted;

    public int Level { get => level; }
    public GameObject EnemyPrefab { get; set; }
    public Animator EnemyAnimator { get; set; }
    public BattleEnemyUI EnemyUI { get; set; }
    public int positionIndex;

    public string EnemyBattleName { get => enemyBattleName; set => enemyBattleName = value; }
    public bool Counted { get => counted; set => counted = value; }
    public List<Item> DropItems { get => dropItems;  }
    public int DropGold { get => dropGold; }


    // コンストラクタ:生成時の初期設定
    public Enemy(EnemyBase eBase)
    {
        // ステータスの初期化
        isPlayer = false;
        enemyBase = eBase;
        // ドロップするものの決定
        dropGold = Random.Range(enemyBase.MinDropGold, enemyBase.MaxDropGold);
        List<Item> items = new List<Item>();
        for(int i = 0;i < enemyBase.DropItemBase.Count; i++)
        {
            Item item = new Item(enemyBase.DropItemBase[i]);
            items.Add(item);
        }
        // ドロップするアイテムの数 0か1こ
        int dropItemNum = Random.Range(0, 2);
        Debug.Log("落とすアイテムの数は"+dropItemNum);
        for(int i = 0;i<dropItemNum; i++)
        {
            int itemNumber = Random.Range(0, items.Count);
            Item item = items[itemNumber];
            Debug.Log("落とすアイテムの名前は" + item.ItemBase.ItemName);
            DropItems.Add(item);
        }
        characterName = enemyBase.EnemyName;
        level = enemyBase.Level;
        currentHP = enemyBase.MaxHp;
        currentMaxHp = enemyBase.MaxHp;
        currentMaxAtk = EnemyBase.Atk;
        currentMaxDef = enemyBase.Def;
        currentMaxAgi = EnemyBase.Agi;
        atk = currentMaxAtk;
        def = currentMaxDef;
        agi = currentMaxAgi;
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
        
        if (isEffective(playerSkill))
        {
            effectiveness = 1.2f;
        }
        else if (isResist(playerSkill))
        {
            effectiveness = 0.7f;
        }

        float skillPower = playerSkill.skillBase.Power;// スキル倍率

        int damage = 0;
        // デバッグモードはプレイヤーの技が強くて敵がめっちゃ弱い
        if (GameManager.instance.playMode == PlayMode.DEBUG)
        {
            damage = 100;
        }
        else if(GameManager.instance.playMode == PlayMode.RELEASE)
        {
            Debug.Log("効果量" + effectiveness);
            float randSeed = Random.Range(0.83f, 1.17f);
            // （攻撃力/2-守備力/4）×変数(5/6~7/6) x 属性効果量
            damage = (int)((player.atk/2 - def/4) * randSeed *skillPower * effectiveness);
        }
        Debug.Log("ダメージ" + damage);

        currentHP -= damage;
        Debug.Log("現在の体力"+currentHP);
        if (currentHP <= 0)
        {
            currentHP = 0;
            return true;
        }

        return false;
    }

    // damage
    public override bool TakeItemDamage(int damageRatio, Character turnCharacter)
    {
        // 合計ダメージ = (基本ダメージ + 攻撃力修正 + レベル修正) × クリティカルヒット倍率 - 敵の防御力
        int damage = 0; // + turnCharacter.atk + turnCharacter.level;
        float randSeed = Random.Range(0.83f, 1.17f);
        // （攻撃力/2-守備力/4）×変数(5/6~7/6)
        damage = (int)((turnCharacter.atk / 2 - def / 4) * randSeed * damageRatio * 2);
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