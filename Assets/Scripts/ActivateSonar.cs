using UnityEngine;
using DG.Tweening; // DOTWEENをインポート
public class ActivateSonar : MonoBehaviour
{
    public GameObject sonar;
    private Tween scaleTween; // Tween型の変数scaleTween
    void Update()
    {
        // 左Shiftか右Shiftのいずれかが押されたとき
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            if (sonar != null)
            {
                sonar.SetActive(true); // ゲームオブジェクトをアクティブにする
                // 既存のTweenがあればキャンセルする
                if (scaleTween != null && scaleTween.IsActive())
                {
                    scaleTween.Kill();
                }
                // 現在のスケールから2へアニメーションする
                Vector3 currentScale = sonar.transform.localScale;
                scaleTween = sonar.transform.DOScale(new Vector3(2f, 2f, 2f), 3f).From(currentScale);
            }
        }
        // 左Shiftか右Shiftのいずれかを離したとき
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            if (sonar != null)
            {
                if (scaleTween != null && scaleTween.IsActive())
                {
                    scaleTween.Kill(); // 現在のスケールアニメーションを停止
                }
                // 現在のスケールから0へアニメーションする
                Vector3 currentScale = sonar.transform.localScale;
                scaleTween = sonar.transform.DOScale(Vector3.zero, 2f).From(currentScale).OnComplete(() => {
                    sonar.SetActive(false); // アニメーション完了後、ゲームオブジェクトを非アクティブにする
                });
            }
        }
    }
}