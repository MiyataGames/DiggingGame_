using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using TMPro;
using UnityEngine.Tilemaps;

public enum FieldGameState
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
    private float keyDirCheck;
    private bool isWallSliding = false;
    private bool isRightWall = false;
    private bool isLeftWall = false;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumingTime = 0.2f;
    private float inputBlockTimer;
    private float wallJumpingCounter;
    [SerializeField] private float wallJumpingDuration = 0.3f;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(8f, 12f);
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform WallCheck;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private CapsuleCollider2D dc;
    private bool jumpPressed = false;
    private bool jumpReleased = false;

    private float vx;
    private float vy;
    private float minVelocityY = -9.0f;
    private float minVelocityX = -9.0f;
    private float maxVelocityY = 10.0f;

    private Define.DirectionNumber currentDirectionNumber;
    [SerializeField] private Menu menu;

    public static FieldGameState filedGameStatus;

    // プレイヤーのアニメーション
    private Animator myAnim;

    // プレイヤーのスプライトレンダラー
    public bool isLeft = false;

    public bool isUp = false;
    private bool isDigging = false;

    [SerializeField] private GameObject digCollider;
    private DigController digController;

    // プレイヤー 仮
    //[SerializeField] private PlayerBase[] playerBasies;

    //[SerializeField] private GameObject[] playerUIs;
    [SerializeField] private Party party;

    // セーブシステム
    [SerializeField] private SaveLoadController saveLoadCtrl;

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
    private int selectedItemIndex = 0;

    // アイテムのターゲットのインデックス番号
    private int selectedItemTargetIndex;

    // ステータス関係 ==========================
    private StatusState statusState = StatusState.STATUS_All;

    // ステータス画面の選択されているインデックス番号
    private int selectedStatusIndex;

    [SerializeField] private PlayerStatusUIsManager playerStatusUIsManager;
    [SerializeField] private StatusDescriptionUIManager statusDescriptionUIManager;

    [SerializeField] private FadeController fadeController;

    [SerializeField] private Tilemap soilTilemap;
    [SerializeField] private TileBase soilTile;
    [SerializeField] private BoxCollider2D SetSoilCollider;

    // バトル関係
    [SerializeField] private int enemyBaseNumber;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        dc = digCollider.GetComponent<CapsuleCollider2D>();
        dc.enabled = false;
        myAnim = GetComponent<Animator>();
        filedGameStatus = FieldGameState.DIGGING;
        currentMenuCommandNum = 0;
        currentItemNum = 0;
        selectedItemIndex = 0;
        selectedStatusIndex = 0;
        selectedItemTargetIndex = 0;
        party.SetupFirst();
        GameManager.instance.InitGame(party);
        // デリゲート
        menu.menuSelectButtonClickedDelegate = SelectMenuButton;
        menu.itemSelectButtonHoverdDelegate = CellButtonOnHover;
        playerStatusUIsManager.statusSelectButtonClickedDelegate = SelectStatusButton;
        // saveLoadCtrl.Load();
        myAnim.SetFloat("isLeft", -1);
        SetSoilCollider.enabled = false;
    }

    private void OnEnable()
    {
        isDigging = false;
        // this.GetComponent<SpriteRenderer>().flipX = false;
        // Debug.Log(this.GetComponent<SpriteRenderer>().flipX);
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        // ゲームがポーズ中だったら全ての処理を受け付けない
        if (GameManager.instance.currentGameState == GameState.POSE)
        {
            vx = 0;
            vy = 0;
            myAnim.SetBool("isWalking", false);
            return;
        }
        // ゲームがメニュー中でないかつ穴掘り中だったら
        if (GameManager.instance.currentGameState != GameState.MENU && filedGameStatus == FieldGameState.DIGGING)
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
                GameManager.instance.currentGameState = GameState.MENU;
                // メニュー画面をひらく
                filedGameStatus = FieldGameState.MENU;
                menu.ActivateMenuPanel(true);
                // ゴールドを表示
                menu.ActivateGoldText(true);
                menu.ActivateMenuSelectArrow((int)MenuCommand.ITEM);
            }
            // マップ
            if (Input.GetKeyDown(KeyCode.M))
            {
                // マップを開く
            }

            // 左右の移動
            keyDirCheck = Input.GetAxis("Horizontal");

            if (Input.GetKey(KeyCode.D))
            {
                if (isDigging == false)
                {
                    isLeft = false;
                    myAnim.SetFloat("isLeft", -1);
                    vx = moveSpeed;

                    myAnim.SetBool("isWalking", true);
                    myAnim.SetFloat("isUp", 0);
                }
            }
            else if (Input.GetKey(KeyCode.A))
            {
                if (isDigging == false)
                {
                    isLeft = true;
                    myAnim.SetFloat("isLeft", 1);
                    vx = -moveSpeed;

                    myAnim.SetBool("isWalking", true);
                    myAnim.SetFloat("isUp", 0);
                }
            }
            else
            {
                vx = 0;
                myAnim.SetBool("isWalking", false);
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (isDigging == false)
                {
                    startDigAction();
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (isDigging == false)
                {
                    if (SetSoilCollider != null)
                    {
                        SetSoilCollider.enabled = true;

                        Bounds bounds = SetSoilCollider.bounds;

                        SetSoilTile(bounds);
                    }
                }
            }

            /*// 上キーを入力
            if (Input.GetKey(KeyCode.W))
            {
                myAnim.SetFloat("isUp", 1);
            }
            // 下キーを入力
            else if (Input.GetKey(KeyCode.S))
            {
                myAnim.SetFloat("isUp", -1);
            }*/

            // ジャンプボタンが押されたことをフラグで管理
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
            {
                jumpPressed = true;
            }

            // ジャンプボタンが離されたときの処理もフラグで管理
            if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0f)
            {
                jumpReleased = true;
            }

            WallSlide();
            WallJump();
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

    // 穴掘り関係
    // 掘るアニメーション終了時にアニメーション側から呼び出し
    public void endDiggingAnim()
    {
        endDigAction();
    }

    public void endDiggingDownUpAnim()
    {
        myAnim.SetFloat("isUp", 0);
        endDigAction();
    }

    private void endDigAction()
    {
        myAnim.SetBool("isDigging", false);
        isDigging = false;
        //dc.enabled = false;
    }

    private void StartDig()
    {
        dc.enabled = true;
    }

    private void EndDig()
    {
        dc.enabled = false;
    }

    private void startDigAction()
    {
        isDigging = true;
        //CapsuleCollider2D dc = digCollider.GetComponent<CapsuleCollider2D>();
        //BoxCollider2D dc = digCollider.GetComponent<BoxCollider2D>();

        if (Input.GetKey(KeyCode.W))
        {
            myAnim.SetFloat("isUp", 1);
            dc.offset = new Vector2(0.0f, 0.68f);
            dc.size = new Vector2(0.76f, 0.45f);
            dc.direction = CapsuleDirection2D.Horizontal;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            myAnim.SetFloat("isUp", -1);
            dc.offset = new Vector2(0.0f, -0.9f);
            dc.size = new Vector2(0.76f, 0.45f);
            dc.direction = CapsuleDirection2D.Horizontal;
        }
        else if (isLeft == true)
        {
            dc.offset = new Vector2(-0.47f, -0.06f);
            dc.size = new Vector2(0.5f, 0.85f);
            dc.direction = CapsuleDirection2D.Vertical;
        }
        else if (isLeft == false)
        {
            dc.offset = new Vector2(0.47f, -0.06f);
            dc.size = new Vector2(0.5f, 0.85f);
            dc.direction = CapsuleDirection2D.Vertical;
        }

        myAnim.SetBool("isJumping", false);
        myAnim.SetBool("isWalking", false);
        myAnim.SetBool("isDigging", true);
    }

    private void SetSoilTile(Bounds bounds)
    {
        Vector3Int min = soilTilemap.WorldToCell(bounds.min);
        Vector3Int max = soilTilemap.WorldToCell(bounds.max);

        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                if (soilTilemap.HasTile(cellPosition) == false)
                {
                    soilTilemap.SetTile(cellPosition, soilTile);
                }
            }
        }

        SetSoilCollider.enabled = false;
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
            // ゴールドを非表示
            menu.ActivateGoldText(false);
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
                filedGameStatus = FieldGameState.MENU;
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
                    players[selectedItemTargetIndex].TakeHealWithItem(item);
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
                filedGameStatus = FieldGameState.ITEM;
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
            filedGameStatus = FieldGameState.MENU;
        }
    }

    private void HandleSystemSelect()
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
                selectedId = i,
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
        /*
        EventTrigger eventTrigger = cellViewObj.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventDate) => CellButtonOnHover(dataIndex));
        eventTrigger.triggers.Add(entry);
        */
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
        selectedItemIndex = selectedIndex;
        // アクティブセルに対してUIの更新をする
        itemPanel.RefreshActiveCellViews();
        Debug.Log("アイテムを使うよ" + party.Players[0].Items[selectedItemIndex].ItemBase.ItemName);
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
            GameManager.instance.Party.Players[0].AddItem(newItem);
            Destroy(other.gameObject);
            LoadItemData();
            /*
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
            */
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
        //Debug.Log(isWallJumping);
        if (!isWallJumping && inputBlockTimer <= 0)
        {
            rb.velocity = new Vector2(vx, rb.velocity.y);
        }
        else if (isWallJumping && inputBlockTimer <= 0)
        {
            vx = Mathf.Clamp(vx, -vx / 2, vx / 2);
            rb.velocity = new Vector2(vx, rb.velocity.y);
        }

        if (isGrounded())
        {
            isWallJumping = false;
            inputBlockTimer = 0f;
            myAnim.SetBool("isJumping", false);
        }

        // ジャンプ処理
        if (jumpPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpPressed = false;
            if (isDigging == false)
            {
                myAnim.SetBool("isWalking", false);
                myAnim.SetBool("isJumping", true);
            }
        }

        // ジャンプボタンが離された時の処理
        if (jumpReleased)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            jumpReleased = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("敵に当たった");
            // バトルシーンに移動する
            //GameManager.instance.CurrentSceneIndex = (int)GameMode.BATTLE_SCENE;
            // バトルシーンに移動する
            //Fade.Instance.RegisterFadeOutEvent(GameManager.instance.StartBattle(collision.gameObject));
            //fadeController.OnFadeOutComplete += GameManager.instance.StartBattle(collision.gameObject);

            //fadeController.FadeOut();
            //Fade.Instance.StartFadeInBattle(collision.gameObject);
            GameManager.instance.StartBattle(collision.gameObject, enemyBaseNumber);
        }
        else if (collision.gameObject.tag == "Town")
        {
            Debug.Log("タウンに切り替え");
            // 街へ入る
            GameManager.instance.CurrentSceneIndex = (int)GameMode.TOWN_SCENE;
            //GameManager.instance.ActivateCurrentScene((int)GameMode.TOWN_SCENE);
        }
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.05f, groundLayer);
    }

    private bool IsWalled()
    {
        Vector2 origin = transform.position;

        if (Physics2D.OverlapCircle(origin - new Vector2(0.25f, 0.0f), 0.1f, groundLayer))
        {
            if (isLeft)
            {
                isLeftWall = true;
                if (isRightWall == true)
                {
                    inputBlockTimer = 0f;
                }
                return true;
            }
        }
        else if (Physics2D.OverlapCircle(origin + new Vector2(0.25f, 0.0f), 0.1f, groundLayer))
        {
            if (!isLeft)
            {
                isRightWall = true;
                if (isLeftWall == true)
                {
                    inputBlockTimer = 0f;
                }
                return true;
            }
        }

        isLeftWall = false;
        isRightWall = false;

        return false;
    }

    private void WallSlide()
    {
        if (IsWalled() && !isGrounded() && keyDirCheck != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            if (inputBlockTimer <= 0)
            {
                isWallJumping = false;
            }

            if (isLeft)
            {
                wallJumpingDirection = Vector2.right.x;
            }
            else
            {
                wallJumpingDirection = Vector2.left.x;
            }

            wallJumpingCounter = wallJumingTime;
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isWallSliding)
        {
            isWallJumping = true;

            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);

            wallJumpingCounter = 0f;
            inputBlockTimer = wallJumpingDuration;
        }

        inputBlockTimer -= Time.deltaTime;
        if (inputBlockTimer <= 0)
        {
            isWallJumping = false;
        }
    }
}