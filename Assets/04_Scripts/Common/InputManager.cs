using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] GameController gameController;
    [SerializeField] GameObject inputUIs;
    [SerializeField] GameObject androidFieldUIs;

    public GameController GameController { get => gameController;}

    // ストーリーが始まった時操作UIを消す
    public void InitStory()
    {
        androidFieldUIs.SetActive(false);
        inputUIs.SetActive(false);
    }
    public void InitBattle()
    {
        inputUIs.SetActive(false);
        androidFieldUIs.SetActive(false);
    }
    // フィールドシーンで操作できるようになったら
    public void EnableInputField()
    {
        inputUIs.SetActive(true);
        androidFieldUIs.SetActive(true);
    }
    // ストーリーシーンで操作できるようになったら
    public void EnableInputStory()
    {
        androidFieldUIs.SetActive(false);
        inputUIs.SetActive(true);
    }
}
