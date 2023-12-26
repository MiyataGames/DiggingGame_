using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
// using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DigController : MonoBehaviour
{
    
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator playerAnimController;
    [SerializeField] private GameObject digCollider; 
    private bool isDigging = false;
    private bool isPlayerLeft = false; //プレイヤーが左を向いているか

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isDigging == false){
            if(Input.GetMouseButtonDown(0)){
                startDig();
            }
        }
    }

    // 掘るアニメーション終了時にアニメーション側から呼び出し
    public void endDiggingAnim(){
        endDig();
    }

    void startDig(){
        isDigging = true;
        CapsuleCollider2D dc = digCollider.GetComponent<CapsuleCollider2D>();

        //プレイヤーの向きのフラグを決定
        if(playerController.isLeft == true){
            isPlayerLeft = true;
        }else{
            isPlayerLeft = false;
        }

        if(Input.GetKey(KeyCode.W)){
            dc.offset = new Vector2(0.0f, 0.68f); 
            dc.size = new Vector2(0.76f, 0.45f); 
            dc.direction = CapsuleDirection2D.Horizontal;
        }else if(Input.GetKey(KeyCode.S)){
            dc.offset = new Vector2(0.0f, -0.85f); 
            dc.size = new Vector2(0.76f, 0.45f); 
            dc.direction = CapsuleDirection2D.Horizontal;
            playerAnimController.SetBool("isUnderDigging",true);
        }else if(isPlayerLeft == true){
            dc.offset = new Vector2(-0.47f, -0.06f); 
            dc.size = new Vector2(0.45f, 0.76f);    
            dc.direction = CapsuleDirection2D.Vertical;
            playerAnimController.SetBool("isDigging",true);
        }else if(isPlayerLeft == false){
            dc.offset = new Vector2(0.47f, -0.06f); 
            dc.size = new Vector2(0.45f, 0.76f);    
            dc.direction = CapsuleDirection2D.Vertical;
            playerAnimController.SetBool("isDigging",true);
        }

        digCollider.SetActive(true);
    }

    void endDig(){
        isDigging = false; 
        digCollider.SetActive(false);
        playerAnimController.SetBool("isDigging",false);
        playerAnimController.SetBool("isUnderDigging",false);
    }
}
