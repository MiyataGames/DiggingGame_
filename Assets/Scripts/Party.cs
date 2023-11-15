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

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Setup()
    {
        players = new List<Player>();
        for (int i = 0; i < playerBasies.Length; i++)
        {
            Player player = new Player(playerBasies[i], 1);
            Debug.Log(player.level);
            players.Add(player);
        }
    }
    /*
    public void LoadPlayerSetUp(List<string> dataStr)
    {
        for (int i = 0; i < dataStr.Count; i++)
        {
            int id = dataStr[i].Value<>
            // IDに該当するプレイヤーをさがす
            Player player = players.Single(value => value.playerID == dataStr.);
            //データを入れる
            player = JsonUtility.FromJson<Player>(dataStr[i]);
        }


    }*/
}
