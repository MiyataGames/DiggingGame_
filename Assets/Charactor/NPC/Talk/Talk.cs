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
public class NpcTalkData{
    public int id;
    public string command;
    public string character;
    public string imageFolderName;
    public int imageNum;
    public string main_text;
    public bool set_branch;
    public string yes_text;
    public string no_text;
    public int chose_yes;
    public int chose_no;
    public int skip_id;
}

[RequireComponent(typeof(CapsuleCollider2D))]
public class Talk : MonoBehaviour
{
    [Header("NPC1のように入力")]
    public string npcNum;
    public NpcTalkData[] npcTalkDatas;

    private Sprite[] charaImage;

    public int currentTextID = 0;
    private string currentCommand;
    private bool isEndOfTalk = false;

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
    private int j = 0;

    void Awake(){
        //テキストファイルの読み込ませるクラス
        TextAsset textAsset = new TextAsset();

        //用意したcsvファイルを読み込む
        textAsset = Resources.Load("NpcTalkData/NPC" + npcNum,typeof(TextAsset)) as TextAsset;
        //実際にデータを変数に格納
        npcTalkDatas = CSVSerializer.Deserialize<NpcTalkData>(textAsset.text);

        charaImage = new Sprite[npcTalkDatas.Length];

        for(int i = 0 ; i < npcTalkDatas.Length;i++){
            if(npcTalkDatas[i].imageFolderName != null ){
                //キャライメージの読み込み
                charaImage[i] = Resources.Load<Sprite>("CharaImage/" + npcTalkDatas[i].imageFolderName + "/" + npcTalkDatas[i].imageNum);
            }
        }
        
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

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Player"){
            isPlayerInErea = true;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        isPlayerInErea = false;
    }



    public void textStart(){
        
        isEndOfTalk = false;

        //会話ダイアログを表示
        SwichTalkDialogActivate();
        //ボタンにReadNextMessageを登録
        readNext.onClick.AddListener(ReadNextMessage);

        currentCommand = npcTalkDatas[currentTextID].command;

        ExecuteCommand(currentCommand);
    }

    public void ReadNextMessage(){

        NextIdCheck();

        if(isEndOfTalk == false){
            ExecuteCommand(currentCommand);
        }
    }

    //コマンドを実行
    private void ExecuteCommand(string nowCommand){

        switch(nowCommand){
            case "set_bgm":
                SetBGM();
                break;
            case "set_text":
                ShowText(npcTalkDatas[currentTextID].main_text);

                if(npcTalkDatas[currentTextID].set_branch){
                    ShowBranchText(npcTalkDatas[currentTextID].yes_text, npcTalkDatas[currentTextID].no_text);
                }

                image.sprite = charaImage[currentTextID];
                break;
            case "fade_blackIn":
                break;
            case "fade_blackOut":
                break;
            default:
                break;
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

        yMassage.text = npcTalkDatas[currentTextID].yes_text;
        nMassage.text = npcTalkDatas[currentTextID].no_text;
    }

    public void ChooseYes(){
        skip(npcTalkDatas[currentTextID].chose_yes);
    }

    public void Chooseno(){
        skip(npcTalkDatas[currentTextID].chose_no);
    }

    //テキストのスキップ
    private void skip(int skip_id)
    {
        //readNextボタンを有効化(クリック使用時)
        //SwichReadNextInteractable();

        isCanNextText = true;

        //選択肢ダイアログを無効化
        SwichChooseDialogActivate();

        currentTextID = skip_id;
        currentCommand = npcTalkDatas[currentTextID].command;

        ExecuteCommand(currentCommand);
    }

    //次のIDのチェック＆決定
    public void NextIdCheck(){
        Debug.Log(currentTextID);
        if(npcTalkDatas[currentTextID].skip_id == 0){
            currentTextID++;
        }else{
            currentTextID = npcTalkDatas[currentTextID].skip_id;
        }

        if(currentTextID < npcTalkDatas.Length){
            currentCommand = npcTalkDatas[currentTextID].command;
        }else{
            currentTextID = 0;
            isEndOfTalk = true;
            isTalkingNow = false;
            //ボタンのイベントを削除
            ClearAllListeners();
            SwichTalkDialogActivate();
        }
    }

    private void ShowText(string message)
    {
        //キャラの名前を表示
        nameText.text = npcTalkDatas[currentTextID].character;
        //会話テキストを表示
        massage.text = npcTalkDatas[currentTextID].main_text;
    }

    private void SetBGM()
    {
        NextIdCheck();

        if(isEndOfTalk == false){
            ExecuteCommand(currentCommand);
        }
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

    void ClearAllListeners(){
        readNext.onClick.RemoveAllListeners();
        yButton.onClick.RemoveAllListeners();
        nButton.onClick.RemoveAllListeners();
    }

}
