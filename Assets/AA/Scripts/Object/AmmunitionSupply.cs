using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmunitionSupply : MonoBehaviour
{
    public GameObject TextG;
    public int Type;
    int[] T_WeapAmm = new int[2] ; //武器總彈藥量
    public int[] AmmSupply =new int[2];
    public GameObject[] ASupply;  //彈藥
    public GameObject Cover;  //蓋子
    public bool CoverOn;
    public bool Open;
    public float Rotation;
    bool interactive=false;
    [SerializeField] GameObject Am_zero_Warn;
    int WeaponType; //武器類型
    bool FirstAmm = false;
    public Material material;
    public Shader shader;
    public float intensity;

    void Awake()
    {
        Am_zero_Warn = GameObject.Find("Am_zero_Warn").gameObject;

    }
    void Start()
    {
        TextG = GameObject.Find("ObjectText");
        AmmSupply = new int[] { 480, 6 };
        T_WeapAmm = new int[] { 300, 30 };


        switch (Type)
        {
            case 0:
                Rotation = Cover.transform.localRotation.x;
                if (CoverOn)
                {
                    Cover.transform.localRotation = Quaternion.Euler(94, 0, 0);
                }
                else
                {
                    Cover.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                break;
            case 1:
                Rotation = Cover.transform.localRotation.x;
                if (CoverOn)
                {
                    Cover.transform.localRotation = Quaternion.Euler(-85f, 0, 0);
                }
                else
                {
                    Cover.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                break;
        }       
    }
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    for (int i = 0; i < shader.GetPropertyCount(); i++)
        //    {
        //        string name = shader.GetPropertyName(i);
        //        int id = shader.GetPropertyNameId(i);
        //        //if (name == "_Glow_Speed")
        //        //{
        //        //    material.SetFloat(id, 10);
        //        //    break;
        //        //}
        //        if (name == "_EmissionColor")
        //        {
        //            Color color = material.GetColor(id);
        //            //Color color = material.GetColor("_EmissionColor");
        //            float factor = Mathf.Pow(2, intensity);
        //            color = new Color(color.r * factor, color.g * factor, color.b * factor, color.a);
        //            material.SetColor(id, color);
        //            print(color);
        //            break;
        //        }
        //    }
        //}
        

        WeaponType = Shooting.WeaponType;
        switch (Type)
        {
            case 0:
                if (AmmSupply[0] <= 0)
                {
                    AmmSupply[0] = 0;
                    ASupply[0].SetActive(false);
                }
                break;
            case 1:
                if (AmmSupply[1] != 6)
                {
                    int D = 6 - AmmSupply[1];
                    for (int i = 0; i < D; i++)
                    {
                        if (D <=6)
                        {
                            ASupply[i].SetActive(false);
                        }
                    }
                }
                break;
        }

        if (interactive)
        {
            if (Open)
            {

                switch (Type)
                {
                    case 0:
                        Rotation += 240 * Time.deltaTime;
                        if (Rotation >= 94)
                        {
                            CoverOn = true;
                            Rotation = 94;
                        }
                        break;
                    case 1:
                        Rotation -= 240 * Time.deltaTime;
                        if (Rotation <= -85)
                        {
                            CoverOn = true;
                            Rotation = -85;
                        }
                        break;
                }
            }
            else
            {
                switch (Type)
                {
                    case 0:
                        Rotation -= 240 * Time.deltaTime;
                        if (Rotation <= 0)
                        {
                            CoverOn = false;
                            Rotation = 0;
                        }
                        break;
                    case 1:
                        Rotation += 240 * Time.deltaTime;
                        if (Rotation >= 0)
                        {
                            CoverOn = false;
                            Rotation = 0;
                        }
                        break;
                }
            }
            Cover.transform.localRotation = Quaternion.Euler(Rotation, 0, 0);
        }
    }
    void HitByRaycast() //被射線打到時會進入此方法
    {
        if (interactive || CoverOn)
        {
            TextG.GetComponent<Text>().text = "按「E」拾取彈藥\n" + "彈藥量 " + AmmSupply[Type];
        }
        else
        {
            switch (Type)
            {
                case 0:
                    TextG.GetComponent<Text>().text = "按「E」打開步槍彈藥盒 ";
                    break;
                case 1:
                    TextG.GetComponent<Text>().text = "按「E」打開左輪彈藥盒 ";
                    break;
            }
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
            switch (Type)
            {
                case 0:
                    if (AmmSupply[0] <= 0) return;
                    if (Shooting.Weapons[0].T_WeapAm < T_WeapAmm[0] && CoverOn)  //玩家總彈藥量是否滿的
                    {
                        Am_zero_Warn.SetActive(false);
                        AudioManager.PickUp(0);
                        if (!FirstAmm)
                        {
                            FirstAmm = true;
                            Shooting.PickUpAmm(0);
                        }
                        //print("彈藥補給");
                        int NeedAmm = T_WeapAmm[0] - Shooting.Weapons[0].T_WeapAm;  //需求彈藥數
                        if (NeedAmm > AmmSupply[0])
                        {
                            NeedAmm = AmmSupply[0];
                        }
                        AmmSupply[0] = AmmSupply[0] - NeedAmm;
                        Shooting.Weapons[0].T_WeapAm += NeedAmm;
                    }
                    break;
                case 1:
                    if (AmmSupply[1] <= 0) return;
                    if (Shooting.Weapons[1].T_WeapAm < T_WeapAmm[1] && CoverOn)  //玩家總彈藥量是否滿的
                    {
                        Am_zero_Warn.SetActive(false);
                        AudioManager.PickUp(0);
                        //print("彈藥補給");
                        int NeedAmm = T_WeapAmm[1] - Shooting.Weapons[1].T_WeapAm;  //需求彈藥數
                        if (NeedAmm > AmmSupply[1])
                        {
                            NeedAmm = AmmSupply[1];
                        }
                        AmmSupply[1] = AmmSupply[1] - NeedAmm;
                        Shooting.Weapons[1].T_WeapAm += NeedAmm;
                    }
                    break;
            }
           
        }
    }
}
