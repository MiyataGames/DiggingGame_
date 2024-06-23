using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CF_Event1 : CharactorFunction
{
    [SerializeField] private StoryEventScript storyEventScript;
    [SerializeField] private GameObject player_Field;
    [SerializeField] private GameObject player_Story;
    [SerializeField] private GameObject syo_FieldPrefab;
    [SerializeField] private GameObject syo_StoryPrefab;
    [SerializeField] private GameObject sontyo_StoryPrefab;
    [SerializeField] private GameObject ShouTile;

    [SerializeField] private Transform FieldParent;
    [SerializeField] private Transform StoryParent;

    private GameObject syo;

    private GameObject sontyo;

    [SerializeField] float cameraOffsetY;
    [SerializeField] float cameraOffsetZ;

    [SerializeField] Transform leftPoint;

    public override void ExecuteCommand(string functionName, string animFuncName)
    {
        if (functionName != null)
        {
            switch (functionName)
            {
                case "AdjustCamera":
                    AdjustCamera();
                    break;
                case "SpawnSyo_Field":
                    SpawanSyo_Filed();
                    break;
                case "SpawnSyo_Story":
                    SpawnSyo_Story();
                    break;
                case "SpawnSontyo_Story":
                    SpawnSontyo_Story();
                    break;
                case "Move2Village":
                    Move2Village();
                    break;
                case "SontyoMove":
                    SontyoMove();
                    break;
                case "StartEvent":
                    StartEvent();
                    break;
                case "CharaLeft":
                    StartCoroutine(CharaLeft());
                    break;
                case "EndEvent":
                    EndEvent();
                    break;
            }
        }

        if (animFuncName != null)
        {
            switch (animFuncName)
            {
                case "VecUp_Syo":
                    CharactorChangeVec(syo, "Up");
                    break;
            }
        }
    }

    // カメラの位置を調整する
    void AdjustCamera()
    {
        // カメラを調整するスクリプトをオフ
        Camera.main.GetComponent<FollowPlayerScript>().enabled = false;
        Camera.main.transform.position = new Vector3(player_Field.transform.position.x, player_Field.transform.position.y + cameraOffsetY, cameraOffsetZ);
    }

    /// <summary>
    /// ショウをフィールドシーン上に生成する
    /// </summary>
    private void SpawanSyo_Filed()
    {
        syo = SpawnCharactor(syo_FieldPrefab, player_Field.transform.position + new Vector3(3, 0), FieldParent);
        ShouTile.SetActive(true);

    }

    /// <summary>
    /// ショウをストーリーシーン上に生成する関数
    /// </summary>
    private void SpawnSyo_Story()
    {
        syo = SpawnCharactor(syo_StoryPrefab, player_Story.transform.position + new Vector3(3, 0), StoryParent);
        CharactorChangeVec(syo, "Left");
    }

    /// <summary>
    /// 村長をストーリーシーン上に生成する関数
    /// </summary>
    private void SpawnSontyo_Story()
    {
        sontyo = SpawnCharactor(sontyo_StoryPrefab, player_Story.transform.position + new Vector3(1.5f, 10), StoryParent);
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

    // 尊重とショウが去る
    private IEnumerator CharaLeft()
    {
        Debug.Log("charaleft実行");
        CharactorChangeVec(sontyo, "Up");
        CharactorChangeVec(syo, "Up");
        storyEventScript.moveFlag = true;
        Animator sontyoAnim = sontyo.GetComponent<Animator>();
        sontyoAnim.SetBool("isWalk", true);
        Animator syoAnim = syo.GetComponent<Animator>();
        syoAnim.SetBool("isWalk", true);
        sontyo.transform.DOMoveY(leftPoint.position.y, 3f);
        syo.transform.DOMoveY(leftPoint.position.y, 3f);
        yield return new WaitForSeconds(3);
        sontyoAnim.SetBool("isWalk", false);
        syoAnim.SetBool("isWalk", false);
        storyEventScript.moveFlag = false;
        Destroy(sontyo.gameObject);
        Destroy(syo.gameObject);
        storyEventScript.ReadNextMessage();

    }

}
