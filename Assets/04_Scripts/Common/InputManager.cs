using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;


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
    [SerializeField] GameObject androidFieldUIs;
    [SerializeField] Button menuButton;

    public GameController GameController { get => gameController;}

    // メニュー画面になったら
    public void OnClickMenu()
    {
        menuButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "戻る";
        gameController.gameObject.SetActive(false);
        androidFieldUIs.SetActive(false);
    }
    // メニュー画面から戻る時
    public void ReturnFromMenu()
    {
        menuButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "メニュー";
        gameController.gameObject.SetActive(true);
        androidFieldUIs.SetActive(true);
    }

    // ストーリーが始まった時操作UIを消す
    public void InitStory()
    {
        androidFieldUIs.SetActive(false);
        gameController.gameObject.SetActive(false);
    }
    public void InitBattle()
    {
        gameController.gameObject.SetActive(false);
        androidFieldUIs.SetActive(false);
    }
    // フィールドシーンで操作できるようになったら
    public void EnableInputField()
    {
        gameController.gameObject.SetActive(true);
        androidFieldUIs.SetActive(true);
    }
    // ストーリーシーンで操作できるようになったら
    public void EnableInputStory()
    {
        androidFieldUIs.SetActive(false);
        gameController.gameObject.SetActive(true);
    }
}
