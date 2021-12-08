using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpWeapons : MonoBehaviour
{
    public GameObject TextG;
    [SerializeField] GameObject Take;
    public int WeaponsType;  // 0=步槍,1=電磁手槍

    void Start()
    {
        TextG = GameObject.Find("ObjectText");
        Take = GameObject.Find("Take");
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {


    }
    void HitByRaycast() //被射線打到時會進入此方法
    {
        TextG.GetComponent<Text>().text = "按住「E」拾取\n電磁手槍";
        QH_interactive.thing();  //呼叫QH_拾取圖案

        if (Take.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.E)) //當按下鍵盤 E 鍵時
            {
                AudioManager.PickUp(0);
                Shooting.PickUpWeapons(WeaponsType);
                gameObject.SetActive(false);
            }
        }
    }
}
