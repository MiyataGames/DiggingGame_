using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Experimental.VFX;
using UnityEngine.VFX;

public class soil : MonoBehaviour
{
    [SerializeField] Tilemap soilTilemap;//土のタイルマップ
    [SerializeField] Collider2D soilCollider;
    //[SerializeField] GameObject hitObj;
    //[SerializeField] GameObject scop;

    //[SerializeField] VisualEffect soilBreakEffect;
    [SerializeField] GameObject soilBreakEffect;
    [SerializeField] AudioSource seAudioSource;
    [SerializeField] AudioClip diggingSE;


    //private List<VisualEffect> soilBreakEffects = new List<VisualEffect>();
    //private List<Vector3> soilBreakEffectsPos = new List<Vector3>();

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

    //ContactPoint2Dを使って消すタイルの座標を求める場合
    /*private void OnCollisionStay2D(Collision2D other)
    {
        //        Debug.Log("hit");


        foreach (ContactPoint2D point in other.contacts)//取得したすべての衝突オブジェクトに対して
        {
            if (other.gameObject.tag == "scop")
            {

                //衝突位置
                Vector2 hitPos = (Vector2)point.point;
                var tilePos = soilTilemap.WorldToCell(hitPos);//衝突位置をタイルマップ座標に変換
                //Debug.Log("dig: " + tilePos);
                //Instantiate(hitObj,hitPos,Quaternion.identity);//衝突点のマーカー
                soilTilemap.SetTile(tilePos, null);//タイルを消す
            }
        }

    }*/

    //バウンディングボックスを使う場合
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "scop")
        {
            // BoxCollider2Dのバウンディングボックスを取得
            CapsuleCollider2D capsuleCollider = collision.collider as CapsuleCollider2D;
            if (capsuleCollider != null)
            {
                Bounds bounds = capsuleCollider.bounds;

                // バウンディングボックスの範囲内のタイルを削除
                DeleteTilesInBounds(bounds);


            }
        }
    }

    private void DeleteTilesInBounds(Bounds bounds)
    {
        Vector3Int min = soilTilemap.WorldToCell(bounds.min);
        Vector3Int max = soilTilemap.WorldToCell(bounds.max);

        float brokenTile = 0;

        /*for (int i = 0; i < soilBreakEffects.Count; i++)
        {
            Destroy(soilBreakEffects[i]);
            soilBreakEffects.Remove(soilBreakEffects[i]);
        }*/

        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                if (soilTilemap.HasTile(cellPosition))
                {
                    if (seAudioSource.isPlaying == false)
                    {
                        seAudioSource.PlayOneShot(diggingSE);
                    }
                    soilTilemap.SetTile(cellPosition, null);
                    Instantiate(soilBreakEffect, soilTilemap.CellToWorld(cellPosition), Quaternion.identity);
                    /*soilBreakEffects.Add(Instantiate(soilBreakEffect, new Vector3(0, 0, 0), Quaternion.identity));
                    soilBreakEffectsPos.Add(soilTilemap.CellToWorld(cellPosition));
                    brokenTile++;*/

                }
            }
        }

        /*for (int i = 0; i < brokenTile; i++)
        {

            soilBreakEffects[i].SetVector3("Position", soilBreakEffectsPos[i]);
            soilBreakEffects[i].SendEvent("OnPlay");

        }*/

    }

}
