using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueEditor : MonoBehaviour
{
    public float coolDown; //冷卻結束時間
    public float coolDownTimer; //冷卻時間計時器
    public Text dialogueText;
    [SerializeField] string[] Name;
    [SerializeField] string[] Dialogue;
    static int spokesman;
    //[SerializeField] int NameNum;  //對話者數量
    [SerializeField] int LineNum;  //句子數量
    [SerializeField] int TextLine;  //當前對話行數
    static int Level =0;
    public static bool StartDialogue;
    public static bool EndDialogue;

    void Start()
    {
        coolDown = 2.2f;  //冷卻結束時間
        coolDownTimer = coolDown+1;
        StartDialogue = EndDialogue = false;
        dialogueText.text = "";
        TextLine = 0;
        Add_Dialogue();     
    }
    void Add_Dialogue()
    {
        Name = new string[1];
        Name[0] = "探勘地主管 : ";
        switch (Level)
        {
            case 0:
                Dialogue = new string[4];
                Dialogue[0] = "歡迎來到「4號探勘地」，我是這裡的主管。";
                Dialogue[1] = "這間主管室負責監控各地的的情況。";
                Dialogue[2] = "你的任務就是保護這區域不受到任何威脅。";
                Dialogue[3] = "旁邊有武器庫，先去領取武器。";
                break;
            case 1:
                Dialogue = new string[2];
                Dialogue[0] = "很好，看來你已經拿到武器了。";
                Dialogue[1] = "是時候執行你的工作了。";
                break;
        }
    }
    void Update()
    {
        if (StartDialogue)  //開始對話
        {
            if (coolDownTimer >= coolDown) //開火冷卻時間，與coolDown 差越小越快
            {
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
            switch (Level)
            {
                case 0:
                    PlayerView.TagetChange();
                    break;
                case 1:
                    break;
            }
        }
    }
    public static void StartConversation(int level, int Who)  //任務階段, 對話者
    {
        StartDialogue = true;
        spokesman = Who;
        Level = level;
    }
}
