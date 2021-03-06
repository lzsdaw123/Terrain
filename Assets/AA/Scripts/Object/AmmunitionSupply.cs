using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmunitionSupply : MonoBehaviour
{
    public GameObject ObjectText;
    public int Type;
    public int[] T_WeapAmm = new int[3] ; //武器總彈藥量
    public int[] AmmSupply =new int[3];  //存量
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
    public string[] WeapT;

    void Awake()
    {
        T_WeapAmm = new int[3]; //武器總彈藥量
        AmmSupply = new int[3];  //存量
        WeapT = new string[3];
    }
    void Start()
    {
        Am_zero_Warn = Save_Across_Scene.Am_zero_Warn;
        ObjectText = Save_Across_Scene.ObjectText;
        AmmSupply = new int[] { 480, 6 ,24 };
        T_WeapAmm = new int[] { 300, 30, 30};
        WeapT = new string[] { "步槍", "左輪", "霰彈槍" };


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
            case 2:
                if (AmmSupply[2] <= 0)
                {
                    AmmSupply[2] = 0;
                    ASupply[0].SetActive(false);
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
            ObjectText.GetComponent<Text>().text = "拾取"+ WeapT[Type] + "彈藥\n" + "彈藥量 " + AmmSupply[Type];
        }
        else
        {
            switch (Type)
            {
                case 0:
                    ObjectText.GetComponent<Text>().text = "打開步槍彈藥盒 ";
                    break;
                case 1:
                    ObjectText.GetComponent<Text>().text = "打開左輪彈藥盒 ";
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
                        FirstAmm = Shooting.FirstAmm;
                        if (!FirstAmm  && WeaponType==0 && !Shooting.SkipTeach)
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
                        else
                        {

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
                        int NeedAmm = T_WeapAmm[1] - Shooting.Weapons[1].T_WeapAm;  //需求彈藥數 = 總彈藥 - 當前總彈藥量
                        if (NeedAmm > AmmSupply[1])  //需求量 > 存量
                        {
                            NeedAmm = AmmSupply[1];
                        }
                        AmmSupply[1] = AmmSupply[1] - NeedAmm;  //存量 = 存量-需求量
                        Shooting.Weapons[1].T_WeapAm += NeedAmm; 
                    }
                    break;
                case 2:
                    if (AmmSupply[2] <= 0) return;
                    if (Shooting.Weapons[2].T_WeapAm < T_WeapAmm[2] && CoverOn)  //玩家總彈藥量是否滿的
                    {
                        Am_zero_Warn.SetActive(false);
                        AudioManager.PickUp(0);
                        FirstAmm = Shooting.FirstAmm;
                        //print("彈藥補給");
                        int NeedAmm = T_WeapAmm[2] - Shooting.Weapons[2].T_WeapAm;  //需求彈藥數
                        if (NeedAmm > AmmSupply[2])
                        {
                            NeedAmm = AmmSupply[2];
                        }
                        AmmSupply[2] = AmmSupply[2] - NeedAmm;
                        Shooting.Weapons[2].T_WeapAm += NeedAmm;
                    }
                    break;
            }
           
        }
    }
}
