using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarDetector : MonoBehaviour
{
    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.CompareTag("Finish"))
    //     {
    //         Debug.Log("ついたよ");
    //         collision.gameObject.SetActive(true);
    //     }
    // }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Item")
        {
            Debug.Log("qqqq");
        }
    }

        private void OnTriggerExit2D(Collider2D collision)
        {
        /*
            if (collision.CompareTag("InvisibleItem"))
            {
                         Debug.Log("qqqa");
                collision.gameObject.SetActive(false);
            }
        */
        }
    
}