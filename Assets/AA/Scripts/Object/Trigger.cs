using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public int Level;
    public int Type;  //0=�Ĥ@��, 1=�ĤG��
    public int Features;  //�\��
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
    public void OnTriggerEnter(Collider other)  //Ĳ�o���d
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
                    case 1:  //Boss2����1�d��
                        boss02_AI.AttackRange = 1;
                        break;
                    case 2:  //Boss2����1�d��k�䦺��
                        boss02_AI.AttackRange = 2;
                        break;
                    case 3:  //Boss2����1�d���䦺��
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
                    case 1:  //Boss2����1 �d��
                        boss02_AI.AttackRange = 1;
                        break;
                    case 2:  //Boss2����1 �d��k�䦺��
                        boss02_AI.AttackRange = 2;
                        break;
                    case 3:  //Boss2����1 �d���䦺��
                        boss02_AI.AttackRange = 3;
                        break;
                    case 4:  //Boss2����2 ���j�d�򦺨�
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
                    case 4:  //Boss2����2 ���j�d�򦺨�
                        mg_Turret_AI.AttackRange = 0;
                        break;
                }
            }
        }
    }
}
