using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpBar : MonoBehaviour
{
    [SerializeField] private Image playerExpBar;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI remainExpText;

    public Image PlayerExpBar { get => playerExpBar; set => playerExpBar = value; }

    public IEnumerator SetExpSmooth(ExpPair expPair)
    {
        Debug.Log("old" + expPair.oldLevel + "new" + expPair.newLevel);
        float sumNextExp = 0;
        float sumGetExp = 0;
        float currentLevel = expPair.oldLevel;
        levelText.text = "Lv" + currentLevel.ToString();
        // 
        for (int i = 0; i < expPair.nextExp.Count; i++)
        {
            sumNextExp += expPair.nextExp[i];
            sumGetExp += expPair.getExp[i];
        }
        // 経験値初期値
        float currentExp = PlayerExpBar.fillAmount * expPair.nextExp[0];
        Debug.Log("expBarの初期の割合"+playerExpBar.fillAmount);
        Debug.Log("exp初期値" + currentExp);
        
        for (int i = 0; i < expPair.getExp.Count; i++)
        {
            float changeAmount;
            float remainNextExp = expPair.nextExp[i];
            changeAmount = remainNextExp * Time.deltaTime;
            while (currentExp <= expPair.getExp[i])
            {
                currentExp += changeAmount;
                remainNextExp -= changeAmount;
                remainExpText.text = remainNextExp.ToString("f0");
                // 現在のExp/レベルが上がるまでの残りの経験値 ここへん？
                playerExpBar.fillAmount = currentExp / expPair.nextExp[i];
                yield return new WaitForEndOfFrame();
            }
            playerExpBar.fillAmount = expPair.getExp[i] / expPair.nextExp[i];

            if (playerExpBar.fillAmount >= 1)
            {
                playerExpBar.fillAmount = 0;
                currentLevel += 1;
                levelText.text = "Lv" + currentLevel.ToString();
            }
        }
        currentLevel = expPair.newLevel;
        levelText.text = "Lv" + currentLevel.ToString();
        WaitForEndOfFrame coroutine = new WaitForEndOfFrame();
        yield return coroutine;
    }
}