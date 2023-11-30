using UnityEngine;
using DG.Tweening; //DOTWEENをインポート

public class ActivateSonar : MonoBehaviour
{
    public GameObject sonar;
    private Tween scaleTween; //tween型の変数scaletween
    

    void Update()
    {
        // 左Shiftか右Shiftのいずれかが押されたとき <-「押下し時」じゃなくて「押してる最中」にする
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            // キーを押してる時にソナーの大きさが最大じゃなかったら
                // ソナーを最大まで大きくする

            // 
            //if (sonar != null)
            //{
            sonar.SetActive(true); // ゲームオブジェクトをアクティブにする
                scaleTween = sonar.transform.DOScale(new Vector3(2f, 2f, 2f), 1.5f).SetLoops(1, LoopType.Yoyo); ; // スケールを0から2にアニメーションする
               
            // sonar.transform.DOScale(Vector3.zero, 2f);
            //}
        }

        // 左Shiftか右Shiftのいずれかを離したとき
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            // キーを離した時にソナーの大きさが0じゃなかったら
                // ソナーを0まで小さくする
            //if (sonar != null)
            //{
            /*
                if (scaleTween != null && scaleTween.IsActive())
                {
                    scaleTween.Kill(); // 現在のスケールアニメーションを停止
                }

                // スケールを徐々に0にアニメーションする
                // DOScale(Vector3.zero, 2f) 2秒後に元の大きさに戻す。
                // OnComplete コールバック関数　ある関数が実行された後に呼び出される関数を無視する
                sonar.transform.DOScale(Vector3.zero, 2f).OnComplete(() => {
                    sonar.SetActive(false); // アニメーション完了後、ゲームオブジェクトを非アクティブにする
                });
            */
            //}
        }
    }
}