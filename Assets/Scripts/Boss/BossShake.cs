using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class BossShake : MonoBehaviour
{
    Transform storySceneTransform;

    public void SetStoryScene(Transform transform)
    {
        storySceneTransform = transform;
    }

    // 画面を揺らす関数
    public void ShakeCamera()
    {
        // 1秒間、強度0.5でカメラを揺らす
        storySceneTransform.DOShakePosition(1f, 0.5f);
    }
}

