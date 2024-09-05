using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

[System.Serializable]
public class EventDatas{
    public int id;
    public string command;
    public string character;
    public string imageFolderName;
    public int imageNum;
    public string mainText;
    public bool setBranch;
    public string yesText;
    public string noText;
    public int choseYes;
    public int choseNo;
    public int skipID;
    public bool eventFlag;
    public int checkNeedEventNum;
    public int eventNum;
    public int afterEventSkipID;
    public string chractorFunction;
    public string animFuncName;
    public string SEpath;
}

[RequireComponent(typeof(CapsuleCollider2D))]
public class StoryEventScript : MonoBehaviour
{
    [Header("Story or SubStory")]
    public string TypeOfStory;
    [Header("イベントが起きる場所 ex.Abokado")]
    public string eventPlace;
    [Header("イベント番号 ex.Event1")]
    public string eventNum;
    public EventDatas[] EventDatas;

    public bool moveFlag = false;

    private Sprite[] charaImage;

    public int currentTextID = 0;
    private string currentCommand;
    private bool isEndOfTalk = false;

    private List<string> afterSetEventFlagTalk = new List<string>();

    [Header("スキップボタン")]
    [SerializeField] Button skipButton;

    [Header("イベント発火にエリア内でFキーを押すか(falseなら範囲内に入った時点でイベント開始)")]
    [SerializeField] private bool TriggerIsFkey = true;

    [Header("ストーリーのフラグ管理をするマネージャー。StoryEventManagerがアタッチされたオブジェクトが必要")]
    [SerializeField] private StoryEventManager storyEventManager;

    [Header("フェードの管理をするマネージャー。FadeControllerがアタッチされたオブジェクトが必要")]
    [SerializeField] private FadeController fadeController;

    [Header("se鳴らすやつ。AudioSourseがアタッチされたオブジェクトが必要")]
    [SerializeField] private AudioSource audioSource;

    [Header("キャラクター関数が詰め込まれたスクリプト。イベントごとに作ってください")]
    [SerializeField] private CharactorFunction charactorFunction;

     [Header("ダイアログに必要なオブジェクトたち")]
    [SerializeField] private GameObject talkDialog; //会話用ダイアログ
    [SerializeField] private GameObject choosedialog; //選択肢用ダイアログ
    [SerializeField] private Button readNext; //次の会話を表示するボタン
    [SerializeField] TextMeshProUGUI massage; //メインテキスト
    [SerializeField] TextMeshProUGUI nameText; //名前用テキスト
    [SerializeField] TextMeshProUGUI yMassage; //肯定選択肢テキスト
    [SerializeField] Button yButton; //肯定ボタン
    [SerializeField] TextMeshProUGUI nMassage; //否定選択肢テキスト
    [SerializeField] Button nButton; //否定ボタン
    [SerializeField] Image image;//キャラ画像

    private bool isPlayerInErea = false; //エリア内にプレイヤーがいるかのフラグ
    private bool isInEventNow = false; //会話中かフラグ
    private bool isCanNextText = true;
    private bool isNowFading = false;
    private bool isSettingEventFlag = false;

    void Awake(){
        //テキストファイルの読み込ませるクラス
        TextAsset textAsset = new TextAsset();

        //用意したcsvファイルを読み込む
        textAsset = Resources.Load(TypeOfStory + "/" + eventPlace + "/" + eventNum,typeof(TextAsset)) as TextAsset;
        //実際にデータを変数に格納
        EventDatas = CSVSerializer.Deserialize<EventDatas>(textAsset.text);

        charaImage = new Sprite[EventDatas.Length];

        for(int i = 0 ; i < EventDatas.Length;i++){
            if(EventDatas[i].imageFolderName != null ){
                //キャライメージの読み込み
                charaImage[i] = Resources.Load<Sprite>("CharaImage/" + EventDatas[i].imageFolderName + "/" + EventDatas[i].imageNum);
                
            }
        }
        Debug.Log(charaImage.Length);
    }

    void Start(){
        
        
        
    }

    void Update(){

        //イベント中ではなく，プレイヤーが会話エリアにいたら
        if(isInEventNow == false && isPlayerInErea == true){
            /* トーク開始をクリックで判定する場合
            if(Input.GetMouseButton(0)){ // そしてマウスの左クリックされたら
                //レイを作成
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                //ヒットしたものが自分なら
                if(hit.collider != null && hit.collider.gameObject == this.gameObject){
                    isTalkingNow = true; //会話中フラグをtrue
                    textStart(); //テキストスタート
                }  
            }*/

            if(TriggerIsFkey == true){
                if(Input.GetKeyDown(KeyCode.F)){
                isInEventNow = true; //イベント中フラグをtrue
                textStart(); //テキストスタート
            }
            }
        }else if(isInEventNow == true && Input.GetKeyDown(KeyCode.F)){
            if(isCanNextText == true){
                ReadNextMessage();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other){ //コリジョンのトリガーが発火したとき
        if(other.gameObject.tag == "Player"){ //タグがPlayerなら
            isPlayerInErea = true;
        }
        if(TriggerIsFkey == false){ //イベント開始トリガーがFキーではなく
            if(isInEventNow == false){ //現在イベント中でないなら
                isInEventNow = true; //イベント中フラグをtrue
                textStart(); //テキストスタート
            }
        }
    }

    void OnTriggerExit2D(Collider2D other){
        isPlayerInErea = false;
    }



    public void textStart(){
        
        isEndOfTalk = false;//使ってない

        //会話ダイアログを表示
        //SwichTalkDialogActivate(true);
        //ボタンにReadNextMessageを登録
        //readNext.onClick.AddListener(ReadNextMessage);

        //最初のコマンドをセット
        currentCommand = EventDatas[currentTextID].command;

        // コマンドを実行する関数
        ExecuteCommand(currentCommand);
    }
    
    //次のコマンドを実行
    public void ReadNextMessage(){

        //Debug.Log(npcTalkDatas[currentTextID]);
        //skipIDがexcelで空白なら0になる
        if(EventDatas[currentTextID].skipID == 0 || isSettingEventFlag == true){
            isSettingEventFlag = false;
            currentTextID++;
        }else{
            currentTextID = EventDatas[currentTextID].skipID;
        }

        if(EventDatas[currentTextID].command == "end_talk"){ //会話終了時にすべてを初期化
            Debug.Log("会話の終了処理");
            currentTextID = 0;
            isEndOfTalk = true;
            isInEventNow = false;
            //ボタンのイベントを削除
            ClearAllListeners();
            SwichTalkDialogActivate(false);
            // このゲームオブジェクトを消去
            Destroy(this.gameObject);

        }
        else{
            currentCommand = EventDatas[currentTextID].command;
            ExecuteCommand(currentCommand);
            //Debug.Log(currentCommand);
        }

        if(isEndOfTalk == false){
            
        }
    }

    //コマンドを実行
    private void ExecuteCommand(string nowCommand){

        switch(nowCommand){
            case "set_bgm":
                SetBGM();
                break;
            case "play_sound":
                PlaySE(EventDatas[currentTextID].SEpath);
                break;
            case "chara_func":
            isCanNextText = false;
                charactorFunction.ExecuteCommand(EventDatas[currentTextID].chractorFunction,EventDatas[currentTextID].animFuncName);
                if(moveFlag == false){
                    ReadNextMessage();
                }
                break;
            case "check_EventFlag":
                CheckEventAvailable(EventDatas[currentTextID].checkNeedEventNum,EventDatas[currentTextID].eventNum,EventDatas[currentTextID].afterEventSkipID);
                break;
            case "set_text":
                ShowText(EventDatas[currentTextID].mainText);

                image.sprite = charaImage[currentTextID];
                //Debug.Log(image);

                if(EventDatas[currentTextID].setBranch){
                    ShowBranchText(EventDatas[currentTextID].yesText, EventDatas[currentTextID].noText);
                    isCanNextText = false;
                }else{
                    isCanNextText = true;
                }
                //Debug.Log(charaImage[currentTextID]);
                
                break;
            case "set_EventFlag":
                SetEventFlag();
                break;
            case "fade_blackOut":
                isCanNextText = false;
                fadeController.OnFadeOutComplete += OnFadeOutComplete;
                fadeController.FadeOut();
                break;
            case "fade_blackWait":
                if(isCanNextText == true){
                        isCanNextText = false;
                    }
                fadeController.OnFadeWaitComplete += OnFadeWaitComplete;
                fadeController.FadeWait();
                break;
            case "fade_blackIn":
                if(isCanNextText == true){
                    isCanNextText = false;
                }
                fadeController.OnFadeInComplete += OnFadeInComplete;
                fadeController.FadeIn();
                break;
            case "DeleteText":
                 DeleteText();
                break;


            default:
                break;
        }
    }

    //SEを鳴らす
    private void PlaySE(string SEpath){
        AudioClip SEClip = Resources.Load<AudioClip>(SEpath);
        Debug.Log(SEClip);
        audioSource.PlayOneShot(SEClip);
        ReadNextMessage();
    }

    //イベントフラグを立てる
    private void SetEventFlag(){
        bool alreadyRegistFlag = false;
        foreach(int i in storyEventManager.story1Flag){
            if(i == EventDatas[currentTextID].eventNum){
                alreadyRegistFlag = true;
            }
        }
        if(alreadyRegistFlag == false){
            storyEventManager.story1Flag.Add(EventDatas[currentTextID].eventNum);
        }
        ReadNextMessage();
    }

    /// <summary>
    /// イベントチェック関数
    /// </summary>
    /// <param name="checkNeedEventNum">現在のイベントフラグを立てるために必要なフラグ番号</param>
    /// <param name="eventNum">現在のフラグ番号</param>
    /// <param name="afterEventSkipID">すでに現在のフラグ番号が立っているときにスキップする番号</param>
    private void CheckEventAvailable(int checkNeedEventNum , int eventNum, int afterEventSkipID){
        
        bool canEvent = false; //現在のフラグを立てられるか
        bool alreadyEvent = false;//すでに現在のフラグが立っているか
        
        //イベントフラグのチェック中
        isSettingEventFlag = true;
        
        foreach(int i in storyEventManager.story1Flag){
            //このイベントを発生させるためのフラグが立っているか
            //Debug.Log(i);
            if(i == checkNeedEventNum){
                canEvent = true;
            }
            //すでにイベントを実行済みか
            if(i == eventNum){
                alreadyEvent = true;
            }
        }
        Debug.Log(canEvent);
        Debug.Log(alreadyEvent);
        if(alreadyEvent == true){
            skip(EventDatas[currentTextID].afterEventSkipID);
        }else if(canEvent == true){
            skip(EventDatas[currentTextID].skipID);
        }else{
            ReadNextMessage();
        }

    }

    private void ShowBranchText(string yText, string nText)
    {
        //readNextボタンを無効化（クリック使用時）
        //SwichReadNextInteractable();

        isCanNextText = false;

        //選択肢ダイアログを有効化
        SwichChooseDialogActivate();

        //ボタンに選択肢処理を登録
        yButton.onClick.AddListener(ChooseYes);
        nButton.onClick.AddListener(Chooseno);

        yMassage.text = EventDatas[currentTextID].yesText;
        nMassage.text = EventDatas[currentTextID].noText;
    }

    public void ChooseYes(){
        //選択肢ダイアログを無効化
        SwichChooseDialogActivate();
        skip(EventDatas[currentTextID].choseYes);
        yButton.onClick.RemoveAllListeners();
        nButton.onClick.RemoveAllListeners();
    }

    public void Chooseno(){
        //選択肢ダイアログを無効化
        SwichChooseDialogActivate();
        skip(EventDatas[currentTextID].choseNo);
        yButton.onClick.RemoveAllListeners();
        nButton.onClick.RemoveAllListeners();
    }

    //テキストのスキップ
    private void skip(int skip_id)
    {
        //readNextボタンを有効化(クリック使用時)
        //SwichReadNextInteractable();

        isCanNextText = true;

        currentTextID = skip_id;
        currentCommand = EventDatas[currentTextID].command;

        ExecuteCommand(currentCommand);
    }

    //次のIDのチェック＆決定
    /*public void NextIdCheck(){
        Debug.Log(currentTextID);
        if(npcTalkDatas[currentTextID].skipID == 0){
            currentTextID++;
        }else{
            currentTextID = npcTalkDatas[currentTextID].skipID;
        }

        if(npcTalkDatas[currentTextID].command == "end_talk"){
            currentCommand = npcTalkDatas[currentTextID].command;
        }else{
            currentTextID = 0;
            isEndOfTalk = true;
            isTalkingNow = false;
            //ボタンのイベントを削除
            ClearAllListeners();
            SwichTalkDialogActivate();
        }
    }*/

    private void ShowText(string message)
    {
        SwichTalkDialogActivate(true);
        //キャラの名前を表示
        nameText.text = EventDatas[currentTextID].character;
        //会話テキストを表示
        massage.text = EventDatas[currentTextID].mainText;
    }



    private void DeleteText()
    {
        ClearAllListeners();
        SwichTalkDialogActivate(false);
        ReadNextMessage();
    }


    private void SetBGM()
    {
        //BGM鳴らすコードかいて（後で）

        //次のコマンドを実行
        ReadNextMessage();

    }

    //トークダイアログの切り替え
    void SwichTalkDialogActivate(bool b)
    {
        if (b == false)
        {
            talkDialog.SetActive(false);
        }
        else
        {
            talkDialog.SetActive(true);
        }
    }

    //選択肢ダイアログの切り替え
    void SwichChooseDialogActivate()
    {
        if (choosedialog.activeSelf)
        {
            choosedialog.SetActive(false);
        }
        else
        {
            choosedialog.SetActive(true);
        }
    }

    void SwichReadNextInteractable()
    {
        if (readNext.interactable)
        {
            readNext.interactable = false;
        }
        else
        {
            readNext.interactable = true;
        }
    }

    //すべてのボタンに登録されている処理を初期化
    void ClearAllListeners(){
        readNext.onClick.RemoveAllListeners();
        yButton.onClick.RemoveAllListeners();
        nButton.onClick.RemoveAllListeners();
    }

    //フェードイン後の処理
     private void OnFadeInComplete()
    {
        Debug.Log("Fade In Complete");
        isCanNextText = true;
        ReadNextMessage();
        fadeController.OnFadeInComplete -= OnFadeInComplete;
    }

    private void OnFadeWaitComplete(){
        Debug.Log("Fade Wait Complete");
        ReadNextMessage();
        fadeController.OnFadeWaitComplete -= OnFadeWaitComplete;
    }

    //フェードアウト後の処理
    private void OnFadeOutComplete()
    {
        Debug.Log("Fade Out Complete");
        ReadNextMessage();
        fadeController.OnFadeOutComplete -= OnFadeOutComplete;
    }

}


