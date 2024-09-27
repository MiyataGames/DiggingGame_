using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject upButton;
    [SerializeField] GameObject leftButton;
    [SerializeField] GameObject rightButton;
    [SerializeField] GameObject downButton;

    private float horizonal;
    private float vertical;

    public float Horizonal { get => horizonal; }
    public float Vertical { get => vertical; }

    // Start is called before the first frame update
    void Awake()
    {
        // UpButtonに処理を追加
        EventTrigger upEventTrigger = upButton.AddComponent<EventTrigger>();
        upEventTrigger.triggers = new List<EventTrigger.Entry>();

        // ボタンが押されたとき
        EventTrigger.Entry tapUpEntry = new EventTrigger.Entry();
        tapUpEntry.eventID = EventTriggerType.PointerDown;
        tapUpEntry.callback.AddListener((x) => OnTapUpButton());

        upEventTrigger.triggers.Add(tapUpEntry);
        // ボタンが離されたとき
        EventTrigger.Entry releaseEntry = new EventTrigger.Entry();
        releaseEntry.eventID = EventTriggerType.PointerUp;
        releaseEntry.callback.AddListener((x) => OnReleaseButton());

        upEventTrigger.triggers.Add(releaseEntry);

        // downButtonに処理を追加
        EventTrigger downEventTrigger = downButton.AddComponent<EventTrigger>();
        downEventTrigger.triggers = new List<EventTrigger.Entry>();

        // ボタンが押されたとき
        EventTrigger.Entry tapDownEntry = new EventTrigger.Entry();
        tapDownEntry.eventID = EventTriggerType.PointerDown;
        tapDownEntry.callback.AddListener((x) => OnTapDownButton());

        downEventTrigger.triggers.Add(tapDownEntry);
        // ボタンが離されたとき
        downEventTrigger.triggers.Add(releaseEntry);

        // leftButtonに処理を追加
        EventTrigger leftEventTrigger = leftButton.AddComponent<EventTrigger>();
        leftEventTrigger.triggers = new List<EventTrigger.Entry>();

        // ボタンが押されたとき
        EventTrigger.Entry tapLeftEntry = new EventTrigger.Entry();
        tapLeftEntry.eventID = EventTriggerType.PointerDown;
        tapLeftEntry.callback.AddListener((x) => OnTapLeftButton());

        leftEventTrigger.triggers.Add(tapLeftEntry);
        // ボタンが離されたとき
        leftEventTrigger.triggers.Add(releaseEntry);


        // rightButtonに処理を追加
        EventTrigger rightEventTrigger = rightButton.AddComponent<EventTrigger>();
        rightEventTrigger.triggers = new List<EventTrigger.Entry>();

        // ボタンが押されたとき
        EventTrigger.Entry tapRightEntry = new EventTrigger.Entry();
        tapRightEntry.eventID = EventTriggerType.PointerDown;
        tapRightEntry.callback.AddListener((x) => OnTapRightButton());

        rightEventTrigger.triggers.Add(tapRightEntry);
        // ボタンが離されたとき
        rightEventTrigger.triggers.Add(releaseEntry);
    }

    void OnTapUpButton()
    {
        vertical = 1;
    }
    void OnReleaseButton()
    {
        vertical = 0;
        horizonal = 0;
    }
    void OnTapDownButton()
    {
        vertical = -1;
    }
    void OnTapLeftButton()
    {
        horizonal = -1;
    }
    void OnTapRightButton()
    {
        horizonal = 1;
    }
}
