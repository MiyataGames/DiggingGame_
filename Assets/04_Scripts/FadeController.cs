using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    [SerializeField] private  Image fadePanel;
    [SerializeField] private float fadeDuration = 1.0f;

    public event Action OnFadeInComplete;
    public event Action OnFadeWaitComplete;
    public event Action OnFadeOutComplete;

    private void Start()
    {
        // 初期状態は完全に透明に設定
        fadePanel.color = new Color(0, 0, 0, 0);
        //Debug.Log(OnFadeInComplete);

    }

    public void StartFadeOutIn()
    {
        StartCoroutine(FadeOutIn());
    }

    private IEnumerator FadeOutIn()
    {
        Debug.Log("フェードOutIn開始");
        fadePanel.color = new Color(0, 0, 0, 1);
        Debug.Log("パネルを黒くした");
        yield return new WaitForSeconds(1);
        Debug.Log("1秒まった");
        StartCoroutine(Fade(0));
        Debug.Log("フェードOutIn終了");

    }

    public void FadeIn()
    {
        StartCoroutine(Fade(0));
    }

    public void FadeWait(){
        StartCoroutine(WaitForSeconds(1));
    }

    public void FadeOut()
    {
        StartCoroutine(Fade(1));
    }

    //フェードコルーチン
    private IEnumerator Fade(float targetAlpha)
    {
        float alpha = fadePanel.color.a;
        Debug.Log("フェードかいし" + alpha);

        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float newAlpha = Mathf.Lerp(alpha, targetAlpha, time / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, newAlpha);
            //Debug.Log(fadePanel.color);
            yield return null;
        }

        fadePanel.color = new Color(0, 0, 0, targetAlpha);

        // コールバックの発火
        if (targetAlpha == 1)
        {
            OnFadeOutComplete?.Invoke();
        }
        else if (targetAlpha == 0)
        {
            OnFadeInComplete?.Invoke();
        }

    }

    //任意の秒数待機
    public IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        OnFadeWaitComplete?.Invoke();
    }

}
