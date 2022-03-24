using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueOptions : MonoBehaviour
{
    public DialogueEditor DialogueEditor;  //任務對話控制器
    public DailyDialogue DailyDialogue;  //NPC對話控制器
    public GameObject DialogueOptionsUI;  //對話選擇UI
    public int missionLevel;  //任務關卡
    public Text[] OptionText;
    public string[] OptionA;
    public string[] OptionB;
    public static int Task=0;
    public static int Name;
    public static bool StartOp;
    public GameObject Weap;

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
        //missionLevel = PlayerView.missionLevel;
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
        if (Task == 0)  //當然 (進行教學)
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
        if (Task == 0)   //下次一定 (跳過教學)
        {
            Shooting.PickUpWeapons(0, 0, Weap);
            Weap.SetActive(false);
            GameObject play = GameObject.Find("POPP").gameObject;
            Weap.transform.parent = play.gameObject.transform;  //變為子物件到玩家身上
            Weap.transform.position = play.gameObject.transform.position;  //位置歸零
            AudioManager.PickUp(2);
            Shooting.PickUpAmm(1);
            Shooting.SkipTeach = true;
            DialogueEditor.TextLine = 0;  //對話句子數歸零
            DialogueEditor.coolDownTimer = DialogueEditor.coolDown;  //重置對話冷卻時間
            PlayerView.missionChange(1, 0);  //改變關卡
            DialogueEditor.StartConversation(1, 0, 0, false, 0);  //跳到對應的對話階段
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
