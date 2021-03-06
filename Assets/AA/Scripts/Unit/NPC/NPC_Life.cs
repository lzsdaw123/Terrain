using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class NPC_Life : MonoBehaviour
{
    public float fullHp, hp, hp_R;  //滿血時數值, 實際, 紅血
    //public Image hpImage, HP_R; //血球的UI物件
    [SerializeField] private bool Dead;  //是否死亡
    public bool P_Dead;  //是否死亡
    [SerializeField] private bool Explode;  //是否死亡
    float time;
    [SerializeField] private float Deadtime;
    public float UItime;
    public NPC_AI NPC_AI;
    public Animator ani; //動畫控制器
    public GameObject Exp, BigExp;  //爆炸,大爆炸
    public bool 無敵;
    public bool Neutral=false;

    void OnDisable()
    {
        
    }
    void Awake()
    {
        time = 0;
        Deadtime = -1;
    }
    void Start()
    {
        hp = fullHp= hp_R = 24; //遊戲一開始時先填滿血
        Dead = Explode = false;
        UItime = 0;
        if (Exp != null) Exp.SetActive(false);
        if(NPC_AI!=null) NPC_AI.enabled = true;
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = true;
        if (Neutral)
        {
            gameObject.layer = LayerMask.NameToLayer("Neutral");
            gameObject.tag = "Neutral";
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Actor");
            gameObject.tag = "NPC";
        }

    }

    public void Damage(float Power) // 接受傷害
    {
        hp -= Power; // 扣血
        //AudioManager.Warn(0);
        if (無敵) hp = fullHp;
        if (hp <= 0)
        {
            if (!Dead)
            {
                ani.SetTrigger("Dead");
                ani.SetBool("Fire", false);
                ani.SetBool("Aim", false);
                gameObject.layer = LayerMask.NameToLayer("Default");
                gameObject.tag = "Untagged";
                Dead = true;
            }
            hp = 0; // 不要扣到負值
        }      
    }
    void Update()
    {
        P_Dead = Dead;
        //hpImage.fillAmount = hp / fullHp; //顯示血球
        //HP_R.fillAmount = hp_R / fullHp; //顯示血球
        //if (Input.GetKeyDown(KeyCode.I))  //自爆測試
        //{
        //    Damage(20);
        //}
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
        if (hp <= 0 && !Dead)
        {
            ani.SetTrigger("Dead");
            ani.SetBool("Fire", false);
            ani.SetBool("Aim", false);
            gameObject.layer = LayerMask.NameToLayer("Default");
            gameObject.tag = "Untagged";
            Dead = true;
        }
        if (Deadtime >= 1)  //關閉整個NPC
        {
            GetComponent<CapsuleCollider>().enabled = false;
            //GetComponent<Rigidbody>().isKinematic = false;
            if (Exp != null) Exp.SetActive(false);
        }
        if (Deadtime >= 6f)
        {
            Deadtime = -1;
            gameObject.SetActive(false);
        }
        if (Deadtime >= 0)
        {
            Deadtime += Time.deltaTime;
            ani.SetBool("Fire", false);
            ani.SetBool("Aim", false);
        }
    }
    public void DeadExp(int N)
    {
        switch (N)
        {
            case 0:
                Destroyed();
                break;
            case 1:
                Deadtime = 0;
                break;
        }
    }
    void Destroyed()  //爆炸特效
    {
        if (!Explode)
        {
            //GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<NavMeshAgent>().enabled = false;
            Explode = true;
            if (Exp != null) Exp.SetActive(true);
            if (NPC_AI != null)  NPC_AI.enabled = false;  //關閉AI腳本
            //AudioManager.explode();   
        }
    }
}
