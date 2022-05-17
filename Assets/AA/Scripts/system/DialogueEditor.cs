using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueEditor : MonoBehaviour
{
    public float coolDown; //冷卻結束時間
    public float coolDownTimer; //冷卻時間計時器
    public Text dialogueText;  //對話輸出
    [SerializeField] string[] Name;  //對話者名子
    [SerializeField] string[] Dialogue;  //對話句子
    static int NpcName;  //對話者
    //[SerializeField] int NameNum;  //對話者數量
    public static int TextLine;  //當前對話行數
    [SerializeField] int SF_TextLine;  //當前對話行數
    public static int missionLevel = 0;  //任務關卡
    public static int missionStage = 0;
    public static bool StartDialogue;
    public static bool EndDialogue;
    public GameObject DialogueOptionsUI;  //對話選擇UI
    bool teaching;
    public bool StartTrach;
    public static int Delay;  //對話延遲
    [SerializeField] int SF_Delay;  //對話延遲
    static int DelayTextLine;
    public int Length;   //字串字元數
    static bool Auto;
    static float ChangeName; //改變對話者
    static int ChangeTextLine;  //第幾句改變
    static int NewName;  //新的對話者
    public static bool UI;  //是否出現任務訊息UI
    public static bool Talking;  //正在對話

    void Start()
    {
        coolDown = 2.4f;  //冷卻結束時間
        coolDownTimer = coolDown+1;
        StartDialogue = EndDialogue = false;
        dialogueText.text = "";
        TextLine = 0;
        Talking = false;
        if (DialogueOptionsUI == null)
        {
            DialogueOptionsUI = GameObject.Find("DialogueOptionsUI").gameObject;
        }
        Add_Dialogue();
        DontDestroyOnLoad(gameObject);  //切換場景時保留
    }
    void Update()
    {
        SF_Delay = Delay;
        SF_TextLine = TextLine;
        int SceneNub = SceneManager.GetActiveScene().buildIndex; //取得當前場景編號
        if (SceneNub == 1 || PlayerResurrection.ReDelete)
        {
            Destroy(gameObject);
        }
        if (StartDialogue)  //開始對話
        {
            Talking = true;
            if (coolDownTimer >= coolDown) //對話冷卻時間，coolDown 越小越快
            {
                if (StartTrach)  //是否跳過開場教學
                {
                    if(PlayerView.missionLevel == 0 && PlayerView.missionStage == 0)
                    {
                        if (!teaching && TextLine == 3)
                        {
                            teaching = true;
                            DialogueOptions.StartOption(0, NpcName);  //呼叫對話選項(0 任務, NPC)
                             //DialogueOptionsUI.SetActive(true);
                             //Settings.pause();
                        }
                    }
                }              
                Add_Dialogue();  //呼叫對話文本
                if (Delay==1)  //延遲對話
                {
                    if(DelayTextLine== TextLine)
                    {
                        dialogueText.text = "";
                        coolDownTimer = 1f;  //再冷卻一次
                        Delay = 2;
                        return;
                    }                 
                }
                //if (missionLevel == 0 && missionStage == 4 && TextLine==4  && !Delay)  //延遲對話
                //{
                //    dialogueText.text = "";
                //    coolDownTimer = 1f;  //再冷卻一次
                //    Delay = true;
                //    return;
                //}
                coolDownTimer = 0;  //重置短對話冷卻
                if (TextLine < Dialogue.Length)
                {
                    Length = Dialogue[TextLine].Length;
                    if (Length >= 18) coolDownTimer = -0.8f;  //重置長對話冷卻
                    if (Length >= 26) coolDownTimer = -1.6f;  //重置更長對話冷卻
                }
                if (TextLine >= Dialogue.Length)
                {
                    StartDialogue = false;
                    EndDialogue = true;
                    dialogueText.text = "";
                    coolDownTimer = coolDown + 1;
                }
                else
                {
                    if (ChangeName != 0)
                    {
                        if (ChangeTextLine == TextLine)  //第幾句改變對話者
                        {
                            NpcName = NewName;
                        }
                    }
                    dialogueText.text = Name[NpcName] + Dialogue[TextLine];  //對話輸出                    
                    TextLine++;
                }
            }
            else
            {
                coolDownTimer += Time.deltaTime;
            }
        }
        if (EndDialogue)  //結束對話
        {
            Delay = 0;
            TextLine = 0;
            EndDialogue = false;
            Talking = false;
            if (Auto)  //自動切換
            {
                PlayerView.TagetChange(0, UI);  //切換任務目標
            }
        }
    }
    //任務關卡, 任務階段, 對話者, 是否自動換任務, 改變對話者(第幾句. 哪位), 是否顯示訊息UI
    public static void StartConversation(int Level , int Stage, int Who, bool auto, float changeName, bool ui) 
    {
        StartDialogue = true;
        NpcName = Who;
        missionLevel = Level;
        missionStage = Stage;
        Auto = auto;
        UI = ui;
        if (changeName != 0)
        {
            ChangeName = changeName;
            ChangeTextLine = (int)ChangeName;
            NewName = (int)(ChangeName - ChangeTextLine) * 10;
        }
    }
    public static void Delayed(int delayTextLine)  //延遲對話(第幾句)
    {
        if (Delay==0)
        {
            Delay = 1;
            DelayTextLine = delayTextLine;
        }
    }
    void Add_Dialogue()  //添加文本
    {
        Name = new string[] { "探勘地主管 : ", "偵查系統 : ", "研究室主管 : " };
        switch (missionLevel) 
        {
            case 0:
                switch (missionStage)
                {
                    case 0:
                        Dialogue = new string[5];
                        Dialogue[0] = "歡迎來到「遺蹟4號探勘地」，我是這裡的主管。";
                        Dialogue[1] = "負責在此監控各地的情況。";
                        Dialogue[2] = "而你的任務是保護這區域不受到任何威脅。";
                        Dialogue[3] = "要進行區域導覽嗎?";
                        Dialogue[4] = "很好，先從門外右方的倉庫介紹。";
                        break;
                    case 1:
                        Dialogue = new string[3];
                        Dialogue[0] = "這裡儲存所有物資與工具。";
                        Dialogue[1] = "從上面運下來的東西都會先進到這裡。";
                        Dialogue[2] = "接著是武器庫，先去隔壁領取武器與彈藥。";
                        break;
                    case 2:
                        Dialogue = new string[2];
                        Dialogue[0] = "很好，已經拿到武器了。";
                        Dialogue[1] = "接下來是彈藥，打開底下的彈藥盒。";
                        break;
                    case 3:
                        Dialogue = new string[4];
                        Dialogue[0] = "這間武器庫存放各種武器與彈藥。";
                        Dialogue[1] = "但目前只授權步槍的使用許可而已。";
                        Dialogue[2] = "等到有新許可下來就到這裡領取。";
                        Dialogue[3] = "再來是隔壁的工作間。";
                        break;
                    case 4:
                        Dialogue = new string[3];
                        Dialogue[0] = "這裡放著倉庫領出的工具。";
                        Dialogue[1] = "還有工作檯能進行改造與維修。";
                        Dialogue[2] = "最後是這裡最重要的地方。";
                        break;
                    case 5:
                        Dialogue = new string[5];
                        Dialogue[0] = "這間核能發電站負責支撐這裡的電力供應。";
                        Dialogue[1] = "產出的電力會輸送到隔壁的蓄電塔保存。";
                        Dialogue[2] = "並由變電站輸出到各個電塔，再分配給各建築。";
                        Dialogue[3] = "所以這裡最為重要，必須優先保護好這裡。";
                        Dialogue[4] = "重點都介紹差不多了，現在該到你的工作崗位。";
                        break;
                }
                break;
            case 1:
                switch (missionStage)
                {
                    case 0:
                        Dialogue = new string[1];
                        Dialogue[0] = "既然不需要就直接到你的工作崗位。";
                        break;           
                }
                break;
            case 2:
                switch (missionStage)
                {
                    case 0:
                        Dialogue = new string[3];
                        Dialogue[0] = "緊急狀態!! 緊急狀態!!";
                        Dialogue[1] = "大門外偵測到大規模怪物入侵。";
                        Dialogue[2] = "變更任務，阻擋怪物入侵。";
                        break;
                    case 1:
                        Dialogue = new string[2];
                        Dialogue[0] = "開放新武器使用許可。";
                        Dialogue[1] = "前往武器庫領取。";
                        break;
                    case 2:
                        Dialogue = new string[2];
                        Dialogue[0] = "每隔一段時間就會開放新武器許可。";
                        Dialogue[1] = "之後都能過來取得。";
                        break;
                    case 3:
                        Dialogue = new string[2];
                        Dialogue[0] = "礦洞偵測到不明生物出現。";
                        Dialogue[1] = "請準備接敵。";
                        break;
                    case 4:
                        Dialogue = new string[3];
                        Dialogue[0] = "成功保衛發電廠了。";
                        Dialogue[1] = "剛剛探勘隊的傳來重大發現。";
                        Dialogue[2] = "接下來請前往研究室。";
                        break;
                    case 5:
                        Dialogue = new string[4];
                        Dialogue[0] = "來的正好,就在剛剛那個不明水晶出現後。";
                        Dialogue[1] = "在下面的遺跡中發現裂口。";
                        Dialogue[2] = "該出發去尋找遺跡的秘密了。";
                        Dialogue[3] = "從研究室後門出去後，搭乘電梯下去吧。";
                        break;
                }
                break;
            case 3:
                switch (missionStage)
                {
                    case 0:
                        Dialogue = new string[1];
                        Dialogue[0] = "大門防線被突破了，快退到第二防線。";
                        break;
                    case 1:
                        Dialogue = new string[1];
                        Dialogue[0] = "第二防線失守，請保護好發電廠。";
                        break;
                    case 2:
                        Dialogue = new string[2];
                        Dialogue[0] = "發電廠被摧毀了。";
                        Dialogue[1] = "撤退。";
                        break;
                }
                break;
            case 4:  //第二關
                switch (missionStage)
                {
                    case 0:
                        Dialogue = new string[4];
                        Dialogue[0] = "真是寬敞的地方。";
                        Dialogue[1] = "先來探索遺跡。";
                        Dialogue[2] = "對了，我剛剛幫你升級過裝甲。";
                        Dialogue[3] = "這樣應該就能抵擋比較多的傷害了。";
                        break;
                    case 1:
                        Dialogue = new string[4];
                        Dialogue[0] = "普通的攻擊無法對機械體造成傷害。";
                        Dialogue[1] = "水晶看起來有辦法擊穿外殼。";
                        Dialogue[2] = "先擊破水晶在攻擊底下的傷口。";
                        Dialogue[3] = "我會把傷口位置用標示出來的。";
                        break;
                    case 2:
                        Dialogue = new string[2];
                        Dialogue[0] = "機械體正在操控機槍砲塔。";
                        Dialogue[1] = "擊毀他或躲避攻擊。";
                        break;
                    case 3:
                        Dialogue = new string[2];
                        Dialogue[0] = "機械體似乎正在釋放巨大能量。";
                        Dialogue[1] = "請盡快阻止。";
                        break;
                    case 4:
                        Dialogue = new string[3];
                        Dialogue[0] = "雖然擊倒機械體，但黑球還在膨脹。";
                        Dialogue[1] = "回去的入口已經被擋住了。";
                        Dialogue[2] = "進入機械體後面的門。";
                        break;
                }
                break;
        }

    }
}
