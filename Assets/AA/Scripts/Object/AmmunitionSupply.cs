using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmunitionSupply : MonoBehaviour
{
    public GameObject T;

    void Start()
    {
        T = GameObject.Find("ObjectText");
    }

    // Update is called once per frame
    void Update()
    {

    }
    void HitByRaycast() //被射線打到時會進入此方法
    {
        T.GetComponent<Text>().text = "按E拾取彈藥";

        if (Input.GetKeyDown(KeyCode.E)) //當按下鍵盤 E 鍵時
        {
            if (Shooting.Total_ammunition < 150)
            {
                print("彈藥補給");
                Shooting.Total_ammunition = 150;
            }
        }
    }
}
