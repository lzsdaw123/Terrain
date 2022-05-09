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
    public int RangeType;
    string[] Text= new string[] {"10m", "20m" };
    public GameObject TagetWall;
    public GameObject UI;
    public Texture UI_image;
    Vector3 TW;  //標靶位置
    float oriRz;
    float Rz = -6.84f;  //拉近靶位
    public TagetWall_Button[] tagetWall_Button;

    void Start()
    {
        TextG = GameObject.Find("ObjectText");
        Botton = false;
        Type = 1;
        oriRz = TagetWall.transform.localPosition.z;
    }

    void Update()
    {
        switch (RangeType)
        {
            case 0:
                if (Botton)
                {
                    TW = TagetWall.transform.localPosition;
                    if (Type == 0)  //拉近
                    {
                        TW.z -= 10 * Time.deltaTime;
                        if (TW.z <= Rz)
                        {
                            TW.z = Rz;
                            Botton = false;
                        }
                    }
                    else if (Type == 1)  //拉遠
                    {
                        TW.z += 10 * Time.deltaTime;
                        if (TW.z >= oriRz)
                        {
                            TW.z = oriRz;
                            Botton = false;
                        }
                    }
                    TagetWall.transform.localPosition = TW;
                }
                break;
            case 1:  //10m /20m
                if (Botton)
                {
                    TW = TagetWall.transform.localPosition;
                    if (Type == 0)
                    {
                        TW.x += 10 * Time.deltaTime;
                        if (TW.x >= 34.41)
                        {
                            TW.x = 34.41f;
                            Botton = false;
                        }
                    }
                    else if (Type == 1)
                    {
                        TW.x -= 10 * Time.deltaTime;
                        if (TW.x <= -0.13)
                        {
                            TW.x = -0.13f;
                            Botton = false;
                        }
                    }
                    TagetWall.transform.localPosition = TW;
                }
                break;
            case 2:
                if (Botton)
                {
                    if (Type == 0)  //拉近
                    {
                        UI.GetComponent<RawImage>().texture = UI_image;
                        UI.SetActive(true);
                        Botton = false;
                    }
                    else if (Type == 1)  //拉遠
                    {
                        UI.SetActive(false);
                        Botton = false;
                    }
                }
                break;
        }
       
    }
    void HitByRaycast() //被射線打到時會進入此方法
    {
        switch (RangeType)
        {
            case 0:
                if (Type == 0)
                {
                    TextG.GetComponent<Text>().text = "標靶復位";
                }else
                {
                    TextG.GetComponent<Text>().text = "查看標靶";
                }
                break;
            case 1:
                TextG.GetComponent<Text>().text = "切換距離\n" + Text[Type];
                break;
            case 2:
                if (Type == 0)
                {
                    TextG.GetComponent<Text>().text = "關閉標靶鏡頭";
                }
                else
                {
                    TextG.GetComponent<Text>().text = "開啟標靶鏡頭";
                }
                break;
        }

        if (Input.GetKeyDown(KeyCode.E)) //當按下鍵盤 E 鍵時
        {
            if (!Botton)
            {
                Botton = true;
                tagetWall_Button[0].Type = 1;
                tagetWall_Button[0].Botton = false;
                tagetWall_Button[1].Type = 1;
                tagetWall_Button[1].Botton = false;
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
