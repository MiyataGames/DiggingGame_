using UnityEngine;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;

// IEnhancedScrollerDelegateインタフェースを継承していることに注意！
public class ScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
{
    public FieldShop fieldShop; // FieldShop クラスの参照
    private List<ScrollerData> _data; // ここで変数を定義
    public ShopBase shopBase;

    public EnhancedScroller myScroller;
    public CallView cellViewPrefab;

void Start()
{
    _data = new List<ScrollerData>();
            Debug.Log(fieldShop.ShopItems.Count);
    if (fieldShop.ShopItems == null)
    {
        Debug.Log("null");
    }
    Debug.Log("aaa");
    foreach (var item in fieldShop.ShopItems)
    {
        Debug.Log("bbb");
        _data.Add(new ScrollerData() { cellText = item.ItemBase.ItemName});
    }
    myScroller.Delegate = this;
    myScroller.ReloadData();
}


    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return fieldShop.ShopItems.Count;
        // return _data.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 50f;
    }

public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
{
    Debug.Log("qqq");
    CallView cell = scroller.GetCellView(cellViewPrefab) as CallView;
    cell.SetData(_data[dataIndex]);
    return cell;
}
}