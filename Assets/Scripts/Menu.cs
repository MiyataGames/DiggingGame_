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


public delegate void MenuSelectButtonClickedDelegate(int menuIndex);
public delegate void ItemSelectButtonHoverdDelegate(int itemIndex);

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject itemPanel;
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private GameObject[] menuSelectArrow;

    // メニューを選択したとき（ボタンを押したときに）に呼ばれる関数をいれるデリゲート
    public MenuSelectButtonClickedDelegate menuSelectButtonClickedDelegate;
    public ItemSelectButtonHoverdDelegate itemSelectButtonHoverdDelegate;

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
    public void ActivateMenuSelectArrow(int menuCommand)
    {
        for (int i = 0; i < menuSelectArrow.Length; i++)
        {
            menuSelectArrow[i].SetActive(false);
        }
        menuSelectArrow[(int)menuCommand].SetActive(true);
    }

    // メニューを開いているとき、メニューのボタンを押したときに呼ばれる関数
    public void OnClickMenu(int menuIndex)
    {
        if (menuSelectButtonClickedDelegate != null) menuSelectButtonClickedDelegate(menuIndex);
    }
    public void OnHoverItem(int itemIndex)
    {
        if (itemSelectButtonHoverdDelegate != null) itemSelectButtonHoverdDelegate(itemIndex);
    }
}