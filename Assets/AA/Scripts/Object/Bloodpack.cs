using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bloodpack : MonoBehaviour
{
    public GameObject T;
    bool StartB;
    float time;
    GameObject Take;

    void Start()
    {
        T = GameObject.Find("ObjectText");
        StartB = false;
        Take = GameObject.Find("Take");
    }

    // Update is called once per frame
    void Update()
    {
        if (Take.activeSelf && StartB)
        {
            HeroLife.AddHp();
        }
        if (!Take.activeSelf)
        {
            AudioManager.PickUp(-1);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            StartB = false;
            AudioManager.PickUp(-1);
        }

    }
    void HitByRaycast() //被射線打到時會進入此方法
    {
        T.GetComponent<Text>().text = "按住「E」修理\n";
        QH_interactive.thing();  //呼叫QH_拾取圖案

        if (Take.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.E)) //當按下鍵盤 E 鍵時
            {
                AudioManager.PickUp(1);
                StartB = true;
            }
        }
    }
}
