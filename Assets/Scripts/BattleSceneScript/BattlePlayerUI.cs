using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattlePlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerLevelText;
    [SerializeField] private HPBar playerHpBar;
    [SerializeField] private SPBar playerSpBar;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI spText;

    [SerializeField] private TextMeshProUGUI currentMaxHpText;
    [SerializeField] private TextMeshProUGUI currentMaxSpText;
    [SerializeField] private GameObject selectedArrow;

    [SerializeField] private Transform playerPos;

    private Player player;

    // プレイヤーの詳しいステータス画面
    //[SerializeField] private GameObject playerDiscriptionPanel;

    /*
        [SerializeField] private Image[] statusSlots;
        [SerializeField] private Sprite[] statusSprites;
        [SerializeField] private Text[] statusTexts;
    */
    public GameObject SelectedArrow { get => selectedArrow; }
    public Transform PlayerPos { get => playerPos; set => playerPos = value; }

    public void SetPlayerData(Player player)
    {
        this.player = player;
        playerNameText.text = player.PlayerBase.PlayerName;
        playerLevelText.text = "Lv." + player.Level;
        playerHpBar.SetHP(player.CurrentHp, player.currentMaxHp);
        playerSpBar.SetSP(player.CurrentSp, player.currentMaxSp);
        hpText.text = player.CurrentHp.ToString();
        spText.text = player.CurrentSp.ToString();
        currentMaxHpText.text = "/" + player.currentMaxHp.ToString();
        currentMaxSpText.text = "/" + player.currentMaxSp.ToString();
        //SetActivenessDiscriptionPanel(false);
    }

    public void UpdateHpSp()
    {
        playerHpBar.SetHP((float)player.CurrentHp, player.currentMaxHp);
        playerSpBar.SetSP((float)player.CurrentSp, player.currentMaxSp);
    }

    /*
        // ステータス詳細画面をセットする関数
        public void SetStatusPanel(Player player)
        {
            // 初期化
            for (int i = 0; i < (int)World.MagicType.END - 1; i++)
            {
                statusSlots[i].sprite = statusSprites[2];
            }

            statusTexts[0].text = player.PlayerBase.PlayerMaxHp.ToString();
            statusTexts[1].text = player.PlayerBase.PlayercurrentMaxSp.ToString();
            statusTexts[2].text = player.PlayerBase.PlayerPhysicAtk.ToString();
            statusTexts[3].text = player.PlayerBase.PlayerPhysicDef.ToString();
            //Debug.Log(player.PlayerBase.PlayerMagicAtk);
            statusTexts[4].text = player.PlayerBase.PlayerMagicAtk.ToString();
            statusTexts[5].text = player.PlayerBase.PlayerMagicDef.ToString();
        }
    */
    /*
        public void SetActivenessDiscriptionPanel(bool activeness)
        {
            playerDiscriptionPanel.SetActive(activeness);
        }
    */
    public void SetActiveSelectedArrow(bool activeness)
    {
        SelectedArrow.SetActive(activeness);
    }
}