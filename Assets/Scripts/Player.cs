using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public PlayerBase PlayerBase { get; set; }

    // UI
    public PlayerFieldUI playerUI;

    // パラメータ
    public int Level { get; set; }

    public int currentHp { get; set; }
    public int currentSp { get; set; }
    public int Atk { get; set; }
    public int Def { get; set; }
    public int Agi { get; set; }

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
    public List<Item> Items { get; set; }

    public Player(PlayerBase pBase, int level)
    {
        isPlayer = true;
        PlayerBase = pBase;
        // あとでレベルごとに変える
        currentHp = 3;
        currentSp = currentMaxSp;
        Atk = PlayerBase.PlayerMaxAtk;
        Def = PlayerBase.PlayerMaxDef;
        Agi = PlayerBase.PlayerMaxAgi;
        // セーブデータがあればアイテムは引継ぎなければ初期化
        Items = new List<Item>();
    }

    public void TakeHeal(Item healItem)
    {
        HealItemBase healItemBase = healItem.ItemBase as HealItemBase;
        if (currentHp + healItemBase.HealPoint > currentMaxHp)
        {
            currentHp = currentMaxHp;
        }
        else
        {
            Debug.Log(currentMaxHp);
            currentHp += healItemBase.HealPoint;
        }
        //Debug.Log(currentHp);
    }
}