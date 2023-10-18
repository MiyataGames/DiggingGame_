using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCellView : EnhancedScrollerCellView
{
    private ItemCellData itemCellData;
    public GameObject selectedIcon;

    // �A�C�e���̖��O�̃e�L�X�g
    public Text itemNameText;

    // �A�C�e���̐��̃e�L�X�g
    public Text itemCountText;

    public void SetData(ItemCellData data)
    {
        itemCellData = data;
        itemNameText.text = data.itemText;
        itemCountText.text = data.itemCountText;
    }

    // UI�̍X�V
    // RefreshCellView���\�b�h���I�[�o�[���C�h����ƁA
    // �f�[�^���ēǂݍ��݂��邱�ƂȂ�UI���X�V���邱�Ƃ��ł��܂��B
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
}