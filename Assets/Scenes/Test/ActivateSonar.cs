using UnityEngine;


// シフトボタンを押したらソナーが使えるスクリプト
public class ActivateSonar : MonoBehaviour
{
    public GameObject targetGameObject; // アクティブにする対象のゲームオブジェクト

    void Update()
    {
        // 左Shiftか右Shiftのいずれかが押されたとき
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            if (targetGameObject != null) // 対象のゲームオブジェクトが設定されているかチェック
            {
                targetGameObject.SetActive(true); // ゲームオブジェクトをアクティブにする
            }
            else
            {
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            if (targetGameObject != null) // 対象のゲームオブジェクトが設定されているかチェック
            {
                targetGameObject.SetActive(false); // ゲームオブジェクトを非アクティブにする
            }
            else
            {
            }
        }
    }
}