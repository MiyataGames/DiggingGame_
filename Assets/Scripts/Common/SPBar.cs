using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SPBar : MonoBehaviour
{
    [SerializeField] private Image spBar;

    public void SetSP(float hp, float maxHp)
    {
        spBar.GetComponent<Image>().fillAmount = hp / maxHp;
    }
}