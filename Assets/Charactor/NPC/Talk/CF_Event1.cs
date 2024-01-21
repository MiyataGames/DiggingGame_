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

    [SerializeField] private Transform FieldParent;
    [SerializeField] private Transform StoryParent;

    private GameObject syo;

    private GameObject sontyo;

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

    private void SpawanSyo_Filed()
    {
        syo = SpawnCharactor(syo_FieldPrefab, player_Field.transform.position + new Vector3(3, 0), FieldParent);
    }

    private void SpawnSyo_Story()
    {
        syo = SpawnCharactor(syo_StoryPrefab, player_Story.transform.position + new Vector3(3, 0), StoryParent);
    }

    private void SpawnSontyo_Story()
    {
        sontyo = SpawnCharactor(sontyo_StoryPrefab, player_Story.transform.position + new Vector3(1.5f, 10), StoryParent);
    }

    private void Move2Village()
    {
        SideView2TopDown();
    }

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



    private void moveCompleteFunc()
    {
        Animator anim = sontyo.GetComponent<Animator>();
        anim.SetBool("isWalk", false);
        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();
    }
}
