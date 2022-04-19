using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroLife : MonoBehaviour
{
    public static float fullHp,hp, hp_R;  //滿血時數值, 實際, 紅血
    public Image HP_W, HP_R; //血球的UI物件
    public static bool Dead;
    public float time=0;
    bool Invincible=false;
    GameObject DeBugT;
    public int HitType;
    public ParticleSystem[] Hit_Player;
    public float HP_R_speed =0.8f;
    bool playing ;

    void Start()
    {
        hp = hp_R = fullHp = 20; //遊戲一開始時先填滿血
        Dead = false;
        DeBugT = GameObject.Find("DeBugT").gameObject;
        DeBugT.SetActive(false);
        for(int i=0; i< Hit_Player.Length; i++)
        {
            if (Hit_Player[i] != null)
            {
                Hit_Player[i].Stop();
                Hit_Player[i].gameObject.SetActive(false);
            }
        }
    }

    public void Damage(float Power) // 接受傷害
    {
        hp -= Power; // 扣血
    }
    public void DamageEffects(int hitType)
    {
        HitType = hitType;
        switch (HitType)
        {
            case 0:  //水晶 尖
                break;
            case 1:  //水晶 長
                break;
            case 2:  //水晶 方
                break;
            case 3:  //蠍子

                break;
        }
        Hit_Player[HitType].gameObject.SetActive(true);
        Hit_Player[HitType].Play();
        playing = true;
    }
    public static void DownDamage(int Dps)
    {
        hp -= Dps;
    }

    void Update()
    {
        if (hp <= 0)
        {
            Hit_Player[HitType].Stop();
            Hit_Player[HitType].gameObject.SetActive(false);
            hp = 0; // 不要扣到負值
            Dead = true;
        }
        if (Hit_Player[HitType] != null)
        {
            if (Hit_Player[HitType].isStopped && playing)  //粒子結束時關掉
            {
                playing = false;
                Hit_Player[HitType].gameObject.SetActive(false);
            }
        }



        HP_W.fillAmount = hp / fullHp; //顯示血球
        HP_R.fillAmount = hp_R / fullHp; //顯示血球
        if (hp != hp_R)
        {
            time +=4* Time.deltaTime;
            if (time >= 2f)
            {
                time = 2;
                hp_R -= HP_R_speed * Time.deltaTime;
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
            DamageEffects(0);
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
