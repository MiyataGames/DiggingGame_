using UnityEngine;

public class ActivateOnSonar : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finish")) // タグ名をチェック
        {
            gameObject.SetActive(true); // このオブジェクトをアクティベート
            Debug.Log("アssssクティベートしました");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Finish")) // タグ名をチェック
        {
            gameObject.SetActive(false); // このオブジェクトを非アクティベート
            Debug.Log("非アクティベートしましたアssssクティベートしまし");
        }
    }
}