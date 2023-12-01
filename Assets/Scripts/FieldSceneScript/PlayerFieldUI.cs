using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFieldUI : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Text statusPlayerNameText;
    [SerializeField] private Text statusPlayerHpText;
    [SerializeField] private Text statusPlayerMaxHpText;
    [SerializeField] private Text statusPlayerSpText;
    [SerializeField] private Text statusPlayerMaxSpText;
    [SerializeField] private Image statusIconImage;
    [SerializeField] public GameObject frame;

    [SerializeField] private HPBar playerHpBar;
    [SerializeField] private SPBar playerSpBar;

    public void SetPlayerStatus(Player player)
    {
        this.player = player;
        statusPlayerNameText.text = player.PlayerBase.PlayerName;
        statusPlayerHpText.text = player.currentHP.ToString() + " / ";
        statusPlayerMaxHpText.text = player.currentMaxHp.ToString();
        statusPlayerSpText.text = player.currentSP.ToString() + " / ";
        statusPlayerMaxSpText.text = player.currentMaxSp.ToString();
        statusIconImage.sprite = player.PlayerBase.PlayerFaceIcon;
    }

    public void UpdateHpSp()
    {
        playerHpBar.SetHP((float)player.currentHP, player.currentMaxHp);
        playerSpBar.SetSP((float)player.currentSP, player.currentMaxSp);
        statusPlayerHpText.text = player.currentHP.ToString() + " / ";
        statusPlayerMaxHpText.text = player.currentMaxHp.ToString();
        statusPlayerSpText.text = player.currentSP.ToString() + " / ";
        statusPlayerMaxSpText.text = player.currentMaxSp.ToString();
    }

    public void SetActivateSelectedFrame(bool activate)
    {
        frame.SetActive(activate);
    }
}