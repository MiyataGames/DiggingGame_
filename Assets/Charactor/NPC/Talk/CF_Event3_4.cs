using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CF_Event3_4 : CharactorFunction
{
    [SerializeField] private StoryEventScript storyEventScript;
    [SerializeField] private GameObject player_Field;
    [SerializeField] private GameObject player_Story;
    [SerializeField] private GameObject syo_FieldPrefab;
    [SerializeField] private GameObject syo_StoryPrefab;
    [SerializeField] private GameObject sontyo_StoryPrefab;

    [SerializeField] private Transform FieldParent;
    [SerializeField] private Transform StoryParent;

    private GameObject syo;
    Animator syoAnim;
    Animator maoAnim;

    private GameObject sontyo;

    // ショウがスポーンする位置
    [SerializeField] Transform shoFirstTransform;
    // 村長がスポーンする位置
    [SerializeField] Transform sontyoFirstTransform;
    // 移動する位置
    [SerializeField] Transform maoFirstTransform;
    // ２番目に移動する位置
    [SerializeField] float moveSecondY;
    // ３番目に移動する位置
    [SerializeField] float moveThirdY;

    public override void ExecuteCommand(string functionName, string animFuncName)
    {
        if (functionName != null)
        {
            switch (functionName)
            {
                case "SpawnSyoMao_Story":
                    SpawnSyoMao_Story();
                    break;
                case "SyoTurnTowardsMao":
                    SyoTurnTowardsMao();
                    break;
                case "MaoTurnTowardsSyo":
                    MaoTurnTowardsSyo();
                    break;
                case "SyoLookAround":
                    StartCoroutine(SyoLookAround());
                    break;
                case "SyoFindMiningMachine":
                    StartCoroutine(SyoFindMiningMachine());
                    break;
                case "MaoFindMiningMachine":
                    StartCoroutine(MaoFindMiningMachine());
                    break;
                case "SontyoSpawn":
                    StartCoroutine(SontyoSpawn());
                    break;
            }
        }
    }



    /// <summary>
    /// ショウとマオをストーリーシーン上に最初に生成する関数
    /// マオとショウを生成してカメラを移動する
    /// </summary>
    private void SpawnSyoMao_Story()
    {
        syo = SpawnCharactor(syo_StoryPrefab, player_Story.transform.position + new Vector3(3, 0), StoryParent);
        player_Story.transform.position = maoFirstTransform.position;
        // 2秒かけてカメラを真ん中からプレイヤーの位置へ
        storyEventScript.moveFlag = true;
        Camera.main.transform.DOMoveY(player_Story.transform.position.y,2f);
        // しょうとマオが歩く カメラはマオを追従する
        Animator syoAnim = syo.GetComponent<Animator>();
        Animator maoAnim = player_Story.GetComponent<Animator>();
        syoAnim.SetBool("isWalk", true);
        maoAnim.SetBool("isWalk", true);
        Camera.main.transform.DOMoveY(player_Story.transform.position.y - moveSecondY, 2f);
        syo.transform.DOMoveY(syo.transform.position.y - moveSecondY,2f);
        player_Story.transform.DOMoveY(player_Story.transform.position.y - moveSecondY,2f).OnComplete(SpawnCompleteFunc);
    }

    // カメラが移動し終わったら次のメッセージ
    void SpawnCompleteFunc()
    {
        storyEventScript.moveFlag = false;
        syoAnim.SetBool("isWalk", false);
        maoAnim.SetBool("isWalk", false);
        storyEventScript.ReadNextMessage();
    }

    // ショウが主人公の方(左)を向く
    void SyoTurnTowardsMao()
    {
        CharactorChangeVec(syo, "Left");
    }

    // ショウが前を向く
    void SyoTurnTowardsFront()
    {
        CharactorChangeVec(syo, "down");
    }

    // 主人公がショウの方(左)を向く
    void MaoTurnTowardsSyo()
    {
        CharactorChangeVec(player_Story, "Right");
    }

    // 主人公が前を向く
    void MaoTurnTowardsFront()
    {
        CharactorChangeVec(player_Story, "down");
    }
    // ショウが周りをキョロキョロする moveFlagの使い方合ってるかわからん
    IEnumerator SyoLookAround()
    {
        storyEventScript.moveFlag = true;
        // 上
        syoAnim.SetFloat("x", 0);
        syoAnim.SetFloat("y", 1);
        yield return new WaitForSeconds(0.5f);
        // 下
        syoAnim.SetFloat("x", 0);
        syoAnim.SetFloat("y", -1);
        yield return new WaitForSeconds(0.5f);
        // 左
        syoAnim.SetFloat("x", -1);
        syoAnim.SetFloat("y", 0);
        yield return new WaitForSeconds(0.5f);
        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();
    }

    // 歩いて採掘機を見つける
    IEnumerator SyoFindMiningMachine()
    {
        storyEventScript.moveFlag = true;
        CharactorChangeVec(syo, "down");
        // ショウが下へ歩く
        syoAnim.SetBool("isWalk", true);
        syo.transform.DOMoveY(syo.transform.position.y - moveThirdY, 1f);
        // マオが下を向く
        CharactorChangeVec(player_Story, "down");
        yield return new WaitForSeconds(1);
        syoAnim.SetBool("isWalk", false);

        storyEventScript.ReadNextMessage();
    }

    // マオも採掘機に近寄る
    IEnumerator MaoFindMiningMachine()
    {
        storyEventScript.moveFlag = true;
        maoAnim.SetBool("isWalk", true);
        player_Story.transform.DOMoveY(player_Story.transform.position.y - moveThirdY, 1f);
        yield return new WaitForSeconds(1);
        syoAnim.SetBool("isWalk", false);
        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();
    }

    // 村長が奥から歩いてくる
    IEnumerator SontyoSpawn()
    {
        storyEventScript.moveFlag = true;
        sontyo = SpawnCharactor(sontyo_StoryPrefab, sontyoFirstTransform.position, StoryParent);
        Camera.main.transform.DOMove(sontyoFirstTransform.position, 2f);
        yield return new WaitForSeconds(2);
        // 村長が歩いてくる
        sontyo.transform.DOMoveY(player_Story.transform.position.y - 2f, 2f);
        Camera.main.transform.DOMoveY(player_Story.transform.position.y - 2f, 2f);
        yield return new WaitForSeconds(2);
        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();
    }


    /// <summary>
    /// 村へ移動する関数
    /// </summary>
        private void Move2Village()
    {
        SideView2TopDown();
    }

    /// <summary>
    /// 村長が移動する関数
    /// </summary>
    private void SontyoMove()
    {
        storyEventScript.moveFlag = true;

        Animator anim = sontyo.GetComponent<Animator>();
        anim.SetBool("isWalk", true);

        var t = sontyo.transform.position;
        sontyo.transform.DOMove(t - new Vector3(0, 8, 0), 2f)
                        .SetEase(Ease.Linear)
                        .OnComplete(moveCompleteFunc);
    }


    /// <summary>
    /// 移動が完了したら実行する関数
    /// </summary>
    private void moveCompleteFunc()
    {
        Animator anim = sontyo.GetComponent<Animator>();
        anim.SetBool("isWalk", false);
        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();
    }
}
