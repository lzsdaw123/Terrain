using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public int Level;
    public int Type;
    public int Features;  //¥\¯à
    public GameObject[] Objects;
    public Boss02_AI boss02_AI;
    //public Level_1 level_1;

    void Start()
    {
    }

    void Update()
    {
        
    }
    public void OnTriggerEnter(Collider other)  //Ä²µoÃö¥d
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
                    case 1:  //Boss2§ðÀ»1½d³ò
                        boss02_AI.AttackRange = 1;
                        break;
                    case 2:  //Boss2§ðÀ»1½d³ò¥kÃä¦º¨¤
                        boss02_AI.AttackRange = 2;
                        break;
                    case 3:  //Boss2§ðÀ»1½d³ò¥ªÃä¦º¨¤
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
                    case 1:  //Boss2§ðÀ»1½d³ò
                        boss02_AI.AttackRange = 1;
                        break;
                    case 2:  //Boss2§ðÀ»1½d³ò¥kÃä¦º¨¤
                        boss02_AI.AttackRange = 2;
                        break;
                    case 3:  //Boss2§ðÀ»1½d³ò¥ªÃä¦º¨¤
                        boss02_AI.AttackRange = 3;
                        break;
                }
            }
        }
    }
}
