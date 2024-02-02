using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character
{
    public bool isPlayer;// いらないかも

    public bool isDying;// 瀕死状態か

    public bool isFainted;//気絶状態か

    public string characterName;
    public int level;
    public int currentMaxHp;
    public int currentMaxSp;
    public int currentHP;
    public int currentSP;

    public int currentMaxAtk;
    public int currentMaxDef;
    public int currentMaxAgi;
    public int atk;
    public int def;
    public int agi;
    public List<EnemySkill> Skills { get; set; }//スキル

    // ステータス変化のレベル
    private int atkChangeLevel = 0;
    private int defChangeLevel = 0;
    private int agiChangeLevel = 0;


    public bool isParalyzed;
    // 状態異常リスト
    public List<StatusCondition> conditions = new List<StatusCondition>();

    /// <summary>
    /// スキルによるダメージ 戦闘不能になったらtrueを返す
    /// </summary>
    /// <param name="enemySkill"></param>
    /// <param name="character"></param>
    /// <returns></returns>
    public virtual bool TakeSkillDamage(EnemySkill enemySkill, Character character)
    {
        return false;
    }
    public virtual bool TakeItemDamage(int damage, Character turnCharacter)
    {
        return false;
    }
    /// <summary>
    /// 状態異常によるダメージ 戦闘不能になったらtrueを返す
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public bool TakeConditionDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            return true;
        }
        return false;
    }

    /// <summary>
    /// // ステータス変化技の関数
    /// </summary>
    /// <param name="status">変化させるステータス</param>
    /// <param name="skillStatusKind">ステータスの下降か上昇か</param>
    #region
    // 任意のステータスを任意の方向に変更する関数
    public void ChangeStatus(STATUS status, SKILL_STATUS_KIND skillStatusKind)
    {
        // ステータス上昇だったら
        if (skillStatusKind == SKILL_STATUS_KIND.UP)
        {
            if (status == STATUS.ATTACK)
            {
                IncreaseAtk();
                return;
            }
            else if (status == STATUS.DEFENSE)
            {
                IncreaseDef();
                return;
            }
            else if (status == STATUS.SPEED)
            {
                IncreaseAgi();
                return;
            }
        }
        // ステータス下降だったら
        else
        {
            if (status == STATUS.ATTACK)
            {
                DecreaseAtk();
                return;
            }
            else if (status == STATUS.DEFENSE)
            {
                DecreaseDef();
                return;
            }
            else if (status == STATUS.SPEED)
            {
                DecreaseAgi();
                return;
            }
        }
    }

    public void ResetAtkStatus()
    {
        Debug.Log("攻撃力を戻したよ");
        if (atkChangeLevel != 0)
        {
            atk = currentMaxAtk;
            atkChangeLevel = 0;
        }
    }
    public void ResetDefStatus()
    {
        Debug.Log("防御力を戻したよ");
        if (defChangeLevel != 0)
        {
            def = currentMaxDef;
            defChangeLevel = 0;
        }
    }
    public void ResetAgiStatus()
    {
        Debug.Log("素早さを戻したよ");
        if (agiChangeLevel != 0)
        {
            agi = currentMaxAgi;
            agiChangeLevel = 0;
        }
    }

    // 攻撃力を上昇させるメソッド
    public void IncreaseAtk()
    {
        if (atkChangeLevel >= 2) // すでに最大変化レベルに達している場合、技を使用しない
        {
            Debug.Log("使えないよ");
        }

        atkChangeLevel++;
        atk = (int)(atk + (atk * 0.1f * atkChangeLevel));
        Debug.Log("攻撃力を" + atkChangeLevel + "段階あげたよ");
    }

    // 攻撃力を下降させるメソッド
    public bool DecreaseAtk()
    {
        if (atkChangeLevel <= -2) // すでに最小変化レベルに達している場合、技を使用しない
        {
            return false;
        }

        atkChangeLevel--;
        atk = (int)(atk + (atk * 0.1f * atkChangeLevel));
        Debug.Log("攻撃力を" + atkChangeLevel + "段階下げたよ");
        return true;
    }

    // 防御力を上昇させるメソッド
    public bool IncreaseDef()
    {
        if (defChangeLevel >= 2) // すでに最大変化レベルに達している場合、技を使用しない
        {
            return false;
        }

        defChangeLevel++;
        def = (int)(def + (def * 0.1f * defChangeLevel));
        Debug.Log("防御力を" + defChangeLevel + "段階あげたよ");
        return true;
    }

    // 防御力を下降させるメソッド
    public bool DecreaseDef()
    {
        if (defChangeLevel <= -2) // すでに最小変化レベルに達している場合、技を使用しない
        {
            return false;
        }

        defChangeLevel--;
        def = (int)(def + (def * 0.1f * defChangeLevel));
        Debug.Log("防御力を" + defChangeLevel + "段階下げたよ");

        return true;
    }
    // スピードを上昇させるメソッド
    public bool IncreaseAgi()
    {
        if (agiChangeLevel >= 2) // すでに最大変化レベルに達している場合、技を使用しない
        {
            return false;
        }

        agiChangeLevel++;
        agi = (int)(agi + (agi * 0.1f * agiChangeLevel));
        Debug.Log("スピードを" + agiChangeLevel + "段階あげたよ");
        return true;
    }

    // スピードを下降させるメソッド
    public bool DecreaseAgi()
    {
        if (agiChangeLevel <= -2) // すでに最小変化レベルに達している場合、技を使用しない
        {
            return false;
        }

        agiChangeLevel--;
        agi = (int)(agi + (agi * 0.1f * agiChangeLevel));
        Debug.Log("スピードを" + agiChangeLevel + "段階下げたよ");
        return true;
    }

    public int GetStatChangeLevel(STATUS STATUS)
    {
        switch (STATUS)
        {
            // ATTCKだったら攻撃力の上昇（下降）レベルを返す
            case STATUS.ATTACK:
                return atkChangeLevel;
            // DEFENSEだったら防御力の上昇（下降）レベルを返す
            case STATUS.DEFENSE:
                return defChangeLevel;
            // SPEEDだったら速度の上昇（下降）レベルを返す
            case STATUS.SPEED:
                return agiChangeLevel;
            default:
                throw new ArgumentOutOfRangeException(nameof(STATUS), STATUS, null);
        }
    }
    #endregion
    // 状態異常の追加
    public void AddCondition(StatusCondition condition)
    {
        conditions.Add(condition);
        switch (condition.type)
        {
            case STATUS_CONDITION_TYPE.POISON:
                Debug.Log("毒にかかった");
                break;
            case STATUS_CONDITION_TYPE.PARALYSIS:
                Debug.Log("まひにかかった");
                break;
            case STATUS_CONDITION_TYPE.BURN:
                ChangeStatus(STATUS.ATTACK, SKILL_STATUS_KIND.DOWN);
                Debug.Log("やけどにかかった");
                break;
            case STATUS_CONDITION_TYPE.FREEZE:
                Debug.Log("凍結にかかった");
                break;
            case STATUS_CONDITION_TYPE.SLEEP:
                Debug.Log("睡眠にかかった");
                break;

        }
    }

    // 状態異常の更新
    public void UpdateConditions()
    {
        for (int i = conditions.Count - 1; i >= 0; i--)
        {
            conditions[i].UpdateCondition(this);
            Debug.Log("残り" + conditions[i].duration);
            if (conditions[i].duration <= 0)
            {
                switch (conditions[i].type)
                {
                    case STATUS_CONDITION_TYPE.POISON:
                        Debug.Log("毒が解けた");
                        break;
                    case STATUS_CONDITION_TYPE.PARALYSIS:
                        Debug.Log("まひが解けた");
                        break;
                    case STATUS_CONDITION_TYPE.BURN:
                        Debug.Log("やけどが解けた");
                        break;
                    case STATUS_CONDITION_TYPE.FREEZE:
                        Debug.Log("凍結が解けた");
                        break;
                    case STATUS_CONDITION_TYPE.SLEEP:
                        Debug.Log("睡眠が解けた");
                        break;

                }
                conditions.RemoveAt(i);
            }
        }
    }

    // 麻痺状態かどうかをチェックするメソッド
    public bool IsCharacterParalyzed()
    {
        foreach (var condition in conditions)
        {
            if (condition is ParalysisCondition)
            {
                return true;
            }
        }
        return false;
    }

    // 麻痺状態かどうかをチェックするメソッド
    public bool IsCharacterSleeped()
    {
        foreach (var condition in conditions)
        {
            if (condition is SleepCondition)
            {
                return true;
            }
        }
        return false;
    }

}