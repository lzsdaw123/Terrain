using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public static int missionLevel = 0;
    public static bool StartDialogue;
    public static bool EndDialogue;
    public GameObject DialogueOptionsUI;  //對話選擇UI
    bool teaching;
    public bool StartTrach;
    public bool Delay=false;  //對話延遲
    public int Length;   //字串字元數

    void Start()
    {
        coolDown = 2.4f;  //冷卻結束時間
        coolDownTimer = coolDown+1;
        StartDialogue = EndDialogue = false;
        dialogueText.text = "";
        TextLine = 0;
        if (DialogueOptionsUI == null)
        {
            DialogueOptionsUI = GameObject.Find("DialogueOptionsUI").gameObject;
        }
        Add_Dialogue();     
    }
    void Update()
    {
        SF_TextLine = TextLine;
        if (StartDialogue)  //開始對話
        {
            if (coolDownTimer >= coolDown) //對話冷卻時間，coolDown 越小越快
            {
                if (StartTrach)  //是否跳過開場教學
                {
                    if (!teaching && TextLine == 3)
                    {
                        teaching = true;
                        DialogueOptions.StartOption(0, NpcName);  //呼叫對話選項(0 任務, NPC)
                        //DialogueOptionsUI.SetActive(true);
                        //Settings.pause();
                    }
                }              
                Add_Dialogue();  //呼叫對話文本
                if (missionLevel == 4 && TextLine==4  && !Delay)  //延遲對話
                {
                    dialogueText.text = "";
                    coolDownTimer = 1f;  //再冷卻一次
                    Delay = true;
                    return;
                }
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
                    Delay = false;
                    dialogueText.text = Name[NpcName] + Dialogue[TextLine];
                    
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
            TextLine = 0;
            EndDialogue = false;
            if (Level_1.LevelA_ <=0)
            {
                PlayerView.TagetChange();  //任務目標切換
            }
        }
    }
    public static void StartConversation(int level, int Who)  //任務階段, 對話者
    {
        StartDialogue = true;
        NpcName = Who;
        missionLevel = level;
    }
    void Add_Dialogue()  //添加文本
    {
        Name = new string[1];
        Name[0] = "探勘地主管 : ";
        switch (missionLevel)
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
                Dialogue = new string[5];
                Dialogue[0] = "很好，看來你已經拿到武器了。";
                Dialogue[1] = "這間武器庫存放各種武器與彈藥。";
                Dialogue[2] = "但目前只授權步槍的使用許可而已。";
                Dialogue[3] = "等到有新許可下來就到這裡領取。";
                Dialogue[4] = "再來是隔壁的工作間。";
                break;
            case 3:
                Dialogue = new string[3];
                Dialogue[0] = "這裡放著倉庫領出的工具。";
                Dialogue[1] = "還有工作檯能進行改造與維修。";
                Dialogue[2] = "最後是這裡最重要的地方。";
                break;
            case 4:
                Dialogue = new string[5];
                Dialogue[0] = "這間核能發電站負責支撐這裡的電力供應。";
                Dialogue[1] = "產出的電力會輸送到隔壁的蓄電塔保存。";
                Dialogue[2] = "並由變電站輸出到各個電塔，再分配給各建築。";
                Dialogue[3] = "所以這裡最為重要，必須優先保護好這裡。";
                Dialogue[4] = "重點都介紹差不多了，現在該到你的工作崗位。";
                break;
            case 5:
                Dialogue = new string[1];
                Dialogue[0] = "既然不需要就直接到你的工作崗位。";
                break;
            case 6:
                Dialogue = new string[2];
                Dialogue[0] = "任務變更，現在開始阻擋怪物入侵。";
                Dialogue[1] = "要防住大門防線。";
                break;
            case 7:
                Dialogue = new string[1];
                Dialogue[0] = "大門防線被突破了，快退到第二防線。";
                break;
            case 8:
                Dialogue = new string[1];
                Dialogue[0] = "第二防線失守，請保護好發電廠。";
                break;
            case 9:
                Dialogue = new string[1];
                Dialogue[0] = "成功保衛發電廠了。";
                break;
        }
    }
}
