using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionLift : MonoBehaviour
{
    public GameObject Muzzle_vfx;
    public Animator ani;
    public AudioSource AudioSource;
    public AudioClip[] AudioClip;
    public AudioManager AudioManager;
    public GameObject[] Hit_vfx;  //彈孔類型
    public ParticleSystem[] Pro;
    public int BulletType;  //子彈類型
    public Transform target;
    public Transform OriTarger;
    public int aTN;
    public Vector3 Atarget;  //攻擊目標座標
    public float AtargetY;  //攻擊目標Y軸加成
    public float[] speed;//飛行速度
    public float liftTime; //生命時間
    public float[] bornTime=new float[3]; //生成時間
    public bool Damage;
    bool AFlyTrack = true;  //子彈飛行軌跡
    public Vector3 OriSize; //預計尺寸
    public Vector3 bornSize; //生成尺寸
    bool Get_ATarget;  //取得攻擊目標
    public Vector3 ABPath;
    Vector3 AAT;
    public bool facingRight = true; //是否面向右邊
    private Vector3 moveDir = Vector3.right;
    //public LayerMask collidLayers = -1; //射線判斷的圖層，-1表示全部圖層
    public float[] power; //子彈威力
    int AttackLv; //傷害等級
    int Level; //難度等級
    public GameObject Muzzle;
    public int cuMuGrid;  //占用槍口格子
    [TagSelector] public string[] damageTags; //要傷害的Tag
    [TagSelector] public string[] ignoreTags; //要忽略的Tag

    public LayerMask[] Ground;  //射線偵測圖層
    public LayerMask layerMask;
    [SerializeField] bool forwardFly;  //是否向前飛
    [SerializeField] bool ReflectionT;  //是否向前飛
    [SerializeField] bool forwardTurn;  //是否向前轉
    public float firstDistance;  //初始距離
    public float oriDistance;  //舊距離
    bool AttackPlay;  //攻擊目標是否為玩家
    public bool Attacking;  //攻擊中
    public bool StartAttack;  //開始攻擊
    public float AttackCTime;  //攻擊倒數
    public ObjectPool pool_Hit;  //物件池
    public GameObject[] Object;
    float Rz;  //旋轉
    public int collisionNub;  //碰撞次數
    public float ReflectTime;
    public  Vector3 Reflection;
    public PhysicMaterial physicMaterial;
    public GameObject[] actors=new GameObject[10];
    public Material[] Material;
    public bool accelerate;  //聲音加速
    public ParticleSystem ParticleSystem;

    public void Init(bool FacingRight) //初始化子彈時順便給定子彈飛行方向
    {
        facingRight = FacingRight;
        //Destroy(gameObject, liftTime); //設置生命時間到自動刪除
    }
    private void Awake()
    {
    }
    void Start()
    {
        pool_Hit = Save_Across_Scene.pool_Hit;
        speed = new float[] {26,40,40 }; //飛行速度
        power =new float[] {30,6.5f,10 };
        AttackLv = 0;
        bornTime = new float[] {150,170, 150f};  //生成速度{快, 慢, 中}
        Atarget = Vector3.zero;
        Attacking = false;
        AudioManager = Save_Across_Scene.AudioManager;  //聲音控制器
        //Pro[0].gameObject.SetActive(false);
        //Pro[1].gameObject.SetActive(false);
        //Pro[2].gameObject.SetActive(false);
        switch (BulletType)
        {
            case 0:  //手榴彈
                Damage = false;
                forwardFly = true;
                StartAttack = true;
                liftTime = 1;
                collisionNub = 0;
                ReflectTime = -1;
                physicMaterial.bounciness = 0.7f;  //反彈強度
                Object[1].SetActive(false);
                Object[0].GetComponent<MeshRenderer>().material = Material[1];
                ExpAudio(1);
                accelerate = false;
                ParticleSystem.Stop();
                var psmain = ParticleSystem.main;
                psmain.duration = 0.5f;
                psmain.startLifetime = 0.25f;
                ParticleSystem.Play();
                //Pro[0].gameObject.SetActive(true);
                //bornSize = Vector3.zero;
                break;
            case 1:  //雞爆炸
                Damage = true;
                forwardFly = false;
                AttackCTime = 2;
                StartAttack = true;
                liftTime = 1;
                Get_ATarget = false;
                OriSize = new Vector3(2.4f, 2.4f, 2.4f);
                bornSize = Vector3.zero;
                transform.localScale = bornSize;
                break;
            case 2:  //多邊 爆炸
                Damage = true;
                forwardFly = false;
                AttackCTime = 2;
                StartAttack = true;
                liftTime = 1;
                Get_ATarget = false;
                OriSize = new Vector3(2.4f, 2.4f, 2.4f);
                bornSize = Vector3.one;
                transform.localScale = bornSize;
                break;
        }
        for (int i = 0; i < actors.Length; i++)
        {
            actors[i] = null;
        }
        ////射線初始位置,射線方向
        //Ray ray = new Ray(transform.position, transform.forward); 
        //RaycastHit hit; //射線擊中資訊
        ////偵測射線判斷，由 自身座標 的 前方 射出，以rayLength為長度，並且只偵測Ground圖層(記得改圖層
        ////Raycast(射線初始位置, 射線方向, 儲存所碰到物件, 射線長度(沒設置。無限長), 設定忽略物件)
        //if (Physics.Raycast(ray, out hit, layerMask)) //擊中牆壁
        //{
        //    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))  //無視怪物
        //    {
        //        return;
        //    }
        //    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Actor"))
        //    {
        //        //hit.transform.SendMessage("Damage", power);
        //        if (hit.collider.tag == "MissionTarget")  //目標
        //        {
        //            //hit.transform.SendMessage("Damage", power);
        //        }
        //    }
        //}
        //在到物體上產生彈孔
        //Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
        //Vector3 pos = hit.point;
    }
    void DifficultyUp()  //難度設定
    {
        AttackLv = Level_1.MonsterLevel;
        Level = Settings.Level;
        //if (AttackLv > 0)
        //{          
        //    if (Level <= 1)  //難度普通以下
        //    {
        //        speed = 60 + (AttackLv * 6);
        //        if (speed >= 60 + (Level * 30))
        //        {
        //            speed = 60 + (Level * 30);
        //            power = 1;
        //        }
        //    }
        //    else  //難度困難
        //    {
        //        speed = 60 + (AttackLv * 12);
        //        if (speed >= 60 + (Level * 30))
        //        {
        //            speed = 120;
        //            power = 2;
        //        }
        //    }
        //}
        //print(speed + ": 速度+傷害 :"+ power);  //最終速度 60 / 90 / 120  最終傷害 1 / 1 / 2
    }
    void OnDisable()
    {
        //DifficultyUp();
        Start();
        if (BulletType == 0)
        {
            Object[0].GetComponent<MeshRenderer>().material = Material[0];
        }
    }
    void Update()
    {
        if (Attacking) liftTime -= Time.deltaTime;
        if (liftTime <= 0)
        {
            liftTime = 0;
            Damage = false;
            bornSize = Vector3.zero;
            //gameObject.SetActive(false);
            //GameObject.Find("ObjectPool").GetComponent<ObjectPool>().RecoveryBoss1Bullet(gameObject);  //回收子彈
            //Boss01_AI.BulletNub++;
            gameObject.SetActive(false);
        }
        switch (BulletType)
        {
            case 0:  //手雷
                if (StartAttack)
                {
                    //改發光
                    //Color color = Color.red;
                    //Material[0].SetFloat("_EmissiveExposureWeight", 0);
                    //Material[0].SetColor("_EmissiveColor", color* 512);
                    Object[2].SetActive(true);
                    AttackCTime += Time.deltaTime;
                    if (AttackCTime >= 1.3f && !accelerate)
                    {
                        accelerate = true;
                        ExpAudio(2);
                        ParticleSystem.Stop();
                        var psmain = ParticleSystem.main;
                        psmain.duration = 0.25f;
                        psmain.startLifetime = 0.125f;
                        ParticleSystem.Play();
                    }
                    if (AttackCTime >= 2.6f)  //爆炸前等待
                    {
                        StartAttack = false;
                        Attacking = true;
                        if (AudioSource !=null)
                        {
                            ExpAudio(0); 
                        }
                    }
                    if (Attacking)
                    {
                        Damage = true;
                        GetComponent<Rigidbody>().isKinematic = true;
                        Object[0].SetActive(false);
                        Object[1].SetActive(true);
                        Object[2].SetActive(false);
                        GetComponent<CapsuleCollider>().isTrigger = true;
                        GetComponent<CapsuleCollider>().radius += bornTime[0] * Time.deltaTime;
                        if(GetComponent<CapsuleCollider>().radius >= 12f)
                        {
                            GetComponent<CapsuleCollider>().radius = 12f;
                        }
                    }
                }
                break;
            case 1:  //雞 爆炸
            case 2:  //多邊 爆炸
                if (bornSize.z >= OriSize.z)
                {
                    bornSize = OriSize;
                    liftTime = 0;
                }
                else
                {
                    if (Damage) bornSize += new Vector3(0.1f, 0.1f, 0.1f) * bornTime[BulletType] * Time.deltaTime;  //生成尺寸
                }
                transform.localScale = bornSize;
                if (StartAttack)
                {
                    AttackCTime += Time.deltaTime;
                    if (AttackCTime >= 2)  //發射前等待
                    {
                        StartAttack = false;
                        Attacking = true;
                        if (AudioSource != null) ExpAudio(0);
                        //Boss01_AI.ReMuzzleGrid(cuMuGrid);
                        //Muzzle = Boss01_AI.PS_muzzle[cuMuGrid];
                        //Muzzle.gameObject.transform.GetChild(ButtleType).gameObject.SetActive(true);
                        //Muzzle.gameObject.transform.GetChild(ButtleType).GetComponent<ParticleSystem>().Play();
                        //Muzzle.transform.localRotation = transform.localRotation;
                    }
                }
                break;
        }               
    }
    void FixedUpdate()
    {
        //target = MonsterAI02.attackTarget;
        switch (BulletType)
        {
            case 0:
                if (forwardFly)
                {
                    transform.Translate(Vector3.forward * speed[BulletType] * Time.deltaTime); //往前移動
                    Rz -= 1080* Time.deltaTime;
                    Object[0].transform.localRotation = Quaternion.Euler(-89.98f, 0, Rz) ;  //旋轉
                    transform.Rotate(new Vector3(0, 0, -3f), Space.Self);  //自體旋轉
                }
                break;
            case 2:
                if (Get_ATarget)
                {
                    AAT = Boss01_AI.AAT;
                    AttackPlay = Boss01_AI.AttackPlay;

                    if (AttackPlay)
                    {
                        AtargetY = AAT.y + 1.2f;
                        Atarget = new Vector3(AAT.x, AtargetY, AAT.z);
                    }
                    else
                    {
                        Atarget = AAT;
                    }
                    if (StartAttack)  //開始攻擊
                    {
                        Vector3 targetDir = Atarget - transform.position;
                        Quaternion rotate = Quaternion.LookRotation(targetDir);
                        transform.localRotation = Quaternion.Slerp(transform.localRotation, rotate, 20 * Time.deltaTime);
                    }
                }
                break;
        }
        

        //if (Attacking)
        //{
        //    Get_ATarget = false;
        //    if (Atarget != Vector3.zero)
        //    {
        //        firstDistance = Vector3.Distance(transform.position, Atarget);  //初始距離
        //        oriDistance = firstDistance;  //舊距離
        //        if (forwardFly)
        //        {
        //            transform.Translate(Vector3.forward * speed[ButtleType] * Time.deltaTime); //往前移動
        //        }
        //        else
        //        {
        //            if (firstDistance != 0)
        //            {

        //                transform.position = Vector3.MoveTowards(transform.position, Atarget, speed[ButtleType] * Time.deltaTime);
        //                firstDistance = Vector3.Distance(transform.position, Atarget);
        //            }
        //            else
        //            {
        //                //liftTime = 0;
        //            }
        //            if (firstDistance == oriDistance)
        //            {
        //                forwardFly = true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        liftTime = 0;
        //    }
        //}
    }
    bool InLayerMask(int layer, LayerMask layerMask) //判斷物件圖層是否在LayerMask內
    {
        return layerMask == (layerMask | (1 << layer));
    }

    private void OnCollisionEnter(Collision collision)
    {
        //若碰撞體在作用圖層內才進行運算
        if (InLayerMask(collision.gameObject.layer, Ground[0]))
        {
            //liftTime = 0;
            for (int i = 0; i < ignoreTags.Length; i++)
            {
                if (collision.gameObject.tag == ignoreTags[i])
                {
                    return; //若對象在忽略Tag，則直接返回不做任何處理
                }
            }

            for (int i = 0; i < damageTags.Length; i++)
            {
                if (collision.gameObject.tag == damageTags[i])
                {
                    if (BulletType==0)
                    {
                        if (Damage)
                        {
                            FindUpParent(collision.transform);  //找有HP的父物件
                            Transform FindUpParent(Transform zi)  //找最大父物件
                            {
                                if (zi.GetComponent<MonsterLife>())
                                {
                                    zi.gameObject.SendMessage("Damage", power[BulletType]); //傷害
                                    print("0 " + zi.gameObject.name);
                                    return zi;
                                }
                                if (zi.GetComponent<Boss_Life>())
                                {
                                    zi.gameObject.SendMessage("Damage", power[BulletType]); //傷害
                                    print("0 " + zi.gameObject.name); 
                                    return zi;
                                }
                                else
                                {
                                    if (zi.parent == null) return zi;
                                    else return FindUpParent(zi.parent);
                                }
                            }
                        }
                    }
                    break; //結束迴圈
                }
            }
        }
        else if (InLayerMask(collision.gameObject.layer, Ground[1]))
        {
            if (BulletType == 0)
            {
                forwardFly = false;
                switch (collisionNub)
                {
                    case 0:
                        physicMaterial.bounciness = 0.5f;
                        collisionNub = 1;
                        break;
                    case 1:
                        physicMaterial.bounciness = 0.3f;
                        collisionNub = 2;
                        break;
                }
                //collisionNub++;
                //print("collisionNub" + collisionNub);
                if (!ReflectionT)
                {
                    ReflectionT = true;
                    //ReflectTime = 0;
                    //ReflectNub++;
                    //Reflection = Vector3.Reflect(transform.position, transform.right) *0.0005f;
                    //Reflection = Vector3.Reflect(transform.position, Vector3.right) *0.0005f;
                    //print("碰撞---------反射");
                }
                else
                {
                    //if (collisionNub >= 3)
                    //{
                    //    forwardTurn = false;
                    //    print("爆炸" + collisionNub);
                    //}
                    //if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    //{
                    //    forwardTurn = true;
                    //}
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collision)  //觸發
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit; //射線擊中資訊
        //偵測射線判斷，由 自身座標 的 前方 射出，以rayLength為長度，並且只偵測Ground圖層(記得改圖層
        //Raycast(射線初始位置, 射線方向, 儲存所碰到物件, 射線長度(沒設置。無限長), 設定忽略物件)
        if (Physics.Raycast(ray, out hit, layerMask)) //擊中牆壁
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))  //無視怪物
            {
                return;
            }
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Actor"))
            {
                if (hit.collider.tag == "MissionTarget")  //目標
                {
                }
            }
        }

        //若碰撞體在作用圖層內才進行運算
        if (InLayerMask(collision.gameObject.layer, Ground[0]))
        {
            //liftTime = 0;
            for (int i = 0; i < ignoreTags.Length; i++)
            {
                if (collision.gameObject.tag == ignoreTags[i])
                {
                    return; //若對象在忽略Tag，則直接返回不做任何處理
                }
            }

            for (int i = 0; i < damageTags.Length; i++)
            {
                if (collision.gameObject.tag == damageTags[i])
                {
                    switch (BulletType)
                    {
                        case 0:
                            if (AttackCTime < 3)
                            {
                                AttackCTime = 3;
                            }
                            if (Damage)
                            {
                                FindUpParent(collision.transform);  //找有HP的父物件
                                Transform FindUpParent(Transform zi)  //找最大父物件
                                {
                                    if (zi.GetComponent<MonsterLife>() || zi.GetComponent<Boss_Life>())
                                    {
                                        for (int i = 0; i < actors.Length; i++)
                                        {
                                            if (actors[i] != zi.gameObject)
                                            {
                                                zi.gameObject.SendMessage("Damage", power[BulletType]); //傷害
                                                //print("傷害 " + zi.gameObject.name);
                                                actors[i] = zi.gameObject;
                                                break;
                                            }
                                        }
                                        return zi;
                                    }
                                    else
                                    {
                                        if (zi.parent == null) return zi;
                                        else return FindUpParent(zi.parent);
                                    }                                   
                                }
                            }
                            break;
                        case 1:
                        case 2:
                            if (Damage)
                            {
                                FindUpParent(collision.transform);  //找有HP的父物件
                                Transform FindUpParent(Transform zi)  //找最大父物件
                                {
                                    if (zi.GetComponent<HeroLife>() || zi.GetComponent<NPC_Life>() || zi.GetComponent<building_Life>() || zi.GetComponent<MissionTarget_Life>())
                                    {
                                        for (int i = 0; i < actors.Length; i++)
                                        {
                                            if (actors[i] != zi.gameObject)
                                            {
                                                //print(actors[i] + "   //  "+ zi.gameObject);
                                                collision.gameObject.SendMessage("Damage", power[BulletType]); //傷害
                                                if (collision.GetComponent<HeroLife>())
                                                {
                                                    collision.gameObject.SendMessage("DamageEffects", BulletType); //傷害特效
                                                    collision.gameObject.SendMessage("hit_Direction", transform); //命中方位
                                                }
                                                actors[i] = zi.gameObject;
                                                break;
                                            }
                                        }
                                        return zi;
                                    }
                                    else
                                    {
                                        if (zi.parent == null) return zi;
                                        else return FindUpParent(zi.parent);
                                    }
                                }
                            }
                            break; //結束迴圈
                    }                  
                }
            }
        } 
        else if (InLayerMask(collision.gameObject.layer, Ground[1]))
        {
            if (BulletType == 0)
            {
                forwardFly = false;
                switch (collisionNub)
                {
                    case 0:
                        physicMaterial.bounciness = 0.5f;
                        collisionNub = 1;
                        break;
                    case 1:
                        physicMaterial.bounciness = 0.3f;
                        collisionNub = 2;
                        break;
                }

                ////collisionNub++;
                //print("collisionNub" + collisionNub);
                if (!ReflectionT)
                {
                    ReflectionT = true;
                    
                    //ReflectTime = 0;
                    //ReflectNub++;
                    //Reflection = Vector3.Reflect(transform.position, Vector3.right);
                    //Reflection = Vector3.Reflect(transform.position, Vector3.right) * 0.0008f;
                    //print("觸發反射");
                }
                else
                {
                    //if (collisionNub >= 3)
                    //{
                    //    forwardTurn = false;
                    //    print("爆炸" + collisionNub);
                    //}
                    //if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    //{
                    //    forwardTurn = true;
                    //}
                }               
            }
            //liftTime = 0;
            ////在到物體上產生彈孔
            //Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            //Vector3 pos = hit.point;
            //pool_Hit.ReUseBoss1Hit(pos, rot, ButtleType);  //從彈孔池取出彈孔
        }
    }
    public void ExpAudio(int Nub)  //怪物音效
    {
        switch (Nub)
        {
            case 0:  //爆炸
                AudioSource.clip = AudioClip[Nub];
                AudioSource.loop = false;
                break;
            case 1:  //倒數聲
                AudioSource.clip = AudioClip[Nub];
                AudioSource.loop = true;
                break;
            case 2:  //加速倒數聲
                AudioSource.clip = AudioClip[Nub];
                AudioSource.loop = true;
                break;
        }
        AudioSource.volume = AudioManager.Slider[2].value;
        AudioSource.mute = AudioManager.muteState[2];
        AudioSource.Play();
    }
}
