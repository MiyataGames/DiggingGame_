using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using System.IO;
//using Newtonsoft.Json;

public class Party : MonoBehaviour
{

    [SerializeField] private PlayerBase[] playerBasies;
    private List<Player> players;
    public List<Player> Players { get => players; }

    // テスト用
    public List<ItemBase> debugItemBase;

    [SerializeField] GameObject resultPlayerUI;
    [SerializeField] private GameObject resultAreaPanel;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetupFirst()
    {
        players = new List<Player>();
        for (int i = 0; i < playerBasies.Length; i++)
        {
            Player player = new Player(playerBasies[i], 1,debugItemBase);
           //  Debug.Log(player.level);
            players.Add(player);
            // リザルトパネル
            Players[i].ResultPlayerUI = Instantiate(resultPlayerUI, Vector3.zero, Quaternion.identity, resultAreaPanel.transform).GetComponent<ResultPlayerUI>();
            Players[i].ResultPlayerUI.SetUpResultPanel(Players[i]);
        }
    }

    // 仲間が加入した時に実行される
    public void SetUp_JoinFriend(Player player)
    {
        Player newPlayer = player;
        newPlayer.ResultPlayerUI = Instantiate(resultPlayerUI, Vector3.zero, Quaternion.identity, resultAreaPanel.transform).GetComponent<ResultPlayerUI>();
        newPlayer.ResultPlayerUI.SetUpResultPanel(newPlayer);
        players.Add(newPlayer);
        // ID順にソート
        players.Sort((player1, player2) => player1.PlayerID - player2.PlayerID);
    }
}
