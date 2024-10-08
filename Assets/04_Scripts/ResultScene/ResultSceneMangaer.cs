using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ResultState
{
    RESULT_LEVELUP_ANIM,
    RESULT_LEVELUP_END,
    RESULT_DROP,
    END
}

public class ResultSceneMangaer : MonoBehaviour
{
    private ResultState resultState = ResultState.RESULT_LEVELUP_ANIM;
    [SerializeField] private Transform winPlayerPos;

    //[SerializeField] private Camera resultSceneCamera;
    [SerializeField] private GameObject resultPanel;

    [SerializeField] private GameObject resultAreaPanel;
    [SerializeField] private GameObject resultDropPanel;
    [SerializeField] private GameObject dropItemAreaPanel;
    [SerializeField] private GameObject dropItemTextPrefab;
    [SerializeField] private GameObject dropGoldText;
    private static readonly int hashWin = Animator.StringToHash("Base Layer.Win");
    private GameObject playerModel;

    private void Update()
    {
        if (resultState == ResultState.RESULT_LEVELUP_END)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                resultState = ResultState.RESULT_DROP;
                // 経験値リザルトをオフ
                resultAreaPanel.SetActive(false);
                // ドロップリザルトをオン
                resultDropPanel.SetActive(true);
                // 必要な情報を書き込む
                ShowDropObjectsText();
            }
        }
        else if (resultState == ResultState.RESULT_DROP)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                DeleteDropObjectTexts();
                GameManager.instance.CurrentSceneIndex = (int)GameMode.FIELD_SCENE;
                // リザルトの子オブジェクトを削除
                //Destroy(playerModel);
                //resultPanel.SetActive(false);
                // 経験値リザルトをオン
                resultAreaPanel.SetActive(true);
            }
        }
        /*
        if (resultState == ResultState.END)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                gameManager.CurrentSceneIndex = (int)GameMode.FIELD_SCENE;
                // リザルトの子オブジェクトを削除
                Destroy(playerModel);
                resultPanel.SetActive(false);
            }
        }*/
    }

    public IEnumerator ResultPlayer(Player player)
    {
        // リザルトをオン
        //playerModel = Instantiate(player.PlayerBase.PlayerModel, winPlayerPos.position, Quaternion.identity);
        // Result????????????
        resultState = ResultState.RESULT_LEVELUP_ANIM;
        IEnumerator enumerator = GameManager.instance.UpdateExpAnimation();
        yield return enumerator;
        // ????????????????
        //player.PlayerAnimator = playerModel.GetComponent<Animator>();
        //player.PlayerAnimator.Play(hashWin);
        yield return null;
        //yield return new WaitForAnimation(player.PlayerAnimator, 0);
        resultState = ResultState.RESULT_LEVELUP_END;
    }

    private void ShowDropObjectsText()
    {
        // ドロップアイテムを表示
        resultDropPanel.SetActive(true);
        DropObjectsStruct dropObjects = GameManager.instance.DropObjects;
        // テキストを表示
        dropGoldText.GetComponent<TextMeshProUGUI>().text = dropObjects.totalDropGold.ToString();
        for (int i = 0; i < dropObjects.dropItems.Count; i++)
        {
            TextMeshProUGUI text = Instantiate(dropItemTextPrefab, dropItemAreaPanel.transform).GetComponent<TextMeshProUGUI>();
            text.text = dropObjects.dropItems[i].ItemBase.ItemName.ToString();
        }
    }

    private void DeleteDropObjectTexts()
    {
        // 子オブジェクトの数を取得
        int childCount = dropItemAreaPanel.transform.childCount;
        // 子オブジェクトを順に取得する
        for (int i = 0; i < childCount; i++)
        {
            Transform childTransform = dropItemAreaPanel.transform.GetChild(i);
            GameObject childObject = childTransform.gameObject;
            Destroy(childObject);
        }
        // ドロップアイテムを非表示
        resultDropPanel.SetActive(false);
    }
}