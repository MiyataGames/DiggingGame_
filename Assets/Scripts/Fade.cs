using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [Tooltip("�t�F�[�h������UI")]
    [SerializeField]
    private Image _fadePanel = default;
    [Tooltip("���s����")]
    [SerializeField]
    private float _fadeTime = 1f;

    private Action[] _onCompleteFadeIn = new Action[0];
    private Action[] _onCompleteFadeOut = new Action[0];

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

    public void StartFadeIn() { StartCoroutine(FadeIn()); }

    public void StartFadeOut() { StartCoroutine(FadeOut()); }

    private IEnumerator FadeIn()
    {
        _fadePanel.gameObject.SetActive(true);

        //���l(�����x)�� 1 -> 0 �ɂ���(���������邭����)
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
        foreach (var action in _onCompleteFadeIn) { action?.Invoke(); }
    }

    private IEnumerator FadeOut()
    {
        _fadePanel.gameObject.SetActive(true);

        //���l(�����x)�� 0 -> 1 �ɂ���(�������Â�����)
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

        foreach (var action in _onCompleteFadeOut) { action?.Invoke(); }
    }

    /// <summary> �t�F�[�h�C�����s��̏�����o�^�i�㏑���j���� </summary>
    public void RegisterFadeInEvent(Action[] actions) { _onCompleteFadeIn = actions; }

    /// <summary> �t�F�[�h�A�E�g���s��̏�����o�^�i�㏑���j���� </summary>
    public void RegisterFadeOutEvent(Action[] actions) { _onCompleteFadeOut = actions; }
}