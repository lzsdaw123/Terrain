using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC_interaction : MonoBehaviour  //NPC互動
{
    [SerializeField] private int NpcName;  //對話者
    static int st_NpcName;  //對話者
    [SerializeField] string[] Name;  //對話者名子
    [SerializeField] private bool RaDialogue;  //隨機對話
    static bool st_RaDialogue;  //隨機對話
    [SerializeField] private bool interact;  //是否可互動
    [SerializeField] private float distance;  //距離
    public static float st_distance;  //距離
    [SerializeField] Camera Camera;
    [SerializeField] Transform camTransform;
    public static bool StartDialogue = true;
    [SerializeField] bool Beside = true;  //是否在旁邊

    public GameObject TextG;  //UI
    [SerializeField] GameObject Take;


    void HitByRaycast() //被射線打到時會進入此方法
    {
        if (interact)
        {
            TextG.GetComponent<Text>().text = "按「E」對話\n" + Name[NpcName];
            QH_interactive.thing();  //呼叫QH_拾取圖案

            if (Take.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.E)) //當按下鍵盤 E 鍵時
                {
                    DailyDialogue.StartConversation(0, NpcName, false, interact);
                }
            }
        }
        else
        {
            TextG.GetComponent<Text>().text = Name[NpcName];
        }
    }
    void Start()
    {
        TextG = GameObject.Find("ObjectText");
        Take = GameObject.Find("Take");
        Name = new string[] { "武器庫管理員", "核電廠工程師" };
    }
    void Update()
    {
        st_NpcName = NpcName;
        st_RaDialogue = RaDialogue;

        if (Camera == null)
        {
            Camera = GameObject.Find("Gun_Camera").gameObject.GetComponent<Camera>();
        }
        camTransform = Camera.transform;  //相機座標
        distance = (camTransform.position - this.transform.position).magnitude / 3.5f;
        st_distance = distance;

        if (distance <= 1.2f)  //靠近NPC
        {
            if (StartDialogue)
            {
                StartDialogue = false;
                Beside = true;
                DailyDialogue.NearNPC(NpcName,  true);
                DailyDialogue.StartConversation(0, NpcName, RaDialogue, false);  //開始對話
            }
        }
        else
        {
            if (Beside)
            {
                Beside = false;
                DailyDialogue.NearNPC(NpcName, false);
            }
            
        }
    }
    public static void EndDialogue()
    {
        StartDialogue = true;

        //if (st_distance <= 1.2f)
        //{
        //    Beside = true;
        //}
        //else
        //{
        //    Beside = false;
        //}
    }
    public static void ReAdd_Dialogue()
    {
        DailyDialogue.StartConversation(0, st_NpcName, st_RaDialogue, false);  //開始對話
    }
}
