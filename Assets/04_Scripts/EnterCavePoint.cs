using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterCavePoint : MonoBehaviour
{
    [SerializeField] FadeController fadeController;
    [SerializeField] GameObject fieldPlayer;
    [SerializeField] Transform fieldPlayer_caveStartPos;
    [SerializeField] GameObject caveTilemap;
    [SerializeField] GameObject playerTilemap;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // フィールドへ移動
        fadeController.StartFadeOutIn();
        caveTilemap.SetActive(true);
        GameManager.instance.CurrentSceneIndex = (int)GameMode.FIELD_SCENE;
        Camera.main.GetComponent<FollowPlayerScript>().enabled = true;
        playerTilemap.SetActive(false);
        fieldPlayer.transform.position = fieldPlayer_caveStartPos.position;
    }
}
