using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grenade : MonoBehaviour
{
    public GameObject T;
    bool StartB;
    [SerializeField] GameObject Take;  //互動圖示UI
    public GameObject GrenadeObj;
    public GameObject Play;

    void Awake()
    {
        Take = GameObject.Find("Take");
    }
    void Start()
    {
        T = GameObject.Find("ObjectText");
        StartB = false;
        GrenadeObj = gameObject;
    }

    void Update()
    {

    }
    void HitByRaycast() //被射線打到時會進入此方法
    {
        if (Shooting.GrenadeNub >= 2)
        {
            T.GetComponent<Text>().text = "手榴彈已達上限";
            QH_interactive.thing();  //呼叫QH_拾取圖案
        }
        else
        {
            T.GetComponent<Text>().text = "按「E」取得手榴彈";
            QH_interactive.thing();  //呼叫QH_拾取圖案

            if (Take.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.E)) //當按下鍵盤 E 鍵時
                {
                    AudioManager.PickUp(0);
                    Play = GameObject.Find("POPP").gameObject;
                    gameObject.transform.parent = Play.gameObject.transform;
                    Shooting.Get_Grenade(GrenadeObj);
                    gameObject.SetActive(false);
                    StartB = true;
                }
            }
        }

    }
}
