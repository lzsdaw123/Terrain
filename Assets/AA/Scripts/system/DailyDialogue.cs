using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyDialogue : MonoBehaviour  //NPC日常對話控制器
{
    public static bool Ra_Dialogue;
    public float coolDown; //冷卻結束時間
    public float coolDownTimer; //冷卻時間計時器
    public Text dialogueText;  //對話輸出
    public static int NpcName; //對話者
    public GameObject[] Npc;
    [SerializeField] string[] Name;  //對話者名子
    [SerializeField] string[] Dialogue;  //對話句子
    public static int TextLine=0;  //當前對話行數
    public static bool StartDialogue;
    public static bool EndDialogue;

    public static int missionLevel = 0;
    public static bool[] Beside;  //是否在旁邊
    [SerializeField] private bool[] SF_Beside;

    //public GameObject DialogueOptionsUI;  //對話選擇UI
    bool teaching;
    public static bool ActiveDialogue;  //主動對話
    public int Length;  //字串字元數
    static bool reAdd_Dialogue;

    void Start()
    {
        coolDown = 2.5f;  //冷卻結束時間
        coolDownTimer = coolDown + 1;
        Beside = new bool[3];
        dialogueText.text = "";
        //if (DialogueOptionsUI == null)
        //{
        //    DialogueOptionsUI = GameObject.Find("DialogueOptionsUI").gameObject;
        //}
    }
    void stopDialogue()
    {
        StartDialogue = false;
        EndDialogue = true;
        dialogueText.text = "";
        coolDownTimer = coolDown + 1;
    }
    public static void NearNPC(int Who, bool beside)
    {
        Beside[Who] = beside;
    }
    void Update()
    {
        SF_Beside = Beside;
        if (StartDialogue)  //開始對話
        {
            //Beside = NPC_interaction.Beside;
            if (ActiveDialogue)  //是否主動對話
            {
                ActiveDialogue = false;
                Add_Dialogue(false);  //添加文本
                TextLine = 0;
                dialogueText.text = Name[NpcName] + Dialogue[TextLine];
                coolDownTimer = coolDown;
                DialogueOptions.StartOption(1, NpcName);  //呼叫對話選項(1 非任務, NPC)
            }
            if (coolDownTimer >= coolDown) //開火冷卻時間，與coolDown 差越小越快
            {
                Add_Dialogue(Ra_Dialogue);  //添加文本
                coolDownTimer = 0;  //重置短對話冷卻
                if (TextLine < Dialogue.Length)
                {
                    Length = Dialogue[TextLine].Length;
                    if (Length >= 18) coolDownTimer = -0.8f;  //重置長對話冷卻
                    if (Length >= 26) coolDownTimer = -1.6f;  //重置更長對話冷卻
                }

                if (!Beside[NpcName])  //中斷對話
                {
                    stopDialogue();
                    return;
                }
                if (Ra_Dialogue)  //是否為隨機對話
                {
                    TextLine = Random.Range(0, Dialogue.Length);
                    dialogueText.text = Name[NpcName] + Dialogue[TextLine];
                }
                else
                {
                    if (TextLine >= Dialogue.Length)
                    {
                        stopDialogue();
                    }
                    else
                    {
                        dialogueText.text = Name[NpcName] + Dialogue[TextLine];
                        TextLine++;
                    }
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
            NPC_interaction.EndDialogue();  //
        }
    }
    public static void StartConversation(int level, int Who, bool Range, bool activeDialogue)  //任務階段, 對話者, 隨機對話, 可互動
    {
        ActiveDialogue = activeDialogue;
        StartDialogue = true;
        Ra_Dialogue = Range;
        NpcName = Who;
        missionLevel = level;
    }
    void Add_Dialogue(bool Ra_Dialogue)
    {
        Name = new string[] { "武器庫管理員 : ", "核電廠工程師 : ", "研究人員 : " };

        switch (NpcName)
        {
            case 0:  //武器庫管理員
                Dialogue = new string[3];
                Dialogue[0] = "小子，我以前和你一樣也是個守衛，直到我的膝蓋中了一槍。";
                Dialogue[1] = "你還沒準備好，兄弟。";
                Dialogue[2] = "你的惡行從愛爾蘭到契丹，無人不知，無人不曉。";
                break;
            case 1:  //核電廠工程師
                if (Ra_Dialogue)
                {
                    Dialogue = new string[3];
                    Dialogue[0] = "核分裂就是力量，朋友。";
                    Dialogue[1] = "我們總是重複同樣的錯誤。";
                    Dialogue[2] = "燃燒吧，反應爐。";
                }
                else
                {
                    Dialogue = new string[7];
                    Dialogue[0] = "想知道這裡是如何發電的嗎?";
                    Dialogue[1] = "簡單來說就是先將燃料棒放入「反應堆」後產生熱能。";
                    Dialogue[2] = "熱能使「冷凝器」來的水加熱到產生蒸氣。";
                    Dialogue[3] = "蒸氣轉動「汽輪機」裡的渦輪產生動能。";
                    Dialogue[4] = "並連接著「發電機組」，最終產生電能。";
                    Dialogue[5] = "而蒸氣則進到「冷凝器」冷卻為水再次循環到「反應堆」。";
                    Dialogue[6] = "「冷凝器」則不斷抽取「水泵」的水進行降溫。";                
                }
                break;
            case 2:  //研究人員
                Dialogue = new string[3];
                Dialogue[0] = "到底還有多少Bug要修阿!!!";
                Dialogue[1] = "兄弟，水晶真漂亮對吧。";
                Dialogue[2] = "一隻蟲子、兩隻蟲子、三隻蟲子...";
                break;
        }
    }
}
