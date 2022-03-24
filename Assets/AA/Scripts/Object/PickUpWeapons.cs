using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpWeapons : MonoBehaviour
{
    public GameObject TextG;
    [SerializeField] GameObject Take;
    GameObject play;
    public int WeaponsType;  // 0=步槍,1=電磁手槍, 2=霰彈槍
    string[] WeaponsText = new string[] { "自動步槍", "電磁手槍", "霰彈槍" };
    public int[] Weapon_of_Pos = new int[2];  //武器放置位置 {主武器,副武器}
    public WeaponValue[] Weapons;
    int _WeaponType; //玩家身上武器類型
    int WeaponPos;  //同武器類型
    int[] Equipment;  //玩家身上擁有的武器
    string WaveText;
    bool WT;

    void Awake()
    {
        Take = GameObject.Find("Take");
    }
    void Start()
    {
        TextG = GameObject.Find("ObjectText");
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
                Weapon_of_Pos = Shooting.Weapon_of_Pos;

                if (Shooting.WeaponsPosOb[WeaponPos] != null)  //武器位置不為空
                {
                    if (Equipment[WeaponsType] == 1)
                    {
                        return;
                    }
                    //print(Shooting.WeaponsPosOb[WeaponPos]);
                    GameObject OriGameObject = Shooting.WeaponsPosOb[WeaponPos];  //換下玩家身上的武器
                    OriGameObject.SetActive(true);
                    OriGameObject.transform.parent = gameObject.gameObject.transform; 
                    OriGameObject.transform.position = gameObject.transform.position;
                    OriGameObject.transform.parent = null;  //從玩家身上離開

                }
                if (Equipment[WeaponsType] == 0)
                {
                    Shooting.PickUpWeapons(WeaponsType, WeaponPos, gameObject);
                    play = GameObject.Find("POPP").gameObject;
                    gameObject.SetActive(false);
                    gameObject.transform.parent = play.gameObject.transform;  //變為子物件到玩家身上
                }
                AudioManager.PickUp(2);
            }
        }
    }
}
