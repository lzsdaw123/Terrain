using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyDialogue : MonoBehaviour
{
    public int NpcType;
    public static bool Ra_Dialogue;
    public float coolDown; //冷卻結束時間
    public float coolDownTimer; //冷卻時間計時器
    public Text dialogueText;  //對話輸出
    public GameObject[] Npc;
    [SerializeField] string[] Name;  //對話者
    [SerializeField] string[] Dialogue;  //對話句子
    public static int TextLine=0;  //當前對話行數
    public static bool StartDialogue;
    public static bool EndDialogue;
    static int spokesman;
    public static int missionLevel = 0;
    public static bool[] Beside;  //是否在旁邊
    [SerializeField] private bool[] SF_Beside;

    void Start()
    {
        coolDown = 2.5f;  //冷卻結束時間
        coolDownTimer = coolDown + 1;
        Beside = new bool[2];
    }
    void stopDialogue()
    {
        StartDialogue = false;
        EndDialogue = true;
        dialogueText.text = "";
        coolDownTimer = coolDown + 1;
    }
    public static void DD(int Who, bool beside)
    {
        Beside[Who] = beside;
    }
    void Update()
    {
        SF_Beside = Beside;
        if (StartDialogue)  //開始對話
        {
            //Beside = NPC_interaction.Beside;
            //print(Beside);
            if (coolDownTimer >= coolDown) //開火冷卻時間，與coolDown 差越小越快
            {
                Add_Dialogue();  //貼加文本
                coolDownTimer = 0;
                
                if (!Beside[spokesman])  //中斷對話
                {
                    stopDialogue();
                    return;
                }
                if (Ra_Dialogue)  //是否為隨機對話
                {
                    coolDown = 3.2f;
                    TextLine = Random.Range(0, Dialogue.Length);
                    dialogueText.text = Name[spokesman] + Dialogue[TextLine];
                }
                else
                {
                    if (TextLine >= Dialogue.Length)
                    {
                        stopDialogue();
                    }
                    else
                    {
                        dialogueText.text = Name[spokesman] + Dialogue[TextLine];
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
    public static void StartConversation(int level, int Who, bool Range)  //任務階段, 對話者, 隨機對話
    {
        StartDialogue = true;
        Ra_Dialogue = Range;
        spokesman = Who;
        missionLevel = level;
    }
    void Add_Dialogue()
    {
        Name = new string[2];
        Name[0] = "武器庫管理員 : ";
        Name[1] = "核電廠工程師 : ";

        switch (spokesman)
        {
            case 0:  //武器庫管理員
                Dialogue = new string[3];
                Dialogue[0] = "小子，我以前和你一樣也是個探勘區警衛，直到我的膝蓋中了一箭。";
                Dialogue[1] = "你還沒準備好，兄弟。";
                Dialogue[2] = "你的惡行從愛爾蘭到契丹無人不知,無人不曉。";
                //Dialogue[3] = "蒸氣轉動「汽輪機」裡的渦輪產生動能。";
                //Dialogue[4] = "並連接著「發電機組」，最終產生電能。";
                //Dialogue[5] = "而蒸氣則進到「冷凝器」冷卻為水再次循環到「反應堆」。";
                //Dialogue[6] = "「冷凝器」則不斷抽取「水泵」的水進行降溫。";
                break;
            case 1:  //核電廠工程師
                Dialogue = new string[7];
                Dialogue[0] = "想知道這裡是如何發電的嗎?";
                Dialogue[1] = "簡單來說就是先將燃料棒放入「反應堆」後產生熱能。";
                Dialogue[2] = "熱能使「冷凝器」來的水加熱到產生蒸氣。";
                Dialogue[3] = "蒸氣轉動「汽輪機」裡的渦輪產生動能。";
                Dialogue[4] = "並連接著「發電機組」，最終產生電能。";
                Dialogue[5] = "而蒸氣則進到「冷凝器」冷卻為水再次循環到「反應堆」。";
                Dialogue[6] = "「冷凝器」則不斷抽取「水泵」的水進行降溫。";
                break;
        }
    }
}
