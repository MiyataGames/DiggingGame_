using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FieldShop : MonoBehaviour
{
    public ShopBase shopBase;
    public GameObject shopItemButtonPrefab; // ボタンのプレファブ
    private List<Item> shopItems = new List<Item>();
    private PlayerController playerController;

    private Item selectedItemEntry;
    public Canvas ShopHud;
    public Canvas BuyHud;
    public Canvas SellHud;
    public Canvas CommandHud;

    public TextMeshProUGUI itemNameField ;
    public TextMeshProUGUI moneyField ;
    public TextMeshProUGUI itemPriceField;
    public TextMeshProUGUI quantityField;
    
    int quantity = 1;
    int totalPrice ;

    int choiceId = -1;

    //なぞ
    public Item item;

     [SerializeField] Party party;
    
    void Start()
    { 
        // GameManagerに後で移す
        party.Setup();
        party = party.GetComponent<Party>();
        InitializeShopItems();
        CreateShopButtons();
    }

    // public void ClickConfirm()
    // {
    //     Debug.Log($"sssss");
    // }
    private void InitializeShopItems()
    {
        foreach (var newItem in shopBase.ShopItems)
    {
        // Item.item は ItemBase オブジェクトです
        /*
        Item newItem = new Item(newItemBase); // Itemオブジェクトを作成
        Item newShopItemEntry = new Item(newItem.ItemBase); // 新しいItemEntryを作成
        shopItems.Add(newShopItemEntry); // ItemEntryをリストに追加
        */
        shopItems.Add(newItem);
    }
    }

    private void CreateShopButtons()
    {
        int buttonSpacing = -40; // ボタン間の間隔
        int currentY = 130; // 現在のY位置

        foreach (var item in shopItems)
        {
            GameObject buttonObj = Instantiate(shopItemButtonPrefab);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Name: " + item.ItemBase.ItemName + ", Price: " + item.Price;

            buttonObj.GetComponent<Button>().onClick.AddListener(() => OnItemButtonClick(item));

            // ボタンの位置を設定
            RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(-50, currentY); // X位置を0に設定
            currentY += buttonSpacing;

            // ボタンをキャンバスまたは特定の親要素にアタッチ
            // ボタンの親をキャンバスに設定
            buttonObj.transform.SetParent(BuyHud.transform, false);
        }
    }

    public void OnItemButtonClick(Item item)
    {
        selectedItemEntry = item;

        // 初期化
        
        if (choiceId != item.Id)
        {
            quantity = 1;
        }

        // アイテムがクリックされたときの処理
        // Debug.Log($"Item clicked: {item.item.ItemName}");

        itemNameField.text = "Name: " +item.ItemBase.ItemName;
        itemPriceField.text = "price: "+(item.Price * quantity).ToString();
        quantityField.text = "quantity: " +quantity.ToString();
         moneyField.text = "pocket: " + party.Players[0].gold.ToString()+"Gold";

        choiceId = item.Id;
    }

    // 選択したアイテムの個数を+1する
    public void UpButtonClick()
    {
        quantity++;
        OnItemButtonClick(selectedItemEntry);
    }

    // 選択したアイテムの個数を-1する
    public void DownButtonClick()
    {
        quantity--;
        OnItemButtonClick(selectedItemEntry);
    }

    // 購入 or 売却画面から選択画面に戻る
    public void ReturnButtonClick()
    {
        // アイテムがクリックされたときの処理
        CommandHud.enabled = true;
        BuyHud.enabled = false;
        SellHud.enabled = false;
    }

    // 購入画面の表示
    public void OnBuyButtonClick()
    {
        selectedItemEntry = shopItems[0];
        moneyField.text = "pocket: " + party.Players[0].gold.ToString()+"Gold";
        itemPriceField.text ="price: " +  selectedItemEntry.Price.ToString();
        CommandHud.enabled = false;
        BuyHud.enabled = true;
        
    }

    // 売却画面の表示
    public void OnSellButtonClick()
    {
        Debug.Log("sle");
        CommandHud.enabled = false;
        SellHud.enabled = true;
    }

    // 選択からゲーム画面に戻る
    public void OnEndButtonClick()
    {
        Debug.Log("end");
        ShopHud.enabled = false;
    }
    
    // 購入する
    public void OnClickPurchase()
    {
        party.Players[0].gold = party.Players[0].gold - (selectedItemEntry.Price*quantity);
        Debug.Log(party.Players[0].gold);
        OnItemButtonClick(selectedItemEntry);
    }
}
