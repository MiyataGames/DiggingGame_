using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

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
    [SerializeField] private float jumpPower;

    private Rigidbody2D rb;
    private float vx;
    private float vy;
    private bool pushFlag;
    private bool jumpFlag;
    private bool groundFlag;

    // Start is called before the first frame update
    private void Start()
    {
        //groundController.DigHoleAllTexture(transform.position, Define.DirectionNumber.NONE);
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    private void Update()
    {

        vx = 0;
        vy = 0;
        
        // A：左
        if (Input.GetKey(KeyCode.A))
        {
            vx = -speed;
        }
        else
        // D：右
        if (Input.GetKey(KeyCode.D))
        {
            vx = speed;
        }

        if(Input.GetKey("space") && groundFlag == true){
            if(pushFlag == false){
                jumpFlag = true;
                pushFlag = true;
            }
        }else{
            pushFlag = false;
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

    private void FixedUpdate() {
        
        rb.velocity = new Vector2(vx,rb.velocity.y);

        if(jumpFlag == true){
            rb.AddForce(new Vector2(0,jumpPower),ForceMode2D.Impulse);
            jumpFlag = false;
        }
    
    }

    void OnTriggerStay2D(Collider2D other){
        groundFlag = true;
    }

    void OnTriggerExit2D(Collider2D other){
        groundFlag = false;
    }
}