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
    private ItemEntry selectedItemEntry;


    public Canvas ShopHud;
    public Canvas BuyHud;
    public Canvas SellHud;
    public Canvas CommandHud;

    public TextMeshProUGUI itemNameField ;
    public TextMeshProUGUI itemPriceField;
    public TextMeshProUGUI quantityField;
    int quantity = 1;
    int totalPrice ;

    int choiceId = -1;


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
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Name: " + itemEntry.item.ItemName + ", Price: " + itemEntry.item.Price;

            buttonObj.GetComponent<Button>().onClick.AddListener(() => OnItemButtonClick(itemEntry));

            // ボタンの位置を設定
            RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(-50, currentY); // X位置を0に設定
            currentY += buttonSpacing;

            // ボタンをキャンバスまたは特定の親要素にアタッチ
            // ボタンの親をキャンバスに設定
            buttonObj.transform.SetParent(BuyHud.transform, false);
        }
    }

    public void OnItemButtonClick(ItemEntry itemEntry)
    {
        selectedItemEntry = itemEntry;

        // 初期化
        if (choiceId != itemEntry.item.Id)
        {
            quantity = 1;
        }

        // アイテムがクリックされたときの処理
        // Debug.Log($"Item clicked: {item.item.ItemName}");

        itemNameField.text = itemEntry.item.ItemName;
        itemPriceField.text = (itemEntry.item.Price * quantity).ToString();
        quantityField.text = quantity.ToString();

        choiceId = itemEntry.item.Id;
    }
    public void UpButtonClick()
    {
        quantity++;
        OnItemButtonClick(selectedItemEntry);
    }
    public void DownButtonClick()
    {
        quantity--;
        OnItemButtonClick(selectedItemEntry);
    }
    public void ReturnButtonClick()
    {
        // アイテムがクリックされたときの処理
        CommandHud.enabled = true;
        BuyHud.enabled = false;
        SellHud.enabled = false;
    }
    public void OnBuyButtonClick()
    {
        selectedItemEntry = shopItems[0];
        itemPriceField.text = selectedItemEntry.item.Price.ToString();
        Debug.Log("s");
        CommandHud.enabled = false;
        BuyHud.enabled = true;
        
    }
    public void OnSellButtonClick()
    {
        Debug.Log("sle");
        CommandHud.enabled = false;
        SellHud.enabled = true;
    }
    public void OnEndButtonClick()
    {
        Debug.Log("end");
        ShopHud.enabled = false;
    }
}
