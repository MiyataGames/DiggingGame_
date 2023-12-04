using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StartFieldTalk : MonoBehaviour
{
    [SerializeField] FieldTalk fieldTalk;
    [SerializeField] string message;
    [SerializeField] string imagePath;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            fieldTalk.DisplayFieldText(message, imagePath);
        }
    }
}
