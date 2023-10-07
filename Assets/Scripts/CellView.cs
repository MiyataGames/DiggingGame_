using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellView : EnhancedScrollerCellView
{
    public Text text;

    public void SetData(CellData data)
    {
        text.text = data.cellText;
    }
}