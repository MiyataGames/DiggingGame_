using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CF_Event5 : CharactorFunction
{
    [SerializeField] private StoryEventScript storyEventScript;
    [SerializeField] private GameObject player_Field;
    [SerializeField] private GameObject player_Story;
    [SerializeField] private GameObject syo_FieldPrefab;
    [SerializeField] private GameObject syo_StoryPrefab;
    [SerializeField] private GameObject mao_StoryPrefab;
    [SerializeField] private GameObject boss_StoryPrefab;

    [SerializeField] private Transform FieldParent;
    [SerializeField] private Transform StoryParent;

    private GameObject syo;

    private GameObject mao;

    private GameObject boss;

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
                case "SpawnMao_Story":
                    SpawnMao_Story();
                    break;
                case "SpawnBoss_Story":
                    SpawnBoss_Story();
                    break;

                case "Move2Village":
                    Move2Village();
                    break;
       
                case "BossMove":
                    StartCoroutine(BossMove());
                    break;
                case "SyoMove":
                    SyoMove();
                    break;
                case "SyoStop":
                    SyoStop();
                    break;
                case "SyoSecondMove":
                   SyoSecondMove();
                    break;
                case "MaoMove":
                    MaoMove();
                    break;

                case "MaoStop":
                    MaoStop();
                    break;
                case "MaoSecondMove":
                    MaoSecondMove();
                    break;
                case "SceneShake":
                    StartCoroutine(SceneShake());
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
        syo = SpawnCharactor(syo_FieldPrefab, player_Field.transform.position + new Vector3(-3f, 5f), FieldParent);
    }

    /// <summary>
    /// ショウをストーリーシーン上に生成する関数
    /// </summary>
    private void SpawnSyo_Story()
    {
        syo = SpawnCharactor(syo_StoryPrefab, player_Story.transform.position + new Vector3(-3f, 5f), StoryParent);
    }

    /// <summary>
    /// 村長をストーリーシーン上に生成する関数
    /// </summary>
    private void SpawnMao_Story()
    {
        mao = SpawnCharactor(mao_StoryPrefab, player_Story.transform.position + new Vector3(-2f, 5f), StoryParent);
    }

    private void SpawnBoss_Story()
    {
        boss = SpawnCharactor(boss_StoryPrefab, player_Story.transform.position + new Vector3(-3f, 5f), StoryParent);
        boss.GetComponent<BossShake>().SetStoryScene(StoryParent);
    }

    /// <summary>
    /// 村へ移動する関数
    /// </summary>
    private void Move2Village()
    {
        SideView2TopDown();
    }

    /// <summary>
    /// maoが移動する関数
    /// </summary>
    private void MaoMove()
    {
        storyEventScript.moveFlag = true;
        Animator maoAnim = mao.GetComponent<Animator>();
        maoAnim.SetBool("isWalk", true);

        var maoPosition = mao.transform.position;
        mao.transform.DOMove(maoPosition - new Vector3(0, 4f, 0), 4f)
            .OnComplete(MaoStop); // アニメーションの完了時に SyoStop を呼び出す
        SyoMove();
        var maoCameraPosition=Camera.main.transform.DOMove(Camera.main.transform.position - new Vector3(0, 3f, 0), 4f);
        
    }

    private void MaoStop()
    {
        storyEventScript.moveFlag = true;
        Animator maoAnim = mao.GetComponent<Animator>();
        maoAnim.SetBool("isWalk", false);
    }


    private void MaoSecondMove()
    {

        storyEventScript.moveFlag = true;
        Animator maoAnim = mao.GetComponent<Animator>();
        maoAnim.SetBool("isWalk", true);

        Debug.Log("実行できた");
        var maoPosition = mao.transform.position;
        mao.transform.DOMove(maoPosition - new Vector3(0, 1.8f, 0), 2f)
            .OnComplete(MaoStop); // アニメーションの完了時に SyoStop を呼び出す
        SyoSecondMove();
        var maoCameraPosition = Camera.main.transform.DOMove(Camera.main.transform.position - new Vector3(0, 1f, 0), 2f);


    }



    private void SyoMove()
    {
        //storyEventScript.moveFlag = true;
        Animator syoAnim = syo.GetComponent<Animator>();
        syoAnim.SetBool("isWalk", true);

        var syoPosition = syo.transform.position;
        syo.transform.DOMove(syoPosition - new Vector3(0, 4f, 0), 4f)
            .OnComplete(SyoStop); // アニメーションの完了時に SyoStop を呼び出す
        Camera.main.transform.DOMove(Camera.main.transform.position - new Vector3(0, 4f, 0), 4f);

    }



    private IEnumerator SceneShake()
    {
        Debug.Log("シーンシェイク");
        Camera.main.DOShakePosition(2f, 1.2f); // 0.2秒間、強度2で揺らす
        yield return new WaitForSeconds(1f); // 揺れの後1秒待機（揺れの0.2秒を含む）


    }




    private void SyoStop()
    {
        storyEventScript.moveFlag = true;
        Animator syoAnim = syo.GetComponent<Animator>();
        syoAnim.SetBool("isWalk", false);
    }


    private void SyoSecondMove()
    {
        storyEventScript.moveFlag = true;
        Animator maoAnim = syo.GetComponent<Animator>();
        maoAnim.SetBool("isWalk", true);

        var syoPosition = syo.transform.position;
        syo.transform.DOMove(syoPosition - new Vector3(0, 1.8f, 0), 2f)
            .OnComplete(SyoStop); // アニメーションの完了時に SyoStop を呼び出す

    }

    private IEnumerator BossMove()
    {


        storyEventScript.moveFlag = true;


        //// カメラの初期位置を新しい位置にリセット
        var cameraInitialPosition = new Vector3(1f, 5f, Camera.main.transform.position.z);

        //Camera.main.transform.position = cameraInitialPosition;

        //bossAnim.SetBool("isWalk", false);
        Vector3 bossCameraPosition = boss.transform.position + new Vector3(0, -1f, Camera.main.transform.position.z);//ボスのカメラ位置
        Debug.Log("カメラ移動開始");
        yield return Camera.main.transform.DOMove(bossCameraPosition, 3f).WaitForCompletion();//現在地(まおが移動した地点)から３秒かけてボスがいる場所に移動
        Debug.Log("カメラ移動終了");
        //yield return new WaitForSeconds(1);//１秒待機
        //bossAnim.SetBool("isWalk", true);//歩くアニメーション実行
        Animator bossAnim = boss.GetComponent<Animator>();
        bossAnim.SetBool("isWalk", true);

        var bossPosition = boss.transform.position;//ボスの最初の位置
        var OneStep = new Vector3(0, 2f, 0);//ボスの1歩の大きさ

        // カメラのオフセットを設定
        var cameraOffset = Camera.main.transform.position - bossPosition;


        for (int i = 1; i <= 1; i++)
        {
            boss.transform.DOMove(bossPosition - OneStep * i, 5f);//
            Camera.main.transform.DOMove(bossPosition - OneStep * i + cameraOffset, 3f);
            //// カメラを揺らす
            //Camera.main.DOShakePosition(0.3f, 1.3f); // 0.2秒間、強度2で揺らす
            //yield return new WaitForSeconds(1f); // 揺れの後1秒待機（揺れの0.2秒を含む）

            //// カメラを再度揺らす
            //Camera.main.DOShakePosition(0.3f, 1.3f); // 再度0.2秒間、強度2で揺らす
            //yield return new WaitForSeconds(1f); // 再度の揺れの後1秒待機（揺れの0.2秒を含む）



            yield return new WaitForSeconds(3);
               // bossAnim.SetBool("isWalk", false);
                //yield return new WaitForSeconds(2);
                //bossAnim.SetBool("isWalk", true);
        }

        bossAnim.SetBool("isWalk", false);　//アニメーションストップ
        yield return new WaitForSeconds(2);//２秒待機
        storyEventScript.moveFlag = false;//
        storyEventScript.ReadNextMessage();//次の文に移動
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
