using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bloodpack : MonoBehaviour
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
        T.GetComponent<Text>().text = "按E修理\n";
        QH_interactive.thing();  //呼叫QH_拾取圖案

        if (Input.GetKeyDown(KeyCode.E)) //當按下鍵盤 E 鍵時
        {
            AudioManager.PickUp(1);
            HeroLife.AddHp(true);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            AudioManager.PickUp(-1);
            HeroLife.AddHp(false);
        }
    }
}
