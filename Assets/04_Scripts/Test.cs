using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Test : MonoBehaviour
{
    public GameObject testUI;
    public GameObject testImage;
    public GameObject canvas;
    public RectTransform testUIPos;
    [SerializeField] private float fadeDistance = 120;
    private Vector3 defaultPos;
    private bool isDisplay;
    void Awake()
    {
        defaultPos = testUI.GetComponent<RectTransform>().position;
        testUIPos = testUI.GetComponent<RectTransform>();
    }

    private void Start()
    {
        StartCoroutine(FadeInDialog());
    }

    IEnumerator FadeInDialog()
    {
        Vector3 newPosition = new Vector3(defaultPos.x - fadeDistance, testUIPos.position.y,testUIPos.position.z);
        testUIPos.position = newPosition;
        Debug.Log(newPosition);
        yield return new WaitForSeconds(3);
        testUIPos.position = defaultPos;
        Debug.Log(defaultPos);
        Instantiate(testImage, defaultPos, Quaternion.identity,canvas.transform);

        /*
        var sequence = DOTween.Sequence();
        sequence.Append(testUI.transform.DOMoveX(defaultPos.x - fadeDistance, 1))
        .AppendInterval(3)
        .Append(testUI.transform.DOMoveX(defaultPos.x, 1))
        .AppendCallback(() =>
        {
            isDisplay = false;
        });*/
    }
}
