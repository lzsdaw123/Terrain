using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpWeapons : MonoBehaviour
{
    public GameObject TextG;
    [SerializeField] GameObject Take;
    public int WeaponsType;  // 0=步槍,1=電磁手槍, 2=霰彈槍
    string[] WeaponsText = new string[] { "自動步槍", "電磁手槍", "霰彈槍" };
    public WeaponValue[] Weapons;
    int _WeaponType; //玩家身上武器類型
    int WeaponPos;  //同武器類型
    int[] Equipment;
    string WaveText;
    bool WT;

    void Start()
    {
        TextG = GameObject.Find("ObjectText");
        Take = GameObject.Find("Take");
        gameObject.SetActive(true);
        WaveText = null;
    }

    // Update is called once per frame
    void Update()
    {


    }
    void HitByRaycast() //被射線打到時會進入此方法
    {
        QH_interactive.thing();  //呼叫QH_拾取圖案
        Weapons = Shooting.Weapons;
        _WeaponType = Shooting.WeaponType;
        Equipment = Shooting.Equipment;
        WeaponPos = Weapons[WeaponsType].WeaponPos;

        if (Equipment[WeaponsType] == 0)
        {
            WaveText = null;
        }
        else
        {
            WaveText = "    已擁有";
        }
        TextG.GetComponent<Text>().text = "按「E」拾取\n"+ WeaponsText[WeaponsType]+ WaveText;

        if (Take.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.E)) //當按下鍵盤 E 鍵時
            {
                if (Equipment[WeaponsType] == 0)
                {
                    Shooting.PickUpWeapons(WeaponsType, WeaponPos);
                    AudioManager.PickUp(0);
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
