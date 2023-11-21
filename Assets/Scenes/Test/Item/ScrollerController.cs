using UnityEngine;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;

// IEnhancedScrollerDelegateインタフェースを継承していることに注意！
public class ScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
{
    private List<ScrollerData> _data;

    public EnhancedScroller myScroller;
    public CallView cellViewPrefab;

    void Start()
    {
        _data = new List<ScrollerData>()
        {
            new ScrollerData() { cellText = "Lion" },
            new ScrollerData() { cellText = "Bear" },
            new ScrollerData() { cellText = "Eagle" },
            new ScrollerData() { cellText = "Dolphin" },
            new ScrollerData() { cellText = "Ant" },
            new ScrollerData() { cellText = "Cat" },
            new ScrollerData() { cellText = "Sparrow" },
            new ScrollerData() { cellText = "Dog" },
            new ScrollerData() { cellText = "Spider" },
            new ScrollerData() { cellText = "Elephant" },
        };

        myScroller.Delegate = this;
        myScroller.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _data.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 100f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        CallView cell = scroller.GetCellView(cellViewPrefab) as CallView;
        cell.SetData(_data[dataIndex]);
        return cell;
    }
}
