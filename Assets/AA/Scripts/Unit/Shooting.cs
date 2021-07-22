using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    public Camera PlayCamera;

    public GameObject bullet;
    public Transform gun;
    public GameObject[] muzzle;

    public float coolDown; //冷卻結束時間
    public float coolDownTimer; //冷卻時間計時器
    public PlayerMove controller;  //角色控制腳本
    public float AniTime, STtime;
    public int[] WeaponType;
    public Animator Weapon;   //動畫控制器
    public int n, m; //武器種類
    public GameObject[] _Animator;
    public Vector3 muzzlePOS;

    public bool BFire;
    public bool DontShooting = false;
    public bool LayDown = false;
    public RuntimeAnimatorController[] controllers;  //動畫控制陣列

    public static int ammunition = 30, Total_ammunition = 150;  //彈藥量
    public static bool Reload = false;   //是否正在換彈
    bool AimIng;
    float FieldOfView;

    void Start()
    {
        coolDown = 0.8f;  //冷卻結束時間

        Weapon.runtimeAnimatorController = controllers[0];

        if (controller == null)
        {
            controller = GetComponent<PlayerMove>();
        }
        coolDownTimer = coolDown + 1;

        AniTime = STtime = 2f;
        n = 0;
        m = 1;

    }
    void Update()
    {
        Weapon.SetBool("C", false);
        DontShooting = AnimEvents.DontShooting;  //取得AnimEvents腳本變數

        if ((Input.GetKeyDown(KeyCode.Q)) && (AniTime >= 2))
        {
            m = n;
            if (n < 1)
            {
                n += 1;
            }
            else
            {
                n = 0;
            }
            //Weapon.SetBool("LayDown", true);
            AniTime = STtime - 1f;
        }
        if (AniTime <= 0)
        {
            _Animator[m].SetActive(false);
            _Animator[n].SetActive(true);
            Weapon = _Animator[n].GetComponent<Animator>();
            AniTime = STtime;
        }
        if (AniTime != STtime)  //換武器時間
        {
            AniTime -= Time.deltaTime;
        }
        muzzlePOS = muzzle[n].GetComponent<Transform>().position;


        if (coolDownTimer > coolDown) //若冷卻時間已到
        {
            //可以發射子彈了
            if ((Input.GetButton("Fire1")) && (DontShooting == false) && (LayDown == false) && (ammunition != 0)) //若按下發射鍵
            {
                coolDownTimer = 0.72f;   //射擊冷卻時間，與coolDown差越小越快
                ammunition--;
                Weapon.SetBool("Fire", true);
                Weapon.SetBool("Aim", false);
                BFire = true;
            }
            else
            {
                Weapon.SetBool("Fire", false);
            }
            FieldOfView = PlayCamera.GetComponent<Camera>().fieldOfView;
            if (Input.GetButton("Fire2") && Reload != true)  //架槍瞄準
            {
                if (AimIng == false)
                {
                    AimIng = true;
                    Weapon.SetTrigger("AimUP");
                }

                Weapon.SetBool("Aim", true);
                ZoomIn();
                if (Input.GetButton("Fire1") && ammunition != 0)
                {
                    Weapon.SetBool("AimFire", true);
                }
                else
                {
                    Weapon.SetBool("AimFire", false);
                }
            }
            else
            {
                AimIng = false;
                ZoomOut();
                Weapon.SetBool("Aim", false);
                Weapon.SetBool("AimFire", false);
            }
        }
        else //否則需要冷卻計時
        {
            coolDownTimer += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.R) && (LayDown == false) && (Total_ammunition != 0))       //換彈藥
        {
            if (Reload == false)
            {
                Reload = true;
                Weapon.SetTrigger("Reload");
            }
        }


        if (Input.GetKeyDown(KeyCode.T))       //收槍
        {
            Reload = false;
            if (LayDown == false)
            {
                LayDown = true;
                Weapon.SetTrigger("LayDown0");
                Weapon.SetBool("LayDown", true);
            }
            else
            {
                LayDown = false;
                Weapon.SetBool("LayDown", false);

            }
        }
        if (Input.GetKeyDown(KeyCode.C))  //看槍
        {
            Weapon.SetBool("C", true);
        }

        if (ammunition <= 0)
        {
            ammunition = 0;
        }
        if (Total_ammunition <= 0)
        {
            Total_ammunition = 0;
        }

    }

    void ZoomIn()
    {
        if (FieldOfView > 20f)
        {
            FieldOfView -= 105f * Time.deltaTime;
            PlayCamera.GetComponent<Camera>().fieldOfView = FieldOfView;
        }
    }
    void ZoomOut()
    {
        if (FieldOfView < 60f)
        {
            FieldOfView += 120f * Time.deltaTime;
            PlayCamera.GetComponent<Camera>().fieldOfView = FieldOfView;
        }
    }
    public static void ReLoad_E()
    {
        ammunition = AnimEvents.ammunition;
        Total_ammunition = AnimEvents.Total_ammunition;
        Reload = false;
    }

    void FixedUpdate()
    {
        if (BFire)
        {
            //建立子彈在鏡頭中心位置
            GameObject obj = Instantiate(bullet, muzzlePOS, PlayCamera.transform.rotation);
            BFire = false;
        }
    }
}
