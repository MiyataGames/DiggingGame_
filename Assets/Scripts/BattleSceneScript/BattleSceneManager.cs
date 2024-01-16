using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using DG.Tweening;
using Unity.VisualScripting;

public enum BattleState
{
    INIT_BATTLE,
    PREPARE_BATTLE,
    DIGGING,
    CHECK_CONDITION,
    PLAYER_ACTION_SELECT,
    PLAYER_MOVE,
    ENEMY_MOVE,
    CHECK_HOLE_POSITION,
    BUSY,
    END_BATTLE,
}

public enum StatusState
{
    STATUS_All,
    STATUS_DISCRIPTION,
    END
}

public enum CHECK_HOLE_POSITION_PHASE
{
    FIRST,
    END
}

public enum Move
{
    ATTACK,
    ITEM,
    STATUS,
    ESCAPE,
    END
}

// スキルについての入力の状態遷移を表す列挙型「InputSkillStatement」を定義する
internal enum InputSkillStatement
{
    INIT_SKILL,
    SKILL_SELECT,
    TARGET_SELECT,
    END_INPUT
}

// アイテムについての入力の状態遷移を表す列挙型「InputItemStatement」を定義する
internal enum InputItemStatement
{
    INIT_ITEM,
    ITEM_SELECT,
    TARGET_SELECT,
    END_INPUT
}

// 穴掘りについての入力の状態遷移を表す列挙型「InputDiggingStatement」を定義する
public enum InputDiggingStatement
{
    INIT_DIGGING,
    BEDDING_ITEM,
    END_INPUT
}

// どこで気絶状態を戻すか問題
// 前のターンに気絶していたかどうか
public class BattleSceneManager : MonoBehaviour, IEnhancedScrollerDelegate
{
    // ステータス関係 =====================
    [SerializeField] GameObject statusPanel;
    [SerializeField] private PlayerStatusUIsManager playerStatusUIsManager;
    [SerializeField] private StatusDescriptionUIManager statusDescriptionUIManager;
    private int selectedStatusIndex;
    // バトル関係 ==========================
    [SerializeField] private List<Enemy> activeEnemies;

    [SerializeField] private List<Player> activePlayers;

    [SerializeField] private float speed;

    //[SerializeField] private DialogBox dialogBox;

    [SerializeField] private BattleCommand battleCommand;
    public BattleState battleState = BattleState.BUSY;
    private InputSkillStatement inputSkillStatement;
    private InputItemStatement inputItemStatement;
    StatusState statusState = StatusState.STATUS_All;

    [SerializeField]
    private List<Character> characters;// 戦闘に参加してるキャラクター

    private int turnCharacterIndex;
    [SerializeField] private Character turnCharacter;// 行動順のキャラクター

    [SerializeField]
    private Enemy[] selectedEnemies;

    [SerializeField]
    private Player[] selectedPlayers;

    /// スキル関係==================================
    private List<SkillCellData> skillDatas;

    private int selectedSkillIndex;// リストの選択されたインデックス

    // デリゲートのためのスクローラー　
    // スクローラーに定義されたタイミングで実行する関数を書く
    public EnhancedScroller playerSkillPanel;

    // スクローラーの各セルのプレハブ
    public EnhancedScrollerCellView cellViewPrefab;

    public float cellSize;

    private int selectedTargetIndex;

    // アイテム関係==================================
    public Player mainPlayer;

    private List<ItemCellData> itemCellDatas;
    private int selectedItemIndex;// 選択されたアイテムのインデックス
    public EnhancedScrollerCellView itemCellViewPrefab;

    //アニメーション関係=============================================
    private static readonly int hashIdle = Animator.StringToHash("Base Layer.Idle");

    private static readonly int hashTurnIdle = Animator.StringToHash("Base Layer.TurnIdle");
    private static readonly int hashRun = Animator.StringToHash("Base Layer.Run");

    private static readonly int hashDamage = Animator.StringToHash("Base Layer.Damage");
    private static readonly int hashDeath = Animator.StringToHash("Base Layer.Death");
    private static readonly int hashNone = Animator.StringToHash("Base Layer.NoneAnimation");

    // private static readonly int hashFaint = Animator.StringToHash("Base Layer.Faint");
    private static readonly int hashHeal = Animator.StringToHash("Base Layer.Heal");

    private static readonly int hashAttack = Animator.StringToHash("Base Layer.Attack");
    private static readonly int hashSkill = Animator.StringToHash("Base Layer.Skill");
    private static readonly int hashUseItem = Animator.StringToHash("Base Layer.UseItem");

    // 現在のMove

    private int currentMove = 1;

    public Character TurnCharacter { get => turnCharacter; set => turnCharacter = value; }

    // 穴掘り関係 ================================
    [SerializeField] private DiggingGridManager diggingGridManager;

    public InputDiggingStatement inputDiggingStatement;
    private CHECK_HOLE_POSITION_PHASE checkHolePositionPhase;
    [SerializeField] private GameObject[] diggingPositions;

    // 移動先のポジション
    private int targetPositionIndex = -1;

    public void StartBattle()
    {
        Application.targetFrameRate = 60;
        currentMove = 1;
        battleState = BattleState.INIT_BATTLE;
        inputSkillStatement = InputSkillStatement.INIT_SKILL;
        inputItemStatement = InputItemStatement.INIT_ITEM;
        inputDiggingStatement = InputDiggingStatement.INIT_DIGGING;
        diggingGridManager.diggingFinishDelegate = FinishDigging;
        diggingGridManager.selectItemDelegate = SelectedItem;
    }

    private void Start()
    {
        playerStatusUIsManager.statusSelectButtonClickedDelegate = SelectStatusButton;
    }

    public void HandleUpdate()
    {
        if (battleState == BattleState.INIT_BATTLE)
        {
            //PrepareInitBattle();
            //InitBattle();
        }
        else if (battleState == BattleState.PREPARE_BATTLE)
        {
            // プレイヤーが走ってくる
            PrepareBattle();
        }
        else if (battleState == BattleState.DIGGING)
        {
            if (inputDiggingStatement == InputDiggingStatement.INIT_DIGGING)
            {
                // アイテム一覧を表示する
                // 穴掘りパネルの入力を受け付ける
                InitDigging();
            }
            else if (inputDiggingStatement == InputDiggingStatement.BEDDING_ITEM)
            {
                // アイテムを選んで
                HandleDiggingItemSelection();
                // 穴に埋める
            }
            else if (inputDiggingStatement == InputDiggingStatement.END_INPUT)
            {
                // 穴掘り処理終了
                // ボタンの入力を受け付けなくする
            }
        }
        else if (battleState == BattleState.CHECK_CONDITION)
        {
            // 状態異常のチェック
            CheckCondition();
        }
        else if (battleState == BattleState.PLAYER_ACTION_SELECT)
        {
                HandleActionSelection();

            
        }
        else if (battleState == BattleState.PLAYER_MOVE)
        {
            if ((Move)currentMove == Move.ATTACK)
            {
                if (inputSkillStatement == InputSkillStatement.INIT_SKILL)
                {
                    PlayerAction();
                }
                else if (inputSkillStatement == InputSkillStatement.SKILL_SELECT)
                {
                    HandleSkillSelection();
                }
                else if (inputSkillStatement == InputSkillStatement.TARGET_SELECT)
                {
                    Player turnPlayer = (Player)turnCharacter;
                    HandleSelectTarget(turnPlayer.Skills[selectedSkillIndex]);
                }
                else if (inputSkillStatement == InputSkillStatement.END_INPUT)
                {
                    PlayerEndSkillInput();
                }
            }
            else if ((Move)currentMove == Move.ITEM)
            {
                if (inputItemStatement == InputItemStatement.INIT_ITEM)
                {
                    PlayerAction();
                }
                else if (inputItemStatement == InputItemStatement.ITEM_SELECT)
                {
                    HandleItemSelection();
                }
                else if (inputItemStatement == InputItemStatement.TARGET_SELECT)
                {
                    HandleSelectItemTarget();
                }
                else if (inputItemStatement == InputItemStatement.END_INPUT)
                {
                    PlayerEndItemInput();
                }
            }else if ((Move)currentMove == Move.STATUS)
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
        }
        /*
        else if (battleState == BattleState.CHECK_HOLE_POSITION)
        {
            CheckHolePosition(targetPositionIndex);
        }*/
        else if (battleState == BattleState.ENEMY_MOVE)
        {
            if (inputSkillStatement == InputSkillStatement.INIT_SKILL)
            {
                EnemyMove();
            }
            else if (inputSkillStatement == InputSkillStatement.SKILL_SELECT)
            {
                EnemySkillSelect();
            }
            else if (inputSkillStatement == InputSkillStatement.TARGET_SELECT)
            {
                EnemyTargetSelect(activeEnemies[0].Skills[selectedSkillIndex]);
            }
            else if (inputSkillStatement == InputSkillStatement.END_INPUT)
            {
                EnemyEndInput();
            }
        }
    }

    // あとでGameManagerに移植する

    private void PrepareInitBattle()
    {
        /*
        // playersとenemiesのリストを用意する
        players = new List<Player>();
        enemies = new List<Enemy>();

        // 1レベルのプレイヤーを生成する
        if (firstBattle == true)
        {
            playerUnit.SetUpFirst(1);
            firstBattle = false;
        }
        else
        {
            playerUnit.SetUp();
        }
        // enemyのセットアップ1~3レベルの敵を生成
        enemyUnit.SetUp(1, 3);
        // playersにソートされたplayerをいれる
        for (int i = 0; i < playerUnit.Players.Length; i++)
        {
            players.Add(playerUnit.Players[i]);
        }
        for (int i = 0; i < enemyUnit.Enemies.Length; i++)
        {
            enemies.Add(enemyUnit.Enemies[i]);
        }

        // 主人公を探す
        mainPlayer = players[0];
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].PlayerID == 0)
            {
                mainPlayer = players[i];
            }
        }

        InitBattle(players, enemies);
        */
    }

    // 戦闘が始まる前に一回だけ実行する===========================
    public void InitBattle(List<Player> players, List<Enemy> enemies, Player mainPlayer)
    {
        inputSkillStatement = InputSkillStatement.INIT_SKILL;
        this.mainPlayer = mainPlayer;
        // プレイヤーの生成
        /*
        if (FirstBattle == true)
        {
            playerUnit.SetUpFirst(1);
            FirstBattle = false;
        }
        else
        {
            playerUnit.SetUp();
        }
        // モンスターの生成
        enemyUnit.SetUp(1, 3);
        */
        characters = new List<Character>();
        activePlayers = new List<Player>();
        activeEnemies = new List<Enemy>();
        //activePlayers = players;
        //activeEnemies = enemies;
        // リストは参照渡しになる
        for (int i = 0; i < players.Count; i++)
        {
            activePlayers.Add(players[i]);
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            activeEnemies.Add(enemies[i]);
        }

        Dictionary<Character, int> agiCharaDictionary = new Dictionary<Character, int>();

        for (int i = 0; i < activePlayers.Count; i++)
        {
            agiCharaDictionary.Add(activePlayers[i], activePlayers[i].PlayerBase.PlayerMaxAgi);
        }
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            agiCharaDictionary.Add(activeEnemies[i], activeEnemies[i].EnemyBase.Agi);
        }

        // 初期化
        characters = new List<Character>();

        // Enemyのソート
        // ソート
        foreach (var Chara in agiCharaDictionary.OrderByDescending(c => c.Value))
        {
            characters.Add(Chara.Key);
            if (Chara.Key.isPlayer)
            {
                // デバッグ用
                Player player = (Player)Chara.Key;
            }
            else
            {
                Enemy enemy = (Enemy)Chara.Key;
            }
        }

        // charactersに代入
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].isPlayer)
            {
                Player player = (Player)characters[i];
                // Debug.Log(player.PlayerBase.PlayerName);
            }
            else
            {
                Enemy enemy = (Enemy)characters[i];
                //Debug.Log(enemy.EnemyBase.EnemyName);
            }
        }
        // 一番早いキャラクターが動くキャラクターturnCharacter
        turnCharacterIndex = 0;
        turnCharacter = characters[turnCharacterIndex];

        // キャラクターが持ち場につく
        battleState = BattleState.PREPARE_BATTLE;
        Debug.Log("初期化完了");
    }

    private void PrepareBattle()
    {
        battleState = BattleState.DIGGING;
    }

    private bool PreparePlayerPosition(float playerSpeed, Transform playerTargetPos, GameObject playerModel, Animator playerAnimator)
    {
        float directionX = playerTargetPos.position.x - playerModel.gameObject.transform.position.x;
        float directionZ = playerTargetPos.position.z - playerModel.gameObject.transform.position.z;

        Vector3 direction = new Vector3(directionX, 0, directionZ);
        playerModel.gameObject.transform.LookAt(playerTargetPos);
        Vector3 directionNormarized = direction.normalized;

        //Debug.Log("目指す場所" + playerTargetPos.position + "現在地" + playerModel.gameObject.transform.position);
        float diffX = playerTargetPos.position.x - playerModel.gameObject.transform.position.x;
        float diffZ = playerTargetPos.position.z - playerModel.gameObject.transform.position.z;
        Vector3 diff = new Vector3(diffX, 0, diffZ);
        if (diff.magnitude < 0.1f)
        {
            // アニメーションの終了
            playerAnimator.SetBool("RunToIdle", true);
            return true;
        }
        playerModel.transform.position += playerSpeed * Time.deltaTime * directionNormarized;

        return false;
    }

    // =========================状態異常をチェックする
    private void CheckCondition()
    {
        Debug.Log("状態異常のチェックフェーズ");
        // 状態異常の更新
        turnCharacter.UpdateConditions();
        // まひだったら
        if (turnCharacter.IsCharacterParalyzed() == true)
        {
            float random = Random.Range(0, 1.0f);
            // 50%の確率で動けない
            if (random < 0.5f)
            {
                Debug.Log("麻痺なのでスキップ");
                SkipTurn();
            }
            else
            {
                if (turnCharacter.isPlayer)
                {
                    battleState = BattleState.PLAYER_MOVE;
                }
                else
                {
                    battleState = BattleState.ENEMY_MOVE;
                }
                inputSkillStatement = InputSkillStatement.INIT_SKILL;
                inputItemStatement = InputItemStatement.INIT_ITEM;
            }
        }
        else if (turnCharacter.IsCharacterSleeped() == true)
        {
            Debug.Log("睡眠中なのでスキップ");
            SkipTurn();
        }
        else
        {
            if (turnCharacter.isPlayer)
            {
                battleState = BattleState.PLAYER_MOVE;
            }
            else
            {
                battleState = BattleState.ENEMY_MOVE;
            }
            inputSkillStatement = InputSkillStatement.INIT_SKILL;
            inputItemStatement = InputItemStatement.INIT_ITEM;
        }
    }

    // 穴掘りフェーズ=======================================

    // 穴掘りパネルを操作する
    private void InitDigging()
    {
        // アイテムの表示
        Debug.Log("---PlayerDIGGINGInit---");
        // スキルパネルを表示
        battleCommand.ActivateSkillCommandPanel(true);
        // 穴掘りパネルを有効にする
        diggingGridManager.StartDigging();

        // EnhancedScrollerのデリゲートを指定する
        // デリゲートを設定することで、スクロールビューが必要な情報を取得
        playerSkillPanel.Delegate = this;
        // 主人公のアイテムをセットする
        LoadDiggingItemData(mainPlayer.items);
        inputDiggingStatement = InputDiggingStatement.BEDDING_ITEM;
    }

    // 穴掘りで埋めるアイテムを選択する
    private void HandleDiggingItemSelection()
    {
        bool selectionChanged = false;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedItemIndex = Mathf.Clamp(selectedItemIndex - 1, 0, itemCellDatas.Count - 1);
            selectionChanged = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedItemIndex = Mathf.Clamp(selectedItemIndex + 1, 0, itemCellDatas.Count - 1);
            selectionChanged = true;
        }

        // アイテム決定
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // アイテムをセットする
            Item item = mainPlayer.items.Find(value => value.ItemBase.ItemName == itemCellDatas[selectedItemIndex].itemText);
            diggingGridManager.SetSelectedItem(item);
        }

        // 選択中
        if (selectionChanged)
        {
            // 選択されたインデックスが更新されたから基礎データを更新する
            for (int i = 0; i < itemCellDatas.Count; i++)
            {
                // 選択中のisSelectedをtrueにする
                itemCellDatas[i].isSelected = (i == selectedItemIndex);
            }

            // アクティブセルに対してUIの更新をする
            playerSkillPanel.RefreshActiveCellViews();
            /*
            // 選択されたインデックスが最下部またはその先にある時
            if (selectedItemIndex >= playerSkillPanel.EndCellViewIndex)
            {
                playerSkillPanel.JumpToDataIndex(selectedItemIndex, 1.0f, 1.0f);
            }
            else if (selectedItemIndex <= playerSkillPanel.StartCellViewIndex)
            {
                // 選択されたインデックスが最上部またはそれ以上にある時
                playerSkillPanel.JumpToDataIndex(selectedItemIndex, 0.0f, 0.0f);
            }*/
        }
    }

    // アイテムが選択された時に呼ばれる関数
    private bool SelectedItem(Item selectedItem)
    {
        if (mainPlayer.items.Find(item => item == selectedItem).ItemCount > 0)
        {
            // プレイヤーの持っているアイテムを探す
            mainPlayer.items.Find(item => item == selectedItem).ItemCount--;
            // パネルの更新
            LoadDiggingItemData(mainPlayer.items);
            playerSkillPanel.RefreshActiveCellViews();
            // 選択している位置まで飛ぶ
            playerSkillPanel.JumpToDataIndex(selectedItemIndex, 1.0f, 1.0f);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void FinishDigging()
    {
        // 穴掘りフェーズの終了処理
        inputDiggingStatement = InputDiggingStatement.END_INPUT;
        battleState = BattleState.CHECK_CONDITION;
        // 必要ならここでgridItemsのデータを他のコンポーネントに渡す
        // パネルの非表示
        battleCommand.ActivateSkillCommandPanel(false);
    }

    // ==========================メインパネルからアクションを選択する
    private float animationTransitionTime = 0;

    private bool onceAnim;

    private void HandleActionSelection()
    {
        Player player = turnCharacter as Player;
        //player.PlayerBattleAnimator.SetBool("IdleToTurnIdle", true);
        //player.PlayerBattleAnimator.SetBool("TurnIdleToIdle", false);
        ///////////// アニメーションの指定 /////////////
        // 10秒立ったら
        // 10秒ごとにフラフラするアニメーション
        /*
        animationTransitionTime += Time.deltaTime;
        Player turnPlayer = (Player)turnCharacter;
        if (animationTransitionTime > 10 && animationTransitionTime < 11)
        {
            // 今選択しているキャラクターの待機モーションを変更する
            if (onceAnim == false)
            {
                turnPlayer.PlayerAnimator.SetTrigger("IdleToIdle2");
                onceAnim = true;
            }
        }
        if (animationTransitionTime > 20)
        {
            onceAnim = false;
            animationTransitionTime = 0;
        }*/

        ////////////////////////////////////////////
        // メインパネルの選択
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < (int)Move.END - 1)
            {
                currentMove++;
                battleCommand.ActivateBattleSelectArrow((Move)currentMove);
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 0)
            {
                currentMove--;
                battleCommand.ActivateBattleSelectArrow((Move)currentMove);
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // UIの非表示
            battleCommand.ActivateBattleCommandPanel(false);
            PlayerMoveInit();
        }
    }

    private void PlayerAction()
    {
        // メインパネルを表示する
        battleState = BattleState.PLAYER_ACTION_SELECT;
        Debug.Log("=====================" + battleState + "====================");

        battleCommand.ActivateBattleCommandPanel(true);

        //初期選択コマンド
        currentMove = (int)Move.ATTACK;
        battleCommand.ActivateBattleSelectArrow((Move)currentMove);
    }

    // スキルとかアイテム
    private void PlayerMoveInit()
    {
        // 気絶しているなら
        if (TurnCharacter.isFainted == true)
        {
            // 復活モーション
            TurnCharacter.isFainted = false;
        }
        battleState = BattleState.PLAYER_MOVE;
        // Moveに応じたパネルを表示する
        if ((Move)currentMove == Move.ATTACK)
        {
            InitSkill();
            inputSkillStatement = ChangeInputSkillStatement();
            Player player = turnCharacter as Player;
            //player.PlayerBattleAnimator.SetBool("IdleToTurnIdle", false);
            //player.PlayerBattleAnimator.SetBool("TurnIdleToIdle", false);
        }
        if ((Move)currentMove == Move.ITEM)
        {
            // アイテムをロードする
            InitItem();
            inputItemStatement = ChangeInputItemStatement();
            Player player = turnCharacter as Player;
            //player.PlayerBattleAnimator.SetBool("IdleToTurnIdle", false);
            //player.PlayerBattleAnimator.SetBool("TurnIdleToIdle", false);
        }
        // ステータス画面を開く
        if ((Move)currentMove == Move.STATUS)
        {
            Debug.Log(currentMove);
            statusPanel.SetActive(true);
            InitStatus();
        }
        // 逃げる
        if ((Move)currentMove == Move.ESCAPE)
        {
            EscapeBattle();
        }
    }

    // ステータス画面 ==========================
    // 操作
    private void HandleStatusSelect()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ステータス画面をとじる
            statusPanel.SetActive(false);
            //currentMove = Move()
            // メインパネルを表示する
            battleCommand.ActivateBattleCommandPanel(true);
            battleState = BattleState.PLAYER_ACTION_SELECT;
        }
    }
    // ステータス画面の初期化
    private void InitStatus()
    {
        statusState = StatusState.STATUS_All;
        // ステータス選択画面だったら
        Debug.Log("ステータス画面を更新");
        for(int i = 0;i < activePlayers.Count; i++)
        {
            // パーティプレイヤーのHPとSPを更新
            // バトル中のプレイヤーの情報
            Player activePlayer = activePlayers[i];
            // 更新先のパーティのプレイヤー
            Player updatePlayer = GameManager.instance.Party.Players.Find(player => player.PlayerID == activePlayer.PlayerID);
            // 更新
            updatePlayer = activePlayer;
        }
        for(int i = 0;i < GameManager.instance.Party.Players.Count;i++)
        {
            Debug.Log("パーティプレイヤーを更新" + GameManager.instance.Party.Players[i].currentSP);
        }
        playerStatusUIsManager.SetUpPlayerStatusUI(GameManager.instance.Party.Players, TARGET_NUM.SINGLE);
        playerStatusUIsManager.selectStatus(selectedStatusIndex);
    }

    
    // マウスでステータスを選択
    public void SelectStatusButton(int selectStatusIndex)
    {
        selectedStatusIndex = selectStatusIndex;
        //playerStatusUIsManager.selectStatus(selectedStatusIndex);
        // 選択
        // 決定キーを押したら
        Debug.Log(selectedStatusIndex);
        InitStatusDiscription();
    }

    // 詳細を表示
    public void InitStatusDiscription()
    {
        statusState = StatusState.STATUS_DISCRIPTION;
        Debug.Log("詳細パネルをつける");
        Debug.Log((Move)currentMove);
        Debug.Log(battleState);
        statusDescriptionUIManager.gameObject.SetActive(true);
        statusDescriptionUIManager.SetUpStatusDescription(GameManager.instance.Party.Players[selectedStatusIndex]);
    }

    private void HandleStatusDescription()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ステータス詳細画面をとじる
            statusDescriptionUIManager.gameObject.SetActive(false);
            statusState = StatusState.STATUS_All;
            // メインパネルを表示する
            battleCommand.ActivateBattleCommandPanel(true);
        }
    }
    // バトル ==============================
    //  スキルの入力の状態を遷移する関数
    private InputSkillStatement ChangeInputSkillStatement()
    {
        InputSkillStatement nextStatement = inputSkillStatement;
        switch (inputSkillStatement)
        {
            case (InputSkillStatement.INIT_SKILL):
                nextStatement = InputSkillStatement.SKILL_SELECT;
                break;

            case (InputSkillStatement.SKILL_SELECT):
                nextStatement = InputSkillStatement.TARGET_SELECT;
                break;

            case (InputSkillStatement.TARGET_SELECT):
                nextStatement = InputSkillStatement.END_INPUT;
                break;

            case (InputSkillStatement.END_INPUT):
                Debug.Log("Statement Error");
                break;
        }
        Debug.Log(nextStatement);
        return nextStatement;
    }

    //  アイテムの入力の状態を遷移する関数
    private InputItemStatement ChangeInputItemStatement()
    {
        InputItemStatement nextStatement = inputItemStatement;
        switch (inputItemStatement)
        {
            case (InputItemStatement.INIT_ITEM):
                nextStatement = InputItemStatement.ITEM_SELECT;
                break;

            case (InputItemStatement.ITEM_SELECT):
                nextStatement = InputItemStatement.TARGET_SELECT;
                break;

            case (InputItemStatement.TARGET_SELECT):
                nextStatement = InputItemStatement.END_INPUT;
                break;

            case (InputItemStatement.END_INPUT):
                Debug.Log("Statement Error");
                break;
        }
        Debug.Log(nextStatement);
        return nextStatement;
    }

    private void InitSkill()
    {
        Debug.Log("---PlayerInitSkill---");
        // スキルパネルを表示
        battleCommand.ActivateSkillCommandPanel(true);
        Player turnPlayer = (Player)TurnCharacter;
        // EnhancedScrollerのデリゲートを指定する
        // デリゲートを設定することで、スクロールビューが必要な情報を取得
        playerSkillPanel.Delegate = this;
        // そのターンのプレイヤーキャラのスキルをセットする
        Player player = (Player)TurnCharacter;
        LoadSkillData(player.Skills);
    }

    private void InitTarget(EnemySkill playerSkill)
    {
        selectedTargetIndex = 0;
        // 敵への効果だったら
        if (playerSkill.skillBase.SkillTargetKind == SKILL_TARGET_KIND.FOE)
        {
            // 全体攻撃だったら
            if (playerSkill.skillBase.SkillTargetNum == TARGET_NUM.ALL)
            {
                selectedEnemies = new Enemy[activeEnemies.Count];
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    activeEnemies[i].EnemyUI.SelectedArrow.SetActive(true);
                    selectedEnemies[i] = activeEnemies[i];
                }
            }
            //　単体攻撃だったら
            else
            {
                activeEnemies[selectedTargetIndex].EnemyUI.SelectedArrow.SetActive(true);
                Debug.Log("selectedTargetIndex初期" + selectedTargetIndex);
            }
        }// プレイヤー側が対象だったら
        else if (playerSkill.skillBase.SkillTargetKind == SKILL_TARGET_KIND.SELF)
        {
            // 対象が全体だったら
            if (playerSkill.skillBase.SkillTargetNum == TARGET_NUM.ALL)
            {
                selectedPlayers = new Player[activePlayers.Count];
                for (int i = 0; i < activePlayers.Count; i++)
                {
                    activePlayers[i].battlePlayerUI.SetActiveSelectedArrow(true);
                    selectedPlayers[i] = activePlayers[i];
                }
            }
            //　対象が単体だったら
            else
            {
                activePlayers[selectedTargetIndex].battlePlayerUI.SetActiveSelectedArrow(true);
                Debug.Log("selectedTargetIndex初期" + selectedTargetIndex);
            }
        }
    }

    // ターゲットの選択
    private void HandleSelectTarget(EnemySkill skill)
    {
        bool selectionChanged = false;

        // 対象が敵だったら
        if (skill.skillBase.SkillTargetKind == SKILL_TARGET_KIND.FOE)
        {
            // 敵が単体だったら
            if (skill.skillBase.SkillTargetNum == TARGET_NUM.SINGLE)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (selectedTargetIndex > 0)
                    {
                        selectedTargetIndex--;
                    }
                    else
                    {
                        selectedTargetIndex = 0;
                    }
                    selectionChanged = true;

                    Debug.Log("selectedTargetIndex左-" + selectedTargetIndex);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (selectedTargetIndex < activeEnemies.Count - 1)
                    {
                        selectedTargetIndex++;
                    }
                    selectionChanged = true;

                    Debug.Log("selectedTargetIndexみぎ+" + selectedTargetIndex);
                }

                if (selectionChanged == true)
                {
                    for (int i = 0; i < activeEnemies.Count; i++)
                    {
                        bool isActiveSelectedArrow = (i == selectedTargetIndex);
                        activeEnemies[i].EnemyUI.SelectedArrow.SetActive(isActiveSelectedArrow);
                    }

                    activeEnemies[selectedTargetIndex].EnemyUI.SelectedArrow.SetActive(true);

                    selectedEnemies = new Enemy[1];
                    selectedEnemies[0] = activeEnemies[selectedTargetIndex];
                }
            }
            else
            {
                selectedEnemies = new Enemy[activeEnemies.Count];
                selectedEnemies = activeEnemies.ToArray();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                // 選択矢印を消す
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    activeEnemies[i].EnemyUI.SelectedArrow.SetActive(false);
                }
                inputSkillStatement = InputSkillStatement.END_INPUT;
            }
        }
        //プレイヤーが対象だったら
        else if (skill.skillBase.SkillTargetKind == SKILL_TARGET_KIND.SELF)
        {
            // 対象が単体だったら
            if (skill.skillBase.SkillTargetNum == TARGET_NUM.SINGLE)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (selectedTargetIndex > 0)
                    {
                        selectedTargetIndex--;
                    }
                    else
                    {
                        selectedTargetIndex = 0;
                    }
                    selectionChanged = true;

                    Debug.Log("selectedTargetIndex左-" + selectedTargetIndex);
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (selectedTargetIndex < activePlayers.Count - 1)
                    {
                        selectedTargetIndex++;
                    }
                    selectionChanged = true;

                    Debug.Log("selectedTargetIndexみぎ+" + selectedTargetIndex);
                }

                if (selectionChanged == true)
                {
                    for (int i = 0; i < activePlayers.Count; i++)
                    {
                        bool isActiveSelectedArrow = (i == selectedTargetIndex);
                        activePlayers[i].battlePlayerUI.SelectedArrow.SetActive(isActiveSelectedArrow);
                    }

                    activePlayers[selectedTargetIndex].battlePlayerUI.SelectedArrow.SetActive(true);

                    selectedPlayers = new Player[1];
                    selectedPlayers[0] = activePlayers[selectedTargetIndex];
                }
            }
            else
            {
                selectedPlayers = new Player[activeEnemies.Count];
                selectedPlayers = activePlayers.ToArray();
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // 選択矢印を消す
                for (int i = 0; i < activePlayers.Count; i++)
                {
                    Debug.Log(i + "," + activePlayers.Count);
                    activePlayers[i].battlePlayerUI.SelectedArrow.SetActive(false);
                }
                inputSkillStatement = InputSkillStatement.END_INPUT;
            }
        }

        // escキーを押したら戻る
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            inputSkillStatement = InputSkillStatement.SKILL_SELECT;// スキル選択状態に戻す
            // ターゲット選択矢印を非表示にする
            for (int i = 0; i < activePlayers.Count; i++)
            {
                activePlayers[i].battlePlayerUI.SelectedArrow.SetActive(false);
            }

            // 選択矢印を消す
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                activeEnemies[i].EnemyUI.SelectedArrow.SetActive(false);
            }
            // スキルパネルを表示する
            battleCommand.ActivateSkillCommandPanel(true);
        }
    }

    private void PlayerEndSkillInput()
    {
        Debug.Log("PlayerEndInput()");
        //StartCoroutine(PerformPlayerSkill());
        PerformPlayerSkill();
    }

    // 全体攻撃のモーションを変更中
    // スキルの発動 IEnumerator
    /*private IEnumerator*/

    private void PerformPlayerSkill()
    {
        battleState = BattleState.BUSY;
        // 技を決定
        Player player = (Player)TurnCharacter;
        /*
        player.PlayerBattleAnimator.SetBool("IdleToTurnIdle", false);
        player.PlayerBattleAnimator.SetBool("TurnIdleToIdle", false);
        */
        EnemySkill playerSkill = player.Skills[selectedSkillIndex];
        // dialogBox.SetMessage("PlayerTurn " + playerSkill.skillBase.SkillName);
        Debug.Log("======" + player.PlayerBase.PlayerName + "PlayerTurn " + playerSkill.skillBase.SkillName + "======");

        Debug.Log("名前" + player.PlayerBase.PlayerName + "発動スキル" + playerSkill.skillBase.SkillName);
        // 消費SP分減らす
        player.UseSp(playerSkill);
        player.battlePlayerUI.UpdateHpSp();
        // 攻撃魔法だったら
        if (playerSkill.skillBase.SkillCategory == SKILL_CATEGORY.ATTACK)
        {
            // ダメージ決定　生きているキャラクター分の配列を用意
            bool[] isDying = new bool[activeEnemies.FindAll(value => value.isDying == false).Count];

            // ターンのプレイヤーのスキル発動モーション
            player.PlayerBattleAnimator.Play(hashSkill);
            //yield return null;// ステートの反映
            //yield return new WaitForAnimation(player.PlayerBattleAnimator, 0);

            // 全体攻撃だったら
            if (playerSkill.skillBase.SkillTargetNum == TARGET_NUM.ALL)
            {
                // アニメーションやUI表示
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    // 敵にスキル発動モーション
                    Instantiate(playerSkill.skillBase.SkillRecieveEffect, activeEnemies[i].EnemyPrefab.transform.position, activeEnemies[i].EnemyPrefab.transform.rotation);
                }
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    // ダメージモーション　敵のアニメーターにダメージのステート追加
                    activeEnemies[i].EnemyAnimator.Play(hashDamage);
                    //yield return null;
                }

                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    // ダメージ　
                    isDying[i] = activeEnemies[i].TakeSkillDamage(playerSkill, (Player)TurnCharacter);
                }
                // 一体のダメージアニメーションがおわったらIdle状態にする
                //yield return new WaitForAnimation(activeEnemies[selectedTargetIndex].EnemyAnimator, 0);

                // HPSPの反映
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    activeEnemies[i].EnemyUI.UpdateHp();
                }
            }
            else//単体攻撃だったら
            {
                if (playerSkill.skillBase.SkillRecieveEffect != null)
                {
                    Instantiate(playerSkill.skillBase.SkillRecieveEffect, activeEnemies[selectedTargetIndex].EnemyPrefab.transform.position, activeEnemies[selectedTargetIndex].EnemyPrefab.transform.rotation);
                }
                isDying[selectedTargetIndex] = activeEnemies[selectedTargetIndex].TakeSkillDamage(playerSkill, activePlayers[0]);
                // アニメーションやUI表示

                // ダメージモーション
                activeEnemies[selectedTargetIndex].EnemyAnimator.Play(hashDamage);
                //yield return null;
                // ダメージアニメーションが終わるまで待つ
                //yield return new WaitForAnimation(activeEnemies[selectedTargetIndex].EnemyAnimator, 0);
                // HPSPの反映
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    activeEnemies[i].EnemyUI.UpdateHp();
                }
            }

            for (int i = 0; i < activeEnemies.Count; i++)
            {
                // 戦闘不能な敵を消す
                if (isDying[i] == true)
                {
                    // i番目の敵のモデルを消す
                    //activeEnemies[i].EnemyModel.SetActive(false);
                    Destroy(activeEnemies[i].EnemyPrefab);
                    // i番目の敵のUIを消す
                    activeEnemies[i].EnemyUI.UnActiveUIPanel();
                    // i番目の敵のisDyingをtrueにする
                    characters.Find(value => value == activeEnemies[i]).isDying = true;
                }
            }
            // 全員戦闘不能ならメッセージ
            // 戦闘不能の敵を検索してリムーブする
            List<Enemy> deadEnemies = activeEnemies.FindAll(value => value.isDying == true);
            for (int i = 0; i < deadEnemies.Count; i++)
            {
                activeEnemies.Remove(deadEnemies[i]);
            }

            Debug.Log("EnemySkillまで1秒まつ");
            //yield return new WaitForSeconds(1);
            // 全員瀕死になったら戦闘不能
            if (isDying.All(value => value == true))
            {
                Debug.Log("戦闘不能");

                battleState = BattleState.BUSY;
                //yield return new WaitForSeconds(0.7f);
                // 3Dモデルの削除
                for (int i = 0; i < activePlayers.Count; i++)
                {
                    // バトルが終了したのでプレイヤーのモデルを消す
                    Destroy(activePlayers[i].PlayerBattleSprite);
                }
                //フィールドのシーンに戻る
                EndBattle();
            }
            else// 一体でも生き残っていれば
            {
                NextTurn();
            }
        }
        // ステータス変化魔法だったら
        else if (playerSkill.skillBase.SkillCategory == SKILL_CATEGORY.STATUS)
        {
            SkillStatusBase skillBase = playerSkill.skillBase as SkillStatusBase;

            // 対象が敵だったら
            if (playerSkill.skillBase.SkillTargetKind == SKILL_TARGET_KIND.FOE)
            {
                // 効果が全体だったら
                if (playerSkill.skillBase.SkillTargetNum == TARGET_NUM.ALL)
                {
                    // 敵全体のステータスが変化する
                    for (int i = 0; i < activeEnemies.Count; i++)
                    {
                        activeEnemies[i].ChangeStatus(skillBase.TargetStatus, skillBase.SkillStatusKind);
                    }

                    // エフェクトを発生させる　あとで
                }// 効果が単体だったら
                else
                {
                    activeEnemies[selectedTargetIndex].ChangeStatus(skillBase.TargetStatus, skillBase.SkillStatusKind);
                    // エフェクトを発生させる　あとで
                }
            }
            // 対象が味方だったら
            else if (playerSkill.skillBase.SkillTargetKind == SKILL_TARGET_KIND.SELF)
            {
                // 効果が全体だったら
                if (playerSkill.skillBase.SkillTargetNum == TARGET_NUM.ALL)
                {
                    // プレイヤーのステータスを変更する　あとで
                    // エフェクトを発生させる　あとで
                }// 効果が単体だったら
                else
                {
                    // プレイヤーのステータスを変更する　あとで
                    // エフェクトを発生させる　あとで
                }
            }
            NextTurn();
        }
        // 状態異常魔法+攻撃魔法だったら
        else if (playerSkill.skillBase.SkillCategory == SKILL_CATEGORY.CONDITION)
        {
            // ダメージ決定　生きているキャラクター分の配列を用意
            bool[] isDying = new bool[activeEnemies.FindAll(value => value.isDying == false).Count];
            SkillConditionBase skillBase = playerSkill.skillBase as SkillConditionBase;
            // 効果が全体だったら
            if (playerSkill.skillBase.SkillTargetNum == TARGET_NUM.ALL)
            {
                // 敵にダメージがはいる
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    isDying[i] = activeEnemies[i].TakeSkillDamage(playerSkill, (Player)TurnCharacter);
                }

                // 敵のアニメーションの再生
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    // ダメージモーション
                    activeEnemies[i].EnemyAnimator.Play(hashDamage);
                    // yield return null;
                    // ダメージアニメーションが終わるまで待つ
                    // yield return new WaitForAnimation(activeEnemies[selectedTargetIndex].EnemyAnimator, 0);
                }

                // HPSPの反映
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    activeEnemies[i].EnemyUI.UpdateHp();
                }

                // 敵が確率で状態異常にかかる
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    float random = Random.Range(0.0f, 1.0f);
                    // 出た値が確率以下なら
                    if (random <= skillBase.ConditionAttackAccuracy)
                    {
                        int duration = Random.Range(2, 4);
                        StatusCondition condition = new StatusCondition(skillBase.Condition, duration);
                        switch (skillBase.Condition)
                        {
                            // 毒状態
                            case STATUS_CONDITION_TYPE.POISON:
                                condition = new PoisonCondition(duration);
                                break;
                            // まひ状態
                            case STATUS_CONDITION_TYPE.PARALYSIS:
                                condition = new ParalysisCondition(duration);
                                break;

                            case STATUS_CONDITION_TYPE.BURN:
                                condition = new BurnCondition(duration);
                                break;

                            case STATUS_CONDITION_TYPE.FREEZE:
                                condition = new FreezeCondition(duration);
                                break;

                            case STATUS_CONDITION_TYPE.SLEEP:
                                condition = new SleepCondition(duration);
                                break;
                        }
                        // 状態異常にかかる　追加する方法がわからん
                        activeEnemies[i].AddCondition(condition);
                        // 状態異常エフェクトの生成　あとで
                    }
                }
            }// 効果が単体だったら
            else
            {
                // 敵にダメージがはいる
                isDying[selectedTargetIndex] = activeEnemies[selectedTargetIndex].TakeSkillDamage(playerSkill, activePlayers[0]);
                // ダメージモーション
                activeEnemies[selectedTargetIndex].EnemyAnimator.Play(hashDamage);
                //yield return null;
                // ダメージアニメーションが終わるまで待つ
                //yield return new WaitForAnimation(activeEnemies[selectedTargetIndex].EnemyAnimator, 0);
                // HPの反映
                activeEnemies[selectedTargetIndex].EnemyUI.UpdateHp();
                // 敵が確率で状態異常にかかる
                float random = Random.Range(0.0f, 1.0f);
                // 出た値が確率以下なら
                if (random <= skillBase.ConditionAttackAccuracy)
                {
                    int duration = Random.Range(2, 4);
                    StatusCondition condition = new StatusCondition(skillBase.Condition, duration);
                    switch (skillBase.Condition)
                    {
                        // 毒状態
                        case STATUS_CONDITION_TYPE.POISON:
                            condition = new PoisonCondition(duration);
                            break;
                        // まひ状態
                        case STATUS_CONDITION_TYPE.PARALYSIS:
                            condition = new ParalysisCondition(duration);
                            break;

                        case STATUS_CONDITION_TYPE.BURN:
                            condition = new BurnCondition(duration);
                            break;

                        case STATUS_CONDITION_TYPE.FREEZE:
                            condition = new FreezeCondition(duration);
                            break;

                        case STATUS_CONDITION_TYPE.SLEEP:
                            condition = new SleepCondition(duration);
                            break;
                    }
                    // 状態異常にかかる　追加する方法がわからん
                    activeEnemies[selectedTargetIndex].AddCondition(condition);
                    Debug.Log("状態異常追加");
                    // 状態異常エフェクトの生成　あとで
                }
            }

            for (int i = 0; i < activeEnemies.Count; i++)
            {
                // 戦闘不能な敵を消す
                if (isDying[i] == true)
                {
                    // i番目の敵のモデルを消す
                    //activeEnemies[i].EnemyModel.SetActive(false);
                    Destroy(activeEnemies[i].EnemyPrefab);
                    // i番目の敵のUIを消す
                    activeEnemies[i].EnemyUI.UnActiveUIPanel();
                    // i番目の敵のisDyingをtrueにする
                    characters.Find(value => value == activeEnemies[i]).isDying = true;
                }
            }
            // 全員戦闘不能ならメッセージ
            // 戦闘不能の敵を検索してリムーブする
            List<Enemy> deadEnemies = activeEnemies.FindAll(value => value.isDying == true);
            for (int i = 0; i < deadEnemies.Count; i++)
            {
                activeEnemies.Remove(deadEnemies[i]);
            }

            Debug.Log("EnemySkillまで1秒まつ");
            //yield return new WaitForSeconds(1);
            // 全員瀕死になったら戦闘不能
            if (isDying.All(value => value == true))
            {
                Debug.Log("戦闘不能");

                battleState = BattleState.BUSY;
                //yield return new WaitForSeconds(0.7f);
                // 3Dモデルの削除
                // プレイヤーのスプライトを消す
                for (int i = 0; i < activePlayers.Count; i++)
                {
                    Destroy(activePlayers[i].PlayerBattleSprite);
                }
                //フィールドのシーンに戻る
                EndBattle();
            }
            else// 一体でも生き残っていれば
            {
                NextTurn();
            }
        }
        // 回復魔法だったら
        else if (playerSkill.skillBase.SkillCategory == SKILL_CATEGORY.HEAL)
        {
            // ターンのプレイヤーのスキル発動モーション

            player.PlayerBattleAnimator.Play(hashSkill);
            //yield return null;// ステートの反映
            //yield return new WaitForAnimation(player.PlayerBattleAnimator, 0);
            /*
            player.PlayerBattleAnimator.SetBool("SkillToIdle", true);
            player.PlayerBattleAnimator.SetBool("IdleToTurnIdle", false);
            */

            // 全体回復だったら
            if (playerSkill.skillBase.SkillTargetNum == TARGET_NUM.ALL)
            {
                for (int i = 0; i < activePlayers.Count; i++)
                {
                    // 味方にスキル発動モーション
                    Vector3 healEffectPosition = new Vector3(activePlayers[i].PlayerBattleSprite.transform.position.x, activePlayers[i].PlayerBattleSprite.transform.position.y - 1.5f, activePlayers[i].PlayerBattleSprite.transform.position.z);
                    Instantiate(playerSkill.skillBase.SkillRecieveEffect, healEffectPosition, activePlayers[i].PlayerBattleSprite.transform.rotation);
                }
                for (int i = 0; i < activePlayers.Count; i++)
                {
                    // 回復モーション
                    activePlayers[i].PlayerBattleAnimator.Play(hashHeal);
                }
                // 一体（回）分だけ待つ
                //yield return new WaitForAnimation(activePlayers[0].PlayerBattleAnimator, 0);
                for (int i = 0; i < activePlayers.Count; i++)
                {
                    // 回復
                    activePlayers[i].TakeHeal(playerSkill, (Player)TurnCharacter);
                }

                // HPSPの反映
                for (int i = 0; i < activePlayers.Count; i++)
                {
                    activePlayers[i].battlePlayerUI.UpdateHpSp();
                }
            }
            else//対象が単体だったら
            {
                Vector3 healEffectPosition = new Vector3(activePlayers[selectedTargetIndex].PlayerBattleSprite.transform.position.x, activePlayers[selectedTargetIndex].PlayerBattleSprite.transform.position.y - 1.5f, activePlayers[selectedTargetIndex].PlayerBattleSprite.transform.position.z);
                Instantiate(playerSkill.skillBase.SkillRecieveEffect, healEffectPosition, activePlayers[selectedTargetIndex].PlayerBattleSprite.transform.rotation);
                // ここ
                activePlayers[selectedTargetIndex].TakeHeal(playerSkill, activePlayers[selectedTargetIndex]);

                // 回復モーション

                activePlayers[selectedTargetIndex].PlayerBattleAnimator.Play(hashHeal);
                //activePlayers[selectedTargetIndex].PlayerBattleAnimator.SetBool("HealToIdle", true);
                //yield return null;// ステートの反映
                //yield return new WaitForAnimation(activePlayers[selectedTargetIndex].PlayerBattleAnimator, 0);

                // HPSPの反映
                for (int i = 0; i < activePlayers.Count; i++)
                {
                    activePlayers[i].battlePlayerUI.UpdateHpSp();
                }
            }
            NextTurn();
        }
        // 移動攻撃魔法だったら
        else if (playerSkill.skillBase.SkillCategory == SKILL_CATEGORY.MOVE_ATTACK)
        {
            MoveSkillBase Skill = playerSkill.skillBase as MoveSkillBase;
            // 移動させる
            StartCoroutine(MoveEnemies(activeEnemies[selectedTargetIndex], Skill.Direction));
        }
        //player.PlayerBattleAnimator.SetBool("IdleToTurnIdle", false);
        //player.PlayerBattleAnimator.SetBool("TurnIdleToIdle", true);
    }

    private struct MovedEnemy
    {
        public Enemy enemy;
        public Direction moveDirection;
        public int targetPosition;
    }

    private List<MovedEnemy> movedEnemies;

    /// <summary>
    /// 移動させる技の移動部分の定義
    /// </summary>
    /// <param name="attackedEnemy">攻撃された敵</param>
    /// <param name="direction">移動させる方向</param>
    private IEnumerator MoveEnemies(Enemy attackedEnemy, Direction direction)
    {
        // ①普通のダメージ処理
        Player player = turnCharacter as Player;
        EnemySkill playerSkill = player.Skills[selectedSkillIndex];
        // ダメージ決定　生きているキャラクター分の配列を用意
        bool[] isDying = new bool[activeEnemies.FindAll(value => value.isDying == false).Count];

        // ターンのプレイヤーのスキル発動モーション
        player.PlayerBattleAnimator.Play(hashSkill);
        /*yield return null;// ステートの反映
        yield return null;// ステートの反映
        yield return new WaitForAnimation(player.PlayerBattleAnimator, 0);
        *//*
        player.PlayerBattleAnimator.SetBool("SkillToIdle", true);
        player.PlayerBattleAnimator.SetBool("IdleToTurnIdle", false);
        */
        // スキルのエフェクトを発動
        if (playerSkill.skillBase.SkillRecieveEffect != null)
        {
            Instantiate(playerSkill.skillBase.SkillRecieveEffect, activeEnemies[selectedTargetIndex].EnemyPrefab.transform.position, activeEnemies[selectedTargetIndex].EnemyPrefab.transform.rotation);
        }
        isDying[selectedTargetIndex] = activeEnemies[selectedTargetIndex].TakeSkillDamage(playerSkill, turnCharacter);

        // 敵のダメージモーション
        activeEnemies[selectedTargetIndex].EnemyAnimator.Play(hashDamage);
        //yield return null;
        // ダメージアニメーションが終わるまで待つ
        //yield return new WaitForAnimation(activeEnemies[selectedTargetIndex].EnemyAnimator, 0);

        // HPSPの反映
        for (int index = 0; index < activeEnemies.Count; index++)
        {
            activeEnemies[index].EnemyUI.UpdateHp();
        }

        for (int index = 0; index < activeEnemies.Count; index++)
        {
            // 戦闘不能な敵を消す
            if (isDying[index] == true)
            {
                // i番目の敵のモデルを消す
                Destroy(activeEnemies[index].EnemyPrefab);
                // i番目の敵のUIを消す
                activeEnemies[index].EnemyUI.UnActiveUIPanel();
                // i番目の敵のisDyingをtrueにする
                characters.Find(value => value == activeEnemies[index]).isDying = true;
            }
        }
        // 全員戦闘不能ならメッセージ
        // 戦闘不能の敵を検索してリムーブする
        List<Enemy> deadEnemies = activeEnemies.FindAll(value => value.isDying == true);
        for (int index = 0; index < deadEnemies.Count; index++)
        {
            activeEnemies.Remove(deadEnemies[index]);
        }

        Debug.Log("EnemySkillまで1秒まつ");
        //yield return new WaitForSeconds(1);
        // 全員瀕死になったら戦闘不能
        if (isDying.All(value => value == true))
        {
            Debug.Log("戦闘不能");

            battleState = BattleState.BUSY;
            //yield return new WaitForSeconds(0.7f);
            // プレイヤーの削除
            for (int index = 0; index < activePlayers.Count; index++)
            {
                Destroy(activePlayers[index].PlayerBattleSprite);
            }
            //フィールドのシーンに戻る
            EndBattle();
        }
        else// 一体でも生き残っていれば
        {
            // ②移動
            movedEnemies = new List<MovedEnemy>();
            float[] diff = { 0, 1.0f, 2.0f };
            float[] diffY = { 0f, 1f, 2f };

            //enemiesToMove.Add(attackedEnemy);
            int currentPosition = attackedEnemy.positionIndex;

            MovedEnemy movedenemy;
            // 軌道上の敵を検出
            foreach (Enemy enemy in activeEnemies)
            {
                // 殴った敵の軌道上にいる敵をみつける
                if (ShouldMoveEnemy(currentPosition, enemy.positionIndex, direction))
                {
                    movedenemy.enemy = enemy;
                    movedenemy.moveDirection = direction;
                    movedenemy.targetPosition = -1;
                    movedEnemies.Add(movedenemy);
                }
            }
            // rightとownなら小さい順
            if (movedEnemies[0].moveDirection == Direction.RIGHT || movedEnemies[0].moveDirection == Direction.DOWN)
            {
                movedEnemies = movedEnemies.OrderByDescending(enemy => enemy.enemy.positionIndex).ToList();
            }
            else
            {
                movedEnemies = movedEnemies.OrderBy(enemy => enemy.enemy.positionIndex).ToList();
            }
            int i = 0;
            Vector3 enemyUIgroundPosition = movedEnemies[0].enemy.EnemyPrefab.transform.InverseTransformPoint(movedEnemies[0].enemy.EnemyUI.transform.position);
            foreach (MovedEnemy enemy in movedEnemies)
            {
                targetPositionIndex = GetNewPositionIndex(enemy.enemy.positionIndex, direction);
                if (targetPositionIndex != enemy.enemy.positionIndex)
                {
                    enemy.enemy.positionIndex = targetPositionIndex;
                    if (direction == Direction.RIGHT)
                    {
                        // 移動
                        enemy.enemy.EnemyPrefab.transform.DOMove(new Vector3(diggingPositions[targetPositionIndex].transform.position.x + diff[i], diggingPositions[targetPositionIndex].transform.position.y, diggingPositions[targetPositionIndex].transform.position.z), 0.5f);

                        enemy.enemy.EnemyUI.transform.DOLocalMoveY(enemyUIgroundPosition.y + diffY[i], 0.5f);
                        //enemy.EnemyPrefab.transform.position = new Vector3(diggingPositions[newPositionIndex].transform.position.x + diff[i], diggingPositions[newPositionIndex].transform.position.y, diggingPositions[newPositionIndex].transform.position.z);
                    }
                    else if (direction == Direction.LEFT)
                    {
                        // 移動
                        enemy.enemy.EnemyPrefab.transform.DOMove(new Vector3(diggingPositions[targetPositionIndex].transform.position.x + diff[i], diggingPositions[targetPositionIndex].transform.position.y, diggingPositions[targetPositionIndex].transform.position.z), 0.5f);
                        enemy.enemy.EnemyUI.transform.DOLocalMoveY(enemyUIgroundPosition.y + diffY[i], 0.5f);
                    }
                    else if (direction == Direction.UP)
                    {
                        // 移動
                        enemy.enemy.EnemyPrefab.transform.DOMove(new Vector3(diggingPositions[targetPositionIndex].transform.position.x, diggingPositions[targetPositionIndex].transform.position.y, diggingPositions[targetPositionIndex].transform.position.z + diff[i]), 0.5f);
                        enemy.enemy.EnemyUI.transform.DOLocalMoveY(enemyUIgroundPosition.y + diffY[i], 0.5f);
                    }
                    else if (direction == Direction.DOWN)
                    {
                        enemy.enemy.EnemyPrefab.transform.DOMove(new Vector3(diggingPositions[targetPositionIndex].transform.position.x, diggingPositions[targetPositionIndex].transform.position.y, diggingPositions[targetPositionIndex].transform.position.z - diff[i]), 0.5f);
                        //enemy.enemy.EnemyUI.transform.DOMoveY(enemyUIgroundPosition.y + diffY[i], 0.5f);
                        enemy.enemy.EnemyUI.transform.DOLocalMoveY(enemyUIgroundPosition.y + diffY[i], 0.5f);
                    }
                    // ちょっと時間をずらして移動
                    //yield return new WaitForSeconds(0.3f);
                }

                i++;
            }
            yield return new WaitForSeconds(0.5f);

            // 穴チェックに移行
            //  ③1回目の穴チェック
            Debug.Log("1回目の穴チェック");
            // 穴にいる人が戦闘不能じゃなければ
            if (activeEnemies.Find(e => e.positionIndex == targetPositionIndex).EnemyPrefab != null)
            {
                CheckHolePosition(targetPositionIndex);
                yield return new WaitForSeconds(0.2f);
            }

            // ④移動２
            // UIを元の位置に戻す
            for (int j = 0; j < movedEnemies.Count; j++)
            {
                Debug.Log("元の高さ" + enemyUIgroundPosition.y);
                movedEnemies[j].enemy.EnemyUI.transform.DOLocalMoveY(enemyUIgroundPosition.y, 0.5f);
            }
            ResolvePositionOverlap();

            //  ⑤２回目の穴チェック
            Debug.Log("2回目の穴チェック");
            foreach (MovedEnemy movedEnemy in movedEnemies)
            {
                Debug.Log(movedEnemy.targetPosition);
                // 移動していたら
                if (movedEnemy.targetPosition != -1)
                {
                    // 穴にいる人が戦闘不能じゃなければ
                    Debug.Log("残っている敵" + activeEnemies.Count);
                    for (int j = 0; j < activeEnemies.Count; j++)
                    {
                        Debug.Log(activeEnemies[j].EnemyBase.EnemyName);
                    }
                    Debug.Log(activeEnemies.Find(e => e.positionIndex == movedEnemy.targetPosition));
                    if (activeEnemies.Find(e => e.positionIndex == movedEnemy.targetPosition) != null)
                    {
                        CheckHolePosition(movedEnemy.targetPosition);
                    }
                }
            }
            yield return new WaitForSeconds(1f);
            NextTurn();
        }
    }

    /*
    private bool ShouldMoveEnemy(int currentPosition, int enemyPosition, Direction direction)
    {
        // 敵を移動させるべきかどうかのロジック
        switch (direction)
        {
            case Direction.RIGHT:
                return enemyPosition / 3 == currentPosition / 3; // 同じ行にいる場合
            case Direction.LEFT:
                return enemyPosition / 3 == currentPosition / 3; // 同じ行にいる場合
            case Direction.UP:
                return enemyPosition % 3 == currentPosition % 3; // 同じ列にいる場合
            case Direction.DOWN:
                return enemyPosition % 3 == currentPosition % 3; // 同じ列にいる場合
            default:
                return false;
        }
    }*/

    private bool ShouldMoveEnemy(int currentPosition, int enemyPosition, Direction direction)
    {
        int currentRow = currentPosition / 3;
        int currentColumn = currentPosition % 3;
        int enemyRow = enemyPosition / 3;
        int enemyColumn = enemyPosition % 3;

        // 敵を移動させるべきかどうかのロジック
        switch (direction)
        {
            case Direction.RIGHT:
                // 同じ行で、現在位置の右側にいる場合
                return enemyRow == currentRow && enemyColumn >= currentColumn;

            case Direction.LEFT:
                // 同じ行で、現在位置の左側にいる場合
                return enemyRow == currentRow && enemyColumn <= currentColumn;

            case Direction.UP:
                // 同じ列で、現在位置の上側にいる場合
                return enemyColumn == currentColumn && enemyRow <= currentRow;

            case Direction.DOWN:
                // 同じ列で、現在位置の下側にいる場合
                return enemyColumn == currentColumn && enemyRow >= currentRow;

            default:
                return false;
        }
    }

    private int GetNewPositionIndex(int currentPosition, Direction direction)
    {
        switch (direction)
        {
            case Direction.RIGHT:
                return (currentPosition / 3) * 3 + 2;

            case Direction.LEFT:
                return (currentPosition / 3) * 3;

            case Direction.UP:
                return currentPosition % 3;

            case Direction.DOWN:
                return (currentPosition % 3) + 6;

            default:
                return currentPosition;
        }
    }

    private int GetDistanceToTarget(int currentPosition, Direction direction)
    {
        // 移動先までの距離を計算するロジック
        switch (direction)
        {
            case Direction.RIGHT:
                return 2 - currentPosition % 3;

            case Direction.LEFT:
                return currentPosition % 3;

            case Direction.UP:
                return currentPosition / 3;

            case Direction.DOWN:
                return 2 - currentPosition / 3;

            default:
                return 0;
        }
    }

    // 敵の移動先の穴にアイテムが埋まっていたらそのアイテムの効果を発動する
    private void CheckHolePosition(int position)
    {
        Debug.Log("===============穴チェック============" + battleState);

        bool[] isDying = new bool[activeEnemies.FindAll(value => value.isDying == false).Count];
        Item item = diggingGridManager.gridItems[position];
        // 敵の移動先の穴にアイテムが埋まっていたら
        if (item != null)
        {
            TrapItemBase itemBase = item.ItemBase as TrapItemBase;
            // アイテムの効果を発動する
            // 全アイテム共通：ダメージ処理
            // もしその位置に敵がいたら
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                if (activeEnemies[i].positionIndex == position)
                {
                    // ダメージ処理
                    isDying[i] = activeEnemies[i].TakeItemDamage(itemBase.BasicDamage, turnCharacter, activeEnemies[i]);
                }
            }
            // HPSPの反映
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                activeEnemies[i].EnemyUI.UpdateHp();
            }
            // 状態異常の処理
            if (itemBase.Condition == STATUS_CONDITION_TYPE.POISON)
            {
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    if (activeEnemies[i].positionIndex == position)
                    {
                        // 状態異常：毒にする
                        int duration = Random.Range(2, 4);
                        StatusCondition condition = new StatusCondition(itemBase.Condition, duration);
                        switch (itemBase.Condition)
                        {
                            // 毒状態
                            case STATUS_CONDITION_TYPE.POISON:
                                condition = new PoisonCondition(duration);
                                break;
                            // まひ状態
                            case STATUS_CONDITION_TYPE.PARALYSIS:
                                condition = new ParalysisCondition(duration);
                                break;

                            case STATUS_CONDITION_TYPE.BURN:
                                condition = new BurnCondition(duration);
                                break;

                            case STATUS_CONDITION_TYPE.FREEZE:
                                condition = new FreezeCondition(duration);
                                break;

                            case STATUS_CONDITION_TYPE.SLEEP:
                                condition = new SleepCondition(duration);
                                break;
                        }
                        // 状態異常にかかる　追加する方法がわからん
                        activeEnemies[i].AddCondition(condition);
                        Debug.Log("状態異常にかかった");
                    }
                }
            }
            // アイテムを消費
            diggingGridManager.UseGridItem(position);
        }

        // 戦闘不能チェック
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            // 戦闘不能な敵を消す
            if (isDying[i] == true)
            {
                // i番目の敵のモデルを消す
                //activeEnemies[i].EnemyModel.SetActive(false);
                Destroy(activeEnemies[i].EnemyPrefab);
                // i番目の敵のUIを消す
                activeEnemies[i].EnemyUI.UnActiveUIPanel();
                // i番目の敵のisDyingをtrueにする
                characters.Find(value => value == activeEnemies[i]).isDying = true;
            }
        }
        // 全員戦闘不能ならメッセージ
        // 戦闘不能の敵を検索してリムーブする
        List<Enemy> deadEnemies = activeEnemies.FindAll(value => value.isDying == true);

        // 全体の戦闘不能チェック
        if (isDying.All(value => value == true))
        {
            Debug.Log("戦闘不能");

            battleState = BattleState.BUSY;
            //yield return new WaitForSeconds(0.7f);
            // 3Dモデルの削除
            for (int i = 0; i < activePlayers.Count; i++)
            {
                //Destroy(activePlayers[i].PlayerModel);
            }
            //フィールドのシーンに戻る
            EndBattle();
        }
        else
        {
            for (int i = 0; i < deadEnemies.Count; i++)
            {
                Debug.Log("死んだ敵を除去");
                activeEnemies.Remove(deadEnemies[i]);
            }
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                Debug.Log("残っている敵" + activeEnemies[i].EnemyBase.EnemyName);
            }
        }
    }

    // 敵の位置が重なっていたら解決する
    private void ResolvePositionOverlap()
    {
        for (int i = 0; i < movedEnemies.Count; i++)
        {
            //foreach (MovedEnemy movedEnemy in movedEnemies)
            //{
            Enemy overlapEnemy = activeEnemies.Find(e => e != movedEnemies[i].enemy && e.positionIndex == movedEnemies[i].enemy.positionIndex);
            MovedEnemy overlappingEnemy;
            overlappingEnemy.enemy = overlapEnemy;
            overlappingEnemy.moveDirection = movedEnemies[i].moveDirection;
            overlappingEnemy.targetPosition = -1;
            if (overlappingEnemy.enemy != null)
            {
                int newPositionIndex = FindNearestEmptyPosition(overlappingEnemy.enemy.positionIndex, overlappingEnemy.moveDirection);
                overlappingEnemy.enemy.positionIndex = newPositionIndex;
                MovedEnemy movedEnemy;
                movedEnemy.enemy = movedEnemies[i].enemy;
                movedEnemy.moveDirection = movedEnemies[i].moveDirection;
                movedEnemy.targetPosition = newPositionIndex;
                movedEnemies[i] = movedEnemy;
                for (int j = 0; j < activeEnemies.Count; j++)
                {
                    Debug.Log("位置" + activeEnemies[j].positionIndex);
                }
                Debug.Log(overlappingEnemy.enemy.EnemyBase.EnemyName + "が" + newPositionIndex + "に移動");
                overlappingEnemy.enemy.positionIndex = newPositionIndex;
                //enemy.EnemyPrefab.transform.DOMove(new Vector3(diggingPositions[targetPositionIndex].transform.position.x + diff[i], diggingPositions[targetPositionIndex].transform.position.y, diggingPositions[targetPositionIndex].transform.position.z), 1);
                if (overlappingEnemy.enemy.EnemyPrefab != null)
                {
                    overlappingEnemy.enemy.EnemyPrefab.transform.DOMove(diggingPositions[newPositionIndex].transform.position, 1);
                }
            }
        }
    }

    // 近くのポジションを探す
    private int FindNearestEmptyPosition(int currentPosition, Direction direction)
    {
        int row = currentPosition / 3;// 行
        int col = currentPosition % 3;// 列
        int newPositionIndex = currentPosition;
        switch (direction)
        {
            case Direction.RIGHT:
                // 一つ右から探索する
                for (int i = col - 1; i >= 0; i--)
                {
                    if (IsPositionEmpty(row * 3 + i))
                    {
                        return row * 3 + i;
                    }
                }
                break;

            // ひとつ隣から探索
            case Direction.LEFT:
                for (int i = col + 1; i < 3; i++)
                {
                    if (IsPositionEmpty(row * 3 + i))
                    {
                        return row * 3 + i;
                    }
                }
                break;

            case Direction.UP:
                for (int i = row + 1; i < 3; i++)
                {
                    if (IsPositionEmpty(i * 3 + col))
                    {
                        return i * 3 + col;
                    }
                }
                break;

            case Direction.DOWN:

                for (int i = row - 1; i >= 0; i--)
                {
                    if (IsPositionEmpty(i * 3 + col))
                    {
                        return i * 3 + col;
                    }
                }
                break;
        }
        return newPositionIndex;// 空いているマスが見つからない場合は元の位置を返す
    }

    private bool IsPositionEmpty(int positionIndex)
    {
        return !activeEnemies.Any(e => e.positionIndex == positionIndex);
    }

    /// <summary>
    /// 次のターンに移行する
    /// </summary>
    private void NextTurn()
    {
        // スキルを一番上からにする
        selectedSkillIndex = 0;
        if (turnCharacterIndex < characters.Count - 1)
        {
            turnCharacterIndex++;
        }
        else
        {
            turnCharacterIndex = 0;
        }
        TurnCharacter = characters[turnCharacterIndex];

        while (TurnCharacter.isDying == true) // 戦闘不能だったら飛ばす
        {
            if (turnCharacterIndex < characters.Count - 1)
            {
                turnCharacterIndex++;
            }
            else
            {
                turnCharacterIndex = 0;
            }
            TurnCharacter = characters[turnCharacterIndex];
        }

        // battleStateの更新
        /*
        if (TurnCharacter.isPlayer)
        {
            battleState = BattleState.PLAYER_MOVE;
            inputSkillStatement = InputSkillStatement.INIT_SKILL;
            Debug.Log(battleState);
        }
        else
        {
            battleState = BattleState.ENEMY_MOVE;
            inputSkillStatement = InputSkillStatement.INIT_SKILL;
            Debug.Log(battleState);
        }
        */
        battleState = BattleState.CHECK_CONDITION;
        Debug.Log("=====================" + battleState + "====================");
        //Debug.Log()
        Debug.Log("順番のキャラクター" + turnCharacterIndex + "生きてるキャラクターの数" + characters.Count(value => value.isDying == false));
    }

    /// <summary>
    /// ターンをスキップする
    /// </summary>
    public void SkipTurn()
    {
        Debug.Log(turnCharacter.characterName + " スキップ");

        // 次のターンに移行
        NextTurn();
    }

    // 敵============================
    private void EnemyMove()
    {
        inputSkillStatement = ChangeInputSkillStatement();
    }

    private void EnemySkillSelect()
    {
        Enemy turnEnemy = (Enemy)characters[turnCharacterIndex];
        selectedSkillIndex = Random.Range(0, turnEnemy.Skills.Count);
        inputSkillStatement = ChangeInputSkillStatement();
        Debug.Log(inputSkillStatement);
    }

    private void EnemyTargetSelect(EnemySkill enemySkill)
    {
        //Debug.Log("EnemyTargetSelect()");
        // ターゲットをランダムで選択
        // 単体だったら Random.Range(min,max) 最大値を含まない
        // 敵が自分自身でなければ
        if (enemySkill.skillBase.SkillTargetKind == SKILL_TARGET_KIND.FOE)
        {
            selectedTargetIndex = Random.Range(0, activePlayers.Count);
        }
        else
        {
            selectedTargetIndex = Random.Range(0, activeEnemies.Count);
        }
        inputSkillStatement = ChangeInputSkillStatement();
    }

    private void EnemyEndInput()
    {
        Enemy turnEnemy = (Enemy)characters[turnCharacterIndex];
        StartCoroutine(PerformEnemySkill(turnEnemy.Skills[selectedSkillIndex]));
    }

    // 敵のスキルの発動
    private IEnumerator PerformEnemySkill(EnemySkill enemySkill)
    {
        Enemy tmpenemy2 = (Enemy)turnCharacter;
        Debug.Log("名前" + tmpenemy2.EnemyBase.EnemyName + "発動スキル" + enemySkill.skillBase.SkillName);
        battleState = BattleState.BUSY;

        // 攻撃魔法なら
        if (enemySkill.skillBase.SkillCategory == SKILL_CATEGORY.ATTACK)
        {
            // デバッグのためのやつ
            Enemy enemy = (Enemy)characters[turnCharacterIndex];
            //dialogBox.SetMessage(enemy.EnemyBase.EnemyName + "EnemyTurn " + enemySkill.skillBase.SkillName);
            Debug.Log("======" + enemy.EnemyBase.EnemyName + "EnemyTurn " + enemySkill.skillBase.SkillName + "======");

            // ダメージ決定 修正 playerUnit.Players.Length→ activePlayers.Count
            bool[] isDying = new bool[activePlayers.FindAll(value => value.isDying == false).Count];

            enemy.EnemyAnimator.Play(hashAttack);
            yield return null;// ステートの反映
            yield return new WaitForAnimation(enemy.EnemyAnimator, 0);

            // 全体選択なら
            if (enemySkill.skillBase.SkillTargetNum == TARGET_NUM.ALL)
            {
                for (int i = 0; i < activePlayers.Count; i++)
                {
                    // 敵にスキル発動モーション
                    Instantiate(enemySkill.skillBase.SkillRecieveEffect, activePlayers[i].PlayerBattleSprite.transform.position, activePlayers[i].PlayerBattleSprite.transform.rotation);
                }

                // アニメーションやUI表示
                for (int i = 0; i < activePlayers.Count; i++)
                {
                    // ダメージモーション　敵のアニメーターにダメージのステート追加
                    activePlayers[i].PlayerBattleAnimator.Play(hashDamage);
                    yield return null;// ステートの反映
                }
                // 一体（回）分だけ待つ
                yield return new WaitForAnimation(activePlayers[0].PlayerBattleAnimator, 0);
                for (int i = 0; i < activePlayers.Count; i++)
                {
                    // 修正　activeEnemies[0]
                    isDying[i] = activePlayers[i].TakeSkillDamage(enemySkill, (Enemy)TurnCharacter);

                    // HPSPの反映
                    activePlayers[i].battlePlayerUI.UpdateHpSp();
                }
            }
            else// 単体選択なら
            {
                isDying[selectedTargetIndex] = activePlayers[selectedTargetIndex].TakeSkillDamage(enemySkill, activeEnemies[0]);

                // アニメーションやUI表示

                Instantiate(enemySkill.skillBase.SkillRecieveEffect, activePlayers[selectedTargetIndex].PlayerBattleSprite.transform.position, activePlayers[selectedTargetIndex].PlayerBattleSprite.transform.rotation);

                // ダメージモーション
                activePlayers[selectedTargetIndex].PlayerBattleAnimator.Play(hashDamage);
                yield return null;

                // ダメージモーションを待つ 追加
                yield return new WaitForAnimation(activePlayers[selectedTargetIndex].PlayerBattleAnimator, 0);

                //battlePlayerUIs[selectedTargetIndex].UpdateHpSp();
                activePlayers[selectedTargetIndex].battlePlayerUI.UpdateHpSp();
            }

            Debug.Log("EnemySkill　1秒まつ");
            yield return new WaitForSeconds(1);
            // 全員戦闘不能ならメッセージ
            for (int i = 0; i < isDying.Length; i++)
            {
                // 戦闘不能なら
                if (isDying[i] == true)
                {
                    // 戦闘不能モーション あとで

                    // リムーブ
                    //characters[i].isDying = true; // 戦闘不能
                    characters.Find(value => value == activePlayers[i]).isDying = true;

                    // デバッグよう
                    Player faintedPlayer = (Player)characters.Find(value => value == activePlayers[i]);
                    Debug.Log(faintedPlayer.PlayerBase.PlayerName + "は戦闘不能" + faintedPlayer.isDying);
                    activePlayers.Remove(faintedPlayer);
                    for (int j = 0; j < activePlayers.Count; j++)
                    {
                        Debug.Log("残りの敵" + activePlayers[j].PlayerBase.PlayerName); ;
                    }
                }
            }

            if (isDying.All(value => value == true))
            {
                Debug.Log("戦闘不能");
                yield return new WaitForSeconds(0.7f);
                //フィールドのシーンに戻る
                EndBattle();
            }
            else
            {
                NextTurn();
            }
        }
        // ステータス変化魔法だったら
        else if (enemySkill.skillBase.SkillCategory == SKILL_CATEGORY.STATUS)
        {
            SkillStatusBase skillBase = enemySkill.skillBase as SkillStatusBase;
            // 対象が敵だったら
            if (enemySkill.skillBase.SkillTargetKind == SKILL_TARGET_KIND.FOE)
            {
                // 効果が全体だったら
                if (enemySkill.skillBase.SkillTargetNum == TARGET_NUM.ALL)
                {
                    for (int i = 0; i < activePlayers.Count; i++)
                    {
                        activePlayers[i].ChangeStatus(skillBase.TargetStatus, skillBase.SkillStatusKind);
                    }

                    // ステータス変化のエフェクトをプレイヤーに出す　あとで
                }// 効果が単体だったら
                else
                {
                    activePlayers[selectedTargetIndex].ChangeStatus(skillBase.TargetStatus, skillBase.SkillStatusKind);
                    // ステータス変化のエフェクトをプレイヤーに出す　あとで
                }
            }
            // 対象が味方だったら
            else if (enemySkill.skillBase.SkillTargetKind == SKILL_TARGET_KIND.SELF)
            {
                // 効果が全体だったら
                if (enemySkill.skillBase.SkillTargetNum == TARGET_NUM.ALL)
                {
                    // ステータス変化魔法を実行する
                    // ステータス変化のエフェクトを敵に出す　あとで
                }// 効果が単体だったら
                else
                {
                    // ステータス変化魔法を実行する
                    // ステータス変化のエフェクトを敵に出す　あとで
                }
            }
        }
        // 回復魔法だったら
        else if (enemySkill.skillBase.SkillCategory == SKILL_CATEGORY.HEAL)
        {
            Enemy enemy = (Enemy)characters[turnCharacterIndex];
            // 入力側のスキルのアニメーションを再生

            enemy.EnemyAnimator.Play(hashSkill);
            yield return null;// ステートの反映
            yield return new WaitForAnimation(enemy.EnemyAnimator, 0);

            // 全体選択なら
            if (enemySkill.skillBase.SkillTargetNum == TARGET_NUM.ALL)
            {
                // 受ける側のスキルのアニメーションを再生
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    activeEnemies[i].EnemyAnimator.Play(hashHeal);
                    yield return null;// ステートの反映
                    Instantiate(enemySkill.skillBase.SkillRecieveEffect, activeEnemies[i].EnemyPrefab.transform.position, activeEnemies[i].EnemyPrefab.transform.rotation);
                }
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    // 回復する
                    activeEnemies[i].TakeHeal(enemySkill);
                    activeEnemies[i].EnemyUI.UpdateHp();
                }
                yield return new WaitForAnimation(activeEnemies[0].EnemyAnimator, 0);
            }
            // 単体なら
            else
            {
                Instantiate(enemySkill.skillBase.SkillRecieveEffect, activeEnemies[selectedTargetIndex].EnemyPrefab.transform.position, activeEnemies[selectedTargetIndex].EnemyPrefab.transform.rotation);
                // ここ
                activeEnemies[selectedTargetIndex].TakeHeal(enemySkill);

                // 回復モーション
                activeEnemies[selectedTargetIndex].EnemyAnimator.Play(hashHeal);
                //activeEnemies[selectedTargetIndex].EnemyAnimator.SetBool("HealToIdle", true);
                yield return null;// ステートの反映
                yield return new WaitForAnimation(activeEnemies[selectedTargetIndex].EnemyAnimator, 0);

                // HPの反映
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    activeEnemies[i].EnemyUI.UpdateHp();
                }
            }
            NextTurn();
        }
    }

    /// ===================================

    // スキルパネルからアクションを選択する
    private void HandleSkillSelection()
    {
        ///////////// アニメーションの指定 /////////////
        // 10秒立ったら
        // 10秒ごとにフラフラするアニメーション
        Player turnPlayer = (Player)TurnCharacter;
        //turnPlayer.PlayerBattleAnimator.SetBool("IdleToTurnIdle", true);// スキル選択中
        ////////////////////////////////////////////////////
        bool selectionChanged = false;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // 指定された値を範囲内にクランプ（制約）するために使用されるUnityの関数です。
            // 値が指定された範囲内にある場合はそのまま返し、範囲外の場合は範囲の最小値または最大値に
            // 制約されます
            selectedSkillIndex = Mathf.Clamp(selectedSkillIndex - 1, 0, skillDatas.Count - 1);
            selectionChanged = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedSkillIndex = Mathf.Clamp(selectedSkillIndex + 1, 0, skillDatas.Count - 1);
            selectionChanged = true;
        }
        // 技決定
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // 技決定の処理
            // SPが足りない
            if (turnPlayer.currentSP < turnPlayer.Skills[selectedSkillIndex].skillBase.Sp)
            {
                // 選択できないよ
                return;
            }
            else
            {
                // UIを非表示
                battleCommand.ActivateSkillCommandPanel(false);
                InitTarget(turnPlayer.Skills[selectedSkillIndex]);
                //ここ
                Debug.Log("activePlayers" + activePlayers.Count + "selectedTargetIndex" + selectedTargetIndex);

                inputSkillStatement = ChangeInputSkillStatement();
            }
        }
        // escキーを押したら戻る
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            inputSkillStatement = InputSkillStatement.INIT_SKILL;// スキルを選択初期状態に戻す
                                                                 // スキルパネルを非表示にする
            battleCommand.ActivateSkillCommandPanel(false);
            // メインパネルを表示する
            battleCommand.ActivateBattleCommandPanel(true);
            Player player = turnCharacter as Player;
            /*
            player.PlayerBattleAnimator.SetBool("TurnIdleToIdle", true);
            player.PlayerBattleAnimator.SetBool("IdleToTurnIdle", false);
            */

            battleState = BattleState.PLAYER_ACTION_SELECT;
        }

        // 選択中
        if (selectionChanged)
        {
            // 選択されたインデックスが更新されたから基礎データを更新する
            for (int i = 0; i < skillDatas.Count; i++)
            {
                // 選択中のisSelectedをtrueにする
                skillDatas[i].isSelected = (i == selectedSkillIndex);
            }

            // アクティブセルに対してUIの更新をする
            playerSkillPanel.RefreshActiveCellViews();
            // 選択されたインデックスが最下部またはその先にある時
            if (selectedSkillIndex >= playerSkillPanel.EndCellViewIndex)
            {
                /// <summary>
                /// Jump to a position in the playerSkillPanel based on a dataIndex. This overload allows you
                /// to specify a specific offset within a cell as well.
                /// dataIndex に基づいて、スクローラー内のある位置にジャンプする。
                /// このオーバーロードでは、セル内の特定のオフセットも指定することができます。
                /// </summary>
                /// <param name="dataIndex">ジャンプ先のdataIndex</param>int
                /// <param name="playerSkillPanelOffset">スクローラーの開始位置（上／左）からのオフセット（0～1）。float
                /// この範囲外は、スクローラーの表示可能領域の前後の位置にジャンプします</param>float
                /// <param name="cellOffset">セルの先頭（上／左）からのオフセット（0～1）。</param>bool
                /// <param name="useSpacing">ジャンプでスクロールの間隔を計算するかどうか</param>TweenType
                /// <param name="tweenType">ジャンプに使用するイージングについて</param>float
                /// <param name="tweenTime">ジャンプポイントまでの補間時間</param>float
                /// <param name="jumpComplete">このデリゲートは、ジャンプが完了したときに起動されます</param>Action
                playerSkillPanel.JumpToDataIndex(selectedSkillIndex, 1.0f, 1.0f);
            }
            else if (selectedSkillIndex <= playerSkillPanel.StartCellViewIndex)
            {
                // 選択されたインデックスが最上部またはそれ以上にある時
                playerSkillPanel.JumpToDataIndex(selectedSkillIndex, 0.0f, 0.0f);
            }
        }
    }

    // アイテム関係============================
    private void InitItem()
    {
        inputItemStatement = InputItemStatement.INIT_ITEM;
        Debug.Log("---PlayerInitItem---");
        // スキルパネルを表示
        battleCommand.ActivateSkillCommandPanel(true);
        // アニメーターの取得
        //turnPlayer.EquipEnemy.EnemyAnimator = playerPersona.GetComponent<Animator>();

        //Vector3 targetPos = turnPlayer.battlePlayerUI.PlayerPos.position;
        //targetPos = new Vector3(turnPlayer.battlePlayerUI.PlayerPos.position.x, turnPlayer.battlePlayerUI.PlayerPersonaPos.position.y - turnPlayer.battlePlayerUI.PlayerPos.position.y, turnPlayer.battlePlayerUI.PlayerPos.position.z);

        // EnhancedScrollerのデリゲートを指定する
        // デリゲートを設定することで、スクロールビューが必要な情報を取得
        playerSkillPanel.Delegate = this;
        // 主人公のアイテムをセットする
        LoadItemData(mainPlayer.items);
    }

    // スキルパネルからアクションを選択する
    private void HandleItemSelection()
    {
        ///////////// アニメーションの指定 /////////////
        // 10秒立ったら
        // 10秒ごとにフラフラするアニメーション
        Player turnPlayer = (Player)TurnCharacter;
        //turnPlayer.PlayerBattleAnimator.SetBool("IdleToTurnIdle", true);
        ////////////////////////////////////////////////////
        bool selectionChanged = false;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // 指定された値を範囲内にクランプ（制約）するために使用されるUnityの関数です。
            // 値が指定された範囲内にある場合はそのまま返し、範囲外の場合は範囲の最小値または最大値に
            // 制約されます
            selectedItemIndex = Mathf.Clamp(selectedItemIndex - 1, 0, itemCellDatas.Count - 1);
            selectionChanged = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedItemIndex = Mathf.Clamp(selectedItemIndex + 1, 0, itemCellDatas.Count - 1);
            selectionChanged = true;
        }
        // アイテム決定
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // UIを非表示
            battleCommand.ActivateSkillCommandPanel(false);
            // 使えるアイテムだったら
            // 回復アイテム
            if (mainPlayer.items[selectedItemIndex].ItemBase.itemType == ItemType.HEAL_ITEM)
            {
                InitItemTarget();
                inputItemStatement = ChangeInputItemStatement();
            }
        }
        // escキーを押したら戻る
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            inputItemStatement = InputItemStatement.INIT_ITEM;// スキルを選択初期状態に戻す
                                                              // スキルパネルを非表示にする
            battleCommand.ActivateSkillCommandPanel(false);
            // メインパネルを表示する
            battleCommand.ActivateBattleCommandPanel(true);
            Player player = turnCharacter as Player;
            /*
            player.PlayerBattleAnimator.SetBool("TurnIdleToIdle", true);
            player.PlayerBattleAnimator.SetBool("IdleToTurnIdle", false);
            */
            battleState = BattleState.PLAYER_ACTION_SELECT;
        }

        // 選択中
        if (selectionChanged)
        {
            // 選択されたインデックスが更新されたから基礎データを更新する
            for (int i = 0; i < itemCellDatas.Count; i++)
            {
                // 選択中のisSelectedをtrueにする
                itemCellDatas[i].isSelected = (i == selectedItemIndex);
            }

            // アクティブセルに対してUIの更新をする
            playerSkillPanel.RefreshActiveCellViews();
            // 選択されたインデックスが最下部またはその先にある時
            if (selectedItemIndex >= playerSkillPanel.EndCellViewIndex)
            {
                playerSkillPanel.JumpToDataIndex(selectedItemIndex, 1.0f, 1.0f);
            }
            else if (selectedItemIndex <= playerSkillPanel.StartCellViewIndex)
            {
                // 選択されたインデックスが最上部またはそれ以上にある時
                playerSkillPanel.JumpToDataIndex(selectedItemIndex, 0.0f, 0.0f);
            }
        }
    }

    private void InitItemTarget()
    {
        // 対象は味方のみ
        HealItemBase itemBase = mainPlayer.items[selectedItemIndex].ItemBase as HealItemBase;
        // 対象が全体だったら
        if (itemBase.TargetNum == TARGET_NUM.ALL)
        {
            selectedPlayers = new Player[activePlayers.Count];
            for (int i = 0; i < activePlayers.Count; i++)
            {
                activePlayers[i].battlePlayerUI.SetActiveSelectedArrow(true);
                selectedPlayers[i] = activePlayers[i];
            }
        }
        //　対象が単体だったら
        else
        {
            activePlayers[selectedTargetIndex].battlePlayerUI.SetActiveSelectedArrow(true);
            Debug.Log("selectedTargetIndex初期" + selectedTargetIndex);
        }
    }

    // ターゲットの選択
    private void HandleSelectItemTarget()
    {
        bool selectionChanged = false;
        HealItemBase itemBase = mainPlayer.items[selectedItemIndex].ItemBase as HealItemBase;

        // 対象が単体だったら
        if (itemBase.TargetNum == TARGET_NUM.SINGLE)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (selectedTargetIndex > 0)
                {
                    selectedTargetIndex--;
                }
                else
                {
                    selectedTargetIndex = 0;
                }
                selectionChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (selectedTargetIndex < activePlayers.Count - 1)
                {
                    selectedTargetIndex++;
                }
                selectionChanged = true;
            }

            if (selectionChanged == true)
            {
                for (int i = 0; i < activePlayers.Count; i++)
                {
                    bool isActiveSelectedArrow = (i == selectedTargetIndex);
                    activePlayers[i].battlePlayerUI.SelectedArrow.SetActive(isActiveSelectedArrow);
                }

                activePlayers[selectedTargetIndex].battlePlayerUI.SelectedArrow.SetActive(true);

                selectedPlayers = new Player[1];
                selectedPlayers[0] = activePlayers[selectedTargetIndex];
            }
        }
        else
        {
            selectedPlayers = new Player[activeEnemies.Count];
            selectedPlayers = activePlayers.ToArray();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // 選択矢印を消す
            for (int i = 0; i < activePlayers.Count; i++)
            {
                Debug.Log(i + "," + activePlayers.Count);
                activePlayers[i].battlePlayerUI.SelectedArrow.SetActive(false);
            }
            inputItemStatement = ChangeInputItemStatement();
        }

        // escキーを押したら戻る
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            inputItemStatement = InputItemStatement.ITEM_SELECT;// スキル選択状態に戻す
                                                                // ターゲット選択矢印を非表示にする
            for (int i = 0; i < activePlayers.Count; i++)
            {
                activePlayers[i].battlePlayerUI.SelectedArrow.SetActive(false);
            }

            // 選択矢印を消す
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                activeEnemies[i].EnemyUI.SelectedArrow.SetActive(false);
            }
            // スキルパネルを表示する
            battleCommand.ActivateSkillCommandPanel(true);
        }
    }

    // アイテムの処理を実行する
    private void PlayerEndItemInput()
    {
        Debug.Log("PlayerItemEndInput()");
        StartCoroutine(PerformPlayerItem());
        // PerformPlayerSkill();
    }

    private IEnumerator PerformPlayerItem()
    {
        Player player = turnCharacter as Player;
        /*
        player.PlayerBattleAnimator.SetBool("IdleToTurnIdle", false);
        player.PlayerBattleAnimator.SetBool("TurnIdleToIdle", false);
        */

        battleState = BattleState.BUSY;
        HealItemBase itemBase = mainPlayer.items[selectedItemIndex].ItemBase as HealItemBase;
        player.PlayerBattleAnimator.Play(hashUseItem);
        // 回復魔法だったら
        // ターンのプレイヤーのスキル発動モーション

        player.PlayerBattleAnimator.Play(hashUseItem);
        yield return null;// ステートの反映
        yield return new WaitForAnimation(player.PlayerBattleAnimator, 0);
        /*
        player.PlayerAnimator.SetBool("SkillToIdle", true);
        player.PlayerAnimator.SetBool("IdleToTurnIdle", false);
        */
        // 全体回復だったら
        if (itemBase.TargetNum == TARGET_NUM.ALL)
        {
            for (int i = 0; i < activePlayers.Count; i++)
            {
                // 味方にスキル発動モーション ここ：アイテムを受けるエフェクトを追加しないといけない あとで
                //Instantiate(playerSkill.skillBase.SkillRecieveEffect, activePlayers[i].PlayerBattleSprite.transform.position, activePlayers[i].PlayerBattleSprite.transform.rotation);
            }
            for (int i = 0; i < activePlayers.Count; i++)
            {
                // 回復モーション
                activePlayers[i].PlayerBattleAnimator.Play(hashHeal);
                // ここをかえる
            }
            // 一体（回）分だけ待つ
            yield return new WaitForAnimation(activePlayers[0].PlayerBattleAnimator, 0);
            for (int i = 0; i < activePlayers.Count; i++)
            {
                // 回復
                activePlayers[i].TakeHealWithItem(mainPlayer.items[selectedItemIndex]);
            }

            // HPSPの反映
            for (int i = 0; i < activePlayers.Count; i++)
            {
                activePlayers[i].battlePlayerUI.UpdateHpSp();
            }
        }
        else//対象が単体だったら
        {
            //Instantiate(playerSkill.skillBase.SkillRecieveEffect, activePlayers[selectedTargetIndex].PlayerModel.transform.position, activePlayers[selectedTargetIndex].PlayerModel.transform.rotation);
            // ここ
            activePlayers[selectedTargetIndex].TakeHealWithItem(mainPlayer.items[selectedItemIndex]);

            // 回復モーション

            activePlayers[selectedTargetIndex].PlayerBattleAnimator.Play(hashHeal);
            //activePlayers[selectedTargetIndex].PlayerBattleAnimator.SetBool("HealToIdle", true);
            yield return null;// ステートの反映
            yield return new WaitForAnimation(activePlayers[selectedTargetIndex].PlayerBattleAnimator, 0);

            // HPSPの反映
            for (int i = 0; i < activePlayers.Count; i++)
            {
                activePlayers[i].battlePlayerUI.UpdateHpSp();
            }
        }

        yield return new WaitForSeconds(1);
        NextTurn();

        player.PlayerBattleAnimator.SetBool("IdleToTurnIdle", false);
        player.PlayerBattleAnimator.SetBool("TurnIdleToIdle", true);
    }

    private void EndBattle()
    {
        GameManager.instance.EndBattle();
        playerSkillPanel.gameObject.SetActive(false);
    }
    private void EscapeBattle()
    {
        GameManager.instance.EscapeBattle();
        playerSkillPanel.gameObject.SetActive(false);
    }

    /// ===================================

    // スキルデータの設定

    private void LoadSkillData(List<EnemySkill> playerSkillDatas)
    {
        Debug.Log("スキルのロード");
        // 適当なデータを設定する
        skillDatas = new List<SkillCellData>();
        for (int i = 0; i < playerSkillDatas.Count; i++)
        {
            skillDatas.Add(new SkillCellData()
            {
                skillText = playerSkillDatas[i].skillBase.SkillName,
                isSelected = i == selectedSkillIndex,
                //type = playerSkillDatas[i].skillBase.MagicType,
                sp = playerSkillDatas[i].skillBase.Sp
            });
        }

        // データが揃ったのでスクローラーをリロードする
        playerSkillPanel.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller playerSkillPanel)
    {
        if ((Move)currentMove == Move.ATTACK)
        {
            return skillDatas.Count;
        }
        else if ((Move)currentMove == Move.ITEM)
        {
            return itemCellDatas.Count;
        }
        return skillDatas.Count;
    }

    public float GetCellViewSize(EnhancedScroller playerSkillPanel, int dataIndex)
    {
        return cellSize;
    }

    // 表示するセルを取得します
    public EnhancedScrollerCellView GetCellView(EnhancedScroller playerSkillPanel, int dataIndex, int cellIndex)
    {
        if ((Move)currentMove == Move.ATTACK)
        {
            SkillCellView cellView = playerSkillPanel.GetCellView(cellViewPrefab) as SkillCellView;
            cellView.name = "Cell" + dataIndex.ToString();
            cellView.SetSkillData(skillDatas[dataIndex]);
            return cellView;
        }
        else if ((Move)currentMove == Move.ITEM)
        {
            ItemCellView cellView = playerSkillPanel.GetCellView(itemCellViewPrefab) as ItemCellView;
            cellView.name = "Cell" + dataIndex.ToString();
            // cellView.SetSkillData(skillDatas[dataIndex]);
            cellView.SetData(itemCellDatas[dataIndex]);
            return cellView;
        }
        // デフォルト
        else
        {
            SkillCellView cellView = playerSkillPanel.GetCellView(cellViewPrefab) as SkillCellView;
            cellView.name = "Cell" + dataIndex.ToString();
            cellView.SetSkillData(skillDatas[dataIndex]);
            return cellView;
        }
    }

    // アイテムデータの設定
    private void LoadItemData(List<Item> playerItemDatas)
    {
        Debug.Log("アイテムをロード");
        // 適当なデータを設定する
        itemCellDatas = new List<ItemCellData>();
        for (int i = 0; i < playerItemDatas.Count; i++)
        {
            Debug.Log(playerItemDatas[i].ItemBase.ItemName);
            itemCellDatas.Add(new ItemCellData()
            {
                itemText = playerItemDatas[i].ItemBase.ItemName,
                isSelected = i == selectedItemIndex,
                itemCountText = playerItemDatas[i].ItemCount.ToString(),
            });
        }

        // データが揃ったのでスクローラーをリロードする
        playerSkillPanel.ReloadData();
    }

    private void LoadDiggingItemData(List<Item> playerItemDatas)
    {
        // 適当なデータを設定する
        itemCellDatas = new List<ItemCellData>();
        int j = 0;
        for (int i = 0; i < playerItemDatas.Count; i++)
        {
            if (playerItemDatas[i].ItemBase.itemType == ItemType.WEAPON)
            {
                itemCellDatas.Add(new ItemCellData()
                {
                    itemText = playerItemDatas[i].ItemBase.ItemName,
                    isSelected = j == selectedItemIndex,
                    itemCountText = playerItemDatas[i].ItemCount.ToString(),
                });
                j++;
            }
        }
        /*for (int i = 0; i < itemCellDatas.Count; i++)
        {
            Debug.Log("入っているアイテムは" + itemCellDatas[i].itemText);
        }*/

        // データが揃ったのでスクローラーをリロードする
        playerSkillPanel.ReloadData();
    }
}

// カスタムコルーチン：現在のアニメーションステートが再生し終わるまで待つ
public class WaitForAnimation : CustomYieldInstruction
{
    private Animator m_animator;
    private int m_lastStateHash = 0;
    private int m_layerNo = 0;

    public WaitForAnimation(Animator animator, int layerNo)
    {
        Init(animator, layerNo, animator.GetCurrentAnimatorStateInfo(layerNo).fullPathHash);
    }

    private void Init(Animator animator, int layerNo, int hash)
    {
        m_layerNo = layerNo;
        m_animator = animator;
        m_lastStateHash = hash;
    }

    public override bool keepWaiting
    {
        get
        {
            var currentAnimatorState = m_animator.GetCurrentAnimatorStateInfo(m_layerNo);
            // WaitForAnimation()が実行された時のアニメーションステートと現在のアニメーションステートが同じ
            // かつ，現在のアニメーションステートが終わったら trueを返してコルーチン終了
            return currentAnimatorState.fullPathHash == m_lastStateHash && (currentAnimatorState.normalizedTime < 1);
        }
    }
}