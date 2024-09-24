using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialog : MonoBehaviour
{
    [SerializeField] GameObject messageField;
    [SerializeField]
    TextMeshProUGUI messageText;// 表示するテキスト
    [SerializeField] float waitTime;

    public void ShowMessageCoroutine(string message)
    {
        messageText.text = message;
        messageField.SetActive(true);
        StartCoroutine(ShowMessage());
    }

    private IEnumerator ShowMessage()
    {
        yield return new WaitForSeconds(waitTime);
        messageText.text = "";
        messageField.SetActive(false);
    }
}
