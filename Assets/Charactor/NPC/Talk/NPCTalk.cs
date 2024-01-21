using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

[System.Serializable]
public class npcTalkDatas{
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
    public string playAnimation;
}

[RequireComponent(typeof(CapsuleCollider2D))]
public class NPCTalk : MonoBehaviour
{
    [Header("NPC1のように入力")]
    public string eventNum;
    public string eventNPCName;
    public npcTalkDatas[] npcTalkDatas;

    private Sprite[] charaImage;

    public int currentTextID = 0;
    private string currentCommand;
    private bool isEndOfTalk = false;

    private List<string> afterSetEventFlagTalk = new List<string>();


    [SerializeField] private StoryEventManager storyEventManager;
    [SerializeField] private FadeController fadeController;
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
    private bool isTalkingNow = false; //会話中かフラグ
    private bool isCanNextText = true;
    private bool isSettingEventFlag = false;

    void Awake(){
        //テキストファイルの読み込ませるクラス
        TextAsset textAsset = new TextAsset();

        //用意したcsvファイルを読み込む
        textAsset = Resources.Load("NpcTalkData/Event/" + eventNum + "/" + eventNPCName,typeof(TextAsset)) as TextAsset;
        //実際にデータを変数に格納
        npcTalkDatas = CSVSerializer.Deserialize<npcTalkDatas>(textAsset.text);

        charaImage = new Sprite[npcTalkDatas.Length];

        for(int i = 0 ; i < npcTalkDatas.Length;i++){
            if(npcTalkDatas[i].imageFolderName != null ){
                //キャライメージの読み込み
                charaImage[i] = Resources.Load<Sprite>("CharaImage/" + npcTalkDatas[i].imageFolderName + "/" + npcTalkDatas[i].imageNum);
                
            }
        }
        Debug.Log(charaImage.Length);
    }

    void Start(){
        fadeController.OnFadeInComplete += OnFadeInComplete;
        fadeController.OnFadeOutComplete += OnFadeOutComplete;
    }

    void Update(){

        //話し中ではなく，プレイヤーが会話エリアにいたら
        if(isTalkingNow == false && isPlayerInErea == true){
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

            if(Input.GetKeyDown(KeyCode.F)){
                isTalkingNow = true; //会話中フラグをtrue
                textStart(); //テキストスタート
            }
        }else if(isTalkingNow == true && Input.GetKeyDown(KeyCode.F)){
            if(isCanNextText == true){
                ReadNextMessage();
            }
        }
    }

    void OnTriggerStay2D(Collider2D other){
        if(other.gameObject.tag == "Player"){
            isPlayerInErea = true;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        isPlayerInErea = false;
    }



    public void textStart(){
        
        isEndOfTalk = false;//使ってない

        //会話ダイアログを表示
        SwichTalkDialogActivate();
        //ボタンにReadNextMessageを登録
        readNext.onClick.AddListener(ReadNextMessage);

        //最初のコマンドをセット
        currentCommand = npcTalkDatas[currentTextID].command;

        // コマンドを実行する関数
        ExecuteCommand(currentCommand);
    }
    
    //次のコマンドを実行
    public void ReadNextMessage(){

        //Debug.Log(npcTalkDatas[currentTextID]);
        //skipIDがexcelで空白なら0になる
        if(npcTalkDatas[currentTextID].skipID == 0 || isSettingEventFlag == true){
            isSettingEventFlag = false;
            currentTextID++;
        }else{
            currentTextID = npcTalkDatas[currentTextID].skipID;
        }

        if(npcTalkDatas[currentTextID].command == "end_talk"){ //会話終了時にすべてを初期化
            
            currentTextID = 0;
            isEndOfTalk = true;
            isTalkingNow = false;
            //ボタンのイベントを削除
            ClearAllListeners();
            SwichTalkDialogActivate();

        }else{
            currentCommand = npcTalkDatas[currentTextID].command;
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
            case "check_EventFlag":
                CheckEventAvailable(npcTalkDatas[currentTextID].checkNeedEventNum,npcTalkDatas[currentTextID].eventNum,npcTalkDatas[currentTextID].afterEventSkipID);
                break;
            case "set_text":
                ShowText(npcTalkDatas[currentTextID].mainText);

                if(npcTalkDatas[currentTextID].setBranch){
                    ShowBranchText(npcTalkDatas[currentTextID].yesText, npcTalkDatas[currentTextID].noText);
                }
                //Debug.Log(charaImage[currentTextID]);
                image.sprite = charaImage[currentTextID];
                //Debug.Log(image);
                break;
            case "set_EventFlag":
                SetEventFlag();
                break;
            case "fade_blackOut":
                fadeController.FadeOut();
                break;
            case "fade_blackIn":
                fadeController.FadeIn();
                break;
            default:
                break;
        }
    }

    private void SetEventFlag(){
        bool alreadyRegistFlag = false;
        foreach(int i in storyEventManager.story1Flag){
            if(i == npcTalkDatas[currentTextID].eventNum){
                alreadyRegistFlag = true;
            }
        }
        if(alreadyRegistFlag == false){
            storyEventManager.story1Flag.Add(npcTalkDatas[currentTextID].eventNum);
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
            skip(npcTalkDatas[currentTextID].afterEventSkipID);
        }else if(canEvent == true){
            skip(npcTalkDatas[currentTextID].skipID);
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

        yMassage.text = npcTalkDatas[currentTextID].yesText;
        nMassage.text = npcTalkDatas[currentTextID].noText;
    }

    public void ChooseYes(){
        //選択肢ダイアログを無効化
        SwichChooseDialogActivate();
        skip(npcTalkDatas[currentTextID].choseYes);
    }

    public void Chooseno(){
        //選択肢ダイアログを無効化
        SwichChooseDialogActivate();
        skip(npcTalkDatas[currentTextID].choseNo);
    }

    //テキストのスキップ
    private void skip(int skip_id)
    {
        //readNextボタンを有効化(クリック使用時)
        //SwichReadNextInteractable();

        isCanNextText = true;

        currentTextID = skip_id;
        currentCommand = npcTalkDatas[currentTextID].command;

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
        //キャラの名前を表示
        nameText.text = npcTalkDatas[currentTextID].character;
        //会話テキストを表示
        massage.text = npcTalkDatas[currentTextID].mainText;
    }

    private void SetBGM()
    {
        //BGM鳴らすコードかいて（後で）

        //次のコマンドを実行
        ReadNextMessage();

    }

    //トークダイアログの切り替え
    void SwichTalkDialogActivate()
    {
        if (talkDialog.activeSelf)
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
    }

    //フェードアウト後の処理
    private void OnFadeOutComplete()
    {
        Debug.Log("Fade Out Complete");
    }

}

