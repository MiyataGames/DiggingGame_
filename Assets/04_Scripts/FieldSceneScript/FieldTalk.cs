using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FieldTalk : MonoBehaviour
{
    public int num;

    [SerializeField] GameObject fieldTalkDialog;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] Image charaImage;
    [SerializeField] private float fadeDistance = 120;

    private bool isDisplay;
    private bool isFadeIn = false;
    private Sprite image;
    private Vector2 defaultPos;
    private float defaultWidth;

    // Start is called before the first frame update
    void Awake()
    {
        defaultPos = fieldTalkDialog.transform.position;
        defaultPos.x = Screen.width + 250;
        Debug.Log("Screen : " + Screen.width);
        fieldTalkDialog.transform.position = defaultPos; 
    }

    public void DisplayFieldText(string message, string imagePath)
    {
        if (isDisplay == false)
        {
            isDisplay = true;
            messageText.text = message;
            print(imagePath);
            image = Resources.Load<Sprite>(imagePath);
            charaImage.sprite = image;
            FadeInDialog();
        }
    }

    void FadeInDialog()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(fieldTalkDialog.transform.DOMoveX(defaultPos.x - fadeDistance, 1))
        .AppendInterval(3)
        .Append(fieldTalkDialog.transform.DOMoveX(defaultPos.x, 1))
        .AppendCallback(() =>
        {
            isDisplay = false;
        });
    }

    public void Test()
    {
        DisplayFieldText("ずいぶん奥まできたなあ", "CharaImage/Player/1");
    }
}
