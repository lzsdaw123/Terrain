using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bloodpack : MonoBehaviour
{
    public GameObject ObjectText;
    bool StartB;
    [SerializeField] GameObject Take;  //互動圖示UI

    void Awake()
    {
    }
    void Start()
    {
        Take = Save_Across_Scene.Take;
        ObjectText = Save_Across_Scene.ObjectText;
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
            ObjectText.GetComponent<Text>().text = "修理包已達上限\n" + "按「Q」 使用";
            QH_interactive.thing();  //呼叫QH_拾取圖案
        }
        else
        {
            ObjectText.GetComponent<Text>().text = "取得修理包\n"+ "按「Q」 使用";
            QH_interactive.thing();  //呼叫QH_拾取圖案

            if (Take.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.E)) //當按下鍵盤 E 鍵時
                {
                    AudioManager.PickUp(3);
                    HeroLife.GetBloodpack();
                    gameObject.SetActive(false);
                    StartB = true;
                }
            }
        }

    }
}
