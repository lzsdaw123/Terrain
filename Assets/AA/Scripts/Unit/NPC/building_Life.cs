using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class building_Life : MonoBehaviour
{
    public float fullHp=20, hp, hp_R;  //滿血時數值, 實際, 紅血
    //public Image hpImage, HP_R; //血球的UI物件
    [SerializeField] bool Dead;  //是否死亡
    float time;
    public float UItime;
    public GameObject NormalVer;
    public GameObject DamageVer;
    public GameObject BaseVer;
    public BoxCollider BoxCollider1;
    public GameObject Exp, BigExp;  //爆炸,大爆炸
    float DeadTime;

    void Awake()
    {
        time = 0;
        DeadTime = 0;
        BaseVer.transform.localRotation = Quaternion.Euler(0,0,0);
    }
    void Start()
    {
        hp = hp_R = fullHp; //遊戲一開始時先填滿血
        Dead = false;
        UItime = 0;
        //Exp.SetActive(false);
        NormalVer.SetActive(true);
        if(DamageVer!=null) DamageVer.SetActive(false);
        BoxCollider1.enabled = true;
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
        //if (DeadTime >= 6)
        //{
        //    DeadTime = 10;
        //}
    }
    void Destroyed()
    {
        NormalVer.SetActive(false);
        if (DamageVer != null) DamageVer.SetActive(true);
        BoxCollider1.enabled = false;


        if (!Dead)
        {
            BaseVer.transform.localRotation = Quaternion.Euler(-20, 0, 0);
            Dead = true;
            //print("守衛已被摧毀");
            //Exp.SetActive(true);
            //AudioManager.explode();   
        }
    }
}
