using TMPro;
using EnhancedUI.EnhancedScroller;

// EnhancedScrollerCellViewを継承している点に注意！
public class CallView : EnhancedScrollerCellView
{
    // CellViewの子オブジェクトのTextをリンクする
    public TextMeshProUGUI text;

    public void SetData(ScrollerData data)
    {
        text.text = data.cellText;
    }
}
 