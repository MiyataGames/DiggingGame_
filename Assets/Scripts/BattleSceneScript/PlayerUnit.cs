using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class PlayerUnit : MonoBehaviour
{
    [SerializeField]
    private PlayerBase[] playerBasies;
    List<Player> sortedBattlePlayers;
    //[SerializeField]
    //private Transform[] playerBeforePos;

    [SerializeField]
    private GameObject[] playerPos;

    public GameObject[] PlayerPos { get => playerPos; }
    public List<Player> SortedBattlePlayers { get => sortedBattlePlayers;}

    //public Transform[] PlayerBeforePos { get => playerBeforePos; }

    [SerializeField] GameObject selectArrowPref;

    // テスト用
    public List<ItemBase> debugItemBase;

    /*
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
            
            players[j].ResultPlayerUI = Instantiate(resultPlayerUI, Vector3.zero, Quaternion.identity, resultAreaPanel.transform).GetComponent<ResultPlayerUI>();
            players[j].ResultPlayerUI.SetUpResultPanel(players[j]);
            
            j++;
        }
    }
    */

    // 前のレベルを引き継いでプレイヤーを生成する
    public void SetUpBattle(Party party)
    {
        sortedBattlePlayers = new List<Player>();
        sortedBattlePlayers = new List<Player>(party.Players);

        Dictionary<Player, int> agiPlayerDic = new Dictionary<Player, int>();

        for (int i = 0; i < SortedBattlePlayers.Count; i++)
        {
            //List<Enemy> enemyList = this.gameObject.transform.Find("Player" + playerBasies[i].PlayerId + "Persona").GetComponent<PersonaParty>().EnemyList;
            agiPlayerDic.Add(SortedBattlePlayers[i], SortedBattlePlayers[i].PlayerBase.PlayerMaxAgi);
        }
        int j = 0;
        // 速さ順にInstantiate
        foreach (var player in agiPlayerDic.OrderByDescending(c => c.Value))
        {

            // 死んでいるならばHP１にして復活
            if (SortedBattlePlayers[j].currentHP == 0)
            {
                SortedBattlePlayers[j].currentHP = 1;
            }
            // 早い順に順番にplayerをいれる
            SortedBattlePlayers[j] = player.Key;// ここでなんか書き換わっている

            //playerModelを生成
            //players[j].PlayerModel = Instantiate(players[j].PlayerBase.PlayerModel, PlayerBeforePos[j].transform);

            SortedBattlePlayers[j].PlayerBattleSprite = Instantiate(SortedBattlePlayers[j].PlayerBase.PlayerBattleSceneSprite, playerPos[j].transform);
            SortedBattlePlayers[j].PlayerBattleAnimator = SortedBattlePlayers[j].PlayerBattleSprite.GetComponent<Animator>();
            SortedBattlePlayers[j].battlePlayerUI = playerPos[j].transform.Find("PlayerCanvas/PlayerStatusPanel").gameObject.GetComponent<BattlePlayerUI>();
            // 選択矢印の生成
            Vector3 selectArrowPosition = new Vector3(SortedBattlePlayers[j].PlayerBattleSprite.transform.position.x, SortedBattlePlayers[j].PlayerBattleSprite.transform.position.y + 2, SortedBattlePlayers[j].PlayerBattleSprite.transform.position.z);
            GameObject selectArrow = Instantiate(selectArrowPref, selectArrowPosition, Quaternion.identity,PlayerPos[j].transform);
            SortedBattlePlayers[j].battlePlayerUI.SelectedArrow = selectArrow;
            SortedBattlePlayers[j].battlePlayerUI.SelectedArrow.SetActive(false);
            SortedBattlePlayers[j].battlePlayerUI.SetPlayerData(SortedBattlePlayers[j]);
            //Debug.Log("ResultPanel/player" + j + "ResultPanel");
            j++;
        }
    }
}