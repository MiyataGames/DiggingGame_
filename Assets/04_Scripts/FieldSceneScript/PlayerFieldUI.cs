using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerFieldUI : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private TextMeshProUGUI statusPlayerNameText;
    /*
    [SerializeField] private TextMeshProUGUI statusPlayerHpText;
    [SerializeField] private TextMeshProUGUI statusPlayerMaxHpText;
    [SerializeField] private TextMeshProUGUI statusPlayerSpText;
    [SerializeField] private TextMeshProUGUI statusPlayerMaxSpText;*/
    [SerializeField] private Image statusIconImage;
    [SerializeField] public GameObject frame;

    [SerializeField] private HPBarWithText playerHpBar;
    [SerializeField] private SPBarWithText playerSpBar;

    public void SetPlayerStatus(Player player)
    {
        this.player = player;
        statusPlayerNameText.text = player.PlayerBase.PlayerName;
        statusIconImage.sprite = player.PlayerBase.PlayerFaceIcon;
    }

    public void UpdateHpSp()
    {
        playerHpBar.SetHP((float)player.currentHP, player.CurrentMaxHp);
        playerSpBar.SetSP((float)player.currentSP, player.CurrentMaxSp);
    }

    public void SetActivateSelectedFrame(bool activate)
    {
        frame.SetActive(activate);
    }
}