using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmunitionSupply : MonoBehaviour
{
    public GameObject T;
    int Total_ammunition;
    public int AmmSupply;
    GameObject ASupply;

    void Start()
    {
        T = GameObject.Find("ObjectText");
        Total_ammunition = Shooting.Total_ammunition;
        ASupply = gameObject.transform.GetChild(0).gameObject;
        AmmSupply = 480;
    }

    // Update is called once per frame
    void Update()
    {
        if (AmmSupply <= 0)
        {
            AmmSupply = 0;
            ASupply.SetActive(false);
        }

    }
    void HitByRaycast() //被射線打到時會進入此方法
    {
        T.GetComponent<Text>().text = "按E拾取彈藥\n"+"彈藥量 "+ AmmSupply;
        QH_interactive.thing();  //呼叫QH_拾取圖案

        if (Input.GetKeyDown(KeyCode.E)) //當按下鍵盤 E 鍵時
        {
            if (Shooting.Total_ammunition < Total_ammunition)
            {
                //print("彈藥補給");
                AmmSupply = AmmSupply - (Total_ammunition - Shooting.Total_ammunition);
                Shooting.Total_ammunition = Total_ammunition;                           
            }
        }
    }
}
