using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    public Camera PlayCamera, GunCamera;

    public ObjectPool pool;
    public GameObject bullet, Muzzle_vfx;  //子彈,槍口火光  
    public ParticleSystem MuSmoke;
    public ParticleSystem MuFire;
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

    public static int ammunition, Total_ammunition;  //彈藥量
    public static bool Reload;   //是否正在換彈
    bool AimIng;
    float FieldOfView;
    float gFieldOfView;

    public ObjectPool pool_Hit;  //物件池
    public RectTransform HitUI;  //命中紅標
    public int HitType;  //彈孔類型變數
    public GameObject Hit_vfx, Hit_vfx_S;  //彈孔類型
    public LayerMask layerMask;  //圖層
    bool NoActor = false;  //擊中玩家
    Quaternion rot;  //彈孔生成角度
    Vector3 pos;  //彈孔生成位置
    public static float power; //子彈威力
    [SerializeField] Transform HIT; //預置彈孔位置
    [SerializeField] static GameObject ReloadWarn;
    [SerializeField] static GameObject Am_zero_Warn;

    void Awake()
    {
        for (int i = 0; i < Hit_vfx.transform.childCount; i++)
        {
            Hit_vfx_S = Hit_vfx.transform.GetChild(i).gameObject;
            Hit_vfx_S.SetActive(false);
        }
        ammunition = 30;
        Total_ammunition = 300;
        power = 1;
        Reload = false;
    }
    void Start()
    {
        coolDown = 0.8f;  //冷卻結束時間
        pool_Hit = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        HitUI = GameObject.Find("HitUI").GetComponent<RectTransform>();
        ReloadWarn = GameObject.Find("ReloadWarn").gameObject;
        Am_zero_Warn = GameObject.Find("Am_zero_Warn").gameObject;
        ReloadWarn.SetActive(false);
        Am_zero_Warn.SetActive(false);
        Hit_vfx_S = null;

        Weapon.runtimeAnimatorController = controllers[0];

        if (controller == null)
        {
            controller = GetComponent<PlayerMove>();
        }
        coolDownTimer = coolDown + 1;

        AniTime = STtime = 2f;
        n = 0;
        m = 1;

        Muzzle_vfx.SetActive(false);
        MuSmoke.Stop();

    }
    void Update()
    {
        if (Time.timeScale == 0) {return;}
        if (Input.GetButton("Fire2") && !Reload && !PlayerMove.m_Jumping )  //右鍵縮放鏡頭
        {
            ZoomIn();
        }
        else
        {
            ZoomOut();
        }
        DontShooting = AnimEvents.DontShooting;  //取得AnimEvents腳本變數

        //if ((Input.GetKeyDown(KeyCode.Q)) && (AniTime >= 2))
        //{
        //    m = n;
        //    if (n < 1)
        //    {
        //        n += 1;
        //    }
        //    else
        //    {
        //        n = 0;
        //    }
        //    //Weapon.SetBool("LayDown", true);
        //    AniTime = STtime - 1f;
        //}
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
            //步槍Z軸準心偏移修正
            GA_R.z += 0.6f;
            if (GA_R.z >= 2.76f) { GA_R.z = 2.76f; }
            //range = Random.Range(-0.05f, 0.05f);  //晃動範圍
            //localEulerAngles跟localRotation的差別
        }
        else
        {
            GA_R.z -= 0.4f;
            if (GA_R.z <= 1) { GA_R.z = 1; }
        }
        GunAimR.transform.localRotation = Quaternion.Euler(0f, -89.66f, GA_R.z);  //Z軸瞄準偏移修正      
        if (coolDownTimer > coolDown) //若冷卻時間已到
        {
            Muzzle_vfx.SetActive(false); //關閉火光
            //可以發射子彈了
            FieldOfView = PlayCamera.GetComponent<Camera>().fieldOfView;
            gFieldOfView = GunCamera.GetComponent<Camera>().fieldOfView;
            //若按下滑鼠右鍵瞄準
            if (Input.GetButton("Fire2") && Reload != true && LayDown == false && !PlayerMove.m_Jumping)  //架槍瞄準
            {
                if (AimIng == false)
                {
                    AimIng = true; //瞄準                  
                    Weapon.SetTrigger("AimUP");                  
                }
                Weapon.SetBool("Aim", true);
                //ZoomIn();
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
                //ZoomOut();
            }
            //若按下滑鼠左鍵開火
            if (Input.GetButton("Fire1") && (DontShooting == false) && (LayDown == false) )
            {
                if(ammunition != 0)
                {
                    MuSmoke.Stop();  //關閉槍口煙霧
                    float rangeY = Random.Range(-40f, 40f);  //射擊水平晃動範圍
                    float rangeX = Random.Range(8f, 16f);  //射擊垂直晃動範圍
                    FireRotateY = (noise * rangeY * (Mathf.Sin(Time.time)) - FireRotateY) / 100;
                    //FireRotateX = (noise * rangeX * (Mathf.Sin(Time.time)) - FireRotateX);
                    FireRotateX = rangeX;
                    if (FireRotateX <= 0) { FireRotateX *= -1; } //強制往上飄
                                                                 //Debug.Log("原本的" + " / " + FireRotateX);
                    if (AimIng || PlayerMove.Squat)
                    {                      
                        FireRotateY /= 2;
                        FireRotateX /= 4;
                    }
                    if (AimIng && PlayerMove.Squat)
                    {
                        FireRotateY /= 4;
                        FireRotateX /= 8;
                    }
                    // Debug.Log("後" + " / " + FireRotateX);

                    transform.localEulerAngles += new Vector3(0.0f, FireRotateY, 0.0f);
                    GunAimR_x.GetComponent<MouseLook>().rotationX -= FireRotateX * Time.smoothDeltaTime;

                    ammunition--;
                    Weapon.SetBool("Fire", true);
                    //Weapon.SetBool("Aim", false);
                    BFire = true;  //生成子彈
                }
                else  //沒子彈
                {
                    GussetMachine();
                }
                coolDownTimer = 0.7f;   //射擊冷卻時間，與coolDown0.8差越小越快
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

        if (Input.GetKeyDown(KeyCode.R) && LayDown == false && Total_ammunition != 0)    //換彈藥
        {
            if (Reload == false)
            {
                Reload = true;
                Weapon.SetTrigger("Reload");
                ReloadWarn.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.T))       //收槍
        {
            Reload = false;
            if (LayDown == false)
            {
                LayDown = true;          
                Weapon.SetTrigger("LayDownT");
                Weapon.SetBool("LayDown", true);            
            }
            else
            {
                LayDown = false;  
                Weapon.SetBool("LayDown", false);
            }         
        }      
        if (Input.GetKeyDown(KeyCode.C) && Reload != true && LayDown == false)  //看槍
        {
            Weapon.SetTrigger("Cherk");
        }
        if (ammunition <= 0)
        {
            ammunition = 0;
        }
        if (Total_ammunition <= 0)
        {
            Total_ammunition = 0;
            Am_zero_Warn.SetActive(true);
        }

        if (HitUI.gameObject.activeSelf)
        {
            HitUI.transform.localScale -= new Vector3(0.15f, 0.15f, 0.15f);
            Vector3 Z = new Vector3(0, 0f, 0f);
            if (HitUI.transform.localScale.x <= Z.x)
            {
                HitUI.gameObject.SetActive(false);
                HitUI.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }
    void ZoomIn()
    {
        if (gFieldOfView > 22f)
        {
            gFieldOfView -= 160f * Time.smoothDeltaTime;
            GunCamera.GetComponent<Camera>().fieldOfView = gFieldOfView;
        }
        if (FieldOfView > 35f)
        {
            FieldOfView -= 140f * Time.smoothDeltaTime;
            PlayCamera.GetComponent<Camera>().fieldOfView = FieldOfView;
        }
    }
    void ZoomOut()
    {
        if (FieldOfView < 55f)
        {
            FieldOfView += 140f * Time.smoothDeltaTime;
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
            pool.ReUse(muzzlePOS, GunCamera.transform.rotation);
            //由攝影機射到是畫面正中央的射線
            Ray ray = GunCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit; //射線擊中資訊
            if (Physics.Raycast(ray, out hit, layerMask)) //擊中牆壁
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))  //彈孔噴黑煙
                {
                    HitType = 0;
                    Debug.DrawLine(ray.origin, hit.point, Color.black, 0.7f, false);
                }
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                {
                    HitType = 0;
                    //繪出起點到射線擊中的綠色線段(起點座標,目標座標,顏色,持續時間,??)      
                    //Debug.DrawLine(ray.origin, hit.point, Color.green, 0.7f, false);                        
                }
                if (hit.collider.tag == "Metal")
                {
                    HitType = 3;
                    AudioManager.Hit(0);
                    Debug.DrawLine(ray.origin, hit.point, Color.blue, 0.3f, false);
                }
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))  //彈孔噴紅血
                {
                    HitType = 1;
                    //Debug.DrawLine(ray.origin, hit.point, Color.red, 0.7f, false);
                    if (hit.collider.tag == "Enemy")  //綠血
                    {
                        HitUI.gameObject.SetActive(true);
                        HitUI.transform.localScale = new Vector3(1f, 1f, 1f);
                        HitType = 2;
                        hit.transform.SendMessage("Damage", power);
                        //Debug.DrawLine(ray.origin, hit.point, Color.blue, 0.3f, true);
                    }
                }
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Actor"))
                {
                    if (hit.collider.tag != "MissionTarget")
                    {
                        NoActor = true;
                    }
                }
            }
            //在到物體上產生彈孔
            rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            pos = hit.point;
            if (NoActor && pos != Vector3.zero)
            {
                NoActor = false;
                pos = HIT.transform.position;
                //pool_Hit.ReUseHit(pos, rot, HitType); ;  //從彈孔池取出彈孔
            }
            else
            {
                pool_Hit.ReUseHit(pos, rot, HitType);  //從彈孔池取出彈孔
            }
            Muzzle_vfx.transform.position = muzzlePOS;
            Muzzle_vfx.transform.rotation = GunCamera.transform.rotation;
            Muzzle_vfx.SetActive(true);
            GunshotsAudio();
            MuSmoke.Play();
            BFire = false;
        }
    }
    void GunshotsAudio()
    {
        AudioManager.PlayGunshotsAudio(1);
    }
    void GussetMachine()  //扣板機
    {
        AudioManager.PlayGunshotsAudio(0);
        ReloadWarn.SetActive(true);
    }
    public static void PlayerRe()
    {
        ammunition = 30;
        Total_ammunition = 300;
        ReloadWarn.SetActive(false);
        Am_zero_Warn.SetActive(false);
    }
    public static void DpsUp()
    {
        power = 1 + Shop.DpsLv;
    }
}
