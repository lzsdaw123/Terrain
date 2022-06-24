using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public int Level;
    public int Type;  //0=第一關, 1=第二關
    public int Features;  //功能
    public GameObject[] Objects;
    public Boss02_AI boss02_AI;
    public Level_1 level_1;
    public MG_Turret_AI mg_Turret_AI;
    public MG_Turret_AI[] mg_Turret_AI_S;
    //public Level_1 level_1;

    public float time;
    public bool StartTime;

    void Start()
    {
        StartTime = false;
        time = 0;
    }

    void Update()
    {
        if (StartTime)
        {
            time += Time.deltaTime;
            if (time>2)
            {
                time = 0;
                StartTime = false;
                switch (Features)
                {
                    case 6:
                        Level_1.LevelB_ = 2;
                        PlayerView.Stop = false;  //UI隱藏
                        PlayerView.UI_Stop = false;
                        PlayerView.missionChange(4, 0);  //改變關卡
                        DialogueEditor.StartConversation(4, 0, 2, false, 0, true);  //開始對話
                        Level_1.UiOpen = true;
                        gameObject.SetActive(false);
                        break;
                }
            }
        }

    }
    public void OnTriggerEnter(Collider other)  //觸發關卡
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Actor"))
        {
            if (other.tag == "Player")
            {
                switch (Features)
                {
                    case 0:
                        switch (Level)
                        {
                            case 0:
                                break;
                            case 1:
                                boss02_AI.Player = other.gameObject;
                                break;
                        }
                        GameObject.Find("Level_1").GetComponent<Level_1>().MissionTrigger(Level, Objects[Type]);
                        gameObject.SetActive(false);
                        break;
                    case 1:  //Boss2攻擊1範圍
                        boss02_AI.AttackRange = 1;
                        break;
                    case 2:  //Boss2攻擊1範圍右邊死角
                        boss02_AI.AttackRange = 2;
                        break;
                    case 3:  //Boss2攻擊1範圍左邊死角
                        boss02_AI.AttackRange = 3;
                        break;
                    case 6:
                        StartTime = true;
                        break;
                    case 7:
                        Objects[0].GetComponent<ElectricDoor>().Animator.SetBool("Open", true);
                        switch (Type)
                        {
                            case 0:
                                PlayerView.missionChange(4, 5);  //改變關卡
                                Level_1.UiOpen = true;
                                break;
                        }
                        break;
                    case 8:
                        Shooting.JumpDown=1;
                        gameObject.SetActive(false);
                        break;
                    case 9:  //跳下去
                        Shooting.JumpDown = 3;
                        Objects[0].GetComponent<MeshCollider>().enabled = false;
                        Objects[1].GetComponent<MeshCollider>().enabled = false;
                        Objects[2].SetActive(false);
                        Objects[3].SetActive(false);
                        Objects[4].SetActive(true);
                        gameObject.SetActive(false);
                        break;
                    case 10:
                        Shooting.JumpDown = 4;
                        Save_Across_Scene.heroLife.closeDamageEffects();
                        Save_Across_Scene.Shooting.closeFireEffects();
                        gameObject.SetActive(false);
                        break;
                }
            }
        }
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Actor"))
        {
            if (other.tag == "Player")
            {
                switch (Features)
                {
                    case 1:  //Boss2攻擊1 範圍
                        boss02_AI.AttackRange = 1;
                        break;
                    case 2:  //Boss2攻擊1 範圍右邊死角
                        boss02_AI.AttackRange = 2;
                        break;
                    case 3:  //Boss2攻擊1 範圍左邊死角
                        boss02_AI.AttackRange = 3;
                        break;
                    case 4:  //Boss2攻擊2 機槍範圍死角
                        mg_Turret_AI.InAttackRange[0] = false;
                        break;
                    case 5:  //Boss2攻擊2 機槍範圍內
                        for(int i=0; i< mg_Turret_AI_S.Length; i++)
                        {
                            mg_Turret_AI_S[i].InAttackRange[1] = true;
                        }
                        break;
                }
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Actor"))
        {
            if (other.tag == "Player")
            {
                switch (Features)
                {
                    case 4:  //Boss2攻擊2 機槍範圍內
                        mg_Turret_AI.InAttackRange[0] = true;
                        break;
                    case 7:
                        Objects[0].GetComponent<ElectricDoor>().Animator.SetBool("Open", false);
                        break;
                }
            }
        }
    }
}
