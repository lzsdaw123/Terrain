using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    public Camera PlayCamera,GunCamera;

    public ObjectPool pool;
    public GameObject bullet;  //子彈
    //public Transform gun;
    public GameObject[] muzzle;  //槍口類型
    public GameObject GunAimR; //槍瞄準鏡上下位移矯正
    public GameObject GunAimR_x;  //X軸瞄準晃動
    private Vector3 GA_R;  //Z軸瞄準偏移修正
    public float noise = 1f; //晃動頻率
    public float noiseRotateX;  //X軸晃動偏移量
    public float noiseRotateY;  //Y軸晃動偏移量
    public float FireRotateY;  //腰射Y軸晃動偏移量
    public float FireRotateX;  //腰射X軸晃動偏移量

    public float coolDown; //冷卻結束時間
    public float coolDownTimer; //冷卻時間計時器
    public PlayerMove controller;  //角色控制腳本
    public float AniTime, STtime;
    public int[] WeaponType;
    public Animator Weapon;   //動畫控制器
    public int n, m; //武器種類
    public GameObject[] _Animator;
    public Vector3 muzzlePOS;  //槍口座標

    public bool BFire;  //生成子彈
    public bool DontShooting = false;
    public bool LayDown = false;
    public RuntimeAnimatorController[] controllers;  //動畫控制陣列

    public static int ammunition = 30, Total_ammunition = 300;  //彈藥量
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
        //if (AimIng != true && DontShooting !=true)
        //{
        //    float oriRotateY = transform.rotation.y;
        //    float oriRotateX = GunAimR_x.GetComponent<MouseLook>().rotationX;

        //    range = Random.Range(-0.6f, 0.6f);  //晃動範圍
        //    noiseRotateY += (noise * (range / 2) * (Mathf.Cos(Time.time)) - noiseRotateY) / 100;
        //    noiseRotateX += (noise * (range / 2) * (Mathf.Sin(Time.time)) - noiseRotateX) / 100;
           
        //    transform.localEulerAngles += new Vector3(0.0f, noiseRotateY, 0.0f);
        //    GunAimR_x.GetComponent<MouseLook>().rotationX += noiseRotateX;         
        //}      

        if (AimIng)  //瞄準
        {
            //Z軸準心偏移修正
            GA_R.z += 0.2f;
            if (GA_R.z >= 2.2f) { GA_R.z = 2.2f; }

            //range = Random.Range(-0.05f, 0.05f);  //晃動範圍

            //localEulerAngles跟localRotation的差別
        }
        else
        {
            GA_R.z -= 0.2f;
            if (GA_R.z <= 0) { GA_R.z = 0; }
        }
        GunAimR.transform.localRotation = Quaternion.Euler(0f, -89.71f, GA_R.z);  //Z軸瞄準偏移修正

        if (coolDownTimer > coolDown) //若冷卻時間已到
        {
            //可以發射子彈了
            FieldOfView = PlayCamera.GetComponent<Camera>().fieldOfView;
            FieldOfView = GunCamera.GetComponent<Camera>().fieldOfView;
            //若按下滑鼠右鍵瞄準
            if (Input.GetButton("Fire2") && Reload != true)  //架槍瞄準
            {
                if (AimIng == false)
                {
                    AimIng = true; //瞄準
                    Weapon.SetTrigger("AimUP");
                }
                Weapon.SetBool("Aim", true);
                ZoomIn();
                //瞄準射擊
                if (Input.GetButton("Fire1") && (DontShooting == false) && (LayDown == false) && (ammunition != 0))
                {
                    Weapon.SetBool("AimFire", true);
                }
                else {Weapon.SetBool("AimFire", false); }
            }
            else
            {
                Weapon.SetBool("AimFire", false);
                Weapon.SetBool("Aim", false);
                AimIng = false;
                ZoomOut();
            }
            //若按下滑鼠左鍵開火
            if (Input.GetButton("Fire1") && (DontShooting == false) && (LayDown == false) && (ammunition != 0))
            {
                float rangeY = Random.Range(-40f, 40f);  //射擊水平晃動範圍
               // float rangeX = Random.Range(3f, 5f);  //射擊垂直晃動範圍
                float rangeX = Random.Range(9f, 15f);  //射擊垂直晃動範圍
                FireRotateY = (noise * rangeY * (Mathf.Sin(Time.time)) - FireRotateY) / 100;
                //FireRotateX = (noise * rangeX * (Mathf.Sin(Time.time)) - FireRotateX);
                FireRotateX = rangeX;
                if (FireRotateX <= 0) { FireRotateX *= -1; } //強制往上飄
                //Debug.Log("原本的" + " / " + FireRotateX);
                if (AimIng == true) {
                    FireRotateY /= 2;
                    FireRotateX /= 3;
                }              
               // Debug.Log("後" + " / " + FireRotateX);

                transform.localEulerAngles += new Vector3(0.0f, FireRotateY, 0.0f);
                GunAimR_x.GetComponent<MouseLook>().rotationX -= FireRotateX * Time.smoothDeltaTime;

                coolDownTimer = 0.72f;   //射擊冷卻時間，與coolDown差越小越快
                ammunition--;
                Weapon.SetBool("Fire", true);
                Weapon.SetBool("Aim", false);
                BFire = true;  //生成子彈
            }
            else
            {
                Weapon.SetBool("Fire", false);
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
            FieldOfView -= 160f * Time.deltaTime;
            PlayCamera.GetComponent<Camera>().fieldOfView = FieldOfView;
            GunCamera.GetComponent<Camera>().fieldOfView = FieldOfView;
        }
    }
    void ZoomOut()
    {
        if (FieldOfView < 60f)
        {
            FieldOfView += 160f * Time.deltaTime;
            PlayCamera.GetComponent<Camera>().fieldOfView = FieldOfView;
            GunCamera.GetComponent<Camera>().fieldOfView = FieldOfView;
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
        if (BFire) //生成子彈
        {
            muzzlePOS = muzzle[n].GetComponent<Transform>().position;

            //建立子彈在鏡頭中心位置
            //GameObject obj = Instantiate(bullet, muzzlePOS, PlayCamera.transform.rotation);
            pool.ReUse(muzzlePOS, PlayCamera.transform.rotation);
            GunshotsAudio();
            BFire = false;
        }
    }
    void GunshotsAudio()
    {
        AudioManager.PlayGunshotsAudio();
    }
}
