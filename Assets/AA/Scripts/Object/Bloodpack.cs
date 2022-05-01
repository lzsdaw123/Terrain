using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bloodpack : MonoBehaviour
{
    public GameObject T;
    bool StartB;
    [SerializeField] GameObject Take;  //互動圖示UI

    void Awake()
    {
        Take = GameObject.Find("Take");
    }
    void Start()
    {
        T = GameObject.Find("ObjectText");
        StartB = false;
    }

    void Update()
    {
        //if (Take.activeSelf && StartB)
        //{
        //    HeroLife.AddHp();
        //}
        //if (!Take.activeSelf)
        //{
        //    AudioManager.PickUp(-1);
        //}
        //if (Input.GetKeyUp(KeyCode.E))
        //{
        //    StartB = false;
        //    AudioManager.PickUp(-1);
        //}

    }
    void HitByRaycast() //被射線打到時會進入此方法
    {
        if (HeroLife.BloodpackNub >= 3)
        {
            T.GetComponent<Text>().text = "修理包已達上限";
            QH_interactive.thing();  //呼叫QH_拾取圖案
        }
        else
        {
            T.GetComponent<Text>().text = "按「E」取得修理包";
            QH_interactive.thing();  //呼叫QH_拾取圖案

            if (Take.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.E)) //當按下鍵盤 E 鍵時
                {
                    AudioManager.PickUp(0);
                    HeroLife.GetBloodpack();
                    gameObject.SetActive(false);
                    StartB = true;
                }
            }
        }

    }
}
