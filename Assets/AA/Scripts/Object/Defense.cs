using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : MonoBehaviour
{
    public Collider target;
    public int defense;
    public static int ST_A_defense;
    public int A_defense;
    public GameObject[] defenseOb;
    public static GameObject[] st_defenseOb;
    public static int s_Level, s_Stage;

    void Awake()
    {
        defenseOb = new GameObject[3];
        defenseOb[0] = GameObject.Find("defense_1").gameObject;
        defenseOb[1] = GameObject.Find("defense_2").gameObject;
        //defenseOb[2] = GameObject.Find("defense_3").gameObject;
        defenseOb[2] = GameObject.Find("defense_3 (1)").gameObject;
        st_defenseOb = defenseOb;
    }
    void Start()
    {
        defenseOb[0].SetActive(true);
        defenseOb[1].SetActive(false);
        defenseOb[2].SetActive(false);
        A_defense = 0;
    }
    void Update()
    {
        ST_A_defense = A_defense;
        s_Level = 2;
        s_Stage = 0;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            switch (defense)
            {
                case 0:
                    A_defense = 1;
                    defenseOb[0].SetActive(false);
                    defenseOb[1].SetActive(true);
                    s_Level = 3;
                    s_Stage = 0;
                    PlayerView.missionChange(3, 0);  //改變關卡
                    DialogueEditor.StartConversation(3, 0, 0, false, 0);
                    break;
                case 1:
                    A_defense = 2;
                    defenseOb[1].SetActive(false);
                    defenseOb[2].SetActive(true);
                    s_Level = 3;
                    s_Stage = 1;
                    PlayerView.missionChange(3, 1);  //改變關卡
                    DialogueEditor.StartConversation(3, 1, 0, false, 0);
                    break;
                case 2:
                    //defenseOb[2].SetActive(false);
                    //defenseOb[2].SetActive(true);
                    break;
            }
        }


        //print("0");
        //if (other == target)
        //{
        //    print("1");
        //}
    }
}
