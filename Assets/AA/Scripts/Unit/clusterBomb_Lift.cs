using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clusterBomb_Lift : MonoBehaviour
{
    public GameObject Muzzle_vfx;
    public Animator ani;
    public GameObject[] Hit_vfx;  //彈孔類型
    public ParticleSystem[] Pro;
    public int ButtleType;  //集束炸彈類型
    public bool clusterBomb_T;  //是否為集束炸彈
    public bool Reverse;  //反轉飛行方向
    public Transform target;
    public Transform OriTarger;
    public int aTN;
    public Vector3 Atarget;  //攻擊目標座標
    public float AtargetY;  //攻擊目標Y軸加成
    public float[] speed;//飛行速度
    public float liftTime; //生命時間
    public float[] bornTime=new float[3]; //生成時間
    bool AFlyTrack = true;  //子彈飛行軌跡
    public Vector3 OriPos; //原本位置
    public Vector3 OriSize; //預計尺寸
    public Vector3 bornSize; //生成尺寸
    bool Get_ATarget;  //取得攻擊目標
    public Vector3 ABPath;
    Vector3 AAT;
    public bool facingRight = true; //是否面向右邊
    private Vector3 moveDir = Vector3.right;
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
    public float firstDistance;  //初始距離
    public float oriDistance;  //舊距離
    bool AttackPlay;  //攻擊目標是否為玩家
    public bool Attacking;  //攻擊中
    public bool StartAttack;  //開始攻擊
    public float AttackCTime;  //攻擊倒數
    public ObjectPool pool_Hit;  //物件池

    public void Init(bool FacingRight) //初始化子彈時順便給定子彈飛行方向
    {
        facingRight = FacingRight;
    }
    private void Awake()
    {
        //pool_Hit = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
    }
    void Start()
    {
        speed = new float[] {60,80,40 }; //飛行速度
        power =new float[] {1,3,5 };
        AttackLv = 0;
        liftTime = 10;
        bornTime = new float[] {10,1, 1.5f};  //生成速度{快, 慢, 中}
        Atarget = Vector3.zero;
        Attacking = false;
        StartAttack = false;
        AttackCTime = 0;

        if (clusterBomb_T)  //是否為集束炸彈
        {
            forwardFly = true;
            Get_ATarget = false;
            bornSize = Vector3.one;
        }
        else
        {
            forwardFly = false;
            Get_ATarget = true;
            Pro[0].gameObject.SetActive(false);
            Pro[1].gameObject.SetActive(false);
            Pro[2].gameObject.SetActive(false);
            OriSize = Pro[ButtleType + 3].transform.localScale * Random.Range(0.5f, 1f);
            switch (ButtleType)
            {
                case 0:  //尖形 結晶
                    Pro[0].gameObject.SetActive(true);
                    bornSize = Vector3.zero;
                    break;
                case 1:  //長方 傳送
                    Pro[1].gameObject.SetActive(true);
                    bornSize = new Vector3(0, 0, 0.4f);
                    break;
                case 2:  //多邊 爆炸
                    Pro[2].gameObject.SetActive(true);
                    bornSize = Vector3.zero;
                    break;
            }
        }
       

        //for (int i=0; i< Pro.Length; i++)
        //{
        //    var main = Pro[i].main;
        //    main.simulationSpeed = 8f;  //加快粒子開始播放時間
        //    if (i == 2)
        //    {
        //        main.simulationSpeed = 2f;  //加快粒子開始播放時間
        //    }           
        //}
        //射線初始位置,射線方向
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
                //hit.transform.SendMessage("Damage", power);
                if (hit.collider.tag == "MissionTarget")  //目標
                {
                    //hit.transform.SendMessage("Damage", power);
                }
            }
        }
        //在到物體上產生彈孔
        //Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
        Vector3 pos = hit.point;
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
        transform.localPosition = OriPos;
        transform.localScale = OriSize;
        AttackCTime = 0;
        Attacking = false;
        Start();
    }
    void Update()
    {
        if (Attacking) liftTime -= Time.deltaTime;
        if (!clusterBomb_T)  //是否為集束炸彈
        {
            if (liftTime <= 0)
            {
                liftTime = 0;
                GameObject.Find("ObjectPool").GetComponent<ObjectPool>().RecoveryBoss1Bullet(gameObject);  //回收子彈
                Boss01_AI.BulletNub++;
            }

            if (bornSize.z >= OriSize.z)
            {
                ani.SetBool("gen_End", true);
            }
            else
            {
                bornSize += new Vector3(0.1f, 0.1f, 0.1f) * bornTime[ButtleType] * Time.deltaTime;  //生成尺寸
            }
            Pro[ButtleType + 3].transform.localScale = bornSize;
            if (StartAttack)
            {
                AttackCTime += Time.deltaTime;
                if (AttackCTime >= 2)  //發射前等待
                {
                    StartAttack = false;
                    Attacking = true;
                    Boss01_AI.ReMuzzleGrid(cuMuGrid);
                    Muzzle = Boss01_AI.PS_muzzle[cuMuGrid];
                    Muzzle.gameObject.transform.GetChild(ButtleType).gameObject.SetActive(true);
                    Muzzle.gameObject.transform.GetChild(ButtleType).GetComponent<ParticleSystem>().Play();
                    Muzzle.transform.localRotation = transform.localRotation;
                }
            }
        }
        else
        {
            if (StartAttack)
            {
                if (AttackCTime >= 1)
                {
                    AttackCTime = -1;
                    Attacking = true;
                    ani.enabled = false;
                    OriPos = transform.localPosition;
                    bornSize = OriSize = transform.localScale;
                }
                else if (AttackCTime >= 0)
                {
                    AttackCTime += Time.deltaTime;
                }
                bornSize -= new Vector3(0.1f, 0.1f, 0.1f) * 0.1f * Time.deltaTime;  //尺寸縮小
                transform.localScale = bornSize;
                if (bornSize.z <= 0)
                {
                    liftTime = 0;
                }
            }
            if (liftTime <= 0)  //生命時間到
            {
                liftTime = 0;
                gameObject.SetActive(false);
                transform.localPosition = OriPos;
                transform.localScale = OriSize;
            }
        }   
    }
    void FixedUpdate()
    {
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

        if (Attacking)
        {
            if (clusterBomb_T)
            {
                if (Reverse)
                {
                    transform.Translate(Vector3.forward * -speed[ButtleType] * Time.deltaTime); //往前移動
                }
                else
                {
                    transform.Translate(Vector3.forward * speed[ButtleType] * Time.deltaTime); //往前移動
                }
            }
            else
            {
                Get_ATarget = false;
                if (Atarget != Vector3.zero)
                {
                    firstDistance = Vector3.Distance(transform.position, Atarget);  //初始距離
                    oriDistance = firstDistance;  //舊距離
                    if (forwardFly)
                    {
                        transform.Translate(Vector3.forward * speed[ButtleType] * Time.deltaTime); //往前移動
                    }
                    else
                    {
                        if (firstDistance != 0)
                        {

                            transform.position = Vector3.MoveTowards(transform.position, Atarget, speed[ButtleType] * Time.deltaTime);
                            firstDistance = Vector3.Distance(transform.position, Atarget);
                        }
                        else
                        {
                            //liftTime = 0;
                        }
                        if (firstDistance == oriDistance)
                        {
                            forwardFly = true;
                        }
                    }
                }
                else
                {
                    liftTime = 0;
                }
            }
        }
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
                    collision.gameObject.SendMessage("Damage", power[ButtleType]); //傷害
                    break; //結束迴圈
                }
            }
        }
        else if (InLayerMask(collision.gameObject.layer, Ground[1]))
        {
            //liftTime = 0;
        }
    }

    private void OnTriggerEnter(Collider collision)  //使用中
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
            liftTime = 0;
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
                    if (collision.GetComponent<HeroLife>() || collision.GetComponent<NPC_Life>() || collision.GetComponent<building_Life>() || collision.GetComponent<MissionTarget_Life>())
                    {
                        collision.gameObject.SendMessage("Damage", power[ButtleType]); //傷害
                        if (collision.GetComponent<HeroLife>())
                        {
                            collision.gameObject.SendMessage("DamageEffects", ButtleType); //傷害
                            collision.gameObject.SendMessage("hit_Direction", transform); //命中方位
                        }
                        break; //結束迴圈
                    }
                }
            }
        } 
        else if (InLayerMask(collision.gameObject.layer, Ground[1]))
        {
            //在到物體上產生彈孔
            if (!clusterBomb_T)
            {
                liftTime = 0;
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
                Vector3 pos = hit.point;
                pool_Hit.ReUseBoss1Hit(pos, rot, ButtleType);  //從彈孔池取出彈孔
            }
            else
            {
                if (liftTime <= 9.6f && Attacking)
                {
                    liftTime = 0;
                }
            }
        }
    }
}
