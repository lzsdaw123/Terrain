using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC_Life : MonoBehaviour
{
    public float fullHp, hp, hp_R;  //滿血時數值, 實際, 紅血
    //public Image hpImage, HP_R; //血球的UI物件
    [SerializeField] bool Dead;  //是否死亡
    float time;
    public float UItime;
    public NPC_AI NPC_AI;
    public GameObject Exp, BigExp;  //爆炸,大爆炸
    float DeadTime;

    void Awake()
    {
        time = 0;
        DeadTime = 0;
    }
    void Start()
    {
        hp = fullHp= hp_R = 20; //遊戲一開始時先填滿血
        Dead = false;
        UItime = 0;
        if (Exp != null) Exp.SetActive(false);
        if(NPC_AI!=null) NPC_AI.enabled = true;
    }

    public void Damage(float Power) // 接受傷害
    {
        hp -= Power; // 扣血
        //AudioManager.Warn(0);
        if (hp <= 0)
        {
            hp = 0; // 不要扣到負值
            //gameObject.SetActive(false);
            Destroyed();
        }      
    }
    void Update()
    {
        //hpImage.fillAmount = hp / fullHp; //顯示血球
        //HP_R.fillAmount = hp_R / fullHp; //顯示血球

        //if (hp != hp_R)
        //{
        //    time += 4 * Time.deltaTime;
        //    if (time >= 2)
        //    {
        //        time = 2;
        //        hp_R -= 1f * Time.deltaTime;
        //    }
        //}
        //if (hp_R <= hp)
        //{
        //    hp_R = hp;
        //    time = 0;
        //}
        //if (!Dead)
        //{
        //    if (hp <= fullHp * 0.12f)  //血量低於安全值
        //    {
        //        if (WarnT)
        //        {
        //            WarnT = false;
        //            AudioManager.Warn(0);
        //        }
        //    }
        //}
        if (Dead)
        {
            DeadTime += Time.deltaTime;
            //AudioManager.Warn(-1);
        }
        if (DeadTime >= 1.6f)
        {
            //BigExp.SetActive(false);
            if (Exp != null)  Exp.SetActive(false);
            gameObject.SetActive(false);
            DeadTime = 2;
        }
        //if (DeadTime >= 6)
        //{
        //    DeadTime = 10;
        //}
    }
    void Destroyed()
    {
        if (!Dead)
        {
            Dead = true;
            //print("守衛已被摧毀");
            if (Exp != null) Exp.SetActive(true);
            if (NPC_AI != null)  NPC_AI.enabled = false;  //關閉AI腳本
            //AudioManager.explode();   
        }
    }
}
