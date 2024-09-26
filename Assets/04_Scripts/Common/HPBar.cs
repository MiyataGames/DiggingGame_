using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Image hpBar;

    public void SetHP(float hp, float maxHp)
    {
        hpBar.GetComponent<Image>().fillAmount = hp / maxHp;
    }
}