using TMPro;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

public delegate void CellButtonIntegerClickedDelegate(int value);

// EnhancedScrollerCellViewを継承している点に注意！
public class CallView : EnhancedScrollerCellView
{
    // CellViewの子オブジェクトのTextをリンクする
    public TextMeshProUGUI text;
    public CellButtonIntegerClickedDelegate cellButtonDataIntegerClicked;
    private int cellId; // このセルのID

    public void SetData(ScrollerData data)
    {
        text.text = data.cellText;
        cellId = data.id;
    }
    public void CellButtonDataInteger_OnClick()
    {
        Debug.Log($"Cell Button Clicked: ID = {cellId}");
        cellButtonDataIntegerClicked(cellId);
    }
}
 