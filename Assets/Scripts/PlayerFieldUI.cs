using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFieldUI : MonoBehaviour
{
    [SerializeField] private Text statusPlayerNameText;
    [SerializeField] private Text statusPlayerHpText;
    [SerializeField] private Text statusPlayerMaxHpText;
    [SerializeField] private Text statusPlayerSpText;
    [SerializeField] private Text statusPlayerMaxSpText;
    [SerializeField] private Image statusIconImage;
    [SerializeField] private GameObject frame;

    private HPBar playerHpBar;
    private SPBar playerSpBar;

    public void SetPlayerStatus(Player player)
    {
        statusPlayerNameText.text = player.PlayerBase.PlayerName;
        statusPlayerHpText.text = player.currentHp.ToString() + " / ";
        statusPlayerMaxHpText.text = player.currentMaxHp.ToString();
        statusPlayerSpText.text = player.currentSp.ToString() + " / ";
        statusPlayerMaxSpText.text = player.currentMaxSp.ToString();
        statusIconImage.sprite = player.PlayerBase.PlayerFaceIcon;
    }

    public void UpdateHpSp(Player player)
    {
        playerHpBar.SetHp((float)player.currentHp, player.currentMaxHp);
        playerSpBar.SetSp((float)player.currentSp, player.currentMaxSp);
    }

    public void SetActivateSelectedFrame(bool activate)
    {
        frame.SetActive(activate);
    }
}