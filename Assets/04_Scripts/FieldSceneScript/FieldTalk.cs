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
    RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        //defaultPos = fieldTalkDialog.transform.position;
        // defaultPos.x = Screen.width + 250;
        rectTransform = fieldTalkDialog.GetComponent<RectTransform>();
        defaultPos.x = GameManager.instance.targetResolution.x /2;
        defaultPos.y = GameManager.instance.targetResolution.y / 4;
        Debug.Log("Screen : " + Screen.width);
        //fieldTalkDialog.transform.position = defaultPos;
        rectTransform.anchoredPosition = defaultPos;


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
        sequence.Append(rectTransform.DOAnchorPos(new Vector2(defaultPos.x - fadeDistance,rectTransform.anchoredPosition.y), 1))
        .AppendInterval(3)
        .Append(rectTransform.DOAnchorPos(defaultPos, 1))
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
