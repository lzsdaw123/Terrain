using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmunitionSupply : MonoBehaviour
{
    public GameObject T;
    int Total_ammunition;
    public int AmmSupply;
    public GameObject ASupply;  //彈藥
    public GameObject Cover;  //蓋子
    public bool CoverOn;
    public bool Open;
    public float Rotation;
    bool interactive=false;

    void Start()
    {
        T = GameObject.Find("ObjectText");
        Total_ammunition = Shooting.Total_ammunition;
        AmmSupply = 480;
        Rotation = Cover.transform.localRotation.x;
        if (CoverOn)
        {
            Cover.transform.localRotation = Quaternion.Euler(94, 0, 0);
        }
        else
        {
            Cover.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    void Update()
    {
        if (AmmSupply <= 0)
        {
            AmmSupply = 0;
            ASupply.SetActive(false);
        }
        if (interactive)
        {
            if (Open)
            {
                Rotation += 120 * Time.deltaTime;
                if (Rotation >= 94)
                {
                    CoverOn = true;
                    Rotation = 94;
                }
            }
            else
            {
                Rotation -= 120 * Time.deltaTime;
                if (Rotation <= 0)
                {
                    CoverOn = false;
                    Rotation = 0;
                }
            }
            Cover.transform.localRotation = Quaternion.Euler(Rotation, 0, 0);
        }
      

    }
    void HitByRaycast() //被射線打到時會進入此方法
    {
        if (interactive || CoverOn)
        {
            T.GetComponent<Text>().text = "按E拾取彈藥\n" + "彈藥量 " + AmmSupply;
        }
        else
        {
            T.GetComponent<Text>().text = "按E打開彈藥箱 ";
        }
        QH_interactive.thing();  //呼叫QH_拾取圖案

        if (Input.GetKeyDown(KeyCode.E)) //當按下鍵盤 E 鍵時
        {
            if (Shooting.Reload) return;
            if (!CoverOn)
            {
                interactive = true;
                Open = true;
            }
            else
            {
                //Open = false;
            }
            if (Shooting.Total_ammunition < Total_ammunition  &&CoverOn)
            {
                AudioManager.PickUp(0);
                //print("彈藥補給");
                AmmSupply = AmmSupply - (Total_ammunition - Shooting.Total_ammunition);
                Shooting.Total_ammunition = Total_ammunition;
            }
        }
    }
}
