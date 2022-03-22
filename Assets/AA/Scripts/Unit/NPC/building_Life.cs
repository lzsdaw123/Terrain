using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class building_Life : MonoBehaviour
{
    public float fullHp=20, hp, hp_R;  //滿血時數值, 實際, 紅血
    //public Image hpImage, HP_R; //血球的UI物件
    public bool TeamDa;
    [SerializeField] bool Dead;  //是否死亡
    float time;
    public float UItime;
    public GameObject NormalVer;  //普通
    public GameObject DamageVer;  //損毀
    public GameObject BaseVer;  //位移
    public BoxCollider[] BoxCollider=new BoxCollider[1];
    public BoxCollider[] TeamBoxCollider;
    public GameObject Exp, BigExp, Smoke;  //爆炸,大爆炸
    ParticleSystem ParticleSystem;
    public float DeadTime;
    public Animator Animator;

    void Awake()
    {
        time = 0;
        DeadTime = -1;
        if (BaseVer != null) BaseVer.transform.localRotation = Quaternion.Euler(0,0,0);
    }
    void Start()
    {
        hp = hp_R = fullHp; //遊戲一開始時先填滿血
        Dead = false;
        UItime = 0;
        //Exp.SetActive(false);
        NormalVer.SetActive(true);
        if(DamageVer!=null) DamageVer.SetActive(false);
        BoxCollider[0].enabled = true;
        if(TeamBoxCollider[0] != null) TeamBoxCollider[0].enabled = true;
        if (Animator != null) Animator.SetBool("Dead", false);
        if (Smoke != null) 
        {
            ParticleSystem = Smoke.GetComponent<ParticleSystem>();      
            Smoke.SetActive(false);
        }
    }

    public void Damage(float Power) // 接受傷害
    {
        hp -= Power; // 扣血
        //AudioManager.Warn(0);
        if (hp <= 0)
        {
            hp = 0; // 不要扣到負值
            if (TeamBoxCollider[0] != null) TeamBoxCollider[0].enabled = false;
            //gameObject.SetActive(false);
            Destroyed();
        }      
    }
    public void TeamDamage(float Power)  //團隊傷害
    {
        if (TeamDa)
        {
            hp -= Power; // 扣血
            if (hp <= 0)
            {
                TeamBoxCollider[0].enabled = false;
                 hp = 0; // 不要扣到負值
                Destroyed();
            }
        }
    }
    void Update()
    {
        //if (Exp.activeSelf)
        //{
        //    DeadTime += Time.deltaTime;
        //    //AudioManager.Warn(-1);
        //}
        //if (DeadTime >= 1.6f)
        //{
        //    BigExp.SetActive(false);
        //    Exp.SetActive(false);
        //    gameObject.SetActive(false);
        //    DeadTime = 2;
        //}
        if (DeadTime >= 0)
        {
            DeadTime += Time.deltaTime;
        }
        if (DeadTime >= 8)
        {
            ParticleSystem.Stop();
        }
        if (DeadTime >= 18)
        {
            DeadTime = -1;
            Smoke.SetActive(false);
        }

    }
    void Destroyed()
    {
        NormalVer.SetActive(false);
        if (DamageVer != null) DamageVer.SetActive(true);
        BoxCollider[0].enabled = false;
        if (Animator != null)  Animator.SetBool("Dead", true);
        if (Smoke != null) 
        {
            ParticleSystem.Play();
            Smoke.SetActive(true);
            DeadTime = 0;
        }


        if (!Dead)
        {
            if (BaseVer != null)  BaseVer.transform.localRotation = Quaternion.Euler(-20, 0, 0);
            Dead = true;
            //Exp.SetActive(true);
            //AudioManager.explode();   
        }
    }
}
