using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class CF_Event2 : CharactorFunction
{

    [SerializeField] private StoryEventScript storyEventScript;
    [SerializeField] private GameObject player_Field;
    [SerializeField] private GameObject player_Story;
    [SerializeField] private GameObject syo_FieldPrefab;
    [SerializeField] private GameObject syo_StoryPrefab;
    [SerializeField] private GameObject sontyo_StoryPrefab;
    [SerializeField] private GameObject mother_StoryPrefab;
    [SerializeField] private GameObject Ground; // Event1のフィールド
    
    [SerializeField] private GameObject SontyoHouse; 
    [SerializeField] private GameObject SontyoHouse1F; 
    [SerializeField] private GameObject SontyoHouse2F; 
    [SerializeField] private GameObject Food; 
    [SerializeField] private Transform FieldParent;
    [SerializeField] private Transform StoryParent;
    [SerializeField] private Transform SontyoPos1; // インスタンスを生成する場所を指定するためのTransform
    [SerializeField] private Transform CameraPos1; // カメラの目標位置となる空のゲームオブジェクトのTransform
    [SerializeField] private Camera storyCamera; // 操作するカメラへの参照
    [SerializeField] private Transform ShouPos1;
    [SerializeField] private Transform ShouPos2;
    [SerializeField] private Transform MaoPos1;
    [SerializeField] private Transform MaoPos2;
    [SerializeField] private Transform MotherPos1;
    [SerializeField] private Transform MotherPos2;
    private GameObject shou;
    private GameObject sontyo;
    private GameObject mother;

    public override void ExecuteCommand(string functionName, string animFuncName)
    {
        if (functionName != null)
        {
            switch (functionName)
            {
                case "SpawnSyo_Story":
                    SpawnSyo_Story();
                    break;
                case "SpawnSontyo_Story":
                    SpawnSontyo_Story();
                    break;
                case "SpawnMother_Story":
                    SpawnMother_Story();
                    break;
                case "Move2SontyoHouse":
                    Move2SontyoHouse();
                    break;
                case "StartEvent":
                    StartEvent();
                    break;
                case "EndEvent":
                    EndEvent();
                    break;
                case "MoveCameraToTarget1":
                    MoveCameraToTarget1();
                    break;
                case "ShoMove1":
                ShoMove1();
                break;
                case "MoveBedRoom":
                MoveBedRoom();
                break;
                case "DarkenCamera":
                DarkenCamera();
                break;
                case "BedRoom2living":
                BedRoom2living();
                break;
                case "AfterFood":
                AfterFood();
                break;
                case "SetMorning":
                SetMorning();
                break;
                case "Living2kichinen":
                Living2kichinen();
                break;
            }
        }

        if (animFuncName != null)
        {
            switch (animFuncName)
            {
                case "VecUp_Syo":
                    CharactorChangeVec(shou, "Up");
                    break;
            }
        }
    }

    /// <summary>
    /// ショウをストーリーシーン上(ShouPos1)に生成する関数
    /// </summary>
    private void SpawnSyo_Story(){
        Vector3 spawnPosition = new Vector3(ShouPos1.position.x, ShouPos1.position.y, 1);
        shou = SpawnCharactor(sontyo_StoryPrefab, spawnPosition, StoryParent);
    }
    
    /// <summary>
    /// 母をストーリーシーン上(MotherPos1)に生成する関数
    /// </summary>
    private void SpawnMother_Story(){
        Vector3 spawnPosition = new Vector3(MotherPos1.position.x, MotherPos1.position.y, 1);
        mother = SpawnCharactor(sontyo_StoryPrefab, spawnPosition, StoryParent);

            
        Vector3 MaoPos = new Vector3(MaoPos2.position.x, MaoPos2.position.y, 1);
        Debug.Log("MaoPos: " + MaoPos);  
        player_Story.transform.position = MaoPos;
    }

    /// <summary>
    /// 村長をストーリーシーン上(SontyoPos1)に生成する関数
    /// </summary>
    private void SpawnSontyo_Story(){
        Vector3 spawnPosition = new Vector3(SontyoPos1.position.x, SontyoPos1.position.y, 1);
        sontyo = SpawnCharactor(sontyo_StoryPrefab, spawnPosition, StoryParent);

    }
    /// <summary>
    /// Shou
    /// </summary>
    private void ShoMove1(){
        storyEventScript.moveFlag = true;

        Animator anim = shou.GetComponent<Animator>();
        anim.SetBool("isWalk", true);
        var sho_pos = sontyo.transform.position;
        StartCoroutine(MoveShou1());
    }
    private void StartWalkingFadeOut(){
        SpriteRenderer spriteRenderer = shou.GetComponent<SpriteRenderer>();
        
        // スプライトをフェードアウト
        spriteRenderer.DOFade(0f, 1f).SetEase(Ease.Linear);
        // ここで歩行アニメーションの継続
        Animator anim = player_Story.GetComponent<Animator>();
        anim.SetBool("isWalk", true);
                StartCoroutine(MoveMao1());
    }

    private void StartWalkingFadeOutMao(){
        SpriteRenderer spriteRenderer = player_Story.GetComponent<SpriteRenderer>();
        
        // スプライトをフェードアウト
        spriteRenderer.DOFade(0f, 1f).SetEase(Ease.Linear);
        // ここで歩行アニメーションの継続
        Animator anim = player_Story.GetComponent<Animator>();
        anim.SetBool("isWalk", true);
    }

    private void MoveBedRoom(){
        // プレイヤーとshouを瞬時に指定された位置に移動
        SontyoHouse1F.SetActive(false);
        SontyoHouse2F.SetActive(true);
        SpriteRenderer spriteRenderer = shou.GetComponent<SpriteRenderer>();
        spriteRenderer.DOFade(1f, 0f).SetEase(Ease.Linear);

        SpriteRenderer spriteRenderer2 = player_Story.GetComponent<SpriteRenderer>();
        spriteRenderer2.DOFade(1f, 0f).SetEase(Ease.Linear);

        Vector3 MaoPos = new Vector3(MaoPos1.position.x, MaoPos1.position.y, 1);
        Debug.Log("MaoPos: " + MaoPos);  
        player_Story.transform.position = MaoPos;
        Debug.Log("playerPos: " + player_Story.transform.position);
        
        Vector3 Shou = new Vector3(ShouPos2.position.x, ShouPos2.position.y, 1);
        shou.transform.position = Shou;
        Animator shou_anim = shou.GetComponent<Animator>();
        shou_anim.SetBool("isWalk", false);
        Animator mao_anim = player_Story.GetComponent<Animator>();
        mao_anim.SetBool("isWalk", false);
        DarkenCamera();
    }
    private void BedRoom2living(){
        MoveMao2();
        Debug.Log("xxxxxx");
          // プレイヤーとshouを瞬時に指定された位置に移動
       
    }

    private void DarkenCamera(){
        float darkenAmount = 2f; // カメラを暗くする量
        var light = storyCamera.GetComponent<Light>();
        if (light != null)
        {
            light.intensity *= darkenAmount; // 明るさを減少
        }
    }
private void Move2SontyoHouse(){
    // Groundを非アクティブにする
    Ground.SetActive(false);

    // SontyoHouseをアクティブにする
    SontyoHouse.SetActive(true);
}

private void AfterFood(){
    Food.SetActive(false);
}
public void MoveCameraToTarget1(){
        Vector3 newPosition = new Vector3(CameraPos1.position.x, CameraPos1.position.y, storyCamera.transform.position.z);
        storyCamera.transform.position = newPosition;
    }

private void SetMorning(){
    Vector3 Shou = new Vector3(ShouPos1.position.x, ShouPos1.position.y, 1);
           shou.transform.position = Shou;
    Vector3 MotherPos = new Vector3(MotherPos2.position.x, MotherPos2.position.y, 1);
           mother.transform.position = MotherPos;
           Debug.Log("morning");

}

private void Living2kichinen(){
    SontyoHouse1F.SetActive(true);
    SontyoHouse2F.SetActive(false);
    SpriteRenderer spriteRenderer = shou.GetComponent<SpriteRenderer>();
    spriteRenderer.DOFade(1f, 0f).SetEase(Ease.Linear);
    Animator anim = player_Story.GetComponent<Animator>();
    anim.SetBool("isWalk", true);
    Debug.Log("a");
    StartCoroutine(MoveMao3());
}

#region 
    // まおがリビングから寝室に移動する
    IEnumerator MoveMao1(){
        var seq = DOTween.Sequence();
        seq.Append(player_Story.transform.DOLocalMove(new Vector3(3, 0, 0), 1f).SetEase(Ease.Linear).SetRelative());
        seq.Append(player_Story.transform.DOLocalMove(new Vector3(0, 1, 0), 1f).SetEase(Ease.Linear).SetRelative());
        seq.Append(player_Story.transform.DOLocalMove(new Vector3(7, 0, 0), 2f).SetEase(Ease.Linear).SetRelative());
        seq.Append(player_Story.transform.DOLocalMove(new Vector3(0, 4, 0), 1f).SetEase(Ease.Linear).SetRelative());

        // 歩きながらフェードアウトする
        seq.AppendCallback(() => StartWalkingFadeOutMao());
        // すべてのアニメーションが終わるのを待つ
        yield return seq.WaitForCompletion();
    }

    //　寝室からリビングに移動する
    IEnumerator MoveMao2(){
        var seq = DOTween.Sequence();
        seq.Append(player_Story.transform.DOLocalMove(new Vector3(2, 5, 0), 1f).SetEase(Ease.Linear).SetRelative());
        seq.Append(player_Story.transform.DOLocalMove(new Vector3(0, 5, 0), 1f).SetEase(Ease.Linear).SetRelative());
        seq.Append(player_Story.transform.DOLocalMove(new Vector3(2, 0, 0), 1f).SetEase(Ease.Linear).SetRelative());
       
        // 歩きながらフェードアウトする
        seq.AppendCallback(() => StartWalkingFadeOutMao());
        // すべてのアニメーションが終わるのを待つ
        yield return seq.WaitForCompletion();
    }

    IEnumerator MoveMao3(){
        var seq = DOTween.Sequence();
        seq.Append(player_Story.transform.DOLocalMove(new Vector3(0, -2, 0), 1f).SetEase(Ease.Linear).SetRelative());
        seq.Append(player_Story.transform.DOLocalMove(new Vector3(-2, 0, 0), 1f).SetEase(Ease.Linear).SetRelative());
        // seq.Append(player_Story.transform.DOLocalMove(new Vector3(0, 2, 0), 2f).SetEase(Ease.Linear).SetRelative());
       
        // すべてのアニメーションが終わるのを待つ
        yield return seq.WaitForCompletion();
    }

    IEnumerator MoveShou1(){
        var seq = DOTween.Sequence();
        seq.Append(shou.transform.DOLocalMove(new Vector3(3, 0, 0), 1f).SetEase(Ease.Linear).SetRelative());
        seq.Append(shou.transform.DOLocalMove(new Vector3(0, 1, 0), 1f).SetEase(Ease.Linear).SetRelative());
        seq.Append(shou.transform.DOLocalMove(new Vector3(7, 0, 0), 2f).SetEase(Ease.Linear).SetRelative());
        seq.Append(shou.transform.DOLocalMove(new Vector3(0, 4, 0), 1f).SetEase(Ease.Linear).SetRelative());

        // 歩きながらフェードアウトする
        seq.AppendCallback(() => StartWalkingFadeOut());
        // すべてのアニメーションが終わるのを待つ
        yield return seq.WaitForCompletion();
        
    }

    IEnumerator BedRoom2Living(){
        MoveMao2();
        yield return null;
        Living2kichinen();
    }


#endregion
}