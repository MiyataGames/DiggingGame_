using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusDescriptionUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] TextMeshProUGUI playerLvText;
    [SerializeField] TextMeshProUGUI[] typeTexts;
    [SerializeField] TextMeshProUGUI playerDiscriptionText;
    [SerializeField] TextMeshProUGUI hPText;
    [SerializeField] TextMeshProUGUI maxHpText;
    [SerializeField] TextMeshProUGUI spText;
    [SerializeField] TextMeshProUGUI MaxSpText;
    [SerializeField] TextMeshProUGUI atkText;
    [SerializeField] TextMeshProUGUI defText;
    [SerializeField] TextMeshProUGUI agiText;
    [SerializeField] TextMeshProUGUI remainExpText;// 次のレベルまでの経験値

    [SerializeField] Image overviewImage;
    [SerializeField] Image hpFilledImage;
    [SerializeField] Image spFilledImage;
    [SerializeField] Image expFilledImage;

    public void SetUpStatusDescription(Player player)
    {
        playerNameText.text = player.PlayerBase.PlayerName;
        playerLvText.text = "Lv" + player.level.ToString();
        for(int i = 0; i < typeTexts.Length; i++)
        {
            typeTexts[i].text = "-";
        }
        for(int i = 0;i < player.PlayerBase.WeakTypes.Length; i++)
        {
            typeTexts[(int)player.PlayerBase.WeakTypes[i] - 1].text = "弱";
        }
        for (int i = 0; i < player.PlayerBase.ResistanceTypes.Length; i++)
        {
            typeTexts[(int)player.PlayerBase.ResistanceTypes[i] - 1].text = "耐";
        }
        playerDiscriptionText.text = player.PlayerBase.PlayerDiscription;
        hPText.text = player.currentHP.ToString();
        maxHpText.text = "/" + player.CurrentMaxHp.ToString();
        spText.text = player.currentSP.ToString();
        MaxSpText.text = "/"+ player.CurrentMaxSp.ToString();
        atkText.text = player.currentMaxAtk.ToString();
        defText.text = player.currentMaxDef.ToString();
        agiText.text = player.currentMaxAgi.ToString();
        remainExpText.text = "あと "+(player.NextExp - player.Exp).ToString();
        Debug.Log((float)player.currentHP / (float)player.CurrentMaxHp);

        Debug.Log(overviewImage);
        Debug.Log(player.PlayerBase.PlayerOverView);
        overviewImage.sprite = player.PlayerBase.PlayerOverView;
        //overviewImage.SetNativeSize();
        hpFilledImage.fillAmount = (float)player.currentHP / (float)player.CurrentMaxHp;
        spFilledImage.fillAmount = (float)player.currentSP / (float)player.CurrentMaxSp;
        expFilledImage.fillAmount = (float)player.Exp / (float)player.NextExp;
    }
}
