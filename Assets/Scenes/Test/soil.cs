using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class soil : MonoBehaviour
{
    [SerializeField] Tilemap soilTilemap;//土のタイルマップ
    [SerializeField] Collider2D soilCollider;
    [SerializeField] GameObject hitObj;
    [SerializeField] GameObject scop;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //OnTriggerでの穴掘りは衝突点を一度に一つしか取れないので廃止
    /*private void OnTriggerStay2D(Collider2D other) {
        Debug.Log("hit");
        if(other.gameObject.tag == "scop"){
            Debug.Log("scop");

            Vector2 hitPos = soilCollider.ClosestPoint(other.transform.position) + new Vector2(0.3f,0.0f);
            Vector3Int tilePos = soilTilemap.WorldToCell(hitPos);
            
            Instantiate(hitObj,hitPos,Quaternion.identity);
            soilTilemap.SetTile(tilePos,null);
            Debug.Log(tilePos);
        }
    }*/

    private void OnCollisionStay2D(Collision2D other)
    {
        //Debug.Log("hit");
        if (other.gameObject.tag == "scop")//スコップタグがついているか
        {
            foreach (ContactPoint2D point in other.contacts)//取得したすべての衝突オブジェクトに対して
            {
                //衝突位置
                Vector2 hitPos = (Vector2)point.point;
                var tilePos = soilTilemap.WorldToCell(hitPos);//衝突位置をタイルマップ座標に変換
                //Debug.Log("dig: " + tilePos);
                //Instantiate(hitObj,hitPos,Quaternion.identity);//衝突点のマーカー
                soilTilemap.SetTile(tilePos, null);//タイルを消す
            }
        }

    }
}
