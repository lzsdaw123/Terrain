using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_interaction : MonoBehaviour
{
    [SerializeField] private GameObject Npc;
    [SerializeField] private int NpcName;
    [SerializeField] private bool ReDialogue;
    [SerializeField] private float distance;  //距離
    public static float st_distance;  //距離
    [SerializeField] Camera Camera;
    [SerializeField] Transform camTransform;
    public static bool StartDialogue = true;
    [SerializeField] bool Beside = true;  //是否在旁邊

    void Start()
    {
    }

    void Update()
    {
        camTransform = Camera.transform;  //相機座標
        distance = (camTransform.position - this.transform.position).magnitude / 3.5f;
        st_distance = distance;

        if (distance <= 1.2f)
        {
            if (StartDialogue)
            {
                StartDialogue = false;
                Beside = true;
                DailyDialogue.DD(NpcName,  true);
                DailyDialogue.StartConversation(0, NpcName, ReDialogue);  //開始對話
            }
        }
        else
        {
            if (Beside)
            {
                Beside = false;
                DailyDialogue.DD(NpcName, false);
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
}
