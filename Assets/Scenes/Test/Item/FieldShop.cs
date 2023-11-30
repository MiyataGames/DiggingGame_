using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;  

public class FieldShop : MonoBehaviour
{
	public ShopBase shopBase;
	public GameObject itemButtunPrefab; // ボタンのプレファブ
	private List<Item> shopItems = new List<Item>();
	public List<Item> ShopItems {get => shopItems; }

	// ショップボタンを格納
	private List<GameObject> buyButtonList = new List<GameObject>();
	private List<GameObject> sellButtonList = new List<GameObject>();


	private Item selectedItemEntry;
	public Canvas ShopHud;
	public Canvas BuyHud;
	public Canvas SellHud;
	public Canvas CommandHud;
	public Canvas CommonHud;

	public TextMeshProUGUI moneyField;
	public TextMeshProUGUI quantityField;
	public TextMeshProUGUI sellPriceField;
	public TextMeshProUGUI itemCount;
	public TextMeshProUGUI  descriptionField;
	int quantity = 1;
	int choiceId = -1;

	[SerializeField] Party party;

	void Awake()
	{
		// GameManagerに後で移す
		party.Setup();
		party = party.GetComponent<Party>();
		InitializeShopItems();
		moneyField.text =  party.Players[0].gold.ToString() + "Gold";
				quantityField.text = "1";
				itemCount.text = "0";
				descriptionField.text = "";
		// CreateShopButtons();
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
			Debug.Log("aaaa");
		}
	}

	private void CreateShopButtons()
	{
		int buttonSpacing = -40; // ボタン間の間隔
		int currentY = 90; // 現在のY位置


		foreach (var item in shopItems)
		{
			GameObject buttonObj = Instantiate(itemButtunPrefab);
			buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Name: " + item.ItemBase.ItemName + ", Price: " + item.Price;

			buttonObj.GetComponent<Button>().onClick.AddListener(() => OnItemButtonClick(item));

			// ボタンの位置を設定
			RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = new Vector2(-150, currentY); // X位置を0に設定
			currentY += buttonSpacing;

			// ボタンをキャンバスまたは特定の親要素にアタッチ
			// ボタンの親をキャンバスに設定
			buttonObj.transform.SetParent(BuyHud.transform, false);
			// 生成されたボタンの参照をリストに追加
			buyButtonList.Add(buttonObj);
		}
	}

	// 売却アイテムボタンをPlayerのItemsのリストから生成
	private void  CreateSellButtons()
	{
		int buttonSpacing = -40; // ボタン間の間隔
		int currentY = 90; // 現在のY位置
		foreach (var playerItem in party.Players[0].Items)
		{
			GameObject buttonObj = Instantiate(itemButtunPrefab);
			buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Name: " + playerItem.ItemBase.ItemName + ", Count: " + playerItem.ItemCount;

			buttonObj.GetComponent<Button>().onClick.AddListener(() => OnPlayerItemButtonClick( playerItem));

			// ボタンの位置を設定
			RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = new Vector2(-150, currentY); // X位置を-50に設定
			currentY += buttonSpacing;

			// ボタンをキャンバスまたは特定の親要素にアタッチ
			buttonObj.transform.SetParent(SellHud.transform, false);
			sellButtonList.Add(buttonObj);
		}
	}


	// 購入するときにitemShopからボタンを生成
	public void OnItemButtonClick(Item item)
	{
		
		selectedItemEntry = item;

		if (choiceId != item.ItemBase.Id)
		{
			quantity = 1;
		}
	// 	if (party.Players[0].Items[item.ItemBase.Id] != null || party.Players[0].Items[item.ItemBase.Id].ItemCount != 0){

	// itemCount.text = party.Players[0].Items[item.ItemBase.Id].ItemCount.ToString();
	// 	}
descriptionField.text = item.ItemBase.Description.ToString();
		// アイテムがクリックされたときの処理
		quantityField.text = quantity.ToString();
		
    // 売却価格の表示
	sellPriceField.text = "Price: " + (item.ItemBase.Price*quantity).ToString();
		moneyField.text = party.Players[0].gold.ToString() + "Gold";

		choiceId = item.ItemBase.Id;
	}// プレイヤーのアイテム選択時の処理
public void OnPlayerItemButtonClick(Item items)
{
    if (items == null)
    {
        Debug.Log("Error: Item is null");
        return;
    }

    // 選択されたアイテムを更新
    selectedItemEntry = items;
    Debug.Log("Selected Item: " + items.ItemBase.ItemName);

    // 売却価格の表示
    sellPriceField.text = "Price: " + items.ItemBase.Price.ToString();

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

	// 購入する
	public void OnClickPurchase()
	{
		party.Players[0].gold = party.Players[0].gold - (selectedItemEntry.Price * quantity);
		//Debug.Log("Current Gold: " + party.Players[0].gold);

		bool itemFound = false;
		for (int i = 0; i < party.Players[0].Items.Count; i++)
		{
			if (shopItems[choiceId].ItemBase.Id == party.Players[0].Items[i].ItemBase.Id)
			{
				// 既に存在するアイテムの場合、個数を増やす
				party.Players[0].Items[i].ItemCount += quantity;
				itemFound = true;
				break;
			}
		}

		if (!itemFound)
		{
			// 新しいアイテムの場合、リストに追加
			party.Players[0].Items.Add(shopItems[choiceId]);
			shopItems[choiceId].ItemCount = quantity;
		}

		OnItemButtonClick(selectedItemEntry);
		//Debug.Log("Number of Items in Inventory: " + party.Players[0].Items.Count);
		Debug.Log(shopItems[choiceId].ItemBase.Id);
	}

	// 売却する
	public void OnSellPriceButtonClick()
	{
		var selectedItem = shopItems[choiceId];

		// プレイヤーのアイテムリストをチェック
	for (int i = 0; i < party.Players[0].Items.Count; i++)
	{
    Item item = party.Players[0].Items[i];

    if (item.ItemBase.Id == selectedItem.ItemBase.Id)
    {
        // アイテムが見つかった場合、半額で売却
        int sellPrice = item.ItemBase.Price / 2;
        party.Players[0].gold += sellPrice; // プレイヤーの所持金を増やす
        party.Players[0].Items[i].ItemCount = party.Players[0].Items[i].ItemCount - quantity;
		OnItemButtonClick(selectedItemEntry);
		CreateSellButtons();
	}
	else 
	{
		Debug.Log("リストにあるけど持っていない");
	}
}

		// アイテムがリストにない場合、売却できない旨のメッセージを表示
		Debug.Log("リストにない");
	}

	public void DestroyAllPrefabInstances()
	{
		foreach (var instance in sellButtonList)
		{
			Destroy(instance);
		}
		foreach (var instance in buyButtonList)
		{
			// ゲームオブジェクトの破棄
			Destroy(instance);
		}
		buyButtonList.Clear(); // リストをクリア
		sellButtonList.Clear(); // リストをクリア
	}

	// >>>>>>>>>>>> UI制御系 >>>>>>>>>>>>

// 購入 or 売却画面から選択画面に戻る
public void ReturnButtonClick()
{
    // 各HUDの表示状態を更新
    CommandHud.enabled = true;
    BuyHud.enabled = false;
    SellHud.enabled = false;
		CommonHud.enabled=false;

    // 生成されたボタンのインスタンスを破棄してUIをクリア
    DestroyAllPrefabInstances();
}

// 購入画面の表示
public void OnBuyButtonClick()
{quantityField.text = "0";
    // ショップのアイテムボタンを生成
    CreateShopButtons();
    	CommonHud.enabled=true;
    // 初期選択アイテムと所持金の表示を設定
    selectedItemEntry = shopItems[0];
    moneyField.text = party.Players[0].gold.ToString() + "Gold";

    // 購入画面のみを表示
    BuyHud.enabled = true;
	SellHud.enabled = false;
}

// 売却画面の表示
public void OnSellButtonClick()
{quantityField.text = "0";
    // プレイヤーのアイテムボタンを生成
    CreateSellButtons();
	CommonHud.enabled=true;

    // 売却画面のみを表示
	    BuyHud.enabled = false;
    SellHud.enabled = true;
}

// 選択からゲーム画面に戻る
public void OnEndButtonClick()
{
    // ショップHUDを非表示にしてゲーム画面に戻る
	CommonHud.enabled=false;
    ShopHud.enabled = false;
	
}

// <<<<<<<<<<<< UI制御系 <<<<<<<<<<<
}
