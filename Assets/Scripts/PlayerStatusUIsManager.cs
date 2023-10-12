using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusUIsManager : MonoBehaviour
{
    private int playersCount = 0;
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private GameObject playerStatusUIsManager;
    private List<PlayerFieldUI> playerFieldUIs;

    public void SetUpPlayerStatusUI(List<Player> players)
    {
        // プレイヤーの人数が変わったときだけ更新する
        if (players.Count != playersCount)
        {
            // 動作確認まだ
            for (int i = 0; i < playersCount; i++)
            {
                Destroy(playerFieldUIs[i]);
            }
            playerFieldUIs = new List<PlayerFieldUI>();
            // ステータス画面のPrefabをInstantiateする
            for (int i = 0; i < players.Count; i++)
            {
                PlayerFieldUI playerFieldUI = Instantiate(playerUIPrefab, playerStatusUIsManager.transform).GetComponent<PlayerFieldUI>();
                // ステータスUIマネジャーに追加する
                playerFieldUIs.Add(playerFieldUI);
                // プレイヤーのUIに追加
                players[i].playerUI = playerFieldUI;
                // ステータス画面に情報を設定する
                playerFieldUI.SetPlayerStatus(players[i]);
            }
        }
        playersCount = players.Count;
    }

    public void selectStatus(int index)
    {
        // フレームをオフ
        for (int i = 0; i < playerFieldUIs.Count; i++)
        {
            playerFieldUIs[i].SetActivateSelectedFrame(false);
        }
        playerFieldUIs[index].SetActivateSelectedFrame(true);
    }
}