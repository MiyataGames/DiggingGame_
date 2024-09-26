using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SPBarWithText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI spText;
    [SerializeField] TextMeshProUGUI spMaxText;
    [SerializeField] private Image spBar;

    public void SetSP(float hp, float maxHp)
    {
        spText.text = hp.ToString();
        spMaxText.text = "/" + maxHp.ToString();
        spBar.GetComponent<Image>().fillAmount = hp / maxHp;
    }
}