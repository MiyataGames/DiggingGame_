using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField]
    private Image _fadePanel = default;

    [SerializeField]
    private float _fadeTime = 1f;

    // public delegate void FadeInCompleteHandler();
    // public event FadeInCompleteHandler OnFadeInComplete;

    public static Fade Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // if (_fadePanel)
        // {
        //     _fadeTime = 0.5f;
        //     RegisterFadeInEvent(new Action[] { () => _fadeTime = 1f });
        //     StartFadeIn();
        // }
    }

    public void StartFadeInBattle(GameObject enemySymbol)
    { StartCoroutine(FadeInBattle(enemySymbol)); }

    public void StartFadeOut()
    { StartCoroutine(FadeOut()); }

    private IEnumerator FadeInBattle(GameObject gameObject)
    {
        _fadePanel.gameObject.SetActive(true);

        GameManager.instance.StartBattle(gameObject, 0);
        float alpha = 1f;
        Color color = _fadePanel.color;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * (1f / _fadeTime);

            if (alpha <= 0f) { alpha = 0f; }

            color.a = alpha;
            _fadePanel.color = color;
            yield return null;
        }

        _fadePanel.gameObject.SetActive(false);
    }

    private IEnumerator FadeOut()
    {
        _fadePanel.gameObject.SetActive(true);

        float alpha = 0f;
        Color color = _fadePanel.color;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * (1f / _fadeTime);

            if (alpha >= 1f) { alpha = 1f; }

            color.a = alpha;
            _fadePanel.color = color;
            yield return null;
        }
    }
}