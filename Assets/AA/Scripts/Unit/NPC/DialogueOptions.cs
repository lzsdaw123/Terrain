using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueOptions : MonoBehaviour
{
    public DialogueEditor DialogueEditor;  //任務對話控制器
    public DailyDialogue DailyDialogue;  //NPC對話控制器
    public GameObject DialogueOptionsUI;  //對話選擇UI
    public Text[] OptionText;
    public string[] OptionA;
    public string[] OptionB;
    public static int Task=0;
    public static int Name;
    public static bool StartOp;

    void Start()
    {
        OptionA = new string[2] { "當然", "說來聽聽" };
        OptionB = new string[1] { "下次一定" };
    }

    void Update()
    {
        if (StartOp)
        {
            StartOp = false;
            DialogueOptionsUI.SetActive(true);
        }
        switch (Task)
        {
            case 0:
                OptionText[0].text = OptionA[0];
                OptionText[1].text = OptionB[0];
                break;
            case 1:
                OptionText[0].text = OptionA[1];
                OptionText[1].text = OptionB[0];
                break;
        }
    }
    public static void StartOption(int task, int Who)  //開始選項(0 任務 / 1 非任務, NPC)
    {
        Settings.pause();
        StartOp = true;        
        Task = task;
        Name = Who;
    }
    public void DialogueOptionA()  //選項A
    {
        Settings.con();
        if (Task == 0)  //進行教學
        {
            Shooting.SkipTeach = false;
            DialogueEditor.coolDownTimer = DialogueEditor.coolDown;
        }
        if (Task == 1) //Yes
        {
            DailyDialogue.coolDownTimer = DailyDialogue.coolDown;
        }
    }
    public void DialogueOptionB()  //選項B
    {
        Settings.con();
        if (Task == 0)   //跳過教學
        {
            Shooting.PickUpWeapons(0, 0, gameObject);
            Shooting.FirstAmm = true;
            Shooting.SkipTeach = true;
            PlayerView.missionLevel = 5;  //對話階段
            DialogueEditor.TextLine = 0;  //對話句子數歸零
            DialogueEditor.coolDownTimer = DialogueEditor.coolDown;  //重置對話冷卻時間
            Level_1.UiOpen = true;
            DialogueEditor.StartConversation(PlayerView.missionLevel, 0);  //跳到對應的對話階段
        }
        if (Task == 1) //No
        {
            //DailyDialogue.TextLine = 0;  //對話句子數歸零
            DailyDialogue.coolDownTimer = DialogueEditor.coolDown;  //重置對話冷卻時間
            DailyDialogue.dialogueText.text = "";
            NPC_interaction.EndDialogue();
        }
    }
}
