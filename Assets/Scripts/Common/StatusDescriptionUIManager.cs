using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusDescriptionUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] TextMeshProUGUI playerLvText;
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
        playerDiscriptionText.text = player.PlayerBase.PlayerDiscription;
        hPText.text = player.currentHP.ToString();
        maxHpText.text = "/" + player.currentMaxHp.ToString();
        spText.text = player.currentSP.ToString();
        MaxSpText.text = "/"+ player.currentMaxSp.ToString();
        atkText.text = player.currentMaxAtk.ToString();
        defText.text = player.currentMaxDef.ToString();
        agiText.text = player.currentMaxAgi.ToString();
        remainExpText.text = "あと "+(player.NextExp - player.Exp).ToString();
        Debug.Log((float)player.currentHP / (float)player.currentMaxHp);

        Debug.Log(overviewImage);
        Debug.Log(player.PlayerBase.PlayerOverView);
        overviewImage.sprite = player.PlayerBase.PlayerOverView;
        //overviewImage.SetNativeSize();
        hpFilledImage.fillAmount = (float)player.currentHP / (float)player.currentMaxHp;
        spFilledImage.fillAmount = (float)player.currentSP / (float)player.currentMaxSp;
        expFilledImage.fillAmount = (float)player.Exp / (float)player.NextExp;
    }
}
