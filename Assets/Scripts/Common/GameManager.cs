using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayMode{
    DEBUG,
    RELEASE
};

public struct DropObjectsStruct
{
    public int totalDropGold;
    public List<Item> dropItems;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject[] fieldObjects;
    public GameObject[] eventSceneObjects;

    private int currentSceneIndex;
    public PlayMode playMode;
    private GameMode currentGameMode;
    public GameState currentGameState;
    public Event1Scene currentEvent1Scene;
    [SerializeField] private BattleSceneManager battleSceneManager;
    [SerializeField] private ResultSceneMangaer resultSceneManager;
    // [SerializeField] private ResultSceneMangaer resultSceneManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerTownController playerTownController;
    [SerializeField] Party party;// パーティ情報　ID順
    [SerializeField] private PlayerUnit playerUnit;
    [SerializeField] private EnemyUnit enemyUnit;
    Player mainPlayer;
    DropObjectsStruct dropObjects;
    // フィールド上の敵のシンボル
    GameObject enemySymbol;
    // バトル中のプレイヤー
    List<Player> battlePlayers;
    [SerializeField]
    private List<Enemy> enemies;// バトル中の敵
    [SerializeField] private ExpSheet expSheet;// 経験値表

    // デバッグ用のボタン
    [SerializeField] GameObject skipButton;

    public Party Party { get => party; set => party = value; }
    public DropObjectsStruct DropObjects { get => dropObjects;}

    void Start()
    {
        if (playMode == PlayMode.DEBUG)
        {
            skipButton.SetActive(true);
        }
        else
        {
            skipButton.SetActive(false);
        }
    }

    private void Update()
    {
//        Debug.Log(currentSceneIndex);
        if (currentSceneIndex == (int)GameMode.FIELD_SCENE)
        {
//            Debug.Log("フィールド実行中でーす");
            playerController.HandleUpdate();
        }
        else if (currentSceneIndex == (int)GameMode.BATTLE_SCENE)
        {
 //           Debug.Log("バトル実行中でーす");
            battleSceneManager.HandleUpdate();
        }else if(currentSceneIndex == (int)GameMode.TOWN_SCENE)
        {
 //           Debug.Log("タウン実行中でーす");
            playerTownController.HandleUpdate();
        }
        /*
        else if (currentSceneIndex == (int)GameMode.RESULT_SCENE)
        {
        }*/
    }

    public int CurrentSceneIndex
    {
        get { return currentSceneIndex; }
        set
        {
            currentSceneIndex = value;
            Debug.Log(currentSceneIndex);
            ActivateCurrentScene(currentSceneIndex);
        }
    }

    // ゲームの全ての初期設定を行う
    public void InitGame(Party party)
    {
        currentSceneIndex = (int)GameMode.FIELD_SCENE;
        this.Party = party;
    }

    public void ActivateCurrentScene(int sceneIndex)
    {
        currentSceneIndex = sceneIndex;
        for (int i = 0; i < fieldObjects.Length; i++)
        {
            fieldObjects[i].SetActive(false);
        }
        fieldObjects[sceneIndex].SetActive(true);
    }

    //private bool FirstBattle = true;

    public void StartBattle(GameObject enemyObj)
    {
        enemySymbol = enemyObj;
        ActivateCurrentScene((int)GameMode.BATTLE_SCENE);
        battleSceneManager.StartBattle();
        battlePlayers = new List<Player>();
        // 生成
        enemies = new List<Enemy>();
        playerUnit.SetUpBattle(Party);
        // モンスターの生成
        enemyUnit.SetUp();
        battlePlayers = new List<Player>(playerUnit.SortedBattlePlayers);
        for(int i = 0;i<battlePlayers.Count; i++)
        {
            Debug.Log("バトルに出たプレイヤーID"+battlePlayers[i].PlayerBase.PlayerId);
        }
        enemies = new List<Enemy>(enemyUnit.Enemies);
        Debug.Log("enemyCount" + enemies.Count);

        // 主人公を探す
        mainPlayer = Party.Players[0];

        battleSceneManager.InitBattle(battlePlayers, enemies,mainPlayer);
    }

    public void EndBattle()
    {
        //ActivateCurrentScene((int)GameMode.FIELD_SCENE);

        CurrentSceneIndex = (int)GameMode.RESULT_SCENE;
        // 報酬をゲットする
        int totalGold = 0;
        // ① お金
        for (int i = 0; i < enemies.Count; i++) {
            totalGold += enemies[i].DropGold;
        }
        dropObjects.totalDropGold = totalGold;
        mainPlayer.Gold += totalGold;
        Debug.Log("落としたお金は" + totalGold);
        // ② アイテム
        List<Item> dropItems = new List<Item>(); ;
        for(int i = 0; i < enemies.Count; i++)
        {
           for(int j = 0;j < enemies[i].DropItems.Count; j++)
            {
                dropItems.Add(enemies[i].DropItems[j]);
            }
        }
        dropObjects.dropItems = new List<Item>(dropItems);
        for(int i = 0; i< dropItems.Count; i++)
        {
            mainPlayer.AddItem(dropItems[i]);
            Debug.Log("落としたアイテムは" + dropItems[i].ItemBase.ItemName);
        }
        StartCoroutine(resultSceneManager.ResultPlayer((Player)battleSceneManager.TurnCharacter));
        if(enemySymbol != null)
        {
            Destroy(enemySymbol);
        }
        
    }

    // 逃げる
    public void EscapeBattle()
    {
        ActivateCurrentScene((int)GameMode.FIELD_SCENE);
        Debug.Log("逃げる");
        Debug.Log(enemySymbol.GetComponent<FieldEnemy>());
        StartCoroutine(enemySymbol.GetComponent<FieldEnemy>().Blinking());
    }

    
    public IEnumerator UpdateExpAnimation()
    {
        // 経験値の処理
        int exp = 0;
        Debug.Log("enemyCount" + enemies.Count);
        for (int i = 0; i < enemies.Count; i++)
        {
            exp += expSheet.sheets[0].list[enemies[i].Level - 1].exp;
            Debug.Log("倒した敵の経験値は" + expSheet.sheets[0].list[enemies[i].Level - 1].exp);
        }
        // floatでよみこまなきゃだめ？
        IEnumerator enumerator = null;
        for (int i = 0; i < battlePlayers.Count; i++)
        {
            Debug.Log(battlePlayers[i].PlayerBase.PlayerName);
            ExpPair expPair = battlePlayers[i].GetExp(exp);
            battlePlayers[i].ResultPlayerUI.SetPlayerNameText();
            enumerator = battlePlayers[i].ResultPlayerUI.UpdateExp(expPair);
            StartCoroutine(enumerator);
            //players[i].ResultPlayerUI.SetPlayerLevelText();
        }
        yield return enumerator;
        //ActivateCurrentScene((int)World.GameMode.FIELD_SCENE);
    }

    // イベント関係
    /// <summary>
    /// 今のイベントのシーンを呼び出す
    /// </summary>
    public Event1Scene CurrenEvent1Scene
    {
        get { return currentEvent1Scene; }
        set
        {
            currentEvent1Scene = value;
            Debug.Log(currentEvent1Scene);
            // イベントでなければ何もしない
            if (currentEvent1Scene != Event1Scene.NONE)
            {
                for (int i = 0; i < (int)Event1Scene.END -1; i++) {
                    if (eventSceneObjects[i] != null)
                    {
                        eventSceneObjects[i].SetActive(false);
                    }
                }
                eventSceneObjects[(int)currentEvent1Scene].SetActive(true);
            }
        }
    }
}

