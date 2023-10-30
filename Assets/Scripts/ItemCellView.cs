using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public delegate void CellButtonClickedDelegate(int value);
public class ItemCellView : EnhancedScrollerCellView
{
    private ItemCellData itemCellData;
    public CellButtonClickedDelegate cellButtonClicked;
    public GameObject selectedIcon;

    // These delegates will publish events when a button is clicke

    // アイテムの名前のテキスト
    public Text itemNameText;

    // アイテムの数のテキスト
    public Text itemCountText;

    public void SetData(ItemCellData data)
    {
        itemCellData = data;
        itemNameText.text = data.itemText;
        itemCountText.text = data.itemCountText;
    }

    // UIの更新
    // RefreshCellViewメソッドをオーバーライドすると、
    // データを再読み込みすることなくUIを更新することができます。
    public override void RefreshCellView()
    {
        //base.RefreshCellView();
        if (itemCellData.isSelected == true)
        {
            selectedIcon.SetActive(true);
        }
        else
        {
            selectedIcon.SetActive(false);
        }
    }

    public void CellButton_OnClick(int selectedItemIndex)
    {
        // fire event if anyone has subscribed to it
        if (cellButtonClicked != null) cellButtonClicked(selectedItemIndex);
    }

}