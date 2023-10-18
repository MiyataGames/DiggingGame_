using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public PlayerBase PlayerBase { get; set; }

    // UI
    public PlayerFieldUI playerUI;

    // �p�����[�^
    public int Level { get; set; }

    public int currentHp { get; set; }
    public int currentSp { get; set; }
    public int Atk { get; set; }
    public int Def { get; set; }
    public int Agi { get; set; }

    // ���x���ɉ�����HP��Ԃ�
    public int currentMaxHp
    {
        get { return Mathf.FloorToInt((PlayerBase.PlayerMaxHp * Level) / 100f) + 10; }
    }

    // ���x���ɉ�����SP��Ԃ�
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
        // ���ƂŃ��x�����Ƃɕς���
        currentHp = 3;
        currentSp = currentMaxSp;
        Atk = PlayerBase.PlayerMaxAtk;
        Def = PlayerBase.PlayerMaxDef;
        Agi = PlayerBase.PlayerMaxAgi;
        // �Z�[�u�f�[�^������΃A�C�e���͈��p���Ȃ���Ώ�����
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