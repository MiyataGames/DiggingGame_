using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public delegate void DiggingFinishDelegate();
public delegate bool SelectItemDelegate(Item item);
public delegate void EnbeddingItemDelegate(Item item);

public class DiggingGridManager : MonoBehaviour
{
    public DiggingFinishDelegate diggingFinishDelegate;
    public SelectItemDelegate selectItemDelegate;

    public GameObject[] buttons;
    public GameObject[,] gridButtons = new GameObject[3, 3]; // 3x3のグリッドボタン
    public Item[] gridItems; // グリッド内のアイテムを記録する配列
    private Item selectedItem; // 選択中のアイテムID
    private int selectedIndex;
    private int selectedItemNum;
    public Sprite soilImage;
    public Sprite holeImage;
    public Sprite transparentImage;
    [SerializeField] private Button finishButton;

    private void Awake()
    {
        selectedItemNum = 0;
        gridItems = new Item[9];
        for (int i = 0; i < gridButtons.GetLength(0); i++)
        {
            for (int j = 0; j < gridButtons.GetLength(1); j++)
            {
                // Debug.Log((i * gridButtons.GetLength(0)) + j);
                gridButtons[i, j] = buttons[(i * gridButtons.GetLength(0)) + j];
                // Debug.Log(i + "," + j + ":" + (i * gridButtons.GetLength(0) + j));
            }
        }
        finishButton.gameObject.SetActive(true);
    }

    public void SetSelectedItem(Item item)
    {
        selectedItem = item;
        // Debug.Log("選んだアイテムは" + selectedItem.ItemBase.ItemName);
    }

    public void GridButtonClicked(int index)
    {
        // Debug.Log("選んだぼたんは" + index);
        //int row = index / 3;
        //int col = index % 3;

        // 埋まっていなかったらアイテムを埋める
        
        if (gridItems[index] == null)
        {
            
            if (selectItemDelegate(selectedItem) == true)
            {
                selectedItemNum++;
                gridItems[index] = selectedItem;
                Image soilImage = buttons[index].GetComponent<Image>();
                Image itemImage = buttons[index].transform.GetChild(0).GetComponent<Image>();
                Debug.Log(selectedItem.ItemBase.ItemName);
                TrapItemBase selectedItemBase = selectedItem.ItemBase as TrapItemBase;
                // スプライトをセットする
                Debug.Log("イメージ" + soilImage);
                Debug.Log(selectedItemBase.ItemName);
                Debug.Log("アイテムのイメージ" + selectedItemBase.itemImageSprite);
                // 穴の画像にする
                soilImage.sprite = holeImage;
                // アイテムの画像を入れる
                itemImage.sprite = selectedItemBase.itemImageSprite;

                if (selectedItemNum >= 3)
                {
                    OnFinishDigging();
                }
            }
            else
            {
                Debug.Log("使えないよ！");
            }
        }
    
        /*
        if (selectedItem >= 0)
        {
            // アイテムをグリッドに設定
            gridItems[row, col] = selectedItem;

            // ここでUIを更新するなどの処理を追加
            Debug.Log($"Item {selectedItem} placed at [{row}, {col}]");
        }*/
        //Debug.Log(gridItems.Count(item => item.ItemBase.ItemName == null));
        // 三つ埋めたら
        /*
        if(gridItems.Count(item => item == null) == 6)
        {
            FinishDigging();
        }*/
        
    }

    // ボタンをきくようにする
    public void StartDigging()
    {
        selectedItemNum = 0;
        for (int i = 0; i < gridButtons.GetLength(0); i++)
        {
            for (int j = 0; j < gridButtons.GetLength(1); j++)
            {
                gridButtons[i, j].gameObject.GetComponent<Image>().sprite = soilImage;
                gridButtons[i, j].GetComponent<Button>().interactable = true;
            }
        }
        finishButton.gameObject.SetActive(true);
    }

    // ボタンを効かないようにする
    private void UnInteractiveButton()
    {
        for (int i = 0; i < gridButtons.GetLength(0); i++)
        {
            for (int j = 0; j < gridButtons.GetLength(1); j++)
            {
                gridButtons[i, j].GetComponent<Button>().interactable = false;
            }
        }
    }

    // battleSceneManagerのFinishDiggingを実行
    public void OnFinishDigging()
    {
        if (diggingFinishDelegate != null) diggingFinishDelegate();
        finishButton.gameObject.SetActive(false);
        UnInteractiveButton();
    }

    // アイテムを消費
    public void UseGridItem(int position)
    {
        Image itemImage = buttons[position].transform.GetChild(0).GetComponent<Image>();
        itemImage.sprite = transparentImage;
        gridItems[position] = null;
    }
}