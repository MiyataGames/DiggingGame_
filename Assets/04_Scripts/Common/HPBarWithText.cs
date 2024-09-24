using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPBarWithText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI hpMaxText;
    [SerializeField] private Image hpBar;

    public void SetHP(float hp, float maxHp)
    {
        hpText.text = hp.ToString();
        hpMaxText.text = "/"+ maxHp.ToString();
        hpBar.GetComponent<Image>().fillAmount = hp / maxHp;
    }
}