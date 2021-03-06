using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    public Camera PlayCamera, GunCamera;

    public ObjectPool pool;
    public MouseLook mouseLook;
    public GameObject bullet;  //子彈  
    public GameObject[] Muzzle_vfx;  //槍口火光  
    public ParticleSystem[] MuSmoke;  //槍口煙霧
    public GameObject[] MuFire_Light;  //開火光影
    public GameObject[] muzzle;  //槍口類型
    public GameObject[] GrenadeMuzzle;  //手雷槍口  
    public GameObject GunAimR_x;  //X軸瞄準晃動  原Camera
    private Vector3 GA_P;  //槍枝Rotation瞄準偏移修正
    private Vector3 GA_R;  //槍枝Rotation瞄準偏移修正
    public float noise = 1f; //晃動頻率
    public float noiseRotateX;  //X軸晃動偏移量
    public float noiseRotateY;  //Y軸晃動偏移量
    public float FireRotateY;  //腰射Y軸晃動偏移量
    public float FireRotateX;  //腰射X軸晃動偏移量
    public float OriFireRotateY;  //Y軸原本位置
    public float OriFireRotateX;  //X軸原本位置
    float FireRotateReTime;

    public float coolDown; //冷卻結束時間
    public float coolDownTimer; //冷卻時間計時器
    public static int FireButtle;  //開火動畫冷卻
    public PlayerMove playerMove;  //角色控制腳本
    public HeroLife heroLife;
    public float AniTime, STtime;
    bool WeapSwitch; //武器切換bool
    public static int WeaponType; //武器類型
    [SerializeField] int SF_WeaponType; //武器類型
    public int NextWeaponType; //武器類型
    public static int PickUpWeapon;  //取得的武器類型
    public static int[] Equipment = new int[3]; //身上持有的武器 {步槍,左輪, 霰彈槍}
    public static int[] Weapon_of_Pos = new int[2];  //武器放置位置 {主武器,副武器}
    public static GameObject[] WeaponsPosOb = new GameObject[2];
    public static bool SwitchWeapon;  //取得武器後切換
    public Animator Weapon;   //動畫控制器
    public static Animator st_Weapon;   //static用動畫控制器
    public static bool[] FirstWeapon= new bool[2];  //第一把武器
    [SerializeField] bool[] SF_FirstWeapon;  //第一把武器
    public static bool FirstAmm;  //第一次拿彈藥
    public GameObject[] _Animator;  //槍枝物件
    public Vector3 muzzlePOS;  //槍口座標

    public bool BFire;  //生成子彈bool
    public bool DontShooting;  //停止射擊bool
    public bool shooting;  //射擊bool
    public bool LayDown;  //收槍bool
    public RuntimeAnimatorController[] controllers;  //動畫控制陣列

    public static bool Reload;  //換彈藥bool
    [SerializeField] bool SF_Reload;  //換彈藥bool
    public static bool ReReload;  //換彈藥bool
    [SerializeField] bool SF_ReReload;  //換彈藥bool
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
    [SerializeField] private GameObject[] SF_WeaponsPosOb;
    bool Fire1st = false;  
    public bool TargetWall;
    public GameObject[] GunFlashlight; //槍枝手電筒
    public static bool SkipTeach;
    Image Aim;
    static UpgradeValue[] 武器欄位;
    [SerializeField] UpgradeValue[] SF_武器欄位;
    public 部位[] 部位;
    [SerializeField] int 部位ID, 零件ID;
    public static bool 換部件;
    public float PowerAdd;
    public float 輸出傷害;
    static bool Bomb=true;
    public bool hitDamage;
    public static int GrenadeNub; //手榴彈數量
    [SerializeField] int SF_GrenadeNub; //手榴彈數量
    public int SaveGN;  //保存手榴彈數量
    public static bool GetGrenade;  //取得手榴彈
    public GameObject GrenadeUI;
    public GameObject[] Grenade;  //手榴彈物件
    public static GameObject GetGrenadeObj;  //取得的手榴彈物件
    public static bool UseGrenade;
    public Material[] material;
    public Texture2D[] texture2Ds;
    public static int[] Save_Magazine=new int[3] {30, 6, 5 };  //保存換過的彈匣
    [SerializeField] int[] SF_Save_Magazine=new int[3];  //保存換過的彈匣
    public static int JumpDown;
    [SerializeField] int SF_JumpDown;
    public float NoM_T;
    public bool F_W;

    public static void StartAll()
    {
        if(!FirstAmm && !FirstWeapon[0]) Weapons[0] = new WeaponValue(0, 2, 400, 0, 0, 30); //沒槍沒子彈
        else if (!FirstAmm) Weapons[0] = new WeaponValue(0, 2, 400, 30, 0, 30);  //只有槍
        //else if (!FirstWeapon) Weapons[0] = new WeaponValue(0, 2, 400, 0, 300);  //只有子彈
        else
        {
            Weapons[0] = new WeaponValue(0, 2, 400, 30, 300, 30);  //步槍(武器位置,威力,射程,彈藥量,總彈藥量,彈匣容量)           
        }
        Weapons[1] = new WeaponValue(1, 1f, 200, 6, 30, 6);  //電磁手槍(武器位置,威力,射程,彈藥量,總彈藥量,彈匣容量)
        Weapons[2] = new WeaponValue(0, 1, 100, 5, 30, 5);  //霰彈槍(武器位置,威力,射程,彈藥量,總彈藥量,彈匣容量)
        for(int i=0; i< Save_Magazine.Length; i++)
        {
            Weapons[0].WeapAm = Save_Magazine[0];
        }
    }
    public void OnBeforeSerialize()  //序列化
    {
        SF_Weapons = Weapons;
        SF_Equipment = Equipment;
        SF_Weapon_of_Pos = Weapon_of_Pos;
        SF_WeaponsPosOb = WeaponsPosOb; 

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
        Weapon_of_Pos = new int[] { -1, -1};  //武器放置位置
        Reload = ReReload = false;
        DontShooting = false;
        LayDown = true;
        WeapSwitch = false;
        FireButtle = 1;
        PickUpWeapon = 0;
        SwitchWeapon = false;
        FirstWeapon = new bool[] { false, false, false };
        F_W = false;
        Grenade = new GameObject[3]; //手雷上限
        PowerAdd = 1;
    }
    void Start()
    {
        coolDown = 1f;  //冷卻結束時間
        coolDownTimer = coolDown + 1;
        GetGrenade = UseGrenade = false;
        pool_Hit = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        ReloadWarn = GameObject.Find("ReloadWarn").gameObject;
        Am_zero_Warn = GameObject.Find("Am_zero_Warn").gameObject;
        ReloadWarn.SetActive(false);
        Am_zero_Warn.SetActive(false);
        Hit_vfx_S = null;
        Weapon.runtimeAnimatorController = controllers[0];

        if (playerMove == null)
        {
            playerMove = GetComponent<PlayerMove>();
        }

        AniTime = STtime = 2f;

        Muzzle_vfx[WeaponType].SetActive(false);
        MuSmoke[WeaponType].Stop();
        MuSmoke[WeaponType].gameObject.SetActive(false);
        for (int i = 0; i < GunFlashlight.Length; i++)  //預設關手電筒
        {
            GunFlashlight[i].SetActive(true);
            MuFire_Light[i].SetActive(false);
        }
        Aim =  Save_Across_Scene.Aim.GetComponent<Image>();
        Weapon.SetBool("LayDown", true);
    }
    void Update()
    {
        SF_WeaponType = WeaponType;
        SF_GrenadeNub = GrenadeNub;
        SF_Reload = Reload;
        SF_ReReload = ReReload;
        SF_Save_Magazine = Save_Magazine;
        SF_武器欄位 = 武器欄位;
        SF_JumpDown = JumpDown;
        if (換部件)
        {
            //print("換部件");
            換部件 = false;
            部位ID = UpgradeWorkbench.部位ID;
            零件ID = UpgradeWorkbench.零件ID;
            for (int i = 0; i < 部位[部位ID].Part.Length; i++)
            {
                部位[部位ID].Part[i].SetActive(false);  //關全部零件
            }
            部位[部位ID].Part[零件ID].SetActive(true);  //打開選擇零件
            switch (部位ID)
            {
                case 0:
                    switch (零件ID)
                    {
                        case 0:  //不使用
                            MuFire_Light[0].GetComponent<Light>().intensity = 675000;  // =( intensity * At* At)
                            MuFire_Light[0].GetComponent<Light>().range = 20;
                            ParticleSystem ps0 = Muzzle_vfx[0].transform.GetChild(1).GetComponent<ParticleSystem>();  //粒子
                            var sh0 = ps0.shape;
                            sh0.angle = 12;
                            Muzzle_vfx[0].transform.GetChild(2).GetComponent<Transform>().localScale = new Vector3(1.5f, 2.756f, 1.5f);
                            break;
                        case 1:  //消焰器
                            MuFire_Light[0].GetComponent<Light>().intensity = 75000;
                            MuFire_Light[0].GetComponent<Light>().range = 5;
                            ParticleSystem ps1 = Muzzle_vfx[0].transform.GetChild(1).GetComponent<ParticleSystem>();
                            var sh1 = ps1.shape;
                            sh1.angle = 3;
                            Muzzle_vfx[0].transform.GetChild(2).GetComponent<Transform>().localScale = new Vector3(0.4f, 2.756f, 0.4f);
                            break;
                        case 2:  //補償器
                            MuFire_Light[0].GetComponent<Light>().intensity = 300000;
                            MuFire_Light[0].GetComponent<Light>().range = 15;
                            ParticleSystem ps2 = Muzzle_vfx[0].transform.GetChild(1).GetComponent<ParticleSystem>();
                            var sh2 = ps2.shape;
                            sh2.angle = 8;
                            Muzzle_vfx[0].transform.GetChild(2).GetComponent<Transform>().localScale = new Vector3(1.1f, 2.756f, 1.1f);
                            break;
                    }
                    break;
                case 1:
                    Weapons[0].WeapAm = 武器欄位[0].Ammo;
                    Weapons[0].Magazine = 武器欄位[0].Ammo;
                    for (int i = 0; i < Save_Magazine.Length; i++)
                    {
                        Save_Magazine[0] = Weapons[0].Magazine;  //只有步槍
                    }                 
                    break;
            }
            GetComponent<Shooting>().enabled = false;
        }



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
        if (SwitchWeapon && !Reload)  //拾取武器並切換
        {
            SwitchWeapon = false;
            NextWeaponType = PickUpWeapon;  //下把武器
            WeapSwitching();  //開始換武器
        }
        if ( AniTime >=2)  //切換武器
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && WeaponType!=0 && !Reload)  //主武器
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
            if (Input.GetKeyDown(KeyCode.Alpha2) && WeaponType != 1 && Equipment[1] == 1 && !Reload)  //副武器
            {
                NextWeaponType = 1;
                WeapSwitching();
            }
        }
        SF_FirstWeapon = FirstWeapon;
        void WeapSwitching()  //武器切換
        {
            if (heroLife.Level >= 4) return;   //水晶感染末期
            if (!WeapSwitch)  //是否切換武器
            {
                if (!FirstWeapon[NextWeaponType] )
                {
                    FirstWeapon[NextWeaponType] = true;
                    if (FirstWeapon[0] && !SkipTeach && !F_W)  //第一次取得步槍和沒跳教學
                    {
                        F_W = true;
                        Ammunition.showUI();
                        DialogueEditor.StartConversation(0, 2, 0, true, 0, true);  //開始對話
                        StartAll();
                    }
                    //if (FirstWeapon[1] && Level_1.TotalStage==1)  //第一次取得左輪
                    //{
                    //    Level_1.UiOpen = true;  //開啟任務UI與音效
                    //    Level_1.StopAttack = false;  //怪物繼續進攻
                    //    Level_1.stageTime = 25;  //怪物繼續倒數開始
                    //    PlayerView.missionChange(Defense.s_Level, Defense.s_Stage);  //改變關卡
                    //    DialogueEditor.StartConversation(2, 2, 0, false, 0, true);  //開始對話
                    //}
                    LayDown = false;
                }
                else
                {
                    Weapon.SetTrigger("LayDownT");
                }
                AniTime = STtime - 1.6f;  //換武器時間
                WeapSwitch = true;
                Ammunition.Switch(NextWeaponType);
            }
        }
        if (AniTime <= 0)
        {
            if (heroLife.Level > 0)
            {
                heroLife.InfectionSW = true;
            }
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
            MuSmoke[WeaponType].gameObject.SetActive(false);
            Weapon = _Animator[WeaponType].GetComponent<Animator>();
            if (Weapons[WeaponType].WeapAm != 0)  //當前武器彈藥大於0
            {
                ReloadWarn.SetActive(false);  //彈藥足
            }
            if(Weapons[WeaponType].T_WeapAm != 0)  //當前武器彈藥大於0
            {
                Am_zero_Warn.SetActive(false);  //總彈藥足
            }
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
                GA_R.x =  0.01f;
                GA_R.y = -89.66f;
                GA_R.z += 16f * Time.smoothDeltaTime;
                if (GA_R.z >= 2.76f) { GA_R.z = 2.76f; }
                GA_R.z = 2.76f;
            }
            if (WeaponType == 1)
            {
                GA_R.x = 0.012f;
                GA_R.y = -90.2f;
                GA_R.z -= 16f * Time.smoothDeltaTime;
                if (GA_R.z <= -1.6f) { GA_R.z = -1.6f; }
                GA_R.z = -1.6f;
            }
            if (WeaponType == 2)
            {
                GA_P.x = 0.03f;
                GA_R.x = -180.01f;
                GA_R.y = 91f;
                GA_R.z = 183.7f;
                //GA_R.z -= 16f * Time.smoothDeltaTime;
                if (GA_R.z <= 183.7) { GA_R.z = 183.7f; }
            }

            //range = Random.Range(-0.05f, 0.05f);  //晃動範圍
            //localEulerAngles跟localRotation的差別
        }
        else  //腰射
        {          
            if (WeaponType == 0)
            {
                GA_P.x = 0.01f;

                GA_R.x = 0.01f;
                GA_R.y = -89.66f;
                GA_R.z -= 13f * Time.smoothDeltaTime;
                if (GA_R.z <= 1) { GA_R.z = 1; }
                GA_R.z = 1;
            }
            if (WeaponType == 1)
            {
                GA_P.x = -0.027f;
                GA_R.x = 0.012f;
                GA_R.y = -90.2f;
                GA_R.z += 10f*Time.smoothDeltaTime;
                if (GA_R.z >= 0.5) { GA_R.z = 0.5f; }
                GA_R.z = 0.5f;
            }
            if (WeaponType == 2)
            {
                GA_P.x = 0.23f;
                GA_R.x = -179.6f;
                GA_R.y = 85.9f;
                //GA_R.z += 16f * Time.smoothDeltaTime;
                GA_R.z = 184.7f;
                if (GA_R.z >= 184.7) { GA_R.z = 184.7f; }
            }
        }
        if (FireButtle == 1)
        {
            Weapon.SetBool("Fire", false);
            Weapon.SetBool("AimFire", false);
        }
        Vector3 oriV3 = _Animator[WeaponType].transform.localPosition;
        _Animator[WeaponType].transform.localPosition = new Vector3(GA_P.x, oriV3.y, oriV3.z);  //槍枝Rotation瞄準偏移修正
        _Animator[WeaponType].transform.localRotation = Quaternion.Euler(GA_R.x, GA_R.y, GA_R.z);  //槍枝Rotation瞄準偏移修正
        mouseLook.Logstr[3] = "\n 武器 position : " + _Animator[WeaponType].transform.position;
        mouseLook.Logstr[4] = "\n 武器 GA_R : " + GA_R;
        mouseLook.Logstr[5] = "\n 武器 localEulerAngles : " + _Animator[WeaponType].transform.localEulerAngles;
        if (coolDownTimer > coolDown) //若冷卻時間已到 可以發射子彈
        {
            Muzzle_vfx[WeaponType].SetActive(false); //關閉火光
            Weapon.SetBool("Fire", false);
            Weapon.SetBool("AimFire", false);
            //if (heroLife.Level >= 4) return;  //水晶感染末期
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
                if (Weapons[WeaponType].WeapAm != 0)
                {
                    if (!shooting)
                    {
                        shooting = true;
                        OriFireRotateY = transform.localEulerAngles.y;
                        OriFireRotateX = GunAimR_x.GetComponent<MouseLook>().rotationX;
                    }
                    MuSmoke[WeaponType].Stop();  //關閉槍口煙霧
                    //--開火後座力-------------------開火後座力--------------------
                    float[] FRxMin = new float[] { 4*2, 14*2, 16*2 };  //最小垂直晃動 x
                    float[] FRxMax = new float[] {8*2, 26*2, 30*2 };  //最大垂直晃動 x
                    float rangeY = Random.Range(-40+20f, 40+20f);  //射擊水平晃動範圍
                    float rangeX = Random.Range(FRxMin[WeaponType], FRxMax[WeaponType]);  //射擊垂直晃動範圍
                    FireRotateY = (武器欄位[WeaponType].Recoil * rangeY * Mathf.Sin(Time.time) - FireRotateY) / 100; //水平後座力(槍口部件* rangeX)
                    //FireRotateX = (noise * rangeX * (Mathf.Sin(Time.time)) - FireRotateX);
                    FireRotateX = rangeX * 武器欄位[WeaponType].Recoil;  //垂直後座力(rangeX * 槍口部件)
                    float rotationY = Random.Range(-1.6f, 1.6f) * 武器欄位[WeaponType].Recoil;  //開火後相機垂直晃動範圍
                    if (FireRotateX <= 0) { FireRotateX *= -1; } //強制往上飄
                    if (AimIng || PlayerMove.Squat)  //瞄準或蹲下
                    {                      
                        FireRotateY /= 2;
                        FireRotateX /= 8;
                        rotationY /= 2f;
                    }
                    if (AimIng && PlayerMove.Squat)  //瞄準並蹲下
                    {
                        FireRotateY /= 4;
                        FireRotateX /= 2;
                        rotationY /= 3.5f;
                    }
                    // Debug.Log("後" + " / " + FireRotateX);
                    //print(FireRotateX + "," + FireRotateY);
                    transform.localEulerAngles += new Vector3(0.0f, FireRotateY, 0.0f) *Time.deltaTime;  //水平晃動
                    GunAimR_x.GetComponent<MouseLook>().rotationY = rotationY * 2 * Time.deltaTime;  //鏡頭垂直晃動
                    float oriX = GunAimR_x.GetComponent<MouseLook>().rotationX;  //原本位置
                    float newX= GunAimR_x.GetComponent<MouseLook>().rotationX- FireRotateX;  //後座力位置
                    //print("舊的" + oriX + "  / 新的" + newX + " /  X :" + FireRotateX + " / Y :" + FireRotateY);
                    if(oriX > newX)//原本位置>後座力位置   垂直晃動
                    {
                        GunAimR_x.GetComponent<MouseLook>().rotationX -= Random.Range(3f, 5f)* 武器欄位[WeaponType].Recoil * Time.deltaTime;  
                    }
                    Weapons[WeaponType].WeapAm--;
                    if (FireButtle==1)
                    {
                        Weapon.SetBool("Fire", true);
                        Weapon.SetTrigger("Fire 0");
                        FireButtle = 0;
                    }
                    MouseLook.Shaking();
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
                if (shooting)
                {
                    float NewFireRotateX = GunAimR_x.GetComponent<MouseLook>().rotationX;
                    //float ddd = (OriFireRotateX - NewFireRotateX) / 2;
                    //float distance = OriFireRotateX - ddd;
                    float distance = OriFireRotateX - 1f;
                    float speed = OriFireRotateX - NewFireRotateX;
                    if (speed < 2) speed = 2;
                    //print("一  "+ OriFireRotateX);
                    //print("現  " + NewFireRotateX);
                    //print("ddd  " + ddd);
                    //print("半  " + distance);
                    if (WeaponType== 0)
                    {
                        if (NewFireRotateX < distance)
                        {
                            FireRotateReTime+=Time.deltaTime;
                            if (FireRotateReTime >= 0.3f)
                            {
                                FireRotateReTime = 0;
                                shooting = false;
                                return;
                            }
                            GunAimR_x.GetComponent<MouseLook>().rotationX += speed * 2f * Time.deltaTime;  //垂直晃動回歸
                        }
                        else
                        {
                            GunAimR_x.GetComponent<MouseLook>().rotationX = distance;
                            shooting = false;
                        }
                        if (NewFireRotateX > OriFireRotateX)
                        {
                            GunAimR_x.GetComponent<MouseLook>().rotationX = NewFireRotateX;
                            shooting = false;
                        }
                    }
                    else
                    {
                        shooting = false;
                    }
                }

                //Vector3 currentVelocity = Vector3.zero;
                //Vector3 pos = transform.localEulerAngles;
                //pos.y = OriFireRotateY;
                //transform.localEulerAngles = Vector3.SmoothDamp(transform.position, pos, ref currentVelocity, 5f, 5f);

                Weapon.SetBool("Fire", false);
            }       
        }
        else //否則需要冷卻計時
        {
            coolDownTimer += Time.deltaTime;
            //Weapon.SetBool("Fire", false);
        }
        if (Input.GetKeyDown(KeyCode.U))  //測試用  武器傷害升級---------------///////////------------
        {
            Weapons[0].power = Weapons[0].power +10;
            Weapons[1].power = Weapons[1].power + 10;
            Weapons[2].power = Weapons[2].power + 10;
        }
        if (Input.GetKeyDown(KeyCode.I))  //測試用  武器傷害升級---------------///////////------------
        {
            Weapons[0].power = 2;
            Weapons[1].power = 1;
            Weapons[2].power = 1;
        }
        if (GetGrenade)  //取得手榴彈
        {
            for (int g = 0; g < Grenade.Length; g++)
            {
                if (Grenade[g] == null)
                {
                    Grenade[g] = GetGrenadeObj;
                    break;
                }
            }
            GetGrenade = false;
            GrenadeNub++;
            GrenadeUI.GetComponent<Text>().text = ""+ GrenadeNub;
        }
        if (UseGrenade)  //使用手榴彈
        {
            UseGrenade = false;
            Grenade[GrenadeNub].transform.position = GrenadeMuzzle[WeaponType].transform.position;
            Grenade[GrenadeNub].transform.rotation = GrenadeMuzzle[WeaponType].transform.rotation;
            Grenade[GrenadeNub].transform.parent = null;
            Grenade[GrenadeNub].SetActive(true);
            Grenade[GrenadeNub].GetComponent<ExplosionLift>().enabled = true;
            //Grenade[GrenadeNub].GetComponent<Grenade>().enabled = false;
            Grenade[GrenadeNub].layer = LayerMask.NameToLayer("Default");
            Grenade[GrenadeNub].GetComponent<Rigidbody>().isKinematic = false;
            Grenade[GrenadeNub].GetComponent<CapsuleCollider>().isTrigger = false;
            Grenade[GrenadeNub].transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
            Grenade[GrenadeNub].transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
            Grenade[GrenadeNub] = null;
        }
        if (Input.GetKeyDown(KeyCode.G) && Bomb && GrenadeNub !=0)  //投擲手榴彈
        {
            GrenadeNub--;            
            GrenadeUI.GetComponent<Text>().text = "" + GrenadeNub;
            Bomb = false;
            Weapon.SetTrigger("Bomb");
        }
        switch (JumpDown)  //降落-------------
        {
            case 1:  //進區域
                LayDown = true;
                Weapon.SetTrigger("LayDownT");
                Weapon.SetBool("LayDown", true);
                break;
            case 2:
                _Animator[WeaponType].SetActive(false);
                break;
            case 3:   //進入掉落 鎖控制   
                PlayerView.Stop = true;  //UI隱藏
                NoM_T += Time.deltaTime;
                if (NoM_T >= 1)
                {
                    NoM_T = 1;
                    playerMove.NoMove = true;
                    playerMove.NoM_V3 = transform.position;
                }
                break;
            case 4:  //到底部
                NoM_T = 0;
                _Animator[WeaponType].SetActive(true);
                Save_Across_Scene.Shooting.closeFireEffects(); //關閉攻擊特效
                LayDown = false;
                playerMove.NoMove = false;
                PlayerView.Stop = false;  //UI隱藏
                PlayerView.missionChange(4, 6);  //改變關卡
                break;
            case 5:
                //JumpDown = 0;
                break;
            case 6:
                _Animator[WeaponType].SetActive(true);
                Save_Across_Scene.Shooting.closeFireEffects(); //關閉攻擊特效
                LayDown = false;
                JumpDown = 0;
                break;
        }
        if (Input.GetKeyDown(KeyCode.T) && FirstWeapon[0] && (JumpDown==0 || JumpDown == 5))       //收槍
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
        if (LayDown)
        {
            Aim.enabled = true;
        }
        else
        {
            Aim.enabled = false;
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
        if (Weapons[WeaponType].T_WeapAm <= 0)  //總彈藥最小為0
        {
            Weapons[WeaponType].T_WeapAm = 0;
            if(Weapons[WeaponType].WeapAm <= 0 && FirstWeapon[0] && FirstAmm)
            {
                Am_zero_Warn.SetActive(true);  //總彈藥不足警告
                Am_zero_Warn.gameObject.transform.GetChild(0).GetComponent<Animator>().SetInteger("Type", 1);
            }
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
        if (MuFire_Light[WeaponType].activeSelf && WeaponType !=0)
        {
            ////print(MuFire_Light[WeaponType].name);
            //MuFire_Light[WeaponType].GetComponent<Light>().range -= 80 * Time.deltaTime;
            //float Range = MuFire_Light[WeaponType].GetComponent<Light>().range;
            //if (Range <= 0)
            //{
            //    MuFire_Light[WeaponType].SetActive(false);
            //    MuFire_Light[WeaponType].GetComponent<Light>().range = 30;
            //}
        }
        //修改武器材質
        if (WeaponType == 2) material[0].SetTexture("_EmissiveColorMap", texture2Ds[Weapons[2].WeapAm]);
    }
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R) && !LayDown && Weapons[WeaponType].T_WeapAm != 0)    //換彈藥
        {
            if (Reload == false)
            {
                Reload = true;
                if (WeaponType == 2)
                {
                    if (Weapons[2].WeapAm >= 5)
                    {
                        return;
                    }
                    Weapon.SetBool("Reloading", true);
                    ReReload = true;
                }
                FireButtle = 0;
                Weapon.SetTrigger("Reload");               
                ReloadWarn.SetActive(false);
            }
        }
        if (ReReload)  //再次裝彈
        {
            FireButtle = 0;
            Reload = true;
            if (WeaponType == 2) Weapon.SetBool("Reloading", true);
            ReloadWarn.SetActive(false);
            //ReReload = false;
        }
        else
        {
            if (WeaponType == 2) Weapon.SetBool("Reloading", false);
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
                    hitDamage = false;
                    PowerAdd = 武器欄位[WeaponType].Power;
                    if (PowerAdd <= 0)
                    {
                        PowerAdd = 武器欄位[WeaponType].Power;
                        if (PowerAdd <= 0) PowerAdd = 1;
                    }
                    //print(PowerAdd);
                    if (hit[n].collider.tag == "NPC")
                    {
                        hit[n].transform.SendMessage("TeamDamage", Weapons[WeaponType].power * PowerAdd);  //造成傷害
                        輸出傷害 = Weapons[WeaponType].power * PowerAdd;
                    }
                    //Debug.DrawLine(ray[n].origin, hit[n].point, Color.green, 1f, false);
                    if (hit[n].collider.gameObject.layer == LayerMask.NameToLayer("Default"))  //彈孔噴黑煙
                    {
                        HitType = 0;
                    }
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
                        AudioManager.Hit(0);  //擊中音效
                        //Debug.DrawLine(ray1[n].origin, hit1[n].point, Color.blue, 0.3f, false);
                    }
                    if (hit[n].collider.gameObject.layer == LayerMask.NameToLayer("Monster"))  //彈孔噴血
                    {
                        HitType = 1;
                        //AudioManager.Hit(2);  //擊中音效
                                              //Debug.DrawLine(ray.origin, hit.point, Color.red, 0.7f, false);

                        if (hit[n].collider.tag == "Enemy" || hit[n].collider.tag == "Target") 
                        {

                            hitDamage = true;
                            int MonsterType = 0;
                            bool isBoss=false;
                            FindUpParent(hit[n].transform);  //找有HP的父物件
                            Transform FindUpParent(Transform zi)  //找最大父物件
                            {
                                if (zi.GetComponent<MonsterLife>())
                                {
                                    MonsterType = zi.gameObject.GetComponent<MonsterLife>().MonsterType;
                                    return zi;
                                }
                                if (zi.GetComponent<Boss_Life>())
                                {
                                    isBoss = zi.gameObject.GetComponent<Boss_Life>().isBoss;
                                    MonsterType = zi.gameObject.GetComponent<Boss_Life>().MonsterType;
                                    if (MonsterType == 1)
                                    {
                                        zi.gameObject.GetComponent<Boss_Life>().Weakness(hit[n].collider.gameObject);
                                    }
                                    return zi;
                                }
                                if (zi.GetComponent<Crystal_Life>())
                                {
                                    MonsterType = 2;
                                    return zi;
                                }
                                else
                                {
                                    if (zi.parent == null) return zi;
                                    else return FindUpParent(zi.parent);
                                }
                            }
                            if (isBoss)
                            {
                                AudioManager.Hit(5);  //擊中音效
                                switch (MonsterType)
                                {
                                    case 0:
                                        HitType = 2;  //水晶
                                        break;
                                    case 1:
                                        break;
                                }
                            }
                            else
                            {
                                AudioManager.Hit(3);  //擊中音效
                                switch (MonsterType)
                                {
                                    case 0:
                                        HitType = 2;  //綠血
                                        break;
                                    case 1:
                                        HitType = 6;  //紫血
                                        break;
                                }
                            }
                            if (hit[n].collider.tag == "Target")
                            {
                                TargetWall = true;
                            }
                        }                                                
                        if (hit[n].collider.tag == "Carapace")  //甲殼
                        {
                            HitType = 4;
                            AudioManager.Hit(1);  //擊中音效
                            if (WeaponType == 1)  //電磁手槍傷害
                            {
                                hit[n].transform.SendMessage("Unit", true);  //攻擊者為玩家?
                                if (n == 0)
                                {
                                    hit[0].transform.SendMessage("Damage", Weapons[WeaponType].power *3.2f * PowerAdd);  //造成一半傷害
                                }
                                else
                                {
                                    hit[n].transform.SendMessage("Damage", Weapons[WeaponType].power *0.8f * PowerAdd);  //造成一半範圍傷害
                                }
                            }
                        }
                    }
                    if (hit[n].collider.tag == "Crystal")  //水晶
                    {
                        if(hit[n].collider.GetComponent<Crystal_Life>())  //如果水晶可破壞
                        {
                            hit[n].collider.SendMessage("Damage", Weapons[WeaponType].power * PowerAdd);  //造成傷害
                            輸出傷害 = Weapons[WeaponType].power * PowerAdd;
                        }
                        AudioManager.Hit(5);  //擊中水晶音效
                        HitType = 8;
                    }
                    if (hitDamage)  //擊中怪物
                    {
                        hitDamage = false;

                        hit[n].transform.SendMessage("Unit", true);  //攻擊者為玩家?
                        if (WeaponType == 1)  //電磁手槍傷害
                        {
                            if (n == 0)
                            {
                                hit[0].transform.SendMessage("Damage", Weapons[WeaponType].power * 7.2f * PowerAdd);  //造成中心傷害
                            }
                            else
                            {
                                hit[n].transform.SendMessage("Damage", Weapons[WeaponType].power * 1.6f * PowerAdd);  //造成範圍傷害
                            }
                        }
                        else
                        {
                            hit[n].transform.SendMessage("Damage", Weapons[WeaponType].power * PowerAdd);  //造成傷害
                            輸出傷害 = Weapons[WeaponType].power * PowerAdd;
                        }
                        //Debug.DrawLine(ray.origin, hit.point, Color.blue, 0.3f, false);
                    }
                }
               
                //在到物體上產生彈孔
                rot = Quaternion.FromToRotation(Vector3.up, hit[n].normal);
                pos = hit[n].point;
                if (WeaponType == 1)
                {            
                    rot = Quaternion.FromToRotation(Vector3.up, hit[0].normal);
                    pos = hit[n].point;
                    HitType = 5;
                    if (TargetWall)
                    {
                        TargetWall = false;
                        HitType = 7;  //靶場彈孔;
                    }
                    if (!Fire1st)
                    {
                        pool_Hit.ReUseHit(pos, rot, HitType);  //從彈孔池取出彈孔
                        Fire1st = true;
                    }
                }
                else
                {
                    if (TargetWall)
                    {
                        TargetWall = false;
                        HitType = 7;  //靶場彈孔;
                    }
                    pool_Hit.ReUseHit(pos, rot, HitType);  //從彈孔池取出彈孔
                }            
            }
            Muzzle_vfx[WeaponType].transform.position = muzzlePOS;
            Muzzle_vfx[WeaponType].transform.rotation = GunCamera.transform.rotation;
            Muzzle_vfx[WeaponType].SetActive(true);
            //MuFire_Light[WeaponType].SetActive(true);
            switch (WeaponType)
            {
                case 0:
                    GunshotsAudio(1);
                    break;
                case 1:
                    GunshotsAudio(4);
                    break;
                case 2:
                    GunshotsAudio(1);
                    break;
            }
            MuSmoke[WeaponType].gameObject.SetActive(true);
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
    public static void ReLoad_E(int Ammo, int T_Ammo)  //換彈結束
    {
        Weapons[WeaponType].WeapAm = Ammo;
        Weapons[WeaponType].T_WeapAm = T_Ammo;
        Reload = false;
        FireButtle = 1;
    }
    public static void ReReLoad(bool Re)  //再次裝填
    {
        ReReload = Re;
    }
    void GunshotsAudio(int Type)  //開槍音效
    {
        AudioManager.PlayGunshotsAudio(Type);
    }
    void GussetMachine()  //扣板機
    {
        AudioManager.PlayGunshotsAudio(0);
        ReloadWarn.SetActive(true);  //換彈警告
        ReloadWarn.gameObject.transform.GetChild(0).GetComponent<Animator>().SetInteger("Type", 1);
    }
    public static void PlayerRe()  //玩家重生
    {
        StartAll();
        ReloadWarn.SetActive(false);
        Am_zero_Warn.SetActive(false);
        Reload = false;
        FireButtle = 1;
    }
    public static void DpsUp()
    {
        //Weapons[WeaponType].power = 1 + Shop.DpsLv;
    }
    public static void Loaded()  //武器上膛
    {
        FireButtle = 1;
    }
    public static void PickUpWeapons(int _WeaponsType, int WeaponPos, GameObject Object)  //拾取武器 (武器類型 , 武器位置, 武器物件)
    {
        if (Weapon_of_Pos[WeaponPos] == -1)  //武器位置為空
        {
            Weapon_of_Pos[WeaponPos] = _WeaponsType; //武器欄位變新武器
            Equipment[_WeaponsType] = 1;  //取得該武器類型
        }
        else if (Weapon_of_Pos[WeaponPos] != -1) //武器位置已有武器
        {
            if(_WeaponsType == Weapon_of_Pos[WeaponPos])  //該武器是否已擁有
            {
                //print("同武器");
                return;
            }
            else
            {
                //print("放下"+Weapon_of_Pos[WeaponPos]);
                Equipment[Weapon_of_Pos[WeaponPos]] = 0; //放下該武器類型
                Weapon_of_Pos[WeaponPos] = _WeaponsType;  //武器欄位變新武器
                Equipment[_WeaponsType] = 1;
                //print("拿取" + _WeaponsType);
            }
        }
        WeaponsPosOb[WeaponPos] = Object;

        PickUpWeapon = _WeaponsType;
        SwitchWeapon = true;
    }
    public static void PickUpAmm(int T)  //第一次拿彈藥
    {
        switch (T)
        {
            case 0:
                Ammunition.showUI();
                if (!FirstAmm && !SkipTeach)  //第一次取得彈藥並且沒跳教學
                {
                    FirstAmm = true;
                    DialogueEditor.StartConversation(0, 3, 0, true, 0, true);  //開始對話
                }
                st_Weapon.SetTrigger("Reload");
                Reload = true;
                FireButtle = 0;
                ReloadWarn.SetActive(false);
                StartAll();
                break;
            case 1:
                if(!FirstAmm && !SkipTeach)
                {
                    FirstAmm = true;
                }
                Ammunition.showUI();
                FireButtle = 0;
                StartAll();
                break;
        }
    }
    public static void UseWork(UpgradeValue[] _武器欄位)
    {
        武器欄位 = _武器欄位;
    }
    public static void Bumb()  //投彈
    {
        UseGrenade = true;
    }
    public static void BumbEnd()  //投彈結束
    {
        Bomb = true;
    }
    public static void Get_Grenade(GameObject grenade)  //取得手榴彈
    {
        GetGrenadeObj = grenade;     
        GetGrenade = true;
    }
    public void closeFireEffects()  //關閉攻擊特效
    {
        for (int i = 0; i < Muzzle_vfx.Length; i++)  //受傷特效
        {
            Muzzle_vfx[i].gameObject.SetActive(false);
            MuSmoke[i].Stop();
            MuSmoke[WeaponType].gameObject.SetActive(false);
        }
    }
}
