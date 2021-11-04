using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroLife : MonoBehaviour
{
    public static float fullHp,hp, hp_R;  //滿血時數值,實際
    public Image HP_W, HP_R; //血球的UI物件
    public static bool Dead;
    public float time=0;

    void Start()
    {
        hp = fullHp= hp_R = 8; //遊戲一開始時先填滿血
        Dead = false;
    }

    public void Damage(float Power) // 接受傷害
    {
        hp -= Power; // 扣血
        if (hp <= 0)
        {
            hp = 0; // 不要扣到負值
            //Debug.Log("死");
            Dead = true;
        }      
    }
    void Update()
    {
        HP_W.fillAmount = hp / fullHp; //顯示血球
        HP_R.fillAmount = hp_R / fullHp; //顯示血球

        if (hp != hp_R)
        {
            time +=4* Time.deltaTime;
            if (time >= 2)
            {
                time = 2;
                hp_R -= 0.5f * Time.deltaTime;
            }
        }
        if(hp_R <=hp)
        {
            hp_R = hp;
            time = 0;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Damage(1);
        }
    }
    public static void PlayerRe()
    {
        hp = fullHp = hp_R = 8; //遊戲一開始時先填滿血
        Dead = false;
    }
}
