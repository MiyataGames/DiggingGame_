using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SPBar : MonoBehaviour
{
    [SerializeField] private Image spBar;
    [SerializeField] private Text spText;
    [SerializeField] private Text maxSpText;

    public void SetSP(float hp, float maxHp)
    {
        spBar.GetComponent<Image>().fillAmount = hp / maxHp;
        spText.text = hp.ToString();
        maxSpText.text = maxHp.ToString();
    }
}