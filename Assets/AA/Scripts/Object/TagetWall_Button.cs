using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TagetWall_Button : MonoBehaviour
{
    public GameObject TextG;
    public bool Botton;
    bool StartAni;
    public int Type;
    string[] Text= new string[] {"10m", "20m" };
    public GameObject TagetWall;
    Vector3 TW;

    void Start()
    {
        TextG = GameObject.Find("ObjectText");
        Botton = false;
        Type = 1;
    }

    void Update()
    {
        if (Botton)
        {

            TW = TagetWall.transform.localPosition;
            if (Type == 0)
            {
                TW.x += 10 * Time.deltaTime;
                if(TW.x >= 34.41)
                {
                    TW.x = 34.41f;
                    Botton = false;
                }
            }
            else if(Type==1)
            {
                TW.x -= 10 * Time.deltaTime;
                if(TW.x <= -0.13)
                {
                    TW.x = -0.13f;
                    Botton = false;
                }
            }
            TagetWall.transform.localPosition = TW;
        }
    }
    void HitByRaycast() //被射線打到時會進入此方法
    {
        TextG.GetComponent<Text>().text = "按「E」切換距離\n" + Text[Type];

        if (Input.GetKeyDown(KeyCode.E)) //當按下鍵盤 E 鍵時
        {
            if (!Botton)
            {
                Botton = true;
                if (Type == 0)
                {                    
                    Type = 1;
                }
                else
                {
                    Type = 0;
                }
            }       
        }
    }
}
