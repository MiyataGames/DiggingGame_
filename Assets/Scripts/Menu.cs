using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuCommand
{
    ITEM,
    STATUS,
    EQUIPMENT,
    SETTING,
    END
}

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject itemPanel;
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private GameObject[] menuSelectArrow;

    // メインパネルをオンオフする関数
    public void ActivateMenuPanel(bool activate)
    {
        menuPanel.SetActive(activate);
    }

    // アイテムパネルをオンオフする
    public void ActivateItemPanel(bool activate)
    {
        itemPanel.SetActive(activate);
    }

    // ステータスパネルをオンオフする
    public void ActivateStatusPanel(bool activate)
    {
        statusPanel.SetActive(activate);
    }

    // メインパネルの矢印をオンオフする
    public void ActivateMenuSelectArrow(MenuCommand menuCommand)
    {
        for (int i = 0; i < menuSelectArrow.Length; i++)
        {
            menuSelectArrow[i].SetActive(false);
        }
        menuSelectArrow[((int)menuCommand)].SetActive(true);
    }
}