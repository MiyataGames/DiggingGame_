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
    public List<Item> ShopItems { get => shopItems; }

    private Item selectedItemEntry;
    public Canvas ShopHud;
    public Canvas CommandHud;
    public Canvas CommonHud;

    public TextMeshProUGUI moneyField;//所持金
    public TextMeshProUGUI quantityField;//UpとDownボタン押した時に変わる個数
    public TextMeshProUGUI sellPriceField;//商品の値段　price
    public TextMeshProUGUI itemCount;//アイテムの個数 myItem
    public TextMeshProUGUI descriptionField;//アイテムの説明記述
    public TextMeshProUGUI tradeTextField;//purchaceボタン
    int quantity = 1;
    int choiceId = -1;

    private List<ScrollerData> _data; // ここで変数を定義
    bool tradeState = true;// true:buy, false:sell
    public EnhancedScroller myScroller;
    public CallView cellViewPrefab;

    [SerializeField] Party party;

    void Start()
    {
        party.Setup();
        party = party.GetComponent<Party>();
        InitializeShopItems();
        _data = new List<ScrollerData>();
        //Debug.Log(party.Players[0].Items[0]);

        if (tradeState) // 購入モードの場合
        {
            Item firstItem = shopItems[0];
            // 購入モードの初期UI設定
            quantityField.text = "1";
            sellPriceField.text = "Price: " + firstItem.ItemBase.Price.ToString();
            //sellPriceField.text = "Price: 40"; // 例として固定値を設定
            //itemCount.text = "myCount:0";
            // プレイヤーがすでに持っているアイテムの数を表示
            // 持ってなかったら
            Debug.Log("アイテムがあるか" + (party.Players[0].Items.FindIndex(item => item.id == firstItem.id) != -1));

            if (party.Players[0].Items.FindIndex(item => item.id == firstItem.id) == -1)
            {
                //0を表示
                itemCount.text = "myCount:0";
            }
            // 持っていたらその個数を表示　あとでデバッグ
            else
            {
                Item item = party.Players[0].Items.Find(item => item.id == firstItem.id);
                itemCount.text = item.ItemCount.ToString();
            }
            descriptionField.text = "";
            CreateCell();
        }
        else // 売却モードの場合
        {
            Item firstItem = party.Players[0].Items[0];
            // 売却モードの初期UI設定
            if (party.Players[0].Items.Count > 0)
            {
                //Item firstItem = party.Players[0].Items[0];
                quantityField.text = "1";
                sellPriceField.text = "Price: " + (firstItem.ItemBase.Price / 2).ToString();
                itemCount.text = "myCount:" + firstItem.ItemCount.ToString();
                descriptionField.text = firstItem.ItemBase.Description.ToString();
                CreateCell();
            }
            else
            {
                // アイテムがない場合の処理
                quantityField.text = "0";
                sellPriceField.text = "Price: 0";
                itemCount.text = "myCount:0";
                descriptionField.text = "No items to sell";
            }
        }

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
        else // 売却モード
        {
            // プレイヤーの所持アイテムリストを生成

            party.Players[0].Items.Sort((item1, item2) => item1.ItemBase.Id.CompareTo(item2.ItemBase.Id)); // アイテムをIDでソート
            int i = 0;
            foreach (var sellItem in party.Players[0].Items)
            {
                _data.Add(new ScrollerData()
                {
                    //id = sellItem.ItemBase.Id,
                    id = i,
                    cellText = sellItem.ItemBase.ItemName
                });
                i++;
            }

            // 最初のアイテムを選択してUIに表示
            if (party.Players[0].Items.Count > 0)
            {
                OnItemButtonClick(party.Players[0].Items[0]);
            }
        }

        myScroller.ReloadData(); // スクローラーのデータをリロード
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
            Item item = new Item(newItem.ItemBase);
            shopItems.Add(item);
        }
    }

    /// <summary>
    /// ScriptableObjectのshopBaseを
    /// </summary>
    // 購入するときにitemShopからボタンを生成
    public void OnItemButtonClick(Item item)
    {
        Debug.Log(item.id);
        selectedItemEntry = item;

        if (choiceId != item.ItemBase.Id)
        {
            quantity = 1;
        }
        descriptionField.text = item.ItemBase.Description.ToString();
        quantityField.text = quantity.ToString();

        // 売却価格の表示を購入/売却の状態に応じて変更
        if (tradeState) // 購入の場合
        {
            sellPriceField.text = "Price: " + (item.ItemBase.Price * quantity).ToString();
            itemCount.text = "myCount:" + selectedItemEntry.ItemCount.ToString();
        }
        else // 売却の場合
        {
            Debug.Log("UIを更新" + item.ItemBase.Price / 2 * quantity);
            //Debug.Log("UI quantityを更新" + quantity);
            // 売却時は選択されているアイテムの売却価格を表示
            sellPriceField.text = "Price: " + ((item.ItemBase.Price / 2) * quantity).ToString();
            itemCount.text = "myCount:" + selectedItemEntry.ItemCount.ToString();

        }

        moneyField.text = party.Players[0].gold.ToString() + "Gold";
        choiceId = item.ItemBase.Id;
    }


    // 選択したアイテムの個数を+1する
    public void UpButtonClick()
    {
        if (tradeState) // 購入モードの場合
        {
            //選んだアイテムの値段✖️個数 ＜　合計金額
            if (selectedItemEntry.ItemBase.Price * quantity < party.Players[0].gold)
            {
                quantity++;
            }
        }
        else // 売却モードの場合
        {
            Debug.Log(quantity);
            if (selectedItemEntry != null && quantity < selectedItemEntry.ItemCount)
            {
                quantity++;
            }
        }

        OnItemButtonClick(selectedItemEntry);
    }

    // 選択したアイテムの個数を-1する
    public void DownButtonClick()
    {
        if (quantity > 0)
        {
            quantity--;
        }

        OnItemButtonClick(selectedItemEntry);
    }
    // 購入する
    public void OnClickPurchase()
    {
        if (tradeState == true)//tradestate=true つまり購入成立だったら
        {
            int totalPrice = selectedItemEntry.Price * quantity;// totalpriceは選択したアイテム金額の合計
            if (totalPrice > party.Players[0].gold || quantity <= 0)//もしアイテム金額の合計がプレイヤーの現在の所持金と選択した個数が０以下なら
            {
                // 購入不可のメッセージを表示
                Debug.Log("購入できません"); //購入できない
                return;
            }

            // 購入可能な場合の処理
            party.Players[0].gold -= totalPrice;//現在の所持金-選択したアイデムの合計金額

            bool itemFound = false;
            for (int i = 0; i < party.Players[0].Items.Count; i++) //i=０と定義、iが選択したアイテム個数より少なかったら、i++
            {
                if (shopItems[choiceId].ItemBase.Id == party.Players[0].Items[i].ItemBase.Id)//もし選んだアイテム番号とプレイヤーの所持アイテムが一致したら
                {
                    party.Players[0].Items[i].ItemCount += quantity;//プレイヤーの所持数にその分の個数を足す
                    itemFound = true;
                    break;
                }
            }

            if (!itemFound)//みつからなかった場合
            {
                party.Players[0].Items.Add(shopItems[choiceId]);//プレイヤーの所持アイテム欄に選んだアイテム追加
                shopItems[choiceId].ItemCount = quantity;//選んだ個数分を追加
            }

            OnItemButtonClick(selectedItemEntry);// 購入ボタン出現
            Debug.Log("購入" + shopItems[choiceId].ItemBase.Id);

            quantity = 1;
            quantityField.text = quantity.ToString();

            // UIの更新
            sellPriceField.text = "Price: " + (selectedItemEntry.ItemBase.Price * quantity).ToString();
            itemCount.text = "myCount:" + selectedItemEntry.ItemCount.ToString();
            descriptionField.text = selectedItemEntry.ItemBase.Description.ToString();
        }
        else//売却の場合
        {
            var selectedItem = shopItems[choiceId];

            for (int i = 0; i < party.Players[0].Items.Count; i++)//プレイヤーが持っているアイテム数だけくり返す
            {
                Item item = party.Players[0].Items[i];//アイテムオブジェクトを取得

                if (item.ItemBase.Id == selectedItem.ItemBase.Id)//もし選んだアイテムと売るアイテムが一致したら
                {
                    // int sellableQuantity = Mathf.Min(item.ItemCount, quantity);//アイテムの所持数と選択した個数の小さい方を取得、売却できる最大個数を求める
                    int sellableQuantity = quantity;
                    int sellPrice = item.ItemBase.Price / 2 * sellableQuantity;//売却金＝選択したアイテムの購入価格の半値✖️個数
                    party.Players[0].gold += sellPrice;//現在の所持金＋売却金額
                    party.Players[0].Items[i].ItemCount -= sellableQuantity;//現在持っているアイテム数から売ったアイテム数の個数をひく
                    // 0個になったらUIを更新
                    if (party.Players[0].Items[i].ItemCount == 0)
                    {
                        Debug.Log("実行");

                        party.Players[0].Items.Remove(item);
                        for (int k = 0; k < party.Players[0].Items.Count; k++)
                        {
                            Debug.Log("アイテム" + party.Players[0].Items[k].id);
                        }
                        int j = 0;
                        _data = new List<ScrollerData>();
                        foreach (var sellItem in party.Players[0].Items)
                        {
                            _data.Add(new ScrollerData()
                            {
                                id = j,
                                cellText = sellItem.ItemBase.ItemName
                            });
                            j++;
                        }
                        myScroller.ReloadData();

                    }
                    OnItemButtonClick(selectedItemEntry);
                    // 売った後のUI更新 あとでなおす
                    
                    quantity = 1;
                    quantityField.text = quantity.ToString();

                    if (party.Players[0].Items.Count == 0)
                    {
                        // 条件を満たした場合の処理
                        sellPriceField.text = "Price:0";
                        itemCount.text = "myCount:0";
                        descriptionField.text = "None";
                    }

                    //sellPriceField.text = "Price: " + (selectedItemEntry.ItemBase.Price * quantity).ToString();
                    //itemCount.text = "myCount:" + selectedItemEntry.ItemCount.ToString();
                    //descriptionField.text = selectedItemEntry.ItemBase.Description.ToString();

                }
                else
                {
                    Debug.Log("リストにあるけど持っていない");
                }
            }

            Debug.Log("購入できない");
        }

        itemCount.text = "myCount:" + selectedItemEntry.ItemCount.ToString();
    }


    // >>>>>>>>>>>> UI制御系 >>>>>>>>>>>>

    // 購入 or 売却画面から選択画面に戻る
    public void ReturnButtonClick()
    {
        // 各HUDの表示状態を更新
        CommandHud.enabled = true;
        CommonHud.enabled = false;
    }

    // 購入画面の表示
    public void OnBuyButtonClick()
    {
        tradeState = true;
        CreateCell();
        CommonHud.enabled = true;
        selectedItemEntry = shopItems[0];

        tradeTextField.text = "Purchase";
        moneyField.text = party.Players[0].gold.ToString() + "Gold";

        // リストの最初のアイテムを選択して表示
        if (party.Players[0].Items.Count > 0)
        {
            OnItemButtonClick(party.Players[0].Items[0]);
        }
    }

    // 売却画面の表示
    public void OnSellButtonClick()
    {
        tradeState = false;
        CreateCell();
        CommonHud.enabled = true;
        tradeTextField.text = "Sell";
        moneyField.text = party.Players[0].gold.ToString() + "Gold";
    }

    // 選択からゲーム画面に戻る
    public void OnEndButtonClick()
    {
        CommonHud.enabled = false;
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

        if (tradeState) // 購入モード
        {
            if (value >= 0 && value < shopItems.Count)
            {
                selectedItemEntry = shopItems[value];
                Debug.Log("アイテムの個数は" + selectedItemEntry.ItemCount);
            }
            else
            {
                Debug.Log("Index out of range for shopItems list");
                return;
            }
        }
        else // 売却モード
        {
            if (value >= 0 && value < party.Players[0].Items.Count)
            {
                selectedItemEntry = party.Players[0].Items[value];

            }
            else
            {
                Debug.Log("Index out of range for player's items list");
                return;
            }
        }

        // アイテムが選択されたときの処理を実行
        OnItemButtonClick(selectedItemEntry);
    }
}