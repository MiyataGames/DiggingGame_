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
            SortedBattlePlayers[j].battlePlayerUI = playerPos[j].transform.Find("PlayerCanvasTextMeshPro/PlayerStatusPanel").gameObject.GetComponent<BattlePlayerUI>();
            Debug.Log(SortedBattlePlayers[j].battlePlayerUI.gameObject.activeSelf);
            Debug.Log(SortedBattlePlayers[j].battlePlayerUI.gameObject.name);
            SortedBattlePlayers[j].battlePlayerUI.gameObject.SetActive(true);
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