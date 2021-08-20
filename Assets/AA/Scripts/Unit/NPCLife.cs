using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCLife : MonoBehaviour
{
    public float fullHp,hp;  //滿血時數值,實際
   // public Image hpImage; //血球的UI物件
    public static bool Dead;


    void Start()
    {
        hp = fullHp=8; //遊戲一開始時先填滿血
        Dead = false;
    }

    public void Damage(float Power) // 接受傷害
    {
        hp -= Power; // 扣血
        if (hp <= 0)
        {
            hp = 0; // 不要扣到負值
            gameObject.SetActive(false);
            Dead = true;
        }      
    }


    void Update()
    {
        //hpImage.fillAmount = hp / fullHp; //顯示血球
    }
}
