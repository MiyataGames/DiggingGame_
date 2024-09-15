using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StartFieldTalk : MonoBehaviour
{
    [SerializeField] FieldTalk fieldTalk;
    [SerializeField] string message;
    [SerializeField] string imagePath;

    bool isTalked;// 一度表示されたかどうか

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "FieldPlayer" || other.gameObject.tag == "scop")
        {
            if(isTalked == true)
            {
                Destroy(this.gameObject);
                return;
            }
            fieldTalk.DisplayFieldText(message, imagePath);
            isTalked = true;
            Debug.Log("アイテム検出");
        }
    }

}
