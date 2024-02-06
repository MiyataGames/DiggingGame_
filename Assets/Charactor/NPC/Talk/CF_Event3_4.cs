using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CF_Event3_4 : CharactorFunction
{
    [SerializeField] private StoryEventScript storyEventScript;
    [SerializeField] AudioSource audioSource;
    [SerializeField] string SEPath;
    [SerializeField] private GameObject player_Field;
    [SerializeField] private GameObject player_Story;
    [SerializeField] private GameObject syo_FieldPrefab;
    [SerializeField] private GameObject syo_StoryPrefab;
    [SerializeField] private GameObject sontyo_StoryPrefab;
    [SerializeField] private GameObject dropRockPrefab;

    [SerializeField] private Transform FieldParent;
    [SerializeField] private Transform StoryParent;
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
    [SerializeField] Transform syoFirstTransform;
    // 村長がスポーンする位置
    [SerializeField] Transform sontyoFirstTransform;
    // 移動する位置
    [SerializeField] Transform maoFirstTransform;
    // ２番目に移動する位置
    [SerializeField] float moveSecondY;
    // ３番目に移動する位置
    [SerializeField] float moveMachineFirstY;
    [SerializeField] float moveFourthY;
    [SerializeField] float moveMachineSecondX;
    [SerializeField] float sontyoMoveToExitY;
    [SerializeField] Transform syoSecondTransform;
    // 村長がスポーンする位置
    //[SerializeField] Transform sontyoSecondTransform;
    // 移動する位置
    [SerializeField] Transform maoSecondTransform;
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
                case "CemeraStartMove":
                    StartCoroutine(CemeraStartMove());
                    break;
                case "SyoTurnTowardsMao":
                    SyoTurnTowardsMao();
                    break;
                case "MaoTurnTowardsSyo":
                    MaoTurnTowardsSyo();
                    break;
                case "MaoTurnTowardsFront":
                    MaoTurnTowardsFront();
                    break;
                case "SyoTurnTowardsFront":
                    SyoTurnTowardsFront();
                    break;
                case "MaoTurnTowardsBack":
                    MaoTurnTowardsBack();
                    break;
                case "SyoTurnTowardsBack":
                    SyoTurnTowardsBack();
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
                case "SontyoHitSyo":
                    StartCoroutine(SontyoHitSyo());
                    break;
                case "SmallEarthquake":
                    SmallEarthquake();
                    break;
                case "BigEarthquake":
                    BigEarthquake();
                    break;
                case "SontyoMoveToExit":
                    StartCoroutine(SontyoMoveToExit());
                    break;
                case "EndOfEarthquake":
                    StartCoroutine(EndOfEarthquake());
                    break;
                case "SyoMaoTurnTowardsBack":
                    SyoMaoTurnTowardsBack();
                    break;
                case "SontyoAttackedByMouster":
                    StartCoroutine(SontyoAttackedByMouster());
                    break;
                case "MaoMoveToSontyo":
                    StartCoroutine(MaoMoveToSontyo());
                    break;
                case "SyoMoveToSontyo":
                    StartCoroutine(SyoMoveToSontyo());
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

    void StartEvent()
    {
        // 操作を受け付けなくする
        GameManager.instance.currentGameState = GameState.POSE;
        // イベント1_3_4
        GameManager.instance.currentEvent1Scene = Event1Scene.EVENT1_3_4;
        Camera.main.transform.position = new Vector3(0, -10, -10);
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
        syo = SpawnCharactor(syo_StoryPrefab, syoFirstTransform.position, StoryParent);
        player_Story.transform.position = maoFirstTransform.position;
        CharactorChangeVec(player_Story, "down");
        // しょうとマオが歩く カメラはマオを追従する
        syoAnim = syo.GetComponent<Animator>();
        maoAnim = player_Story.GetComponent<Animator>();
    }

    IEnumerator CemeraStartMove()
    {
        // 2秒かけてカメラを真ん中からプレイヤーの位置へ
        storyEventScript.moveFlag = true;
        float cameraPosY = player_Story.transform.position.y;
        Camera.main.transform.DOMoveY(cameraPosY - CameraOffsetY, 2f);
        yield return new WaitForSeconds(2);
        syoAnim.SetBool("isWalk", true);
        maoAnim.SetBool("isWalk", true);
        cameraPosY = player_Story.transform.position.y - moveSecondY;
        Camera.main.transform.DOMoveY(cameraPosY - CameraOffsetY, 2f);
        float targetPosY = syo.transform.position.y - moveSecondY;
        syo.transform.DOMoveY(targetPosY, 2f);
        targetPosY = player_Story.transform.position.y - moveSecondY;
        player_Story.transform.DOMoveY(targetPosY, 2f).OnComplete(() => SpawnCompleteFunc());
    }

    // カメラが移動し終わったら次のメッセージ
    void SpawnCompleteFunc()
    {
        storyEventScript.moveFlag = false;
        syoAnim.SetBool("isWalk", false);
        //maoAnim.SetBool("isWalk", false);
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
        // 右
        syoAnim.SetFloat("x", 1);
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
        syo.transform.DOMoveY(syo.transform.position.y - moveMachineFirstY, 1f);
        // カメラの移動
        Camera.main.transform.DOMoveY(syo.transform.position.y - moveMachineFirstY - CameraOffsetY, 1f);
        // マオが下を向く
        CharactorChangeVec(player_Story, "down");
        yield return new WaitForSeconds(1);
        // ショウが横へ行く
        CharactorChangeVec(syo, "Right");
        syoAnim.SetBool("isWalk", true);
        syo.transform.DOMoveX(syo.transform.position.x + moveMachineSecondX, 1f);
        Camera.main.transform.DOMoveX(syo.transform.position.x + moveMachineSecondX, 1f);
        yield return new WaitForSeconds(1);
        syoAnim.SetBool("isWalk", false);


        storyEventScript.ReadNextMessage();
    }

    // マオも採掘機に近寄る
    IEnumerator MaoFindMiningMachine()
    {
        storyEventScript.moveFlag = true;
        maoAnim.SetBool("isWalk", true);
        player_Story.transform.DOMoveY(player_Story.transform.position.y - moveMachineFirstY - 1, 1f);
        yield return new WaitForSeconds(1);
        maoAnim.SetBool("isWalk", false);
        CharactorChangeVec(player_Story, "Right");
        maoAnim.SetBool("isWalk", true);
        player_Story.transform.DOMoveX(player_Story.transform.position.x + moveMachineSecondX +1.2f, 1f);
        yield return new WaitForSeconds(1);
        maoAnim.SetBool("isWalk", false);

        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();
    }

    // 村長が奥から歩いてくる
    IEnumerator SontyoSpawn()
    {
        storyEventScript.moveFlag = true;
        sontyo = SpawnCharactor(sontyo_StoryPrefab, sontyoFirstTransform.position, StoryParent);
        Animator sontyoAnimator = sontyo.GetComponent<Animator>();
        Camera.main.transform.DOMoveY(sontyoFirstTransform.position.y, 2f);
        yield return new WaitForSeconds(2);
        sontyoAnimator.SetBool("isWalk", true);
        // 村長が歩いてくる
        sontyo.transform.DOMoveY(syo.transform.position.y, 2f);
        Camera.main.transform.DOMoveY(syo.transform.position.y - CameraOffsetY, 2f);
        yield return new WaitForSeconds(2);
        CharactorChangeVec(sontyo, "Right");
        sontyo.transform.DOMoveX(sontyo.transform.position.x + moveMachineSecondX, 1f);
        yield return new WaitForSeconds(1);
        sontyoAnimator.SetBool("isWalk", false);
        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();
    }

    // 村長がショウを殴る
    IEnumerator SontyoHitSyo()
    {
        // ショウとマオが村長の方を向く
        CharactorChangeVec(syo, "Left");
        CharactorChangeVec(player_Story, "Left");
        float duration = 0.25f;
        storyEventScript.moveFlag = true;
        float sontyoInitialPosX = sontyo.transform.position.x;
        // しょうを殴る
        sontyo.transform.DOMoveX(syo.transform.position.x - 0.5f, duration);
        yield return new WaitForSeconds(duration);
        // ショウが震える
        syo.transform.DOShakePosition(0.5f, duration);
        // 村長が戻る
        sontyo.transform.DOMoveX(sontyoInitialPosX, duration);
        yield return new WaitForSeconds(duration);
        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();

    }

    // 地響き小
    void SmallEarthquake()
    {
        tween = Camera.main.DOShakePosition(duration: 10, strength: 0.2f, vibrato: 3,fadeOut: false).SetLoops(-1);
    }

    // 地響き大
    void BigEarthquake()
    {

        tween.Kill();
        tween = Camera.main.DOShakePosition(duration: 3, strength: 1, fadeOut: false).SetLoops(-1);
    }

    IEnumerator SontyoMoveToExit()
    {
        storyEventScript.moveFlag = false;
        Animator sontyoAnim = sontyo.GetComponent<Animator>();
        sontyoAnim.SetFloat("x", 0);
        sontyoAnim.SetFloat("y", 1);
        sontyoAnim.SetBool("isWalk", true);
        sontyo.transform.DOMoveY(sontyo.transform.position.y + sontyoMoveToExitY, 2);
        yield return new WaitForSeconds(2);
        sontyoAnim.SetBool("isWalk", false);
        storyEventScript.moveFlag = true;
        storyEventScript.ReadNextMessage();
    }


    // 地響きを止めるショウとマオの位置を変える
    IEnumerator EndOfEarthquake()
    {
        tween.Kill();
        Debug.Log(tween);
        storyEventScript.moveFlag = true;
        player_Story.transform.position = maoSecondTransform.position;
        syo.transform.position = syoSecondTransform.position;
        Vector3 dropRockPos = new Vector3(0, syo.transform.position.y + 2 , syo.transform.position.z);
        Instantiate(dropRockPrefab, dropRockPos, Quaternion.identity, TilemapParent);
        Camera.main.transform.position = new Vector3(maoSecondTransform.position.x, maoSecondTransform.position.y - CameraOffsetY, CameraBufferZ);
        // 村長を削除
        Destroy(sontyo.gameObject);
        Vector3 CameraPos = new Vector3(player_Story.transform.position.x, player_Story.transform.position.y- CameraOffsetY, CameraBufferZ);
        Camera.main.transform.DOMove(CameraPos,2f);
        yield return new WaitForSeconds(2);
        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();
    }

    // ショウとマオが後ろを振り返る
    void SyoMaoTurnTowardsBack()
    {
        CharactorChangeVec(syo, "Up");
        CharactorChangeVec(player_Story, "Up");
    }

    // モンスター咆哮 & カメラ移動
    IEnumerator SontyoAttackedByMouster()
    {
        storyEventScript.moveFlag = true;
        // モンスター咆哮 
        AudioClip SEClip = Resources.Load<AudioClip>(SEPath);
        audioSource.PlayOneShot(SEClip);
        // カメラ移動
        Camera.main.transform.DOMoveY(Camera.main.transform.position.y +2f - CameraOffsetY, 2f);
        yield return new WaitForSeconds(2);
        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();
    }

    // マオが駆け寄る
    IEnumerator MaoMoveToSontyo()
    {
        storyEventScript.moveFlag = true;
        Camera.main.transform.DOMoveY(syo.transform.position.y - CameraOffsetY, 2f);
        yield return new WaitForSeconds(2);
        float duration = 0.25f;
        maoAnim.SetBool("isWalk", true);
        player_Story.transform.DOMoveY(player_Story.transform.position.y + moveFourthY, duration);
        yield return new WaitForSeconds(duration);
        maoAnim.SetBool("isWalk", false);
        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();
    }
    // ショウが駆け寄る
    IEnumerator SyoMoveToSontyo()
    {
        float duration = 0.25f;
        storyEventScript.moveFlag = true;
        syoAnim.SetBool("isWalk", true);
        syo.transform.DOMoveY(syo.transform.position.y + moveFourthY, duration);
        yield return new WaitForSeconds(duration);
        syoAnim.SetBool("isWalk", false);
        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();
    }

    // 次のイベントまでの用意（プレイヤーが動けるようになるまでの準備）
    void PrepareNext()
    {
        Destroy(syo.gameObject);
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
