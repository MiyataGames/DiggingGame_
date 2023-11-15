using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character
{
    public bool isPlayer;// いらないかも

    public bool isDying;// 瀕死状態か

    public bool isFainted;//気絶状態か

    public int level;
    public int currentHP;
    public int currentSP;
    public int atk;
    public int def;
    public int agi;
    public List<EnemySkill> Skills { get; set; }//スキル

    // ステータス変化のレベル
    private int atkChangeLevel = 0;
    private int defChangeLevel = 0;
    private int agiChangeLevel = 0;

    /// <summary>
	/// レベルに応じた初期値を設定する関数
	/// </summary>
    public virtual void InitStatusValue(int level)
	{

	}

    // ステータス変化技の関数
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
}