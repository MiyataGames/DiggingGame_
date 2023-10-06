﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStatus
{
    DIGGING,
    MENU,
}

public class Define
{
    public enum DirectionNumber
    {
        RIGHT_UP,
        RIGHT,
        RIGHT_DOWN,
        DOWN,
        LEFT_DOWN,
        LEFT,
        LEFT_UP,
        NONE
    }

    // 方向　右上、右、右下、下、左下、左、左上
    public static Vector2[] directions = { new Vector2(1, 1), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1), new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, 0) };
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GroundController groundController;
    private Define.DirectionNumber currentDirectionNumber;
    [SerializeField] private Menu menu;

    private GameStatus currentGameStatus;
    private int currentMenuCommandNum;
    private int currentItemNum;

    private Player player;

    // Start is called before the first frame update
    private void Start()
    {
        currentGameStatus = GameStatus.DIGGING;
        currentMenuCommandNum = 0;
        currentItemNum = 0;
        groundController.DigHoleAllTexture(transform.position, Define.DirectionNumber.NONE);
        player = new Player("マオ");
    }

    // Update is called once per frame
    private void Update()
    {
        // サーチ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // サーチ機能
        }
        // メニュー画面
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // メニュー画面をひらく
            currentGameStatus = GameStatus.MENU;
            menu.ActivateMenuPanel(true);
            menu.ActivateMenuSelectArrow(MenuCommand.ITEM);
        }
        // マップ
        if (Input.GetKeyDown(KeyCode.M))
        {
            // マップを開く
        }

        // 穴掘り中だったら
        if (currentGameStatus == GameStatus.DIGGING)
        {
            // 移動
            /*
            // W：上
            if (Input.GetKey(KeyCode.W))
            {
                this.transform.position += new Vector3(0, 1, 0) * speed * Time.deltaTime;
            }*/

            // A+W：左上
            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W))
            {
                this.transform.position += new Vector3(-1, 1, 0).normalized * speed * Time.deltaTime;
                currentDirectionNumber = Define.DirectionNumber.LEFT_UP;
                groundController.DigHoleAllTexture(transform.position, currentDirectionNumber);
            }
            else
            //D+W：右上
            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W))
            {
                this.transform.position += new Vector3(1, 1, 0).normalized * speed * Time.deltaTime;
                currentDirectionNumber = Define.DirectionNumber.RIGHT_UP;
                groundController.DigHoleAllTexture(transform.position, currentDirectionNumber);
            }
            else
            // 左下
            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
            {
                this.transform.position += new Vector3(-1, -1, 0).normalized * speed * Time.deltaTime;
                currentDirectionNumber = Define.DirectionNumber.LEFT_DOWN;
                groundController.DigHoleAllTexture(transform.position, currentDirectionNumber);
            }
            else
            // 右下
            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
            {
                this.transform.position += new Vector3(1, -1, 0).normalized * speed * Time.deltaTime;
                currentDirectionNumber = Define.DirectionNumber.RIGHT_DOWN;
                groundController.DigHoleAllTexture(transform.position, currentDirectionNumber);
            }
            else
            // A：左
            if (Input.GetKey(KeyCode.A))
            {
                this.transform.position += new Vector3(-1, 0, 0) * speed * Time.deltaTime;
                currentDirectionNumber = Define.DirectionNumber.LEFT;
                groundController.DigHoleAllTexture(transform.position, currentDirectionNumber);
            }
            else
            // S：下
            if (Input.GetKey(KeyCode.S))
            {
                this.transform.position += new Vector3(0, -1, 0) * speed * Time.deltaTime;
                currentDirectionNumber = Define.DirectionNumber.DOWN;
                groundController.DigHoleAllTexture(transform.position, currentDirectionNumber);
            }
            else
            // D：右
            if (Input.GetKey(KeyCode.D))
            {
                this.transform.position += new Vector3(1, 0, 0) * speed * Time.deltaTime;
                currentDirectionNumber = Define.DirectionNumber.RIGHT;
                groundController.DigHoleAllTexture(transform.position, currentDirectionNumber);
            }
            else
            {
                if (Input.GetKeyUp(KeyCode.D))
                {
                    transform.position += new Vector3(Define.directions[((int)currentDirectionNumber)].normalized.x * 0.2f, Define.directions[((int)currentDirectionNumber)].normalized.y * 0.2f, 0);
                }
                else if (Input.GetKeyUp(KeyCode.A))
                {
                    transform.position += new Vector3(Define.directions[((int)currentDirectionNumber)].normalized.x * 0.2f, Define.directions[((int)currentDirectionNumber)].normalized.y * 0.2f, 0);
                }
                else if (Input.GetKeyUp(KeyCode.S))
                {
                    transform.position += new Vector3(Define.directions[((int)currentDirectionNumber)].normalized.x * 0.2f, Define.directions[((int)currentDirectionNumber)].normalized.y * 0.2f, 0);
                }
            }

            // 移動しているキーを話したら
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
            {
                // transform.position += new Vector3(Define.directions[((int)currentDirectionNumber)].normalized.x * 0.2f, Define.directions[((int)currentDirectionNumber)].normalized.y * 0.2f, 0);
            }
        }
        // メインパネルを選択中だったら
        if (currentGameStatus == GameStatus.MENU)
        {
            HandleMenuSelect();
        }
    }

    private void HandleMenuSelect()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMenuCommandNum < (int)MenuCommand.END - 1)
            {
                currentMenuCommandNum++;
                menu.ActivateMenuSelectArrow((MenuCommand)currentMenuCommandNum);
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMenuCommandNum > 0)
            {
                currentMenuCommandNum--;
                menu.ActivateMenuSelectArrow((MenuCommand)currentMenuCommandNum);
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // メニューを選択
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // メニュー画面を閉じる
                menu.ActivateMenuPanel(false);
                currentGameStatus = GameStatus.DIGGING;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < player.Items.Count; i++)
        {
            Debug.Log(player.Items[i].ItemBase.ItemName + ":" + player.Items[i].ItemCount);
        }
        // 衝突したのがアイテムだったら
        if (other.tag == "Item")
        {
            Item newItem = other.GetComponent<Item>();
            // 同じアイテムがあるか検索
            if (player.Items.Count > 0)
            {
                for (int i = 0; i < player.Items.Count; i++)
                {
                    if (newItem.ItemBase.Id == player.Items[i].ItemBase.Id)
                    {
                        // あったら個数を増やして破棄
                        player.Items[i].ItemCount++;
                        Destroy(other.gameObject);
                        return;
                    }
                }
            }
            // なければ新しく追加する
            player.Items.Add(other.GetComponent<Item>());
            player.Items[player.Items.Count - 1].ItemCount++;
            Debug.Log(player.Items[0].ItemBase.ItemName);
            Destroy(other.gameObject);
            // idが早い順に並べる
            player.Items.Sort((x, y) => y.Id - x.Id);
        }
    }
}