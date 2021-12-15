using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueEditor : MonoBehaviour
{
    public float coolDown; //冷卻結束時間
    public float coolDownTimer; //冷卻時間計時器
    public Text dialogueText;  //對話輸出
    [SerializeField] string[] Name;  //對話者
    [SerializeField] string[] Dialogue;  //對話句子
    static int spokesman;
    //[SerializeField] int NameNum;  //對話者數量
    public static int TextLine;  //當前對話行數
    public static int missionLevel = 0;
    public static bool StartDialogue;
    public static bool EndDialogue;
    public GameObject teachingUI;
    bool teaching;

    void Start()
    {
        coolDown = 2.3f;  //冷卻結束時間
        coolDownTimer = coolDown+1;
        StartDialogue = EndDialogue = false;
        dialogueText.text = "";
        TextLine = 0;
        Add_Dialogue();     
    }
    void Update()
    {
        if (StartDialogue)  //開始對話
        {
            if (coolDownTimer >= coolDown) //開火冷卻時間，與coolDown 差越小越快
            {
                //if (!teaching && TextLine == 3)
                //{
                //    teaching = true;
                //    teachingUI.SetActive(true);
                //    Settings.pause();
                //}
                Add_Dialogue();
                coolDownTimer = 0;
                if (TextLine >= Dialogue.Length)
                {
                    StartDialogue = false;
                    EndDialogue = true;
                    dialogueText.text = "";
                    coolDownTimer = coolDown + 1;
                }
                else
                {
                    dialogueText.text = Name[spokesman] + Dialogue[TextLine];
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
            PlayerView.TagetChange();  //任務目標切換
        }
    }
    public static void StartConversation(int level, int Who)  //任務階段, 對話者
    {
        StartDialogue = true;
        spokesman = Who;
        missionLevel = level;
    }
    void Add_Dialogue()  //貼加文本
    {
        Name = new string[1];
        Name[0] = "探勘地主管 : ";
        switch (missionLevel)
        {
            case 0:
                Dialogue = new string[6];
                Dialogue[0] = "歡迎來到「4號探勘地」，我是這裡的主管。";
                Dialogue[1] = "負責在此監控各地的的情況。";
                Dialogue[2] = "而你的任務是保護這區域不受到任何威脅。";
                Dialogue[3] = "是否進行區域導覽。";
                Dialogue[4] = "現在開始進行區域導覽。";
                Dialogue[5] = "先從門外右方的倉庫介紹。";
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
                Dialogue = new string[7];
                Dialogue[0] = "這間核能發電站負責支撐這裡的電力供應。";
                Dialogue[1] = "燃料棒進入反應堆後產生熱能。";
                Dialogue[2] = "熱能使水加熱到產生大量蒸氣。";
                Dialogue[3] = "蒸氣轉動汽輪機裡的渦輪產生動能。";
                Dialogue[4] = "並連接著發電機組，最終產生電能。";
                Dialogue[5] = "而蒸氣則進到冷凝器冷卻為水再次循環到反應堆。";
                Dialogue[6] = "冷凝器則不斷抽取水泵的水進行降溫。";
                break;
        }
    }
}
