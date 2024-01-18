using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultPlayerUI : MonoBehaviour
{
    [SerializeField] private ExpBar playerExpBar;
    [SerializeField] private TextMeshProUGUI playerNameText;
    private Player player;

    public void SetUpResultPanel(Player player)
    {
        this.player = player;

    }

    public IEnumerator UpdateExp(ExpPair expPair)
    {
        //playerExpBar.PlayerExpBar.fillAmount = (float)player.Exp / (float)player.NextExp;
        Debug.Log("現在の経験値"+player.Exp);
        Debug.Log("割合"+(float)player.Exp/(float)player.NextExp);
        for(int i = 0; i < expPair.getExp.Count; i++)
        {
            Debug.Log("i"+i+"得た経験値"+expPair.getExp[i]);
            Debug.Log("i" + i + "次までの経験値" + expPair.nextExp[i]);
        }
        IEnumerator enumerator = playerExpBar.SetExpSmooth(expPair);
        yield return enumerator;
    }

    public void SetPlayerNameText()
    {
        playerNameText.text = player.PlayerBase.PlayerName;
    }
}