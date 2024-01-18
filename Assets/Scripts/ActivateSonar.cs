using UnityEngine;
using DG.Tweening; // DOTWEENをインポート
public class ActivateSonar : MonoBehaviour
{
    public GameObject sonar;
    private Tween scaleTween; // Tween型の変数scaleTween
    [SerializeField] Vector3 sarchScale = new Vector3(5f, 5f, 5f);
    [SerializeField] float scaleTime = 3;
    [SerializeField] float shrinkTime = 2;

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
                scaleTween = sonar.transform.DOScale(sarchScale, scaleTime).From(currentScale);
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
                scaleTween = sonar.transform.DOScale(Vector3.zero, shrinkTime).From(currentScale).OnComplete(() => {
                    sonar.SetActive(false); // アニメーション完了後、ゲームオブジェクトを非アクティブにする
                });
            }
        }
    }
}