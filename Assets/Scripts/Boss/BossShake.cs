using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class BossShake : MonoBehaviour
{
    Transform storySceneTransform;

    private AudioSource audioSource;
    private AudioClip footSoundClip;



    public void SetStoryScene(Transform transform)
    {
       
        storySceneTransform = transform; // または適切な Transform を設定

        // AudioSource コンポーネントを追加し、設定
        audioSource = gameObject.AddComponent<AudioSource>();

        // footSound 音楽ファイルをロード
        footSoundClip = Resources.Load<AudioClip>("SE/Monster/footSound");
        audioSource.clip = footSoundClip;
    }

    // 画面を揺らす関数
    public void ShakeCamera()
    {
        // 1秒間、強度0.5でカメラを揺らす
        Debug.Log(audioSource);
        audioSource.Play();
        Debug.Log("dddddda");
        storySceneTransform.DOShakePosition(1f, 0.8f);

    }
}

