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
    //public Level_1 level_1;

    void Start()
    {
    }

    void Update()
    {
        
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
                        boss02_AI.Player = other.gameObject;
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
                        mg_Turret_AI.AttackRange = 1;
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
                    case 4:  //Boss2攻擊2 機槍範圍死角
                        mg_Turret_AI.AttackRange = 0;
                        break;
                }
            }
        }
    }
}
