using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public delegate void DiggingFinishDelegate();
public delegate bool SelectItemDelegate(Item item);

public class DiggingGridManager : MonoBehaviour
{
    public DiggingFinishDelegate diggingFinishDelegate;
    public SelectItemDelegate selectItemDelegate;

    public GameObject[] buttons;
    public GameObject[,] gridButtons = new GameObject[3,3]; // 3x3のグリッドボタン
    public Item[] gridItems; // グリッド内のアイテムを記録する配列
    private Item selectedItem; // 選択中のアイテムID
    int selectedIndex;
    int selectedItemNum;
    [SerializeField] Button finishButton;

    void Start()
    {
        selectedItemNum = 0;
        gridItems = new Item[9];
        for(int i = 0;i < gridButtons.GetLength(0); i++)
        {
            for(int j = 0; j < gridButtons.GetLength(1); j++)
            {
                Debug.Log((i * gridButtons.GetLength(0))+j);
                gridButtons[i, j] = buttons[(i * gridButtons.GetLength(0)) + j];
                Debug.Log(i + "," + j + ":" + (i * gridButtons.GetLength(0) + j));
            }
        }
        finishButton.gameObject.SetActive(true);
    }

    public void SetSelectedItem(Item item)
    {
        selectedItem = item;
        Debug.Log("選んだアイテムは"+selectedItem);
    }

    public void GridButtonClicked(int index)
    {
        //int row = index / 3;
        //int col = index % 3;

        // 埋まっていなかったらアイテムを埋める
        if (gridItems[index] == null)
        {
            if (selectItemDelegate(selectedItem) == true)
            {
                selectedItemNum++;
                gridItems[index] = selectedItem;
                Image image = buttons[index].GetComponent<Image>();
                TrapItemBase selectedItemBase = selectedItem.ItemBase as TrapItemBase;
                // スプライトをセットする
                image.sprite = selectedItemBase.itemImageSprite;
                // あとでデリゲート
                // アイテムを探して減らす
                /*if (battleSceneManager.mainPlayer.items.Find(item => item.ItemBase.ItemName == selectedItem.ItemBase.ItemName).ItemCount > 0)
                {
                    battleSceneManager.mainPlayer.items.Find(item => item.ItemBase.ItemName == selectedItem.ItemBase.ItemName).ItemCount--;
                    Debug.Log(battleSceneManager.mainPlayer.items.Find(item => item.ItemBase.ItemName == selectedItem.ItemBase.ItemName).ItemCount);
                    // パネルの更新
                }*/
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

    // ボタンを効かないようにする
    private void UnInteractiveButton()
    {
        for (int i = 0; i < gridButtons.GetLength(0); i++) {
            for (int j = 0; j < gridButtons.GetLength(1); j++)
            {
                gridButtons[i,j].GetComponent<Button>().interactable = false;
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
}
