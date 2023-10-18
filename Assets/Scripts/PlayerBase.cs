using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Base/PlayerBase")]
public class PlayerBase : ScriptableObject
{
    [SerializeField] private int playerId;
    [SerializeField] private string playerName;
    [SerializeField] private Sprite playerFaceIcon;

    [SerializeField] private int playerMaxLv;
    [SerializeField] private int playerMaxHp;
    [SerializeField] private int playerMaxSp;
    [SerializeField] private int playerMaxAtk;// �U����
    [SerializeField] private int playerMaxDef;// �h���
    [SerializeField] private int playerMaxAgi;// �f����

    public int PlayerId { get => playerId; }
    public string PlayerName { get => playerName; }
    public Sprite PlayerFaceIcon { get => playerFaceIcon; }
    public int PlayerMaxLv { get => playerMaxLv; }
    public int PlayerMaxHp { get => playerMaxHp; }
    public int PlayerMaxSp { get => playerMaxSp; }
    public int PlayerMaxAtk { get => playerMaxAtk; }
    public int PlayerMaxDef { get => playerMaxDef; }
    public int PlayerMaxAgi { get => playerMaxAgi; }
}