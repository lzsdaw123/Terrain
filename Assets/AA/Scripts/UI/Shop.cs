using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public GameObject LvUpUI;
    public Text KillPointsT;
    public Text[] Lv;
    public Text[] Points;
    public GameObject[] UpgradeObject;

    public static int HpLv;  //血量等級
    public static int DpsLv;  //傷害等級
    public int HpPoints;  //血量點數
    public int DpsPoints;  //傷害點數
    public static int KillPoints;  //擊殺點數

    bool OpenT;

    void Awake()
    {
        HpLv = DpsLv = 0;
        HpPoints = DpsPoints = 10;
        KillPoints = -8;
        LvUpUI.SetActive(false);
    }
    void Start()
    {
        OpenT = true;
        ButtonAudio();
        Lv[0].text = "Lv." + HpLv;
        Lv[1].text = "Lv." + DpsLv;
        Points[0].text = HpPoints + " 擊殺數";
        Points[1].text = DpsPoints + " 擊殺數";
    }

    void Update()
    {
        KillPointsT.text = "怪物擊殺點數 : " + (KillPoints);

        //if (Input.GetKeyDown(KeyCode.Tab))
        //{
        //    if (OpenT)
        //    {                
        //        OpenUI();
        //    }
        //    else
        //    {                
        //        closeUI();
        //    }
        //}
        //if (Input.GetKeyDown(KeyCode.J))  //開發者模式
        //{
        //    KillPoints += 10;
        //}
    }
    public static void AddKillScore()
    {
        KillPoints += 1;
    }
    public void OpenUI()
    {
        OpenT = false;
        ButtonAudio();
        LvUpUI.SetActive(true);
        Settings.pause();
    }
    public void closeUI()
    {
        OpenT = true;
        ButtonAudio();
        LvUpUI.SetActive(false);
        Settings.con();
    }
    public void HpLvUp()  //血量升級
    {
        ButtonAudio();
        if (HpLv >= 3) return;
        if (KillPoints >= HpPoints)
        {        
            KillPoints -= HpPoints;
            if (HpLv >= 2)
            {
                HpLv = 3;
                Lv[0].text = "Lv.Max";
                Points[0].text = "0 擊殺數";
            }
            else
            {
                HpLv++;              
                HpPoints += 10;
                Lv[0].text = "Lv." + HpLv;
                Points[0].text = HpPoints + " 擊殺數";
            }
            HeroLife.HpUp();
        }
    }
    public void DpsLvUp()
    {
        ButtonAudio();
        if (DpsLv >= 3) return;
        if (KillPoints >= DpsPoints)
        {
            KillPoints -= DpsPoints;
            if (DpsLv >= 2)
            {
                DpsLv = 3;
                Lv[1].text = "Lv.Max";
                Points[1].text = "0 擊殺數";
            }
            else
            {
                DpsLv++;
                DpsPoints += 10;
                Lv[1].text = "Lv." + DpsLv;
                Points[1].text = DpsPoints + " 擊殺數";
            }
            Shooting.DpsUp();
        }
    }

    void ButtonAudio()
    {
        AudioManager.Button();
    }
}
