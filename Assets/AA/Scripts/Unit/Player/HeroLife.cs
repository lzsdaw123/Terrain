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
    bool Invincible=false;
    GameObject DeBugT;
    public ParticleSystem S_HIT;

    void Start()
    {
        fullHp = 20;
        hp = hp_R = fullHp; //遊戲一開始時先填滿血
        Dead = false;
        DeBugT = GameObject.Find("DeBugT").gameObject;
        DeBugT.SetActive(false);
        S_HIT.Stop();
    }

    public void Damage(float Power) // 接受傷害
    {
        hp -= Power; // 扣血
        S_HIT.gameObject.SetActive(true);
        S_HIT.Play();

        if (hp <= 0)
        {
            S_HIT.Stop();
            S_HIT.gameObject.SetActive(false);
            hp = 0; // 不要扣到負值
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
            if (time >= 2f)
            {
                time = 2;
                hp_R -= 0.8f * Time.deltaTime;
            }
        }
        if(hp_R <=hp)
        {
            hp_R = hp;
            time = 0;
        }

        if (Input.GetKeyDown(KeyCode.L)) //開發者模式
        {
            Damage(5);
        }
        if (hp >= fullHp)
        {
            hp = fullHp;
        }
        if (Input.GetKeyDown(KeyCode.K))  //開發者模式
        {         
            if (Invincible)
            {
                DeBugT.SetActive(false);
                Invincible = false;
            }
            else
            {
                DeBugT.SetActive(true);
                Invincible = true;                
            }
        }
        if(Invincible) hp = fullHp;
    }
    public static void PlayerRe()  //重生
    {
        hp = hp_R = fullHp; //遊戲一開始時先填滿血        
        Dead = false;
    }
    public static void AddHp()
    {
        hp += 1.6f * Time.smoothDeltaTime;
    }
    public static void HpUp()
    {
        fullHp = 20 + Shop.HpLv * 5;
        hp= hp_R = fullHp;
    }
}
