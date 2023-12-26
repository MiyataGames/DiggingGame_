using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerScript : MonoBehaviour
{
    public Transform player; // プレイヤーのTransform
    public float thresholdX = 5.0f; // X座標のしきい値
    public float thresholdY = 5.0f; // Y座標のしきい値

    private float offsetX; // カメラとプレイヤーのX座標の距離
    private float offsetY; // カメラとプレイヤーのY座標の距離
    private Vector3 prePlayerPos; // 前フレームのプレイヤーの位置

    void Start()
    {
        // 初期オフセットを計算
        offsetX = transform.position.x - player.position.x;
        offsetY = transform.position.y - player.position.y;
        prePlayerPos = player.position;
    }

    void LateUpdate()
    {
        Vector3 currentCamPos = transform.position;

        offsetX = transform.position.x - player.position.x;
        offsetY = transform.position.y - player.position.y;

        // X座標の追従処理
        if (Mathf.Abs(offsetX) > thresholdX)
        {
            currentCamPos.x = currentCamPos.x + (player.position.x - prePlayerPos.x);
        }

        // Y座標の追従処理
        currentCamPos.y = Mathf.Lerp(
            transform.position.y,
            player.position.y + thresholdY, // カメラzの位置
            5.0f * Time.deltaTime);

        // カメラ位置の更新
        transform.position = currentCamPos;
        prePlayerPos = player.position; // ここを修正
    }
}

