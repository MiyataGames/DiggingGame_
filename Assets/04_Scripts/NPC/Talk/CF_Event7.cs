using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CF_Event7 : CharactorFunction
{
    //[SerializeField] private StoryEventScript storyEventScript;
    [SerializeField] AudioSource audioSource;
    [SerializeField] string SEPath;
    [SerializeField] private GameObject player_Story;
    [SerializeField] private GameObject syo_StoryPrefab;
    [SerializeField] private GameObject sontyo_StoryPrefab;
    [SerializeField] private GameObject disableRockPrefab;
    [SerializeField] GameObject clane;

    [SerializeField] private Transform TilemapParent;

    // カメラのz座標
    [SerializeField] float CameraBufferZ = -6.53f;
    [SerializeField] float CameraOffsetY = 2;
    private GameObject syo;
    Animator syoAnim;
    Animator maoAnim;

    private GameObject sontyo;
    // 保存したいTweenがある時
    Tween tween;

    // ショウがスポーンする位置
    [SerializeField] Transform syoEvent7FirstTransform;
    // 村長がスポーンする位置
    [SerializeField] Transform sontyoEvent7FirstTransform;
    // 移動する位置
    [SerializeField] Transform maoEvent7FirstTransform;
    [SerializeField] float claneMoveXFirst;
    [SerializeField] Transform claneFinalPos;
    // 村長がクレーンからX座標左側移動する
    [SerializeField] float moveSontyoX;
    public override void ExecuteCommand(string functionName, string animFuncName)
    {
        if (functionName != null)
        {
            switch (functionName)
            {
                case "StartEvent":
                    StartEvent();
                    break;
                case "SpawnSyoMao_Story":
                    SpawnSyoMao_Story();
                    break;
                case "ClaneMove":
                    StartCoroutine(ClaneMove());
                    break;
                case "DestroyDisableRock":
                    DestroyDisableRock();
                    break;
                case "SontyoMoveFromClane":
                    StartCoroutine(SontyoMoveFromClane());
                    break;
                case "SyoTurnToAndMoveRight":
                    SyoTurnToRight();
                    break;
                case "SyoTurnTowardsFront":
                    SyoTurnTowardsFront();
                    break;
                case "SyoTurnTowardsMao":
                    SyoTurnTowardsMao();
                    break;
                case "SontyoMoveToMao":
                    StartCoroutine(SontyoMoveToMao());
                    break;
                case "PrepareNext":
                    PrepareNext();
                    break;
                case "EndEvent":
                    EndEvent();
                    break;
            }
        }
    }

    protected override void StartEvent()
    {
        // 操作を受け付けなくする
        GameManager.instance.currentGameState = GameState.POSE;
        // イベント1_7
        GameManager.instance.CurrenEvent1Scene = Event1Scene.EVENT1_7;
        // プレイヤーカメラ追従を有効に
        Camera.main.GetComponent<FollowPlayerScript>().enabled = false;
        // 当たり判定をオフ
        this.gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        Move2Village();
    }

    /// <summary>
    /// ショウとマオをストーリーシーン上に最初に生成する関数
    /// マオとショウを生成してカメラを移動する
    /// </summary>
    private void SpawnSyoMao_Story()
    {
        syo = SpawnCharactor(syo_StoryPrefab, syoEvent7FirstTransform.position, StoryParent);
        player_Story.transform.position = maoEvent7FirstTransform.position;
        Camera.main.transform.position = new Vector3(player_Story.transform.position.x, player_Story.transform.position.y, -10);
        CharactorChangeVec(player_Story, "down");
        // しょうとマオが歩く カメラはマオを追従する
        syoAnim = syo.GetComponent<Animator>();
        maoAnim = player_Story.GetComponent<Animator>();
    }

    IEnumerator ClaneMove()
    {
        storyEventScript.moveFlag = true;
        clane.transform.DOShakePosition(0.01f, 0.5f);
        yield return new WaitForSeconds(0.6f);
        clane.transform.DOMoveX(clane.transform.position.x + claneMoveXFirst,2f);
        yield return new WaitForSeconds(2f);
        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();
    }

    // 邪魔な岩が消える
    void DestroyDisableRock()
    {
        clane.transform.position = claneFinalPos.position;
        disableRockPrefab.SetActive(false);
        Vector3 cameraPos = new Vector3(clane.transform.position.x, clane.transform.position.y, CameraBufferZ);
        Camera.main.transform.position = cameraPos;
    }

    // 村長がクレーンから降りる
    IEnumerator SontyoMoveFromClane()
    {

        Vector3 sontyoPos = new Vector3(claneFinalPos.position.x, claneFinalPos.position.y - 1f, claneFinalPos.position.z);
        sontyo = SpawnCharactor(sontyo_StoryPrefab, sontyoPos, StoryParent);
        storyEventScript.moveFlag = true;
        Animator sontyoAnim = sontyo.GetComponent<Animator>();
        CharactorChangeVec(sontyo, "Left");
        sontyoAnim.SetBool("isWalk", true);
        sontyo.transform.DOMoveX(sontyo.transform.position.x - moveSontyoX, 6f);
        Camera.main.transform.DOMoveX(sontyo.transform.position.x - moveSontyoX, 6f);
        yield return new WaitForSeconds(6);
        sontyoAnim.SetBool("isWalk", false);
        CharactorChangeVec(sontyo, "Left");
        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();
    }

    void SyoTurnToRight()
    {
        CharactorChangeVec(syo, "Right");
    }

    // ショウが主人公の方(左)を向く
    void SyoTurnTowardsMao()
    {
        CharactorChangeVec(syo, "Left");
    }

    IEnumerator SontyoMoveToMao()
    {
        storyEventScript.moveFlag = true;
        Animator sontyoAnim = sontyo.GetComponent<Animator>();
        // 修正
        CharactorChangeVec(sontyo, "Left");
        sontyoAnim.SetBool("isWalk", true);
        float sontyoPosX = player_Story.transform.position.x+1;
        sontyo.transform.DOMoveX(sontyoPosX,1f);
        yield return new WaitForSeconds(1);
        // 修正
        CharactorChangeVec(sontyo, "Up");
        float sontyoPosY = player_Story.transform.position.y;
        sontyo.transform.DOMoveY(sontyoPosY, 1f);
        yield return new WaitForSeconds(1);
        sontyoAnim.SetBool("isWalk", false);
        CharactorChangeVec(sontyo, "Left");
        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();
    }

    // ショウが前を向く
    void SyoTurnTowardsFront()
    {
        CharactorChangeVec(syo, "down");
    }

    // ショウが後ろを向く
    void SyoTurnTowardsBack()
    {
        CharactorChangeVec(syo, "Up");
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

    // 主人公が後ろを向く
    void MaoTurnTowardsBack()
    {
        CharactorChangeVec(player_Story, "Up");
    }



    // 次のイベントまでの用意（プレイヤーが動けるようになるまでの準備）
    void PrepareNext()
    {
        Destroy(syo.gameObject);
        Destroy(sontyo.gameObject);
        CharactorChangeVec(player_Story, "down");
        // プレイヤーカメラ追従を有効に
        Camera.main.GetComponent<FollowPlayerScript>().enabled = true;
    }

    /// <summary>
    /// 村へ移動する関数
    /// </summary>
    private void Move2Village()
    {
        SideView2TopDown();
    }

}
