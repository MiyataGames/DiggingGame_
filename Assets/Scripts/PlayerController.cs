using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    //[SerializeField] private GroundController groundController;
    private Define.DirectionNumber currentDirectionNumber;

    private bool ajustPosition = false;

    // Start is called before the first frame update
    private void Start()
    {
        //groundController.DigHoleAllTexture(transform.position, Define.DirectionNumber.NONE);
    }

    // Update is called once per frame
    private void Update()
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
           // groundController.DigHoleAllTexture(transform.position, currentDirectionNumber);
        }
        else
        //D+W：右上
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W))
        {
            this.transform.position += new Vector3(1, 1, 0).normalized * speed * Time.deltaTime;
            currentDirectionNumber = Define.DirectionNumber.RIGHT_UP;
           // groundController.DigHoleAllTexture(transform.position, currentDirectionNumber);
        }
        else
        // A：左
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.position += new Vector3(-1, 0, 0) * speed * Time.deltaTime;
            currentDirectionNumber = Define.DirectionNumber.LEFT;
            //groundController.DigHoleAllTexture(transform.position, currentDirectionNumber);
        }
        else
        // S：下
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.position += new Vector3(0, -1, 0) * speed * Time.deltaTime;
            currentDirectionNumber = Define.DirectionNumber.DOWN;
           // groundController.DigHoleAllTexture(transform.position, currentDirectionNumber);
        }
        else
        // D：右
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.position += new Vector3(1, 0, 0) * speed * Time.deltaTime;
            currentDirectionNumber = Define.DirectionNumber.RIGHT;
           // groundController.DigHoleAllTexture(transform.position, currentDirectionNumber);
        }
        else
        // 左下
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
        {
            this.transform.position += new Vector3(-1, -1, 0).normalized * speed * Time.deltaTime;
            currentDirectionNumber = Define.DirectionNumber.LEFT_DOWN;
           // groundController.DigHoleAllTexture(transform.position, currentDirectionNumber);
        }
        else
        // 右下
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
        {
            this.transform.position += new Vector3(1, -1, 0).normalized * speed * Time.deltaTime;
            currentDirectionNumber = Define.DirectionNumber.RIGHT_DOWN;
           // groundController.DigHoleAllTexture(transform.position, currentDirectionNumber);
        }

        // 移動しているキーを話したら
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
        {
            // transform.position += new Vector3(Define.directions[((int)currentDirectionNumber)].normalized.x * 0.2f, Define.directions[((int)currentDirectionNumber)].normalized.y * 0.2f, 0);
        }

        // サーチ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // サーチ機能
        }
        // メニュー画面
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // メニュー画面をひらく
        }
        // マップ
        if (Input.GetKeyDown(KeyCode.M))
        {
            // マップを開く
        }
    }
}