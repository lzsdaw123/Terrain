using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : MonoBehaviour
{
    public Collider target;
    public int defense;
    public GameObject[] defenseOb;

    void Awake()
    {
        defenseOb = new GameObject[3];
        defenseOb[0] = GameObject.Find("defense_1").gameObject;
        defenseOb[1] = GameObject.Find("defense_2").gameObject;
        defenseOb[2] = GameObject.Find("defense_3").gameObject;
    }
    void Start()
    {
        defenseOb[0].SetActive(true);
        defenseOb[1].SetActive(false);
        defenseOb[2].SetActive(false);
        
    }
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            switch (defense)
            {
                case 0:
                    defenseOb[0].SetActive(false);
                    defenseOb[1].SetActive(true);
                    DialogueEditor.StartConversation(7, 0);
                    print("0");
                    break;
                case 1:
                    defenseOb[1].SetActive(false);
                    defenseOb[2].SetActive(true);
                    print("1");
                    break;
                case 2:
                    defenseOb[2].SetActive(false);
                    //defenseOb[2].SetActive(true);
                    print("2");
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
