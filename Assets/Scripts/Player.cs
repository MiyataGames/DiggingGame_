


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public int playerID;
    public int level;
    public int currentHP;
    public int currentSP;
    public int atk;
    public int def;
    public int agi;
    // 所持金
    public int gold;

    public PlayerBase PlayerBase { get; set; }

    // UI
    public PlayerFieldUI playerUI;
    // パラメータ
    public int Level { get => level; }

    public int CurrentHp { get => currentHP; set => currentHP = value; }
    public int CurrentSp { get => currentSP; set => currentSP = value; }
    public int Atk { get => atk; set => atk = value; }
    public int Def { get => def; set => def = value; }
    public int Agi { get => agi; set => agi = value; }
    // プレイヤーの所持金
    public int Gold { get => gold; set => gold = value; }
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
        // Debug.Log("ID" + playerID);
        // あとでレベルごとに変える
        this.level = level;
        CurrentHp = 3;
        CurrentSp = currentMaxSp;
        Atk = PlayerBase.PlayerMaxAtk;
        Def = PlayerBase.PlayerMaxDef;
        Agi = PlayerBase.PlayerMaxAgi;
        // セーブデータがあればアイテムは引継ぎなければ初期化
        Items = new List<Item>();
        // 1000Gold持たせておく
        gold = 1000;
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

    public bool TakeHeal(Item healItem)
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
}