using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.EventSystems;
using System.Linq;


public class PlayerTownController : MonoBehaviour, IEnhancedScrollerDelegate
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
        // デリゲート
        menu.menuSelectButtonClickedDelegate = SelectMenuButton;
        menu.itemSelectButtonHoverdDelegate = CellButtonOnHover;
        playerStatusUIsManager.statusSelectButtonClickedDelegate = SelectStatusButton;
        // saveLoadCtrl.Load();
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        Debug.Log("実行中");
        // 穴掘り中だったら
        if (currentGameStatus == GameStatus.DIGGING)
        {
            Debug.Log("移動できるはず");
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
            Item newItem = other.GetComponent<Item>();
            // 同じアイテムがあるか検索
            if (party.Players[0].Items.Count > 0)
            {
                for (int i = 0; i < party.Players[0].Items.Count; i++)
                {
                    if (newItem.ItemBase.Id == party.Players[0].Items[i].ItemBase.Id)
                    {
                        // あったら個数を増やして破棄
                        party.Players[0].Items[i].ItemCount++;
                        Destroy(other.gameObject);
                        return;
                    }
                }
            }
            // なければ新しく追加する
            party.Players[0].Items.Add(other.GetComponent<Item>());
            party.Players[0].Items[party.Players[0].Items.Count - 1].ItemCount++;
            Debug.Log(party.Players[0].Items[0].ItemBase.ItemName);
            Destroy(other.gameObject);
            // idが早い順に並べる
            party.Players[0].Items.Sort((x, y) => y.Id - x.Id);
            LoadItemData();
        }

    }
    #endregion
    #region 
    // ステータス関係　=======================
    private void InitStatus()
    {
        // ステータス選択画面だったら
        if (currentGameStatus == GameStatus.STATUS)
        {
            playerStatusUIsManager.SetUpPlayerStatusUI(party.Players, TARGET_NUM.SINGLE);
            playerStatusUIsManager.selectStatus(selectedStatusIndex);
        }
        else if (currentGameStatus == GameStatus.ITEM)
        {
            // ここ
            HealItemBase healItemBase = party.Players[0].Items[selectedItemIndex].ItemBase as HealItemBase;
            Debug.Log("healItemIsAll" + healItemBase.TargetNum);
            playerStatusUIsManager.SetUpPlayerStatusUI(party.Players, healItemBase.TargetNum);
            if (healItemBase.TargetNum == TARGET_NUM.SINGLE)
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
            if (healItemBase.TargetNum == TARGET_NUM.ALL)
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
                        usedItem.Add(party.Players[i].TakeHealWithItem(item));
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
                    if (party.Players[selectedItemTargetIndex].TakeHealWithItem(item) == true)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("敵に当たった");
            // バトルシーンに移動する
            //GameManager.instance.CurrentSceneIndex = (int)GameMode.BATTLE_SCENE;
            // バトルシーンに移動する
            GameManager.instance.StartBattle();
            // 敵オブジェクトを破壊
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Town")
        {
            // 街へ入る
            GameManager.instance.CurrentSceneIndex = (int)GameMode.TOWN_SCENE;
        }
    }

}