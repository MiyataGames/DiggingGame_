using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResultState
{
    RESULT_LEVELUP,
    RESULT_DROP,
    END
}

public class ResultSceneMangaer : MonoBehaviour
{
    private ResultState resultState = ResultState.RESULT_LEVELUP;
    [SerializeField] private Transform winPlayerPos;
    //[SerializeField] private Camera resultSceneCamera;
    [SerializeField] private GameObject resultPanel;
    private static readonly int hashWin = Animator.StringToHash("Base Layer.Win");
    [SerializeField] private GameManager gameManager;
    private GameObject playerModel;

    private void Update()
    {
        if (resultState == ResultState.END)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                gameManager.CurrentSceneIndex = (int)GameMode.FIELD_SCENE;
                // リザルトの子オブジェクトを削除
                Destroy(playerModel);
                resultPanel.SetActive(false);
            }
        }
    }

    public IEnumerator ResultPlayer(Player player)
    {
        //playerModel = Instantiate(player.PlayerBase.PlayerModel, winPlayerPos.position, Quaternion.identity);
        // Result????????????
        resultPanel.SetActive(true);
        resultState = ResultState.RESULT_LEVELUP;
        IEnumerator enumerator = gameManager.UpdateExpAnimation();
        yield return enumerator;
        // ????????????????
        //player.PlayerAnimator = playerModel.GetComponent<Animator>();
        //player.PlayerAnimator.Play(hashWin);
        yield return null;
        //yield return new WaitForAnimation(player.PlayerAnimator, 0);
        resultState = ResultState.END;
    }
}