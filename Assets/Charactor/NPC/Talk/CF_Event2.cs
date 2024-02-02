using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CF_Event2 : CharactorFunction
{
    [SerializeField] private StoryEventScript storyEventScript;
    [SerializeField] private GameObject player_Field;
    [SerializeField] private GameObject player_Story;
    [SerializeField] private GameObject syo_FieldPrefab;
    [SerializeField] private GameObject syo_StoryPrefab;
    [SerializeField] private GameObject sontyo_StoryPrefab;
    [SerializeField] private GameObject mother_StoryPrefab;

    [SerializeField] private Transform FieldParent;
    [SerializeField] private Transform StoryParent;

    private GameObject syo;

    private GameObject sontyo;
    private GameObject mother;

    public override void ExecuteCommand(string functionName, string animFuncName)
    {
        if (functionName != null)
        {
            switch (functionName)
            {
                case "SpawnSyo_Field":
                    SpawanSyo_Filed();
                    break;
                case "SpawnSyo_Story":
                    SpawnSyo_Story();
                    break;
                case "SpawnSontyo_Story":
                    SpawnSontyo_Story();
                    break;
                case "SpawnMother_Story":
                    SpawnMother_Story();
                    break;
                case "Move2Village":
                    Move2Village();
                    break;
                case "SontyoMove":
                    SontyoMove();
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

    /// <summary>
    /// ショウをフィールドシーン上に生成する
    /// </summary>
    private void SpawanSyo_Filed()
    {
        syo = SpawnCharactor(syo_FieldPrefab, player_Field.transform.position + new Vector3(3, 0), FieldParent);
    }

    /// <summary>
    /// ショウをストーリーシーン上に生成する関数
    /// </summary>
    private void SpawnSyo_Story()
    {
        syo = SpawnCharactor(syo_StoryPrefab, player_Story.transform.position + new Vector3(3, 0), StoryParent);
    }

    private void SpawnMother_Story()
    {
        Debug.Log("a");
        mother = SpawnCharactor(mother_StoryPrefab, player_Story.transform.position + new Vector3(3.5f, 0), StoryParent);
    }
    /// <summary>
    /// 村長をストーリーシーン上に生成する関数
    /// </summary>
    private void SpawnSontyo_Story()
    {
        sontyo = SpawnCharactor(sontyo_StoryPrefab, player_Story.transform.position + new Vector3(4.0f, 0), StoryParent);
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
