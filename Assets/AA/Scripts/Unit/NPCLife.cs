using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCLife : MonoBehaviour
{
    public float fullHp, hp, hp_R;  //滿血時數值,實際
    public Image hpImage, HP_R; //血球的UI物件
    public GameObject HP_O,warnUI;
    public static bool Dead;
    float time=0;
    public float UItime;
    public GameObject Exp;

    void Start()
    {
        hp = fullHp= hp_R = 10; //遊戲一開始時先填滿血
        Dead = false;
        warnUI.SetActive(false);
        UItime = 0;
        Exp.SetActive(false);
    }

    public void Damage(float Power) // 接受傷害
    {
        hp -= Power; // 扣血
        warnUI.SetActive(true);
        if (hp <= 0)
        {
            hp = 0; // 不要扣到負值
            //gameObject.SetActive(false);
            Destroyed();
        }      
    }
    void Update()
    {
        hpImage.fillAmount = hp / fullHp; //顯示血球
        HP_R.fillAmount = hp_R / fullHp; //顯示血球

        if (hp != hp_R)
        {
            time += 4 * Time.deltaTime;
            if (time >= 2)
            {
                time = 2;
                hp_R -= 1f * Time.deltaTime;
            }
        }
        if (hp_R <= hp)
        {
            hp_R = hp;
            time = 0;
        }
        if(hp<= fullHp * 0.5f)
        {
            HP_O.SetActive(true);                
        }
        if (warnUI.activeSelf)
        {
            UItime += Time.deltaTime;
            if (UItime >= 1)
            {
                warnUI.SetActive(false);
                UItime = 0;
            }
        }
    }
    void Destroyed()
    {
        if (!Dead)
        {
            Dead = true;
            print("發電站已被摧毀");
            Exp.SetActive(true);
            AudioManager.explode();
        }
    }
}
