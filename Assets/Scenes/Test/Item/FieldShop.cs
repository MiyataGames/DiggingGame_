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

	private Item selectedItemEntry;
	public Canvas ShopHud;
	public Canvas CommandHud;
	public Canvas CommonHud;

	public TextMeshProUGUI moneyField;
	public TextMeshProUGUI quantityField;
	public TextMeshProUGUI sellPriceField;
	public TextMeshProUGUI itemCount;
	public TextMeshProUGUI descriptionField;
	public TextMeshProUGUI tradeTextField;
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
			Debug.Log("ddds");
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
	}

	// 選択したアイテムの個数を+1する
	public void UpButtonClick()
	{
		if (quantity <100)
        {
			quantity++;
		}
		OnItemButtonClick(selectedItemEntry);
	}

	// 選択したアイテムの個数を-1する
	public void DownButtonClick()
	{
		if (quantity >0 )
		{
			quantity--;
		}

		OnItemButtonClick(selectedItemEntry);
	}

	// 購入する
	public void OnClickPurchase()
	{
		if (tradeState == true)
		{
			party.Players[0].gold = party.Players[0].gold - (selectedItemEntry.Price * quantity);

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
			Debug.Log("購入" + shopItems[choiceId].ItemBase.Id);
		}
		else
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

		itemCount.text = "myCount:" + selectedItemEntry.ItemCount.ToString();
	}

	// >>>>>>>>>>>> UI制御系 >>>>>>>>>>>>

// 購入 or 売却画面から選択画面に戻る
public void ReturnButtonClick()
{
    // 各HUDの表示状態を更新
    CommandHud.enabled = true;
	CommonHud.enabled=false;
}

// 購入画面の表示
public void OnBuyButtonClick()
{
	tradeState = true;
	CreateCell();
	CommonHud.enabled=true;
    selectedItemEntry = shopItems[0];
	tradeTextField.text = "Purchase";
	moneyField.text = party.Players[0].gold.ToString() + "Gold";
}

// 売却画面の表示
public void OnSellButtonClick()
{
	tradeState = false;
	CreateCell();
	CommonHud.enabled=true;
	tradeTextField.text = "Sell";
	moneyField.text = party.Players[0].gold.ToString() + "Gold";
}

// 選択からゲーム画面に戻る
public void OnEndButtonClick()
{
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
		Debug.Log("Cell Data Integer Button Clicked! Value = " + value);

		if (tradeState == true)
        {
			selectedItemEntry = shopItems[value];
			sellPriceField.text = "Price: " + (selectedItemEntry.ItemBase.Price * quantity).ToString();
			// Debug.Log("追加したもの"+shopItems[value].ItemBase.name);
		}
        else
        {
			selectedItemEntry = party.Players[0].Items[value];
		}
		// 自分の所持数を表示
		// valueがリストの範囲内にあるかどうかをチェック
		OnItemButtonClick(selectedItemEntry);
		itemCount.text = "myCount:" + party.Players[0].Items[value].ItemCount.ToString();

		// 個数を表示
		quantityField.text = quantity.ToString();

		// 説明を表示
		Debug.Log("説明:"+selectedItemEntry.ItemBase.Description.ToString());
		descriptionField.text = selectedItemEntry.ItemBase.Description.ToString();
		quantityField.text = "1";

		////の表示
		//moneyField.text = party.Players[0].gold.ToString() + "Gold";

		//choiceId = item.ItemBase.Id;
	}
}
