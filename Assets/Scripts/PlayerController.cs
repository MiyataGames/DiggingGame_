using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.EventSystems;
using System.Linq;
public enum GameStatus
{
    DIGGING,
    MENU,
    ITEM,
    STATUS,
    SYSTEM,
}

public enum ItemUseStatus
{
    SELECT_ITEM,
    SELECT_TARGET,
    USE_ITEM
}

public class Define
{
    public enum DirectionNumber
    {
        RIGHT_UP,
        RIGHT,
        RIGHT_DOWN,
        DOWN,
        LEFT_DOWN,
        LEFT,
        LEFT_UP,
        NONE
    }

    // 方向　右上、右、右下、下、左下、左、左上
    public static Vector2[] directions = { new Vector2(1, 1), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1), new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, 0) };
}


public class PlayerController : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;

    [SerializeField] private Rigidbody2D rb;
    private float vx;
    private float vy;
    private float minVelocityY = -9.0f;
    private float maxVelocityY = 10.0f;
    private bool pushFlag;
    private bool jumpFlag;
    private bool groundFlag;
    private Define.DirectionNumber currentDirectionNumber;
    [SerializeField] private Menu menu;

    public static GameStatus currentGameStatus;

    // プレイヤー 仮
    //[SerializeField] private PlayerBase[] playerBasies;

    //[SerializeField] private GameObject[] playerUIs;
    [SerializeField] Party party;
    // セーブシステム
    [SerializeField] SaveLoadController saveLoadCtrl;
    // メニュー画面 =======================
    private int currentMenuCommandNum;
    // アイテム関係 ======================

    private ItemUseStatus currentItemUseStatus;
    private int currentItemNum;
    private List<ItemCellData> itemCellData;

    public EnhancedScroller itemPanel;
    public EnhancedScrollerCellView cellViewPrefab;
    public float cellSize = 100f;

    // アイテムの選択されているインデックス番号
    private int selectedItemIndex;

    // アイテムのターゲットのインデックス番号
    private int selectedItemTargetIndex;


    // ステータス関係 ==========================
    // ステータス画面の選択されているインデックス番号
    private int selectedStatusIndex;



    [SerializeField] private PlayerStatusUIsManager playerStatusUIsManager;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentGameStatus = GameStatus.DIGGING;
        currentMenuCommandNum = 0;
        currentItemNum = 0;
        selectedItemIndex = 0;
        selectedStatusIndex = 0;
        selectedItemTargetIndex = 0;
        party.Setup();
        menu.menuSelectButtonClickedDelegate = SelectMenuButton;
        menu.itemSelectButtonHoverdDelegate = CellButtonOnHover;
        playerStatusUIsManager.statusSelectButtonClickedDelegate = SelectStatusButton;
        // saveLoadCtrl.Load();
    }

    // Update is called once per frame
    private void Update()
    {
        // 穴掘り中だったら
        if (currentGameStatus == GameStatus.DIGGING)
        {
            // サーチ
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // サーチ機能
            }
            // メニュー画面
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // メニュー画面をひらく
                currentGameStatus = GameStatus.MENU;
                menu.ActivateMenuPanel(true);
                menu.ActivateMenuSelectArrow((int)MenuCommand.ITEM);
            }
            // マップ
            if (Input.GetKeyDown(KeyCode.M))
            {
                // マップを開く
            }
            // 移動
            vx = 0;
            vy = 0;

            // A：左
            if (Input.GetKey(KeyCode.A))
            {
                vx = -speed;
            }
            else
            // D：右
            if (Input.GetKey(KeyCode.D))
            {
                vx = speed;
            }

            if (Input.GetKey("space") && groundFlag == true)
            {
                if (pushFlag == false)
                {
                    jumpFlag = true;
                    pushFlag = true;
                }
            }
            else
            {
                pushFlag = false;
            }
        }

        // メインパネルを選択中だったら
        else if (currentGameStatus == GameStatus.MENU)
        {
            HandleMenuSelect();
        }
        // アイテムを選択中だったら
        else if (currentGameStatus == GameStatus.ITEM)
        {
            HandleItemSelect();
        }
        // ステータスを選択中だったら
        else if (currentGameStatus == GameStatus.STATUS)
        {
            HandleStatusSelect();
        }
        // システムを選択中だったら
        else if (currentGameStatus == GameStatus.SYSTEM)
        {
            HandleSystemSelect();
        }
    }

    private void HandleMenuSelect()
    {

        /*
        // キー操作
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMenuCommandNum < (int)MenuCommand.END - 1)
            {
                currentMenuCommandNum++;
                menu.ActivateMenuSelectArrow((MenuCommand)currentMenuCommandNum);
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMenuCommandNum > 0)
            {
                currentMenuCommandNum--;
                menu.ActivateMenuSelectArrow((MenuCommand)currentMenuCommandNum);
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // メニューを選択
            // アイテムコマンドだったら
            if (currentMenuCommandNum == ((int)MenuCommand.ITEM))
            {
                // アイテムパネルを開く
                menu.ActivateItemPanel(true);
                currentGameStatus = GameStatus.ITEM;
                menu.ActivateMenuPanel(false);
                InitItem();
            }
            if (currentMenuCommandNum == (int)MenuCommand.STATUS)
            {
                menu.ActivateStatusPanel(true);
                currentGameStatus = GameStatus.STATUS;
                InitStatus();
            }
        }
        */
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // メニュー画面を閉じる
            menu.ActivateMenuPanel(false);
            currentGameStatus = GameStatus.DIGGING;
        }
    }

    // マウスでメニューを選択する
    public void SelectMenuButton(int selectMenuIndex)
    {
        currentMenuCommandNum = selectMenuIndex;
        // メニューを選択
        // アイテムコマンドだったら
        if (currentMenuCommandNum == ((int)MenuCommand.ITEM))
        {
            // アイテムパネルを開く
            menu.ActivateItemPanel(true);
            currentGameStatus = GameStatus.ITEM;
            menu.ActivateMenuPanel(false);
            InitItem();
        }
        if (currentMenuCommandNum == (int)MenuCommand.STATUS)
        {
            menu.ActivateMenuPanel(false);
            menu.ActivateStatusPanel(true);
            currentGameStatus = GameStatus.STATUS;
            InitStatus();
        }
        if (currentMenuCommandNum == (int)MenuCommand.SYSTEM)
        {
            menu.ActivateSystemPanel(true);
            menu.ActivateMenuPanel(false);
            currentGameStatus = GameStatus.SYSTEM;
        }
    }
    private void HandleItemSelect()
    {
        if (currentItemUseStatus == ItemUseStatus.SELECT_ITEM)
        {
            /*
            // キー操作
            bool selectionChanged = false;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                // 値を範囲内にする
                selectedItemIndex = Mathf.Clamp(selectedItemIndex - 1, 0, itemCellData.Count - 1);
                selectionChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedItemIndex = Mathf.Clamp(selectedItemIndex + 1, 0, itemCellData.Count - 1);
                selectionChanged = true;
            }

            // 選択中
            if (selectionChanged)
            {
                for (int i = 0; i < itemCellData.Count; i++)
                {
                    itemCellData[i].isSelected = (i == selectedItemIndex);
                }
                // アクティブセルに対してUIの更新をする
                itemPanel.RefreshActiveCellViews();

                if (selectedItemIndex >= itemPanel.EndCellViewIndex)
                {
                    itemPanel.JumpToDataIndex(selectedItemIndex, 1.0f, 1.0f);
                }
                else if (selectedItemIndex <= itemPanel.StartCellViewIndex)
                {
                    itemPanel.JumpToDataIndex(selectedItemIndex, 0.0f, 0.0f);
                }
            }

            // アイテムを使用する
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("アイテムを使うよ");
                // 使うアイテムの情報を保持
                Item item = players[0].Items[selectedItemIndex];

                // 回復アイテムだったら
                if (item.ItemBase.itemType == ItemType.HEAL_ITEM)
                {
                    // アイテムパネルを消す
                    menu.ActivateItemPanel(false);
                    // ステータス画面を開く
                    menu.ActivateStatusPanel(true);
                    // ステータス画面の更新
                    playerStatusUIsManager.SetUpPlayerStatusUI(players);
                    playerStatusUIsManager.selectStatus(selectedItemTargetIndex);
                }
                // ターゲットを選ぶステータスに移行
                currentItemUseStatus = ItemUseStatus.SELECT_TARGET;
            }
            */
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // アイテム画面を閉じてメニュー画面を開く
                menu.ActivateMenuPanel(true);
                menu.ActivateItemPanel(false);
                currentGameStatus = GameStatus.MENU;
                currentItemUseStatus = ItemUseStatus.SELECT_ITEM;
            }
        }
        else if (currentItemUseStatus == ItemUseStatus.SELECT_TARGET)
        {
            /*
            // キー操作
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (selectedItemTargetIndex < players.Count - 1)
                {
                    selectedItemTargetIndex++;
                    playerStatusUIsManager.selectStatus(selectedItemTargetIndex);
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (selectedItemTargetIndex > 0)
                {
                    selectedItemTargetIndex--;
                    playerStatusUIsManager.selectStatus(selectedItemTargetIndex);
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {   // アイテムの残りが0だったら
                if (players[0].Items[selectedItemIndex].ItemCount == 0)
                {
                    Debug.Log("使えないよ");
                }
                else
                {
                    Item item = players[0].Items[selectedItemIndex];
                    // アイテムの効果発動
                    players[selectedItemTargetIndex].TakeHeal(item);
                    players[selectedItemTargetIndex].playerUI.UpdateHpSp();

                    // アイテムの数を減らす
                    players[0].Items[selectedItemIndex].ItemCount--;
                }
            }
            */
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // 0のものをアイテムリストから除外する
                if (party.Players[0].Items[selectedItemIndex].ItemCount == 0)
                {
                    party.Players[0].Items.RemoveAt(selectedItemIndex);
                    if (selectedItemIndex != 0)
                    {
                        selectedItemIndex--;
                    }
                }
                // アイテムパネルの更新
                LoadItemData();
                itemPanel.RefreshActiveCellViews();
                // ステータス画面を閉じてアイテム画面を開く
                menu.ActivateStatusPanel(false);
                menu.ActivateItemPanel(true);
                currentGameStatus = GameStatus.ITEM;
                currentItemUseStatus = ItemUseStatus.SELECT_ITEM;
            }
        }
    }

    private void HandleStatusSelect()
    {
        /*
        // キー操作
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedStatusIndex < players.Count - 1)
            {
                selectedStatusIndex++;
                playerStatusUIsManager.selectStatus(selectedStatusIndex);
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selectedStatusIndex > 0)
            {
                selectedStatusIndex--;
                playerStatusUIsManager.selectStatus(selectedStatusIndex);
            }
        }
        */
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // アイテム画面を閉じてメニュー画面を開く
            menu.ActivateMenuPanel(true);
            menu.ActivateStatusPanel(false);
            currentGameStatus = GameStatus.MENU;
        }
    }

    void HandleSystemSelect()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // システム画面を閉じてメニュー画面を開く
            menu.ActivateMenuPanel(true);
            menu.ActivateSystemPanel(false);
            currentGameStatus = GameStatus.MENU;
        }
    }

    #region 
    // アイテム関係 ===========

    private void InitItem()
    {
        itemPanel.Delegate = this;
        LoadItemData();
        itemPanel.RefreshActiveCellViews();
        currentItemUseStatus = ItemUseStatus.SELECT_ITEM;
    }

    // アイテムのロード
    private void LoadItemData()
    {
        itemCellData = new List<ItemCellData>();
        for (int i = 0; i < party.Players[0].Items.Count; i++)
        {
            // Debug.Log(players[0].Items[i].ItemBase.ItemName);
            itemCellData.Add(new ItemCellData()
            {
                isSelected = i == selectedItemIndex,// 選択されているか
                itemText = party.Players[0].Items[i].ItemBase.ItemName,
                itemCountText = party.Players[0].Items[i].ItemCount.ToString()
            });
        }
        itemPanel.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller playerItemPanel)
    {
        return itemCellData.Count;
    }

    public float GetCellViewSize(EnhancedScroller playerItemPanel, int dataIndex)
    {
        return cellSize;
    }

    // 表示するセルの取得
    public EnhancedScrollerCellView GetCellView(EnhancedScroller playerItemPanel, int dataIndex, int cellIndex)
    {
        ItemCellView cell = playerItemPanel.GetCellView(cellViewPrefab) as ItemCellView;
        cell.cellButtonClicked = CellButtonClicked;

        cell.name = "Cell" + dataIndex.ToString();
        cell.SetData(itemCellData[dataIndex]);
        GameObject cellViewObj = cell.gameObject;
        // イベントトリガーを追加してホバー時に実行する関数を指定する
        EventTrigger eventTrigger = cellViewObj.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventDate) => CellButtonOnHover(dataIndex));
        eventTrigger.triggers.Add(entry);
        return cell;
    }

    // セルにホバーしたとき実行する関数 
    public void CellButtonOnHover(int selectedIndex)
    {
        selectedItemIndex = selectedIndex;
        for (int i = 0; i < itemCellData.Count; i++)
        {
            itemCellData[i].isSelected = (i == selectedIndex);
        }
        // 見た目の更新
        itemPanel.RefreshActiveCellViews();
    }
    private void CellButtonClicked(int selectedIndex)
    {
        // アクティブセルに対してUIの更新をする
        itemPanel.RefreshActiveCellViews();
        Debug.Log("アイテムを使うよ");
        // 使うアイテムの情報を保持
        Item item = party.Players[0].Items[selectedItemIndex];

        // 回復アイテムだったら
        if (item.ItemBase.itemType == ItemType.HEAL_ITEM)
        {
            HealItemBase healItem = party.Players[0].Items[selectedItemIndex].ItemBase as HealItemBase;
            // アイテムパネルを消す
            menu.ActivateItemPanel(false);
            // ステータス画面を開く
            menu.ActivateStatusPanel(true);
            // ステータス画面の更新
            InitStatus();
        }
        // ターゲットを選ぶステータスに移行
        currentItemUseStatus = ItemUseStatus.SELECT_TARGET;
        Debug.Log("Cell Data Integer Button Clicked! Value = " + selectedItemIndex);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        /*
        for (int i = 0; i < player.Items.Count; i++)
        {
            Debug.Log(player.Items[i].ItemBase.ItemName + ":" + player.Items[i].ItemCount);
        }*/
        // 衝突したのがアイテムだったら
        if (other.tag == "Item")
        {
            // Item newItem = other.GetComponent<Item>();

            // FieldItemコンポーネントからItemインスタンスを取得

            // フィールドアイテムのアイテムプロパティからItemインスタンスを取得
            FieldItem fieldItemComponent = other.GetComponent<FieldItem>();
            // Item newItem = other.GetComponent<FieldItem>().item;
            Item newItem = fieldItemComponent.item;
            // 同じアイテムがあるか検索
            // if (party.Players[0].Items.Count > 0)
            // {
            //     for (int i = 0; i < party.Players[0].Items.Count; i++)
            //     {
            //         if (newItem.ItemBase.Id == party.Players[0].Items[i].ItemBase.Id)
            //         {
            //             // あったら個数を増やして破棄
            //             party.Players[0].Items[i].ItemCount++;
            //             Destroy(other.gameObject);
            //             return;
            //         }
            //     }
            // }

            // 持っているアイテムかどうかを確認する
            bool itemExists = false;

            for (int i = 0; i < party.Players[0].Items.Count; i++)
            {
                if (newItem.ItemBase.Id == party.Players[0].Items[i].ItemBase.Id)
                {
                    // あったら個数を増やしてアイテム破棄
                    party.Players[0].Items[i].ItemCount++;
                    Destroy(other.gameObject);
                    itemExists = true;
                    break;
                }
            }

            // ない場合新しく追加する
            // party.Players[0].Items.Add(other.GetComponent<Item>());
            // party.Players[0].Items[party.Players[0].Items.Count - 1].ItemCount++;
            if (!itemExists)
            {
                party.Players[0].Items.Add(newItem);
                newItem.ItemCount = 1;
                // アイテム破棄
                Destroy(other.gameObject);
            }


            // Debug.Log(party.Players[0].Items[0].ItemBase.ItemName);
            Destroy(other.gameObject);
            // idが早い順に並べる
            party.Players[0].Items.Sort((x, y) => y.Id - x.Id);

            // ログ出す
            for (int i = 0; i < party.Players[0].Items.Count; i++)
            {
                var item = party.Players[0].Items[i];
                Debug.Log($"Item Name: {item.ItemBase.ItemName}, Item Count: {item.ItemCount}");
            }
        }
        LoadItemData();
    }
    #endregion
    #region 
    // ステータス関係　=======================
    private void InitStatus()
    {
        // ステータス選択画面だったら
        if (currentGameStatus == GameStatus.STATUS)
        {
            playerStatusUIsManager.SetUpPlayerStatusUI(party.Players, false);
            playerStatusUIsManager.selectStatus(selectedStatusIndex);
        }
        else if (currentGameStatus == GameStatus.ITEM)
        {
            // ここ
            HealItemBase healItemBase = party.Players[0].Items[selectedItemIndex].ItemBase as HealItemBase;
            Debug.Log("healItemIsAll" + healItemBase.IsAll);
            playerStatusUIsManager.SetUpPlayerStatusUI(party.Players, healItemBase.IsAll);
            if (healItemBase.IsAll == false)
            {
                playerStatusUIsManager.selectStatus(selectedItemTargetIndex);
            }
        }
    }
    // マウスでステータスを選択　または　アイテムを使用するターゲットの選択
    public void SelectStatusButton(int selectStatusIndex)
    {
        // ステータス選択画面だったら
        if (currentGameStatus == GameStatus.STATUS)
        {
            selectedStatusIndex = selectStatusIndex;
            //playerStatusUIsManager.selectStatus(selectedStatusIndex);
            // 選択
            // 決定キーを押したら
            Debug.Log(selectedStatusIndex);
        }
        else if (currentGameStatus == GameStatus.ITEM)
        {
            List<bool> usedItem = new List<bool>();
            HealItemBase healItemBase = party.Players[0].Items[selectedItemIndex].ItemBase as HealItemBase;
            Debug.Log("えらばれているのは" + selectedItemIndex);
            // 全体アイテムだったら
            if (healItemBase.IsAll)
            {
                // アイテムを使う
                // アイテムの残りが0だったら
                if (party.Players[0].Items[selectedItemIndex].ItemCount == 0)
                {
                    Debug.Log("アイテムがないから使えないよ");
                }
                else
                {
                    Item item = party.Players[0].Items[selectedItemIndex];
                    // アイテムの効果発動
                    for (int i = 0; i < party.Players.Count; i++)
                    {
                        // 全員falseだったらアイテムを使わない
                        usedItem.Add(party.Players[i].TakeHeal(item));
                        party.Players[i].playerUI.UpdateHpSp();
                    }
                    // アイテムの数を減らす
                    if (usedItem.All(value => value == true))
                    {
                        party.Players[0].Items[selectedItemIndex].ItemCount--;
                    }
                    else
                    {
                        Debug.Log("体力が満タンで使えないよ");
                    }
                }
            }
            // アイテムだったらターゲットを選択する
            else
            {
                selectedItemTargetIndex = selectStatusIndex;
                // アイテムを使う
                // アイテムの残りが0だったら
                if (party.Players[0].Items[selectedItemIndex].ItemCount == 0)
                {
                    Debug.Log("アイテムがないから使えないよ");
                }
                else
                {
                    Item item = party.Players[0].Items[selectedItemIndex];
                    // アイテムの効果発動 体力が満タンでなければ
                    if (party.Players[selectedItemTargetIndex].TakeHeal(item) == true)
                    {
                        party.Players[selectedItemTargetIndex].playerUI.UpdateHpSp();
                        // アイテムの数を減らす
                        party.Players[0].Items[selectedItemIndex].ItemCount--;
                    }
                    else
                    {
                        Debug.Log("体力が満タンで使えないよ");
                    }
                }
            }
        }
    }
    #endregion
    // システム関係　=======================
    // マウスでセーブを選択
    public void SaveButton()
    {
        saveLoadCtrl.Save();
    }

    public void LoadButton()
    {
        saveLoadCtrl.Load();
    }
    private void FixedUpdate()
    {
        if (jumpFlag == true)
        {
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            jumpFlag = false;
        }

        float velocityY = Mathf.Clamp(rb.velocity.y, minVelocityY, jumpPower);
        rb.velocity = new Vector2(vx, velocityY);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "ground" || other.gameObject.tag == "digObject" || other.gameObject.tag == "Enemy")
        {
            groundFlag = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        groundFlag = false;
    }

}