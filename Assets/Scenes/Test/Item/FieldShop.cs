using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FieldShop : MonoBehaviour
{
    public ShopBase shopBase;
    public GameObject shopItemButtonPrefab; // ボタンのプレファブ
    private List<ItemEntry> shopItems = new List<ItemEntry>();
    public GameObject countPanel; // 数量を確認するもの
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    
 

    void Start()
    {
        InitializeShopItems();
        CreateShopButtons();
    }

    // public void ClickConfirm()
    // {
    //     Debug.Log($"sssss");
    // }
    private void InitializeShopItems()
    {
        foreach (var itemEntry in shopBase.ShopItems)
        {
            shopItems.Add(itemEntry);
        }
    }

    private void CreateShopButtons()
    {  
        int buttonSpacing = -40; // ボタン間の間隔
        int currentY = 130; // 現在のY位置

        foreach (var itemEntry in shopItems)
        {
            GameObject buttonObj = Instantiate(shopItemButtonPrefab);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Name: "+itemEntry.item.ItemName + ", Price: " + itemEntry.item.Price;

            buttonObj.GetComponent<Button>().onClick.AddListener(() => OnItemButtonClick(itemEntry));

            // ボタンの位置を設定
            RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(-50, currentY); // X位置を0に設定
            currentY += buttonSpacing;

            // ボタンをキャンバスまたは特定の親要素にアタッチ
            buttonObj.transform.SetParent(this.transform, false);
        }
    }

    public void OnItemButtonClick(ItemEntry item)
    {
        // アイテムがクリックされたときの処理
        // Debug.Log($"Item clicked: {item.item.ItemName}");
        countPanel.SetActive (true);
        
        itemNameText.text = item.item.ItemName;
        itemPriceText.text = item.item.Price.ToString();
    }
    public void ReturnButtonClick()
    {
        // アイテムがクリックされたときの処理
        countPanel.SetActive (false);
    }
}
