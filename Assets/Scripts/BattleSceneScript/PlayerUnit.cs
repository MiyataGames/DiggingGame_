using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class PlayerUnit : MonoBehaviour
{
    [SerializeField]
    private PlayerBase[] playerBasies;

    //[SerializeField]
    //private Transform[] playerBeforePos;

    [SerializeField]
    private GameObject[] playerPos;

    [SerializeField]
    private Player[] players;

    public Player[] Players { get => players; }

    public GameObject[] PlayerPos { get => playerPos; }
    //public Transform[] PlayerBeforePos { get => playerBeforePos; }
    // [SerializeField] private GameObject resultCanvas;


    // テスト用
    public List<ItemBase> debugItemBase;

    public void SetUpFirst(int level)
    {
        int playerNum = 2;
        players = new Player[playerNum];
        Dictionary<Player, int> agiPlayerDic = new Dictionary<Player, int>();

        // agiPlayerDicにはプレイヤーの情報とAgiの情報を対にしていれる
        for (int i = 0; i < playerNum; i++)
        {
            //List<Enemy> enemyList = this.gameObject.transform.Find("Player" + playerBasies[i].PlayerId + "Persona").GetComponent<PersonaParty>().EnemyList;
            // レベル１で生成
            Debug.Log(debugItemBase[0].ItemName);
            Player player = new Player(playerBasies[i], level,debugItemBase);
            agiPlayerDic.Add(player, player.PlayerBase.PlayerMaxAgi);
        }

        // 速さ順にInstantiate
        int j = 0;
        foreach (var player in agiPlayerDic.OrderByDescending(c => c.Value))
        {
            // 早い順に順番にplayerをいれる
            players[j] = player.Key;
            
            //playerModelを生成
            //players[j].PlayerModel = Instantiate(players[j].PlayerBase.PlayerModel, PlayerBeforePos[j].transform);
            
            players[j].PlayerBattleSprite = Instantiate(players[j].PlayerBase.PlayerBattleSceneSprite, playerPos[j].transform);
            players[j].PlayerBattleAnimator = players[j].PlayerBattleSprite.GetComponent<Animator>();
            players[j].battlePlayerUI = playerPos[j].transform.Find("PlayerCanvas/PlayerStatusPanel").gameObject.GetComponent<BattlePlayerUI>();
            players[j].battlePlayerUI.SetPlayerData(players[j]);
            //Debug.Log("ResultPanel/player" + j + "ResultPanel");
            /*
            players[j].ResultbattlePlayerUI = resultCanvas.transform.Find("ResultPanel/player" + j + "ResultPanel").GetComponent<ResultbattlePlayerUI>();
            players[j].ResultbattlePlayerUI.SetUpResultPanel(players[j]);
            */
            j++;
        }
    }

    // 前のレベルを引き継いでプレイヤーを生成する
    public void SetUp()
    {
        Dictionary<Player, int> agiPlayerDic = new Dictionary<Player, int>();

        for (int i = 0; i < Players.Length; i++)
        {
            //List<Enemy> enemyList = this.gameObject.transform.Find("Player" + playerBasies[i].PlayerId + "Persona").GetComponent<PersonaParty>().EnemyList;
            agiPlayerDic.Add(Players[i], Players[i].PlayerBase.PlayerMaxAgi);
        }
        int j = 0;
        // 速さ順にInstantiate
        foreach (var player in agiPlayerDic.OrderByDescending(c => c.Value))
        {
            // 死んでいるならばHP１にして復活
            if(players[j].currentHP == 0)
            {
                players[j].currentHP = 1;
            }
            players[j] = player.Key;

            //players[j].PlayerModel = Instantiate(players[j].PlayerBase.PlayerModel, PlayerBeforePos[j].transform);
            //players[j].PlayerAnimator = players[j].PlayerModel.GetComponent<Animator>();

            players[j].PlayerBattleSprite = Instantiate(players[j].PlayerBase.PlayerBattleSceneSprite, playerPos[j].transform);
            players[j].battlePlayerUI = playerPos[j].transform.Find("PlayerCanvas/PlayerStatusPanel").gameObject.GetComponent<BattlePlayerUI>();
            players[j].battlePlayerUI.SetPlayerData(players[j]);
            j++;
        }
    }
}