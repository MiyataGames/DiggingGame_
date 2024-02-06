using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//using UnityEngine.Tilemaps;


public class CF_Event6  : CharactorFunction
{
    [SerializeField] private StoryEventScript storyEventScript;
    [SerializeField] private GameObject player_Field;
    [SerializeField] private GameObject player_Story;
    [SerializeField] private GameObject syo_FieldPrefab;
    [SerializeField] private GameObject syo_StoryPrefab;
    [SerializeField] private GameObject mao_StoryPrefab;
    [SerializeField] private GameObject boss_StoryPrefab;
    [SerializeField] private GameObject sontyo_StoryPrefab;
    [SerializeField] private GameObject mob1_StoryPrefab;
    [SerializeField] private GameObject mob2_StoryPrefab;
    [SerializeField] private GameObject kakushimap;
    [SerializeField] private GameObject sontyoposition6;
    [SerializeField] private GameObject maoposition6;
    [SerializeField] private GameObject syoposition6;
    [SerializeField] private GameObject bossposition6;
    [SerializeField] private GameObject mob1position6;
    [SerializeField] private GameObject mob2position6;



    [SerializeField] private Transform FieldParent;
    [SerializeField] private Transform StoryParent;


    private AudioSource audioSource;
    private AudioClip zihibikiSoundClip;
    private GameObject syo;

    private GameObject mao;

    private GameObject boss;

    private GameObject sontyo;

    private GameObject mob1;

    private GameObject mob2;



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
                case "SpawnSontyo_Story":
                    SpawnSontyo_Story();
                    break;
                case "SpawnBoss_Story":
                    SpawnBoss_Story();
                    break;

                case "SpawnMob1_Story":
                    SpawnMob1_Story();
                    break;
                case "SpawnMob2_Story":
                    SpawnMob2_Story();
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
                case "SontyoMove":
                    SontyoMove();
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
                case "WaitFive":
                    WaitFive();
                    break;
                case "KakushiOn":
                    KakushiOn();
                    break;
                case "Mob1Move":
                    Mob1Move();
                    break;
                case "Mob2Move":
                    Mob2Move();
                    break;
                case "SontyoStop":
                    SontyoStop();
                    break;
                case "HideBoss":
                    HideBoss();
                    break;
                case "StartEvent":
                    StartEvent();
                    break;
                case "EndEvent":
                    EndEvent();
                    break;
                case "ShakeBo":
                    ShakeBo();
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


        // AudioSource コンポーネントを追加し、設定
        audioSource = gameObject.AddComponent<AudioSource>();

        // footSound 音楽ファイルをロード
        zihibikiSoundClip = Resources.Load<AudioClip>("SE/Monster/zihibiki");
        audioSource.clip = zihibikiSoundClip;
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
        syo = SpawnCharactor(syo_StoryPrefab, syoposition6.transform.position, StoryParent);
    }

    /// <summary>
    /// 村長をストーリーシーン上に生成する関数
    /// </summary>
    private void SpawnMao_Story()
    {
        mao = SpawnCharactor(mao_StoryPrefab,maoposition6.transform.position, StoryParent);
    }


    private void SpawnSontyo_Story()
    {

        sontyo = SpawnCharactor(sontyo_StoryPrefab, sontyoposition6.transform.position, StoryParent);
    }


    private void SpawnMob1_Story()
    {

        mob1 = SpawnCharactor(mob1_StoryPrefab, mob1position6.transform.position, StoryParent);
    }

    private void SpawnMob2_Story()
    {

        mob2 = SpawnCharactor(mob2_StoryPrefab, mob2position6.transform.position, StoryParent);
    }


    private void SpawnBoss_Story()
    {
        boss = SpawnCharactor(boss_StoryPrefab,bossposition6.transform.position, StoryParent);
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
        mao.transform.DOMove(maoPosition - new Vector3(0, -0.6f, 0), 1f)
            .OnComplete(MaoStop); // アニメーションの完了時に SyoStop を呼び出す
        SyoMove();
        
        var maoCameraPosition=Camera.main.transform.DOMove(Camera.main.transform.position - new Vector3(0, -0.6f, 0), 1f);
        
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

        // 移動前の元の位置を保存
        Vector3 originalPosition = mao.transform.position;
        // 移動する新しい位置を計算
        Vector3 movePosition = originalPosition - new Vector3(0, -0.2f, 0);

        // 移動と戻る動作を連続して実行するSequenceを作成
        Sequence sequence = DOTween.Sequence();

        // 1秒かけて指定した位置に移動し、移動完了時にShakeBoを実行
        sequence.Append(mao.transform.DOMove(movePosition, 0.3f).OnComplete(ShakeBo));

        // 移動完了後、すぐに元の位置に戻る
        sequence.Append(mao.transform.DOMove(originalPosition, 0.3f).OnComplete(() => {
            // 元の位置に戻った後の処理
            MaoStop(); // 移動が完了したらMaoのアニメーションを停止
        }));

        // アニメーション開始
        sequence.Play();
    }




    private void ShakeBo()
    {
        if (boss != null) // ボスの GameObject が存在することを確認
        {
            // ボスを揺らす。ここでは、持続時間を 1 秒、強度を 0.5、振動数を 10 に設定しています。
            boss.transform.DOShakePosition(1f, 0.8f, 15);
        }
    }





    //private void SontyoMove()
    //{
    //    storyEventScript.moveFlag = true;

    //    Animator sontyoAnim = sontyo.GetComponent<Animator>();
    //    sontyoAnim.SetBool("isWalk", true);

    //    var t = sontyo.transform.position;
    //    sontyo.transform.DOMove(t - new Vector3(0, 4, 0), 4f)
    //        .OnComplete(SontyoStop);

    //}

    //private void SontyoStop()
    //{
    //    storyEventScript.moveFlag = true;
    //    Animator sontyoAnim = sontyo.GetComponent<Animator>();
    //    sontyoAnim.SetBool("isWalk", false);
    //}


    private void SontyoMove()
    {
        storyEventScript.moveFlag = true;
        Animator sontyoAnim = sontyo.GetComponent<Animator>();
        sontyoAnim.SetBool("isWalk", true);

        var sontyoPosition = sontyo.transform.position;
        sontyo.transform.DOMove(sontyoPosition - new Vector3(0, 3.5f, 0), 4f)
            .OnComplete(SontyoStop); // アニメーションの完了時に SyoStop を呼び出す
        Mob1Move();
        Mob2Move();
        
        var SontyoCameraPosition = Camera.main.transform.DOMove(Camera.main.transform.position - new Vector3(0, 3f, 0), 4f);

    }

    private void SontyoStop()
    {
        storyEventScript.moveFlag = true;
        Animator sontyoAnim = sontyo.GetComponent<Animator>();
        sontyoAnim.SetBool("isWalk", false);
    }




    private void Mob1Move()
    {
        storyEventScript.moveFlag = true;

        Animator Mob1anim = mob1.GetComponent<Animator>();
        Mob1anim.SetBool("isWalk", true);

        var t = mob1.transform.position;
        mob1.transform.DOMove(t - new Vector3(0, 3, 0), 4f)
                        .SetEase(Ease.Linear)
                        .OnComplete(Mob1Stop);
        
    }

    private void Mob1Stop()
    {
        storyEventScript.moveFlag = true;
        Animator Mob1Anim = mob1.GetComponent<Animator>();
        Mob1Anim.SetBool("isWalk", false);
    }

    private void Mob2Move()
    {
        storyEventScript.moveFlag = true;

        Animator Mob2anim = mob2.GetComponent<Animator>();
        Mob2anim.SetBool("isWalk", true);

        var t = mob2.transform.position;
        mob2.transform.DOMove(t - new Vector3(0, 3, 0), 4f)
                        .SetEase(Ease.Linear)
                        .OnComplete(Mob2Stop);
        Mob1Move();
    }

    private void Mob2Stop()
    {
        storyEventScript.moveFlag = true;
        Animator Mob2Anim = mob2.GetComponent<Animator>();
        Mob2Anim.SetBool("isWalk", false);
    }





    private void SyoMove()
    {
        // storyEventScript.moveFlag = true;
        Animator syoAnim = syo.GetComponent<Animator>();
        if (syoAnim != null) // Animator コンポーネントが存在するかチェック
        {
            syoAnim.SetBool("isWalk", true);

            Debug.Log("動いた");
            var syoPosition = syo.transform.position;
            syo.transform.DOMove(syoPosition - new Vector3(0, -1f, 0), -1f)
                .OnComplete(SyoStop); // アニメーションの完了時に SyoStop を呼び出す
        }
        else
        {
            Debug.LogError("Animator component not found on " + syo.name);
        }

        Camera.main.transform.DOMove(Camera.main.transform.position - new Vector3(0, -1, 0), 1f);
    }




    private IEnumerator SceneShake()
    {
        Debug.Log("シーンシェイク");
        
        Camera.main.DOShakePosition(6f, 1.5f); // 0.2秒間、強度2で揺らす
        audioSource.Play();
        yield return new WaitForSeconds(1f); // 揺れの後1秒待機（揺れの0.2秒を含む）


    }




    private void SyoStop()
    {
        storyEventScript.moveFlag = true;
        Animator syoAnim = syo.GetComponent<Animator>();
        syoAnim.SetBool("isWalk", false);
    }


    private IEnumerator WaitFive()
    {
        yield return new WaitForSeconds(5);
        // 5秒後に実行したい処理をここに記述
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


        SpawnBoss_Story();
   

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

    public void KakushiOn()
    {
        Debug.Log("aaaaabbbbcccccccc");
        kakushimap.SetActive(true);

    }

    public void HideBoss()
    {
        if (boss != null) // Check if the boss GameObject is not null
        {
            // Get the SpriteRenderer component from the boss GameObject
            SpriteRenderer spriteRenderer = boss.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // Fade the sprite to 0 alpha over 2 seconds
                spriteRenderer.DOFade(0f, 3f).OnComplete(() => boss.SetActive(false));
            }
            else
            {
                Debug.LogWarning("HideBoss called but SpriteRenderer is missing on the boss GameObject.");
            }
        }


    }

    private void BossFinishedMoving()
    {
        // 移動が完了した後の処理をここに記述
        Debug.Log("Boss has finished moving.");
        // 例えば、ボスを非アクティブにするなど
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
