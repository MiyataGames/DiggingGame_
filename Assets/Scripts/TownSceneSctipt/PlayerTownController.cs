using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.EventSystems;
using System.Linq;
using System;

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

    public static FieldGameState filedGameStatus;

    // プレイヤーのアニメーション
    private Animator myAnim;

    private enum PlayerVec
    {
        Left,
        Right,
        Up,
        Down
    }

    private PlayerVec playerVec;

    private bool isDigging = false;
    private bool isJumping = false;
    //[SerializeField] private GameObject digCollider;

    //DigController digController;


    // プレイヤー 仮
    //[SerializeField] private PlayerBase[] playerBasies;

    //[SerializeField] private GameObject[] playerUIs;
    [SerializeField] Party party;
    // セーブシステム
    //[SerializeField] SaveLoadController saveLoadCtrl;
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
    StatusState statusState = StatusState.STATUS_All;
    // ステータス画面の選択されているインデックス番号
    private int selectedStatusIndex;
    [SerializeField] private PlayerStatusUIsManager playerStatusUIsManager;
    [SerializeField] private StatusDescriptionUIManager statusDescriptionUIManager;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        //filedGameStatus = FieldGameState.DIGGING;
        currentMenuCommandNum = 0;
        currentItemNum = 0;
        selectedItemIndex = 0;
        selectedStatusIndex = 0;
        selectedItemTargetIndex = 0;
        //party.SetupFirst();
        //GameManager.instance.InitGame(party);                                                                
        // デリゲート
        menu.menuSelectButtonClickedDelegate = SelectMenuButton;
        menu.itemSelectButtonHoverdDelegate = CellButtonOnHover;
        playerStatusUIsManager.statusSelectButtonClickedDelegate = SelectStatusButton;
        // saveLoadCtrl.Load();
        myAnim.SetFloat("x", 1);
    }

    void Start()
    {
        playerVec = PlayerVec.Right;
    }

    void OnEnable()
    {
        isDigging = false;
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        // ゲームがポーズ中でないかつ穴掘り中だったら
        if (GameManager.instance.currentGameState == GameState.PLAYING || filedGameStatus == FieldGameState.DIGGING)
        {
            // サーチ
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // サーチ機能
            }
            // メニュー画面
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // ポーズ中にする
                GameManager.instance.currentGameState = GameState.POSE;
                // メニュー画面をひらく
                filedGameStatus = FieldGameState.MENU;
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

            if (Input.GetKey(KeyCode.A))// A：左
            {
                vx = -speed;
                playerVec = PlayerVec.Left;
                myAnim.SetFloat("x", -1);
            }
            else if (Input.GetKey(KeyCode.D))// D：右
            {
                vx = speed;
                playerVec = PlayerVec.Right;
                myAnim.SetFloat("x", 1);
            }
            else if (Input.GetKey(KeyCode.W))// 上キーを入力
            {
                vy = speed;
                playerVec = PlayerVec.Up;
                myAnim.SetFloat("y", 1);
            }
            else if (Input.GetKey(KeyCode.S))// 下キーを入力
            {
                vy = -speed;
                playerVec = PlayerVec.Down;
                myAnim.SetFloat("y", -1);
            }


            // 移動キーを離したら
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            {
                //
                myAnim.SetBool("isWalking", false);
            }

        }

        // メインパネルを選択中だったら
        else if (filedGameStatus == FieldGameState.MENU)
        {
            HandleMenuSelect();
        }
        // アイテムを選択中だったら
        else if (filedGameStatus == FieldGameState.ITEM)
        {
            HandleItemSelect();
        }
        // ステータスを選択中だったら
        else if (filedGameStatus == FieldGameState.STATUS)
        {
            if (statusState == StatusState.STATUS_All)
            {
                HandleStatusSelect();
            }
            else if (statusState == StatusState.STATUS_DISCRIPTION)
            {
                HandleStatusDescription();
            }
        }
        // システムを選択中だったら
        else if (filedGameStatus == FieldGameState.SYSTEM)
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
            // ポーズ終了
            GameManager.instance.currentGameState = GameState.PLAYING;
            filedGameStatus = FieldGameState.DIGGING;
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
            filedGameStatus = FieldGameState.ITEM;
            menu.ActivateMenuPanel(false);
            InitItem();
        }
        if (currentMenuCommandNum == (int)MenuCommand.STATUS)
        {
            menu.ActivateMenuPanel(false);
            menu.ActivateStatusPanel(true);
            filedGameStatus = FieldGameState.STATUS;
            InitStatus();
        }
        if (currentMenuCommandNum == (int)MenuCommand.SYSTEM)
        {
            menu.ActivateSystemPanel(true);
            menu.ActivateMenuPanel(false);
            filedGameStatus = FieldGameState.SYSTEM;
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
                filedGameStatus = FieldGameState.MENU;
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
                filedGameStatus = FieldGameState.ITEM;
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
            filedGameStatus = FieldGameState.MENU;
        }
    }

    void HandleSystemSelect()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // システム画面を閉じてメニュー画面を開く
            menu.ActivateMenuPanel(true);
            menu.ActivateSystemPanel(false);
            filedGameStatus = FieldGameState.MENU;
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
        if (item.ItemBase.ItemType == ItemType.HEAL_ITEM)
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
            Item newItem = other.GetComponent<FieldItem>().Item;
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
            party.Players[0].Items.Add(other.GetComponent<FieldItem>().Item);
            party.Players[0].Items[party.Players[0].Items.Count - 1].ItemCount++;
            Debug.Log(party.Players[0].Items[0].ItemBase.ItemName);
            Destroy(other.gameObject);
            // idが早い順に並べる
            party.Players[0].Items.Sort((x, y) => y.Id - x.Id);
            LoadItemData();
        }
        if (other.tag == "Coin")
        {
            int coinValue = other.GetComponent<FieldCoin>().Price;
            party.Players[0].Gold = party.Players[0].Gold + coinValue;
            print("CoinValue:" + party.Players[0].Gold);
            Destroy(other.gameObject);
            return;
        }

    }
    #endregion
    #region 
    // ステータス関係　=======================
    private void InitStatus()
    {
        // ステータス選択画面だったら
        if (filedGameStatus == FieldGameState.STATUS)
        {
            playerStatusUIsManager.SetUpPlayerStatusUI(party.Players, TARGET_NUM.SINGLE);
            playerStatusUIsManager.selectStatus(selectedStatusIndex);
        }
        else if (filedGameStatus == FieldGameState.ITEM)
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
        if (filedGameStatus == FieldGameState.STATUS)
        {
            selectedStatusIndex = selectStatusIndex;
            // ステータス画面の表示
            //playerStatusUIsManager.selectStatus(selectedStatusIndex);
            // 選択
            // 決定キーを押したら詳細パネルを初期化・表示する
            InitStatusDiscription();
            Debug.Log(selectedStatusIndex);
        }
        else if (filedGameStatus == FieldGameState.ITEM)
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

    // ステータス詳細を表示
    public void InitStatusDiscription()
    {
        statusState = StatusState.STATUS_DISCRIPTION;
        statusDescriptionUIManager.SetUpStatusDescription(GameManager.instance.Party.Players[selectedStatusIndex]);
        Debug.Log("詳細パネルをつける");
        statusDescriptionUIManager.gameObject.SetActive(true);
    }

    // ステータス詳細画面の操作
    private void HandleStatusDescription()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ステータス詳細画面をとじる
            statusDescriptionUIManager.gameObject.SetActive(false);
            statusState = StatusState.STATUS_All;
        }
    }
    #endregion
    // システム関係　=======================
    // マウスでセーブを選択
    /*
    public void SaveButton()
    {
        saveLoadCtrl.Save();
    }

    public void LoadButton()
    {
        saveLoadCtrl.Load();
    }
    */
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(vx, vy);
    }

    private void OnTriggerStay2D(Collider2D other)
    {

    }

    private void OnTriggerExit2D(Collider2D other)
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.tag);

    }

}