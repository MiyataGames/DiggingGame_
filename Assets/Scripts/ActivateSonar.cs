using UnityEngine;
using DG.Tweening; //DOTWEENをインポート

public class ActivateSonar : MonoBehaviour
{
    public GameObject targetGameObject;
    private Tween scaleTween; //tween型の変数scaletween
    

    void Update()
    {
        // 左Shiftか右Shiftのいずれかが押されたとき
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            if (targetGameObject != null)
            {
                targetGameObject.SetActive(true); // ゲームオブジェクトをアクティブにする
                scaleTween = targetGameObject.transform.DOScale(new Vector3(2f, 2f, 2f), 3f); // スケールを0から2にアニメーションする
            }
        }

        // 左Shiftか右Shiftのいずれかを離したとき
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            if (targetGameObject != null)
            {
                if (scaleTween != null && scaleTween.IsActive())
                {
                    scaleTween.Kill(); // 現在のスケールアニメーションを停止
                }

                // スケールを徐々に0にアニメーションする
                // DOScale(Vector3.zero, 2f) 2秒後に元の大きさに戻す。
                // OnComplete コールバック関数　ある関数が実行された後に呼び出される関数を無視する
                targetGameObject.transform.DOScale(Vector3.zero, 2f).OnComplete(() => {
                    targetGameObject.SetActive(false); // アニメーション完了後、ゲームオブジェクトを非アクティブにする
                });
            }
        }
    }
}