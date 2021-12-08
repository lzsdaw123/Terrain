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
    public ParticleSystem MuSmoke;  //槍口煙霧
    public ParticleSystem MuFire;
    public GameObject[] muzzle;  //槍口類型
    public GameObject GunAimR_x;  //X軸瞄準晃動
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
    public static int PickUpWeapon;  //身上武器類型
    public static bool SwitchWeapon;
    public Animator Weapon;   //動畫控制器
    public GameObject[] _Animator;  //槍枝物件
    public Vector3 muzzlePOS;  //槍口座標

    public bool BFire;  //生成子彈bool
    public bool DontShooting;  //停止射擊bool
    public bool LayDown;  //收槍bool
    public RuntimeAnimatorController[] controllers;  //動畫控制陣列

    public static int ammunition, Total_ammunition;  //當前武器彈藥量
    public static int[] WeapAm = new int[] { 30, 6 };  //武器彈藥量
    public static int[] T_WeapAm = new int[] { 300, 30 }; //武器總彈藥量
    public static bool Reload;   //換彈藥bool
    bool AimIng;  //瞄準中bool
    float FieldOfView;  //玩家相機視野
    float gFieldOfView;  //武器相機視野

    public ObjectPool pool_Hit;  //物件池
    public int HitType;  //彈孔類型變數
    public GameObject Hit_vfx, Hit_vfx_S;  //彈孔類型
    public LayerMask layerMask;  //圖層
    Quaternion rot;  //彈孔生成角度
    Vector3 pos;  //彈孔生成位置
    public static float[] power = new float[] { 2,10}; //子彈威力
    [SerializeField] static GameObject ReloadWarn;
    [SerializeField] static GameObject Am_zero_Warn;

    void Awake()
    {
        for (int i = 0; i < Hit_vfx.transform.childCount; i++)
        {
            Hit_vfx_S = Hit_vfx.transform.GetChild(i).gameObject;
            Hit_vfx_S.SetActive(false);
        }
        WeaponType = 0;
        _Animator[0].SetActive(true);
        _Animator[1].SetActive(false);

        power = new float[] { 2, 10 };
        Reload = false;
        DontShooting = false;
        LayDown = false;
        WeapSwitch = false;
        FireButtle = 1;
        PickUpWeapon = 0;
        SwitchWeapon = false;
    }
    void Start()
    {
        coolDown = 0.8f;  //冷卻結束時間
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
        MuSmoke.Stop();

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
        if (SwitchWeapon && WeaponType != 1)
        {
            SwitchWeapon = false;
            NextWeaponType = PickUpWeapon;           
            WeapSwitching();
        }
        if ( AniTime >=2)  //切換武器
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && WeaponType!=0)  //主武器
            {
                NextWeaponType = 0;
                WeapSwitching();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && WeaponType != 1 && PickUpWeapon==1)  //副武器
            {
                NextWeaponType = 1;
                WeapSwitching();
            }
        }
        void WeapSwitching()
        {
            if (!WeapSwitch)
            {
                Weapon.SetTrigger("LayDownT");
                AniTime = STtime - 1.6f;
                WeapSwitch = true;
            }
        }
        if (AniTime <= 0)
        {
            WeapSwitch = false;
            _Animator[WeaponType].SetActive(false);
            switch (NextWeaponType)
            {
                case 0:
                    WeaponType = 0;
                    break;
                case 1:
                    WeaponType = 1;
                    break;
            }
            _Animator[WeaponType].SetActive(true);
            MuSmoke.Stop();
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
                if (Input.GetButton("Fire1") && DontShooting == false && LayDown == false && WeapAm[WeaponType] != 0)
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
                if(WeapAm[WeaponType] != 0)
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

                    WeapAm[WeaponType]--;
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
                switch (WeaponType)  //開火冷卻時間，與coolDown 0.8差越小越快
                {
                    case 0:
                        coolDownTimer = 0.7f;
                        break;
                    case 1:
                        coolDownTimer = 0.4f;   
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
        if (WeapAm[WeaponType] <= 0)
        {
            WeapAm[WeaponType] = 0;
        }
        if (T_WeapAm[WeaponType] <= 0)
        {
            T_WeapAm[WeaponType] = 0;
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
                GunCamera.GetComponent<Camera>().fieldOfView = gFieldOfView;
            }
            if (FieldOfView > 35f)
            {
                FieldOfView -= 140f * Time.smoothDeltaTime;
                if (FieldOfView <= 35f)
                {
                    FieldOfView = 35f;
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
                GunCamera.GetComponent<Camera>().fieldOfView = FieldOfView;
            }
        }
    } 
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R) && LayDown == false && T_WeapAm[WeaponType] != 0)    //換彈藥
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
            muzzlePOS = muzzle[WeaponType].GetComponent<Transform>().position;
            //建立子彈在鏡頭中心位置
            //GameObject obj = Instantiate(bullet, muzzlePOS, PlayCamera.transform.rotation);
            pool.ReUse(muzzlePOS, GunCamera.transform.rotation);
            //由攝影機射到是畫面正中央的射線
            Ray ray = GunCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit; //射線擊中資訊
            float distance = 400;
            if (Physics.Raycast(ray, out hit, distance, layerMask)) //擊中圖層
            {
                //print(hit.collider.name);
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))  //彈孔噴黑煙
                {
                    HitType = 0;
                    Debug.DrawLine(ray.origin, hit.point, Color.black, 0.7f, false);
                }
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                {
                    HitType = 0;
                    //繪出起點到射線擊中的綠色線段(起點座標,目標座標,顏色,持續時間,是否被靠近相機的物體遮住)      
                    //Debug.DrawLine(ray.origin, hit.point, Color.green, 0.7f, false);                        
                }
                if (hit.collider.tag == "Metal")  //金屬
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

                        HitType = 2;
                        hit.transform.SendMessage("Unit", true);  //攻擊者為玩家?
                        hit.transform.SendMessage("Damage", power[WeaponType]);  //造成傷害
                        //Debug.DrawLine(ray.origin, hit.point, Color.blue, 0.3f, false);
                    }
                    if (hit.collider.tag == "Carapace")  //甲殼
                    {
                        HitType = 4;
                        if (WeaponType == 1)
                        {
                            hit.transform.SendMessage("Damage", 5);  //造成傷害
                        }
                    }
                }
            }
            //在到物體上產生彈孔
            rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            pos = hit.point;
            if (WeaponType == 1)
            {
                HitType = 5;
            }
            pool_Hit.ReUseHit(pos, rot, HitType);  //從彈孔池取出彈孔
            Muzzle_vfx[WeaponType].transform.position = muzzlePOS;
            Muzzle_vfx[WeaponType].transform.rotation = GunCamera.transform.rotation;
            Muzzle_vfx[WeaponType].SetActive(true);
            GunshotsAudio();
            MuSmoke.Play();
            BFire = false;
        }
    }
    public static void ReLoad_E()  //換彈結束
    {
        WeapAm[WeaponType] = AnimEvents.ammunition;
        T_WeapAm[WeaponType] = AnimEvents.Total_ammunition;
        Reload = false;
        FireButtle = 1;
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
        WeapAm = new int[] { 30, 6 };
        T_WeapAm = new int[] { 300, 30 };
        ReloadWarn.SetActive(false);
        Am_zero_Warn.SetActive(false);
    }
    public static void DpsUp()
    {
        power[WeaponType] = 1 + Shop.DpsLv;
    }
    public static void Loaded()
    {
        FireButtle = 1;
    }
    public static void PickUpWeapons(int Nub)
    {
        PickUpWeapon = Nub;
        SwitchWeapon = true;
    }
}
