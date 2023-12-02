using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;

public class FieldShop : MonoBehaviour, IEnhancedScrollerDelegate
{
	public ShopBase shopBase;
	public GameObject itemButtunPrefab; // ボタンのプレファブ
	private List<Item> shopItems = new List<Item>();
	public List<Item> ShopItems {get => shopItems; }

	// ショップボタンを格納
	//private List<GameObject> buyButtonList = new List<GameObject>();
	//private List<GameObject> sellButtonList = new List<GameObject>();


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

	private List<ScrollerData> _data; // ここで変数を定義
	bool tradeState = true;// true:buy, false:sell
	public EnhancedScroller myScroller;
	public CallView cellViewPrefab;

	[SerializeField] Party party;

	void Start()
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
		_data = new List<ScrollerData>();
		CreateCell();

		myScroller.Delegate = this;
		myScroller.ReloadData();
	}

	#region EnhancedScroller Handlers
	/// <summary>
	/// 売却・購入に合わせてCellの中身を更新する
	/// </summary>
	public void CreateCell()
    {
		// リストの内容をクリア
		_data.Clear();

		// 購入する場合のCellを生成
		if (tradeState == true)
		{
			foreach (var buyItem in ShopItems)
			{
				_data.Add(new ScrollerData()
				{
					id = buyItem.ItemBase.Id,
					cellText = buyItem.ItemBase.ItemName
				});

			}
		}
		// 売却する場合のCellを生成
		else
        {
			foreach (var SellItem in party.Players[0].Items)
            {
				_data.Add(new ScrollerData()
				{
					id = SellItem.ItemBase.Id,
					cellText = SellItem.ItemBase.ItemName
				});
			}
        }
		// スクローラーのビューを更新
		myScroller.ReloadData();
	}

	public int GetNumberOfCells(EnhancedScroller scroller)
	{
		return _data.Count;
	}

	#endregion

	/// <summary>
	/// ScriptableObjectのshopBaseをListに格納
	/// </summary>
	private void InitializeShopItems()
	{
		foreach (var newItem in shopBase.ShopItems)
		{
			shopItems.Add(newItem);
			Debug.Log("aaaa");
		}
	}

	/// <summary>
	/// ScriptableObjectのshopBaseを
	/// </summary>


	// 購入するときにitemShopからボタンを生成
	public void OnItemButtonClick(Item item)
	{
		
		selectedItemEntry = item;

		if (choiceId != item.ItemBase.Id)
		{
			quantity = 1;
		}
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
		//CreateSellButtons();
	}
	else 
	{
		Debug.Log("リストにあるけど持っていない");
	}
}

		// アイテムがリストにない場合、売却できない旨のメッセージを表示
		Debug.Log("リストにない");
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
	tradeState = true;
	CreateCell();

	// 生成されたボタンのインスタンスを破棄してUIをクリア
	// DestroyAllPrefabInstances();
	}

// 購入画面の表示
public void OnBuyButtonClick()
{
		tradeState = false;
		CreateCell();
		quantityField.text = "0";
    // ショップのアイテムボタンを生成
    //CreateShopButtons();
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
    //CreateSellButtons();
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

	public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
	{
		// ここでは各セルのサイズを返します（例：60ユニット）
		return 60f;
	}

	public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
	{
		// ここではセルビューを生成し、データを設定します
		CallView cellView = scroller.GetCellView(cellViewPrefab) as CallView;
		cellView.SetData(_data[dataIndex]);
		cellView.cellButtonDataIntegerClicked = CellButtonDataIntegerClicked;

		return cellView;
	}
	
	private void CellButtonDataIntegerClicked(int value)
	{
		// Debug.Log("Cell Data Integer Button Clicked! Value = " + value);
		if (tradeState == true)
        {
			selectedItemEntry = shopItems[value];
			Debug.Log("追加したもの"+shopItems[value].ItemBase.name);
		}
        else
        {
			selectedItemEntry = party.Players[0].Items[value];

		}
	}


	#region 使わない関数
	//private void CreateShopButtons()
	//{
	//	int buttonSpacing = -40; // ボタン間の間隔
	//	int currentY = 90; // 現在のY位置


	//	foreach (var item in shopItems)
	//	{
	//		GameObject buttonObj = Instantiate(itemButtunPrefab);
	//		buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Name: " + item.ItemBase.ItemName + ", Price: " + item.Price;

	//		buttonObj.GetComponent<Button>().onClick.AddListener(() => OnItemButtonClick(item));

	//		// ボタンの位置を設定
	//		RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
	//		rectTransform.anchoredPosition = new Vector2(-150, currentY); // X位置を0に設定
	//		currentY += buttonSpacing;

	//		// ボタンをキャンバスまたは特定の親要素にアタッチ
	//		// ボタンの親をキャンバスに設定
	//		buttonObj.transform.SetParent(BuyHud.transform, false);
	//		// 生成されたボタンの参照をリストに追加
	//		buyButtonList.Add(buttonObj);
	//	}
	//}

	//// 売却アイテムボタンをPlayerのItemsのリストから生成
	//private void CreateSellButtons()
	//{
	//	int buttonSpacing = -40; // ボタン間の間隔
	//	int currentY = 90; // 現在のY位置
	//	foreach (var playerItem in party.Players[0].Items)
	//	{
	//		GameObject buttonObj = Instantiate(itemButtunPrefab);
	//		buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = "Name: " + playerItem.ItemBase.ItemName + ", Count: " + playerItem.ItemCount;

	//		buttonObj.GetComponent<Button>().onClick.AddListener(() => OnPlayerItemButtonClick(playerItem));

	//		// ボタンの位置を設定
	//		RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
	//		rectTransform.anchoredPosition = new Vector2(-150, currentY); // X位置を-50に設定
	//		currentY += buttonSpacing;

	//		// ボタンをキャンバスまたは特定の親要素にアタッチ
	//		buttonObj.transform.SetParent(SellHud.transform, false);
	//		sellButtonList.Add(buttonObj);
	//	}
	//}
	#endregion
}
