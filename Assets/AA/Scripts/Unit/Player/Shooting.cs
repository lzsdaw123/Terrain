using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    public Camera PlayCamera, GunCamera;

    public ObjectPool pool;
    public GameObject bullet;  //子彈  
    public GameObject[] Muzzle_vfx;  //槍口火光  
    public ParticleSystem[] MuSmoke;  //槍口煙霧
    public GameObject MuFire_Light;
    public GameObject[] muzzle;  //槍口類型
    public GameObject GunAimR_x;  //X軸瞄準晃動  原Camera
    private Vector3 GA_R;  //槍枝Rotation瞄準偏移修正
    public float noise = 1f; //晃動頻率
    public float noiseRotateX;  //X軸晃動偏移量
    public float noiseRotateY;  //Y軸晃動偏移量
    public float FireRotateY;  //腰射Y軸晃動偏移量
    public float FireRotateX;  //腰射X軸晃動偏移量

    public float coolDown; //冷卻結束時間
    public float coolDownTimer; //冷卻時間計時器
    static int FireButtle;  //開火動畫冷卻
    public PlayerMove controller;  //角色控制腳本
    public float AniTime, STtime;
    bool WeapSwitch; //武器切換bool
    public static int WeaponType; //武器類型
    public int NextWeaponType; //武器類型
    public static int PickUpWeapon;  //取得的武器類型
    public static int[] Equipment = new int[3]; //身上持有的武器 {步槍,左輪, 霰彈槍}
    public static int[] Weapon_of_Pos = new int[2];  //武器放置位置 {主武器,副武器}
    public static bool SwitchWeapon;  //取得武器後切換
    public Animator Weapon;   //動畫控制器
    public static Animator st_Weapon;   //static用動畫控制器
    public static bool FirstWeapon;  //第一把武器
    public static bool FirstAmm;  //第一次拿彈藥
    public GameObject[] _Animator;  //槍枝物件
    public Vector3 muzzlePOS;  //槍口座標

    public bool BFire;  //生成子彈bool
    public bool DontShooting;  //停止射擊bool
    public bool LayDown;  //收槍bool
    public RuntimeAnimatorController[] controllers;  //動畫控制陣列

    public static bool Reload;  //換彈藥bool
    bool AimIng;  //瞄準中bool
    float FieldOfView;  //玩家相機視野
    float gFieldOfView;  //武器相機視野

    public ObjectPool pool_Hit;  //物件池
    public int HitType;  //彈孔類型變數
    public GameObject Hit_vfx, Hit_vfx_S;  //彈孔類型
    public LayerMask layerMask;  //圖層
    Quaternion rot;  //彈孔生成角度
    Vector3 pos;  //彈孔生成位置
    [SerializeField] static GameObject ReloadWarn;
    [SerializeField] static GameObject Am_zero_Warn;
    public static WeaponValue[] Weapons=new WeaponValue[3];
    [SerializeField] private WeaponValue[] SF_Weapons;  //序列化用
    [SerializeField] private int[] SF_Equipment;
    [SerializeField] private int[] SF_Weapon_of_Pos;
    bool Fire1st = false;  
    public bool TargetWall;
    public GameObject[] GunFlashlight; //槍枝手電筒
    public BoxCollider GunCollider;
    public static bool SkipTeach;

    public static void StartAll()
    {
        if(!FirstAmm && !FirstWeapon) Weapons[0] = new WeaponValue(0, 2, 400, 0, 0); //沒槍沒子彈
        else if (!FirstAmm) Weapons[0] = new WeaponValue(0, 2, 400, 30, 0);  //只有槍
        else if (!FirstWeapon) Weapons[0] = new WeaponValue(0, 2, 400, 0, 300);  //只有子彈
        else
        {
            Weapons[0] = new WeaponValue(0, 2, 400, 30, 300);  //步槍(武器位置,威力,射程,彈藥量,總彈藥量)           
        }
        Weapons[1] = new WeaponValue(1, 10, 200, 6, 30);  //電磁手槍(武器位置,威力,射程,彈藥量,總彈藥量)
        Weapons[2] = new WeaponValue(0, 1, 100, 5, 30);  //霰彈槍(武器位置,威力,射程,彈藥量,總彈藥量)
    }
    public void OnBeforeSerialize()  //序列化
    {
        SF_Weapons = Weapons;
        SF_Equipment = Equipment;
        SF_Weapon_of_Pos = Weapon_of_Pos; 
    }
    void Awake()
    {
        StartAll();
        OnBeforeSerialize();
        for (int i = 0; i < Hit_vfx.transform.childCount; i++)
        {
            Hit_vfx_S = Hit_vfx.transform.GetChild(i).gameObject;
            Hit_vfx_S.SetActive(false);
        }
        WeaponType = 0;
        _Animator[0].SetActive(true);
        _Animator[1].SetActive(false);

        Equipment = new int[] { 0, 0, 0};  //身上持有的武器
        Weapon_of_Pos = new int[] { 0, 0};  //武器放置位置
        Reload = false;
        DontShooting = false;
        LayDown = true;
        WeapSwitch = false;
        FireButtle = 1;
        PickUpWeapon = 0;
        SwitchWeapon = false;
        FirstWeapon = false;
    }
    void Start()
    {
        coolDown = 1f;  //冷卻結束時間
        coolDownTimer = coolDown + 1;
        pool_Hit = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
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

        AniTime = STtime = 2f;

        Muzzle_vfx[WeaponType].SetActive(false);
        MuFire_Light.SetActive(false);
        MuSmoke[WeaponType].Stop();
        Weapon.SetBool("LayDown", true);
        for (int i = 0; i < GunFlashlight.Length; i++)  //預設關手電筒
        {
            GunFlashlight[i].SetActive(false);
        }
    }
    void Update()
    {
        if (Time.timeScale == 0) {return;}
        FieldOfView = PlayCamera.GetComponent<Camera>().fieldOfView;
        gFieldOfView = GunCamera.GetComponent<Camera>().fieldOfView;
        if (Input.GetButton("Fire2") && !Reload && !PlayerMove.m_Jumping )  //右鍵縮放鏡頭
        {
            ZoomIn();
        }
        else
        {
            ZoomOut();
        }
        DontShooting = AnimEvents.DontShooting;  //取得AnimEvents腳本變數
        if (SwitchWeapon)  //拾取武器並切換
        {
            SwitchWeapon = false;
            NextWeaponType = PickUpWeapon;
            
            WeapSwitching();  //開始換武器
        }
        if ( AniTime >=2)  //切換武器
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && WeaponType!=0)  //主武器
            {
                if(Equipment[0] == 1)
                {
                    NextWeaponType = 0;
                    WeapSwitching();
                }
                if (Equipment[2] == 1)
                {
                    NextWeaponType = 2;
                    WeapSwitching();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && WeaponType != 1 && Equipment[1] == 1)  //副武器
            {
                NextWeaponType = 1;
                WeapSwitching();
            }
        }
        void WeapSwitching()  //武器切換
        {
            if (!WeapSwitch)
            {
                if (!FirstWeapon )
                {
                    FirstWeapon = true;
                    Ammunition.showUI();
                    if (FirstWeapon && FirstAmm && !SkipTeach)  //第一次取得武器和彈藥
                    {
                        Level_1.NextTask(2);
                    }
                    LayDown = false;
                    StartAll();
                }
                else
                {
                    Weapon.SetTrigger("LayDownT");
                }
                AniTime = STtime - 1.6f;
                WeapSwitch = true;
            }
        }
        if (AniTime <= 0)
        {
            WeapSwitch = false;
            _Animator[WeaponType].SetActive(false);
            switch (NextWeaponType)  //切換武器模型
            {
                case 0:
                    WeaponType = 0;
                    break;
                case 1:
                    WeaponType = 1;
                    break;
                case 2:
                    WeaponType = 2;
                    break;
            }
            _Animator[WeaponType].SetActive(true);
            MuSmoke[WeaponType].Stop();
            Weapon = _Animator[WeaponType].GetComponent<Animator>();
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
            //槍枝準心偏移修正
            if (WeaponType == 0)
            {
                GA_R.z += 0.6f;
                GA_R.y = -89.66f;
                if (GA_R.z >= 2.76f) { GA_R.z = 2.76f; }
            }
            if (WeaponType == 1)
            {
                GA_R.y = -90.2f;
                GA_R.z -= 15f * Time.smoothDeltaTime;
                if (GA_R.z <= -1.6f) { GA_R.z = -1.6f; }
            }

            //range = Random.Range(-0.05f, 0.05f);  //晃動範圍
            //localEulerAngles跟localRotation的差別
        }
        else  //腰射
        {          
            if (WeaponType == 0)
            {
                GA_R.z -= 0.4f;
                GA_R.y = -89.66f;
                if (GA_R.z <= 1) { GA_R.z = 1; }
            }
            if (WeaponType == 1)
            {
                GA_R.y = -90.2f;
                GA_R.z += 9f*Time.smoothDeltaTime;
                if (GA_R.z >= 0.5) { GA_R.z = 0.5f; }
            }
        }
        if (FireButtle == 1)
        {
            Weapon.SetBool("Fire", false);
            Weapon.SetBool("AimFire", false);
        }
        _Animator[WeaponType].transform.localRotation = Quaternion.Euler(0.01f, GA_R.y, GA_R.z);  //槍枝Rotation瞄準偏移修正
        if (coolDownTimer > coolDown) //若冷卻時間已到 可以發射子彈
        {
            Muzzle_vfx[WeaponType].SetActive(false); //關閉火光
            Weapon.SetBool("Fire", false);
            Weapon.SetBool("AimFire", false);
            //若按下滑鼠右鍵瞄準
            if (Input.GetButton("Fire2") && !Reload && LayDown == false && !PlayerMove.m_Jumping && !WeapSwitch)  //架槍瞄準
            {
                if (AimIng == false)
                {
                    AimIng = true; //瞄準                  
                    Weapon.SetTrigger("AimUP");                  
                }
                Weapon.SetBool("Aim", true);
                //ZoomIn();
                //瞄準射擊
                if (Input.GetButton("Fire1") && DontShooting == false && LayDown == false && Weapons[WeaponType].WeapAm != 0)
                {                 
                    if (FireButtle == 1)
                    {
                        Weapon.SetBool("AimFire", true);
                        FireButtle = 0;
                    }
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
            if (Input.GetButton("Fire1") && DontShooting == false && LayDown == false && !Reload && !WeapSwitch)
            {
                if(Weapons[WeaponType].WeapAm != 0)
                {
                    MuSmoke[WeaponType].Stop();  //關閉槍口煙霧
                    float[] FRxMin = new float[] { 8, 12, 14 };
                    float[] FRxMax = new float[] {16, 24, 28 };
                    float rangeY = Random.Range(-40f, 40f);  //射擊水平晃動範圍
                    float rangeX = Random.Range(FRxMin[WeaponType], FRxMax[WeaponType]);  //射擊垂直晃動範圍
                    FireRotateY = (noise * rangeY * Mathf.Sin(Time.time) - FireRotateY) / 100;
                    //FireRotateX = (noise * rangeX * (Mathf.Sin(Time.time)) - FireRotateX);
                    FireRotateX = rangeX;
                    if (FireRotateX <= 0) { FireRotateX *= -1; } //強制往上飄
                    if (AimIng || PlayerMove.Squat)  //瞄準或蹲下
                    {                      
                        FireRotateY /= 2;
                        FireRotateX /= 4;
                    }
                    if (AimIng && PlayerMove.Squat)  //瞄準並蹲下
                    {
                        FireRotateY /= 4;
                        FireRotateX /= 8;
                    }
                    // Debug.Log("後" + " / " + FireRotateX);
                    //print(FireRotateX + "," + FireRotateY);
                    transform.localEulerAngles += new Vector3(0.0f, FireRotateY, 0.0f) *Time.deltaTime;
                    GunAimR_x.GetComponent<MouseLook>().rotationX -= FireRotateX * Time.deltaTime;
                    Weapons[WeaponType].WeapAm--;
                    if (FireButtle==1)
                    {
                        Weapon.SetBool("Fire", true);
                        FireButtle = 0;
                    }
                    BFire = true;  //生成子彈
                    //Weapon.SetBool("Aim", false);                    
                }
                else  //沒子彈
                {
                    GussetMachine();
                }
                switch (WeaponType)  //開火冷卻時間，與coolDown 1差越小越快
                {
                    case 0:
                        coolDownTimer = 0.9f;
                        break;
                    case 1:
                        coolDownTimer = 0.2f;   
                        break;
                    case 2:
                        coolDownTimer = 0.25f;
                        break;
                }
            }
            else
            {
                Weapon.SetBool("Fire", false);
            }       
        }
        else //否則需要冷卻計時
        {
            coolDownTimer += Time.deltaTime;
            //Weapon.SetBool("Fire", false);
        }      
        if (Input.GetKeyDown(KeyCode.T) && FirstWeapon)       //收槍
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
        if (Input.GetKeyDown(KeyCode.F))  //槍枝手電筒
        {
            if (!GunFlashlight[WeaponType].activeSelf)
            {
                for (int i =0; i< GunFlashlight.Length; i++)
                {
                    GunFlashlight[i].SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < GunFlashlight.Length; i++)
                {
                    GunFlashlight[i].SetActive(false);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.C) && Reload != true && LayDown == false)  //看槍
        {
            Weapon.SetTrigger("Cherk");
        }
        if (Weapons[WeaponType].WeapAm <= 0)  //彈藥最小為0
        {
            Weapons[WeaponType].WeapAm = 0;
        }
        if (Weapons[WeaponType].T_WeapAm <= 0 && FirstWeapon && FirstAmm)  //總彈藥最小為0
        {
            Weapons[WeaponType].T_WeapAm = 0;
            Am_zero_Warn.SetActive(true);
        }

        void ZoomIn()  //鏡頭拉近
        {
            if (gFieldOfView > 22f)
            {
                gFieldOfView -= 160f * Time.smoothDeltaTime;
                if (WeaponType == 0)
                {
                    if (gFieldOfView <= 22f)
                    {
                        gFieldOfView = 22f;
                    }
                }
                else
                {
                    if (gFieldOfView <= 40f)
                    {
                        gFieldOfView = 40f;
                    }
                }
                //GunCamera.GetComponent<Camera>().fieldOfView = gFieldOfView;
            }
            if (FieldOfView > 32)
            {
                FieldOfView -= 140f * Time.smoothDeltaTime;
                if (FieldOfView <= 32f)
                {
                    FieldOfView = 32f;
                }
                PlayCamera.GetComponent<Camera>().fieldOfView = FieldOfView;
            }
        }
        void ZoomOut()  //鏡頭拉遠
        {
            if (FieldOfView < 55f)
            {
                FieldOfView += 140f * Time.smoothDeltaTime;
                if (FieldOfView >= 55f)
                {
                    FieldOfView = 55f;
                }
                PlayCamera.GetComponent<Camera>().fieldOfView = FieldOfView;
                //GunCamera.GetComponent<Camera>().fieldOfView = FieldOfView;
            }
        }
        TargetWall = ShootingRange.TargetWall;
        if (MuFire_Light.activeSelf)
        {
            MuFire_Light.GetComponent<Light>().range -= 80 * Time.deltaTime;
            float Range = MuFire_Light.GetComponent<Light>().range;
            if (Range <= 0)
            {
                MuFire_Light.SetActive(false);
                MuFire_Light.GetComponent<Light>().range = 20;
            }
        }
    } 
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R) && !LayDown && Weapons[WeaponType].T_WeapAm != 0)    //換彈藥
        {
            if (Reload == false)
            {
                FireButtle = 0;
                Reload = true;
                Weapon.SetTrigger("Reload");
                ReloadWarn.SetActive(false);
            }
        }
        if (BFire) //生成子彈
        {
            muzzlePOS = muzzle[WeaponType].GetComponent<Transform>().position;  //槍口位置
            //GameObject obj = Instantiate(bullet, muzzlePOS, PlayCamera.transform.rotation); //建立子彈在鏡頭中心位置
            if(WeaponType==0) pool.ReUse(muzzlePOS, GunCamera.transform.rotation);  //呼叫子彈

            int rayNum = 1;  //射線數量       
            float distance = Weapons[WeaponType].distance;  //射程
            float MixR = 2, MaxR = 2;  //畫面位置
            if (WeaponType == 0)  //步槍
            {
                rayNum = 1;
                MixR = 2f;
                MaxR = 2f;
            }
            if (WeaponType == 1)  //手槍
            {
                rayNum = 9;                
            }
            if (WeaponType == 2)  //霰彈槍
            {
                rayNum = 12;
                MixR = 1.86f;
                MaxR = 2.17f;
            }
            Ray[] ray = new Ray[rayNum];
            RaycastHit[] hit = new RaycastHit[rayNum]; //射線擊中資訊       

            for (int n = 0; n < ray.Length; n++)
            {
                if (WeaponType == 1)  //手槍
                {
                    float[] RangeX = new float[] { 2f, 2f, 2.03f, 2.04f, 2.03f, 2f, 1.97f, 1.96f, 1.97f};
                    float[] RangeY = new float[] { 2f, 2.04f, 2.03f, 2f, 1.97f, 1.96f, 1.97f, 2f, 2.03f};
                    //由攝影機射到是畫面正中央的射線
                    ray[n] = GunCamera.ScreenPointToRay(new Vector3(Screen.width / RangeX[n], Screen.height / RangeY[n], 0));
                }
                else
                {
                    float RangeX = Random.Range(MixR, MaxR);
                    float RangeY = Random.Range(MixR, MaxR);
                    //由攝影機射到是畫面正中央的射線
                    ray[n] = GunCamera.ScreenPointToRay(new Vector3(Screen.width / RangeX, Screen.height / RangeY, 0));
                }
                if (Physics.Raycast(ray[n], out hit[n], distance, layerMask))  //擊中圖層
                {
                    //Debug.DrawLine(ray[n].origin, hit[n].point, Color.green, 1f, false);
                    if (hit[n].collider.gameObject.layer == LayerMask.NameToLayer("Ground"))  //彈孔噴黑煙
                    {
                        HitType = 0;
                        //Debug.DrawLine(ray.origin, hit.point, Color.black, 0.7f, false);
                    }
                    if (hit[n].collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                    {
                        HitType = 0;
                        //繪出起點到射線擊中的綠色線段(起點座標,目標座標,顏色,持續時間,是否被靠近相機的物體遮住)      
                        //Debug.DrawLine(ray.origin, hit.point, Color.green, 0.7f, false);
                    }
                    if (hit[n].collider.tag == "Metal")  //金屬
                    {
                        HitType = 3;
                        AudioManager.Hit(0);
                        //Debug.DrawLine(ray1[n].origin, hit1[n].point, Color.blue, 0.3f, false);
                    }
                    if (hit[n].collider.gameObject.layer == LayerMask.NameToLayer("Monster"))  //彈孔噴紅血
                    {
                        HitType = 1;
                        //Debug.DrawLine(ray.origin, hit.point, Color.red, 0.7f, false);
                        if (hit[n].collider.tag == "Enemy")  //綠血
                        {
                            HitType = 2;
                            hit[n].transform.SendMessage("Unit", true);  //攻擊者為玩家?
                            if (WeaponType == 1)  //電磁手槍傷害
                            {
                                if (n == 0)
                                {
                                    hit[0].transform.SendMessage("Damage", Weapons[WeaponType].power / 2 +1);  //造成傷害
                                }
                                else
                                {
                                    hit[n].transform.SendMessage("Damage", Weapons[WeaponType].power / 10);  //造成範圍傷害
                                }
                            }
                            else
                            {
                                hit[n].transform.SendMessage("Damage", Weapons[WeaponType].power);  //造成傷害
                            }
                            //Debug.DrawLine(ray.origin, hit.point, Color.blue, 0.3f, false);
                        }
                        if (hit[n].collider.tag == "Carapace")  //甲殼
                        {
                            HitType = 4;
                            if (WeaponType == 1)
                            {
                                hit[n].transform.SendMessage("Unit", true);  //攻擊者為玩家?
                                if (n == 0)
                                {
                                    hit[0].transform.SendMessage("Damage", Weapons[WeaponType].power / 5);  //造成一半傷害
                                }
                                else
                                {
                                    hit[n].transform.SendMessage("Damage", Weapons[WeaponType].power / 20);  //造成一半範圍傷害
                                }
                            }
                        }
                    }
                }
                //在到物體上產生彈孔
                rot = Quaternion.FromToRotation(Vector3.up, hit[n].normal);
                pos = hit[n].point;
                if (TargetWall) HitType = 0;
                if (WeaponType == 1)
                {            
                    rot = Quaternion.FromToRotation(Vector3.up, hit[0].normal);
                    pos = hit[n].point;
                    HitType = 5;
                    if (!Fire1st)
                    {
                        pool_Hit.ReUseHit(pos, rot, HitType);  //從彈孔池取出彈孔
                        Fire1st = true;
                    }
                }
                else
                {
                    pool_Hit.ReUseHit(pos, rot, HitType);  //從彈孔池取出彈孔
                }            
            }
            Muzzle_vfx[WeaponType].transform.position = muzzlePOS;
            Muzzle_vfx[WeaponType].transform.rotation = GunCamera.transform.rotation;
            Muzzle_vfx[WeaponType].SetActive(true);
            MuFire_Light.SetActive(true);
            GunshotsAudio();
            MuSmoke[WeaponType].transform.position = muzzlePOS;
            MuSmoke[WeaponType].Play();
            BFire = false;
            Fire1st = false;
        }
    }
    void LateUpdate()
    {
        OnBeforeSerialize();
        st_Weapon = Weapon;
    }
    public static void ReLoad_E()  //換彈結束
    {
        Weapons[WeaponType].WeapAm = AnimEvents.ammunition;
        Weapons[WeaponType].T_WeapAm = AnimEvents.Total_ammunition;
        Reload = false;
        FireButtle = 1;
    }
    void GunshotsAudio()  //開槍音效
    {
        AudioManager.PlayGunshotsAudio(1);
    }
    void GussetMachine()  //扣板機
    {
        AudioManager.PlayGunshotsAudio(0);
        ReloadWarn.SetActive(true);
    }
    public static void PlayerRe()  //玩家重生
    {
        StartAll();
        ReloadWarn.SetActive(false);
        Am_zero_Warn.SetActive(false);
    }
    public static void DpsUp()
    {
        Weapons[WeaponType].power = 1 + Shop.DpsLv;
    }
    public static void Loaded()  //武器上膛
    {
        FireButtle = 1;
    }
    public static void PickUpWeapons(int _WeaponsType, int WeaponPos)  //拾取武器
    {
        if (Equipment[_WeaponsType] != 1)  //身上武器是否已有同個武器
        {
            PickUpWeapon = _WeaponsType;
            Equipment[_WeaponsType] = 1;

            if (Weapon_of_Pos[WeaponPos] == 0)  //武器位置是否為空
            {
                Weapon_of_Pos[WeaponPos] = 1;
            }
            else if (Weapon_of_Pos[WeaponPos] == 1)
            {
                Weapon_of_Pos[WeaponPos] = 1;
            }
            SwitchWeapon = true;
        }   
    }
    public static void PickUpAmm()  //第一次拿彈藥
    {
        FirstAmm = true; 
        Ammunition.showUI();
        if (FirstWeapon && FirstAmm && !SkipTeach)  //第一次取得武器和彈藥
        {
            Level_1.NextTask(2);
        }
        st_Weapon.SetTrigger("Reload");
        Reload = true;
        FireButtle = 0;
        ReloadWarn.SetActive(false);
        StartAll();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other == GunCollider)
        {
            //print("碰到槍了");

            if (other.gameObject.layer != LayerMask.NameToLayer("Weapon"))
            {

            }
        }
    }
}
