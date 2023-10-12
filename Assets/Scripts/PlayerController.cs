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

    public Rigidbody2D rb;

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

    private void FixedUpdate() {
        // A：左
        if (Input.GetKey(KeyCode.A))
        {
            rb.velocity = new Vector3(-speed,0, 0);
        }
        else
        // D：右
        if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector3(speed,0, 0);
        }
        
    
    }
}