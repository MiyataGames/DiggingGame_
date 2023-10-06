using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class soil : MonoBehaviour
{
    [SerializeField] Tilemap soilTilemap;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("hit");
        if(other.gameObject.tag == "Player"){
            Debug.Log("scop");
            Vector2 hitPos = other.ClosestPoint(transform.position);
            Vector3Int tilePos = soilTilemap.WorldToCell(hitPos);
            
            soilTilemap.SetTile(tilePos,null);
            Debug.Log(tilePos);
        }
    }*/

    private void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("hit");
        if(other.gameObject.tag == "Player"){
            foreach (ContactPoint2D point in other.contacts) {
                //衝突位置
                Vector2 hitPos = (Vector2)point.point;
                var tilePos = soilTilemap.WorldToCell(hitPos);
                Debug.Log("dig: " + tilePos);
                soilTilemap.SetTile(tilePos,null);
            }
        }    
        
    }
}
