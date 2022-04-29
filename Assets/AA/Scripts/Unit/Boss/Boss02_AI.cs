using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;


public class Boss02_AI : MonoBehaviour
{
    public NavMeshAgent agent;  //尋徑代理人
    private SpawnRayReg reg;    //生怪註冊器
    private SpawnRay spawnRay;  //生怪后蟲
    private Vector3 target; // 尋徑目標點
    public GameObject[] MissionTarget;  //任務目標點
    [SerializeField] GameObject 目前進攻目標;
    [SerializeField] GameObject 目前攻擊目標;
    public GameObject Player;  //玩家
    public GameObject Eye;  //眼睛
    public GameObject[] MG_Turret;  //機槍塔
    public GameObject[] defenseOb;
    //public Defense Defense;
    public int A_defense;
    GameObject tagObject;
    public ObjectPool pool;
    [SerializeField] private AnimEvents AnimEvents;
    public Animator Total_ani; //動畫控制器
    public Animator ani; //動畫控制器
    public Animator Scenes_ani; //場景動畫控制器
    public float arriveDistance = 4f; // 到達目的地的距離
    private bool moving = false;  //是否要移動角色
    private float speed = 0; //animator裡面用的speed數值
    public float agentSpeed = 9f;  //尋徑速度

    // run 的%不用指定,因為除了 idle 和 walk,剩下就是 run
    public float idlePercent = 10;
    public float walkPercent = 50;


    // 若一段時間沒到達目的或是 idle 一段時間,需強制更換行動
    public float changeActionMin = 5f; // 行為的執行時間範圍
    public float changeActionMax = 8f; // 行為的執行時間範圍
    [SerializeField] private float actionTimer = 0; // 行為計時器
    private float nextActionTime = 0; // 到下個行為的時間

    public float heightOfEye = 1.7f; // 眼睛高度
    public float heightOfTarget = 1f; // 敵物要掃描的高度
    public float lookDistance = 10f; // 看見玩家的距離
    [Range(0, 180)] public float lookAngle = 80; // 可見的夾角,左右各 80 度,所以可視範圍為 160 度
    [Range(0, 180)] public float AttackAngle = 80; // 可見的夾角,左右各 80 度,所以可視範圍為 160 度
    [Range(0, 180)] public float ArangeAngle = 80; // 可見的夾角,左右各 80 度,所以可視範圍為 160 度
    public LayerMask actorLayer = 0; // 角色所在的圖層
    public LayerMask layerMask;  //圖層
    [TagSelector] public string[] playerTags = { "Player" };
    public static Transform attackTarget; // 搜尋到最近的攻擊目標
    public static Transform[] oriTarget = new Transform[3]; // 任務的攻擊目標
    public static Vector3 AAT; // 搜尋到最近的攻擊目標
    public static int aTN;
    private float targetDistance = 2000; // 與最近攻擊目標的距離
    [SerializeField] private float attackDistance; // 攻擊角度距離
    [SerializeField] private float ArangeDistance; // 攻擊範圍距離
    [SerializeField] bool AttackAngleT = false;
    [SerializeField] private bool attacking;
    private int bulletAttack;
    [SerializeField] int SF_bulletAttack;
    public static bool AttackPlay;
    bool TrPlayer;
    [SerializeField] private bool Fire;
    public static int BulletType;  //子彈類型
    public bool AttackStatus;  //攻擊狀態
    public AttackLevel attackLv1 = new AttackLevel(false, 2f, 3f, 80f, 1f); //第一段攻擊力 (威力,距離,角度,高度)
    public int SaveBT;
    public GameObject bullet;
    public int Level;  //攻擊等級
    public static int BulletNub;  //子彈數量
    [SerializeField] int SF_BulletNub;  //子彈數量
    [SerializeField] private GameObject[] muzzle;  //槍口
    public GameObject Muzzle_vfx;  //槍口火光  
    public ParticleSystem MuFire;  //槍口火光特效
    public Material[] MuzzleMaterial;  //槍口材質球
    public static GameObject[] PS_muzzle;  //槍口
    public static int[] muzzleGrid;  //槍口格子
    [SerializeField] int[] SF_muzzleGrid;  //槍口格子
    public static int cuMuGrid;  //當前槍口格子
    int MaxMuGrid;  //最大槍口格子數
    [SerializeField] private Vector3 muzzlePOS;  //槍口座標
    public GameObject[] RigTarget;  //槍口瞄準目標
    public static bool StartAttack;  //進入攻擊狀態
    public float StartTime; ///進入攻擊前的等待時間
    public float LockTime;  //鎖定時間
    public float overheatTime;  //過熱冷卻時間
    public bool overheatLock;  //過熱鎖定
    public bool Reload;  
    public int AttackMode;  //攻擊模式
    public int AttackRange;  //攻擊範圍
    public GameObject[] MA_Rig;  //Rig連結
    public float MA_weight;  //Rig連結權重

    private AttackUtility attackUtility = new AttackUtility();
    public float coolDown;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 eyeHi = new Vector3(0, heightOfEye, 0); // 眼睛高度
        Gizmos.color = Color.cyan; // 設為青色
        Quaternion arcRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -lookAngle, 0));// 計算弧線的起始角度
        GizmosExtension.DrawSector(transform.position + eyeHi, lookDistance, lookAngle * 2, arcRotation);// 畫扇形 

         //Vector3 weaponHi; // 武器高度
        Gizmos.color = Color.red; // 設為紅色
        Quaternion AttackRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -AttackAngle, 0));// 計算弧線的起始角度
        GizmosExtension.DrawSector(transform.position + eyeHi, attackDistance, AttackAngle * 2, AttackRotation);// 畫扇形 

        Gizmos.color = Color.blue; // 設為藍色
        Quaternion ArangeRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -ArangeAngle, 0));// 計算弧線的起始角度
        GizmosExtension.DrawSector(transform.position + eyeHi, ArangeDistance, ArangeAngle * 2, ArangeRotation);// 畫扇形 

    }
#endif

    void Start()
    {
        pool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        agent = GetComponent<NavMeshAgent>();
        //ani = GetComponent<Animator>();       
        agent.enabled = true;
        attacking = false;
        bulletAttack = 0;
        Fire = false;
        AttackStatus = false;
        coolDown = 1;
        BulletNub = 30;  //子彈數
        MaxMuGrid = muzzle.Length;
        //muzzleGrid = new int[MaxMuGrid];
        StartAttack = false;
        Muzzle_vfx.SetActive(false);
        MuzzleMaterial[0].SetFloat("_EmissiveExposureWeight", 1);
        Reload = false;
        StartTime = -1;
        Level = 1;

        //GameObject Mo1B = Instantiate(MBullet, MBulletPool.transform) as GameObject;   //無法生成


        //reg = GetComponent<SpawnRayReg>();
        //spawnRay = reg.mother;  //取得怪物的母體

        //取得一個尋徑目標點
        //target = GetNavTarget();  //取得一個目標
        //agent.destination = target;  //把目標設到尋徑裡面
        target = transform.position; //先令目標點等於角色所在位置
        //agent.destination = target; //設置尋徑目標點
        agent.speed = 0; //要用動畫去移動,因此尋徑的速度要設為 0

        //if (ani == null)
        //{
        //    ani = GetComponent<Animator>(); //自動取得動畫控制器
        //}
    }

    public void AttackLv1()
    {
        attackUtility.AttackTargets(attackLv1, transform, playerTags, actorLayer);
    }


    Vector3 GetNavTarget()
    {
        //float nx = Random.Range(-10f, 10f);
        //float nz = Random.Range(-10f, 10f);
        //return new Vector3(nx, 0, nz);
        RaycastHit hit = new RaycastHit();
        if (spawnRay.TryGetRandomLivePoint(out hit)) // 試著取得一個目標
        {
            return hit.point;
        }
        else // 若沒取到可走的目標點
        {
            return transform.position; // 傳回當前位置
        }
    }

    /// <summary>
    /// 取得尋徑路徑的移動方向
    /// </summary>
    /// <param name="forward">true:向前進移動 false:向後退移動</param>
    /// <param name="nav">尋徑代理人</param>
    /// <returns></returns>
    Quaternion GetNavRotation(bool forward, NavMeshAgent nav)
    {
        if (nav.hasPath) // 若有路徑
        {
            Vector3 v1 = new Vector3(nav.path.corners[0].x, 0,
            nav.path.corners[0].z);//當前的點
            Vector3 v2 = new Vector3(nav.path.corners[1].x, 0,
            nav.path.corners[1].z);//下一個點

            // 計算移動向量的旋轉方向
            if (forward)
                return Quaternion.LookRotation(v2 - v1);
            else
                return Quaternion.LookRotation(v1 - v2);
        }
        else
        {
            return transform.rotation;
        }
    }

    Vector3 GetNavDirection(bool forward, NavMeshAgent nav)
    {
        if (nav.hasPath) // 若有路徑
        {
            Vector3 v1 = new Vector3(nav.path.corners[0].x, 0,
            nav.path.corners[0].z);//當前的點
            Vector3 v2 = new Vector3(nav.path.corners[1].x, 0,
            nav.path.corners[1].z);//下一個點


            // 計算移動向量的旋轉方向
            if (forward)
                return (v2 - v1);
            else
                return (v1 - v2);
        }
        else
        {
            return Vector3.zero;
        }
    }
    float GetXZAngle(Vector3 forward, Vector3 selfPosition, Vector3
    targetPosition, bool signedNumber)
    {
        Vector3 targetdir = targetPosition - selfPosition;
        // 計算夾角
        float angle = Vector2.Angle(new Vector2(forward.x, forward.z), new
    Vector2(targetdir.x, targetdir.z));

        if (signedNumber) // 若需區分左右
        {
            Vector3 crossV = Vector3.Cross(forward, targetdir); // 外積
            if (crossV.y < 0) // 若目標在左邊
                angle *= -1;
        }

        return angle;
    }

    /// <summary>
    /// 搜尋身邊的角色是否為玩家
    /// </summary>
    /// <param name="playerTags">玩家的 tag 列表</param>
    /// <param name="Player">回傳搜尋到的玩家 transform</param>
    /// <param name="Distance">回傳該玩家的距離</param>
    /// <returns></returns>
    bool FindNearestPlayer(string[] PlayerTags, out Transform Player, out float Distance)
    {
        Transform player = null;
        float nd = 10000f;
        bool fined = false;

        // 取得範圍內所有角色
        Collider[] actors = Physics.OverlapSphere(transform.position, lookDistance, actorLayer);
        for (int i = 0; i < actors.Length; i++)
        {
            bool isPlayer = false;
            for (int pt = 0; pt < PlayerTags.Length; pt++) // 判斷該角色是否為玩家
            {
                if (actors[i].tag == PlayerTags[pt])
                {
                    isPlayer = true;
                    break;
                }
            }
 
            if (isPlayer) // 若角色是玩家
            {
                // 若在視線角度內
                if (GetXZAngle(transform.forward, transform.position,
               actors[i].transform.position, false) < lookAngle)
                {
                    //print("視線角度內+"+actors[i]);
                    if (NoObstacle(actors[i].transform)) // 若中間沒有障礙物
                    {
                        // 判斷距離看是否是最近的一個玩家
                        float d = Vector3.Distance(transform.position,
                       actors[i].transform.position);

                        if (d < nd)  //比原本還近
                        {
                            nd = d;
                            player = actors[i].transform;
                        }
                        tagObject = player.gameObject;
                        //判斷在攻擊範圍內
                        if (nd < ArangeDistance || GetXZAngle(transform.forward, transform.position,
                                tagObject.transform.position, false) < ArangeAngle)
                        {
                            agent.speed = 0;
                            AttackAngleT = true;
                            //if (coolDown >= 1)
                            //{
                            //    //判斷在攻擊角度內
                            //    if (GetXZAngle(transform.forward, transform.position,
                            //        tagObject.transform.position, false) < AttackAngle)
                            //    {
                            //        agent.speed = 0;
                            //        AttackAngleT = true;
                            //        //print("攻擊角度內" + tagObject);
                            //    }
                            //    else
                            //    {
                            //        //若不在攻擊角度內轉向目標
                            //        //Vector3 targetDir = tagObject.transform.position - transform.position;
                            //        //Quaternion rotate = Quaternion.LookRotation(targetDir);
                            //        //transform.localRotation = Quaternion.Slerp(transform.localRotation, rotate, 50f * Time.deltaTime);
                            //        //print("轉向" + tagObject);
                            //    }
                            //    coolDown = 1f;
                            //}
                            //else
                            //{
                            //    //coolDown += Time.deltaTime;
                            //}
                            
                        }
                        fined = true;                     
                    }
                }
            }
        }
        Player = player;
        Distance = nd;
        return fined;     
    }
    // 用射線判斷眼睛到目標間是否有障礙物
    bool NoObstacle(Transform targetActor)
    {
        bool ret = true;

        // 打一條射線看之間是否有障礙物
        Vector3 origin = transform.position + new Vector3(0, heightOfEye, 0);
        Vector3 targetPos = targetActor.position + new Vector3(0, heightOfTarget, 0);
        Vector3 direct = targetPos - origin;
        Ray ray = new Ray(origin, direct);
        RaycastHit hit = new RaycastHit();
        float distance = Vector3.Distance(transform.position, targetActor.position);
        int maskMonster = 1 << LayerMask.NameToLayer("Monster");

        if (Physics.Raycast(ray, out hit, distance, 0xFFFF - actorLayer)) //若有障礙物
        {

#if UNITY_EDITOR
            Debug.DrawRay(origin, hit.point - origin, Color.yellow);

#endif
            if (Physics.Raycast(ray, out hit, distance, maskMonster))  //無視Monster圖層
            {

            }
            else
            {
                // 射線有打到東西表示角色間有障礙物
                ret = false;
            }
        }

        return ret;
    }

    void Update()
    {
        SF_BulletNub = BulletNub;
        SF_bulletAttack = bulletAttack;
        SF_muzzleGrid = muzzleGrid;
        PS_muzzle = muzzle;
        if (attackTarget != null)
        {
            目前攻擊目標 = attackTarget.gameObject;
        }

        //if (coolDown >= 0.7f && BulletNub>0)  //攻擊冷卻時間
        //{
        //    Fire = true;
        //    attacking = false;
        //    coolDown = 0;
        //    bulletAttack = 1;
        //}
        //if (coolDown >= 0)
        //{
        //    coolDown += Time.deltaTime;
        //}
        if (StartTime >= 0)
        {
            StartTime += Time.deltaTime;
            if (StartTime >= 2f)
            {
                StartTime = -1;
                StartAttack = true;
                MG_Turret[0].GetComponent<MG_Turret_AI>().Player = Player;
                MG_Turret[1].GetComponent<MG_Turret_AI>().Player = Player;
                MG_Turret[2].GetComponent<MG_Turret_AI>().Player = Player;
            }
        }
        if (StartAttack)
        {
            Vector3 origin = Eye.transform.position;
            Vector3 targetPos = Player.transform.position +new Vector3(0, 2, 0);
            Vector3 direct = targetPos - origin;
            Ray ray = new Ray(origin, direct);
            RaycastHit hit = new RaycastHit(); //射線擊中資訊
            //float distance = Vector3.Distance(origin, targetPos);

            if (Physics.Raycast(ray, out hit, 70f, layerMask)) 
            {
                //Debug.DrawLine(ray.origin, hit.point, Color.black, 0.5f, false);  //黑線

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Actor"))  //取得玩家
                {
                    if (hit.collider.tag == "Player")  //玩家
                    {
                        //Debug.DrawLine(ray.origin, hit.point, Color.green, 1f, false);  //綠線
                        if (BulletNub <= 0)   //武器1 過熱
                        {
                            Reload = true; //冷卻狀態
                            overheatLock = true;   // 過熱鎖定
                            BulletNub = 0;
                            bulletAttack = 0;
                            ReLoad();
                            //Attack();
                        }
                        else  //武器1 攻擊
                        {
                            LockTime = 0;
                            Fire = true;
                            attacking = true;
                            Attack();
                            //print("攻擊");
                        }
                    }
                }

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))  //玩家躲在掩體
                {
                    if (hit.collider.tag == "Crystal")  //水晶
                    {
                        //Debug.DrawLine(ray.origin, hit.point, Color.yellow, 1f, false);  //黃線
                        LockTime += Time.deltaTime;
                        if (LockTime >= 2)
                        {
                            //AttackMode = 2;   //攻擊模式2
                            LockTime = 2;
                            //武器1 冷卻
                            //bulletAttack = 0;
                            Reload = true;
                            overheatLock = false; 
                            ReLoad();
                            Attack();
                            //print("被擋住了");
                        }
                    }
                }
            }
            switch (AttackRange)
            {
                case 1:  //攻擊1 範圍
                    if (Reload && overheatLock)  //處於冷卻狀態 並過熱鎖定
                    {
                        AttackMode = 2;
                        //print("過熱中");
                    }
                    if(Reload && !overheatLock)  //處於冷卻狀態 並 非過熱鎖定
                    {
                        Reload = false;
                        AttackMode = 1;  //攻擊模式1
                        MG_Turret[0].GetComponent<MG_Turret_AI>().StartAttack = false;
                        MG_Turret[1].GetComponent<MG_Turret_AI>().StartAttack = false;
                        MG_Turret[2].GetComponent<MG_Turret_AI>().StartAttack = false;
                        //print("過熱但開火");
                    }
                    else if(!Reload)  //處於非冷卻狀態
                    {
                        AttackMode = 1;  //攻擊模式1
                        MG_Turret[0].GetComponent<MG_Turret_AI>().StartAttack = false;
                        MG_Turret[1].GetComponent<MG_Turret_AI>().StartAttack = false;
                        MG_Turret[2].GetComponent<MG_Turret_AI>().StartAttack = false;
                        //print("開火");
                    }
                    MA_weight += 1.5f * Time.deltaTime;
                    if (MA_weight >= 1) MA_weight = 1;
                    break;
                case 2:  //攻擊1 右邊死角範圍
                    Reload = true;
                    overheatLock = false;
                    ReLoad();
                    AttackMode = 2;  //攻擊模式2
                    MA_weight -= 0.5f * Time.deltaTime;
                    if (MA_weight <= 0.4) MA_weight = 0.4f;
                    MG_Turret[1].GetComponent<MG_Turret_AI>().StartAttack = true;
                    MG_Turret[2].GetComponent<MG_Turret_AI>().StartAttack = true;
                    break;
                case 3:  //攻擊1 左邊死角範圍
                    Reload = true;
                    overheatLock = false;
                    ReLoad();
                    AttackMode = 2;  //攻擊模式2
                    MA_weight -= 0.5f * Time.deltaTime;
                    if (MA_weight <= 0.5) MA_weight = 0.5f;
                    MG_Turret[0].GetComponent<MG_Turret_AI>().StartAttack = true;
                    MG_Turret[1].GetComponent<MG_Turret_AI>().StartAttack = true;
                    MG_Turret[2].GetComponent<MG_Turret_AI>().StartAttack = true;
                    break;
            }
            MA_Rig[0].GetComponent<MultiAimConstraint>().weight = MA_weight;  //槍口連結
            ani.SetInteger("AttackMode", AttackMode);
        }

        //if (FindNearestPlayer(playerTags, out attackTarget, out targetDistance))// 若有掃描到玩家
        //{
        //    //actionTimer = nextActionTime; // 把計時器設為時間已到,當玩家離開視線時能強制更換行為
        //    // 與攻擊目標的距離
        //    float d = Vector3.Distance(transform.position, attackTarget.position);
        //    if (d < attackDistance) // 玩家距離小於攻擊距離,攻擊玩家
        //    {
        //        if (attacking)  // 若在攻擊狀態中,一定要等攻擊完才做下一次的動作
        //        {
        //            Vector3 atP = new Vector3(attackTarget.position.x, attackTarget.position.y, attackTarget.position.z);
        //            AAT = atP;
        //            return;
        //        }
        //        if (AttackAngleT)
        //        {
        //            Fire = true;
        //            AttackPlay = true;
        //            attacking = true;
        //            Attack();
        //            //print("小於攻擊距離 攻擊+" + attackTarget);
        //        }
        //        else
        //        {
        //            AttackPlay = false;
        //        }
        //    }
        //    else // 玩家距離大於攻擊距離,進行追踪
        //    {
        //        //TrPlayer = true;
        //        //Fire = false;
        //        //TrackingPlayer();
        //        return;
        //    }
        //}
        //else 
        //{
        //    //float d = Vector3.Distance(transform.position, attackTarget.position);
        //    //if (d < arriveDistance)  //若距離小於停止距離
        //    //{
        //    //    //print("警戒");
        //    //}


        //    //取得角色與目標的距離
        //    //print(oriTarget[defense]);
        //    //float dn = Vector3.Distance(transform.position, oriTarget[A_defense].position);
        //    //moving = true;
        //    //if (dn < attackDistance) // 玩家距離小於攻擊距離,攻擊玩家
        //    //{
        //    //    //AttackPlay = false;
        //    //    //Fire = true;
        //    //    //Attack();
        //    //    TrPlayer = false;
        //    //    TrackingPlayer();
        //    //    Fire = false;
        //    //}
        //    //else // 玩家距離大於攻擊距離,進行追踪
        //    //{
        //    //    TrPlayer = false;
        //    //    TrackingPlayer();
        //    //    Fire = false;
        //    //}
        //}
        if (moving)   //若要移動，進行方向修正
        {
            transform.rotation = GetNavRotation(true, agent);
        }
    }
    void ReLoad()  //武器冷卻
    {
        if (Reload)
        {
            overheatTime = MuzzleMaterial[0].GetFloat("_EmissiveExposureWeight");
            overheatTime += 0.018f * Time.deltaTime;
            if (overheatTime >= 1)
            {
                overheatTime = 1;
                Reload = false;
                BulletNub = 30;
            }
            MuzzleMaterial[0].SetFloat("_EmissiveExposureWeight", overheatTime);
        }
    }
    void FixedUpdate()
    {
        if (StartAttack)
        {
            RigTarget[0].transform.position = Player.transform.position;  //頭部鎖定玩家
            RigTarget[1].transform.position = Player.transform.position;  //槍口鎖定玩家
        }

        if (bulletAttack >= 1)
        {
            attacking = false;
            bulletAttack = 0;
            //int muzzleRange = Random.Range(0, MaxMuGrid);
            //if (muzzleGrid[muzzleRange] <= 0)
            //{
            //    muzzleGrid[muzzleRange] = 1;
            //}
            //else
            //{
            //    return;
            //}
            Vector3 origin = muzzle[0].transform.position;
            Vector3 targetPos = Player.transform.position + new Vector3(0, 2, 0);
            Vector3 direct = targetPos - origin;
            Ray ray = new Ray(origin, direct);
            RaycastHit hit = new RaycastHit(); //射線擊中資訊
            if (Physics.Raycast(ray, out hit, 70f, layerMask))
            {
                //Debug.DrawLine(ray.origin, hit.point, Color.black, 0.5f, false);  //黑線

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Actor"))  //取得玩家
                {
                    if (hit.collider.tag == "Player")  //玩家
                    {
                        Debug.DrawLine(ray.origin, hit.point, Color.green, 0.1f, false);  //綠線
                        AAT = hit.transform.position;
                    }
                }
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))  //玩家躲在掩體
                {
                    if (hit.collider.tag == "Crystal")  //水晶
                    {
                        Debug.DrawLine(ray.origin, hit.point, Color.yellow, 0.1f, false);  //黃線
                        AAT = hit.transform.position;
                    }
                }
            }

            //if (attackTarget != null) 目前攻擊目標 = attackTarget.gameObject;
            muzzlePOS = muzzle[0].transform.position;
            Vector3 AttacktargetDir = targetPos - muzzlePOS;  //子彈轉向目標
            Quaternion rotate = Quaternion.LookRotation(AttacktargetDir);
            cuMuGrid = 0;
            //ButtleType = Random.Range(0, 2);  //子彈類型
            switch (Level)
            {
                case 1:
                    BulletType = 1;
                    break;
                case 2:
                    BulletType = 2;
                    break;
            }
            pool.ReUseBoss2Bullet(muzzlePOS, rotate, BulletType, cuMuGrid);  //生成子彈
            //SaveBT = ButtleType;
            //for (int i=0; i< 2; i++)
            //{
            //    muzzle[cuMuGrid].gameObject.transform.GetChild(i).gameObject.SetActive(false);
            //}
            //coolDown = 0;
            //print("bulletAttack" + coolDown);
        }
    }
    private void TrackingPlayer()
    {
        //ani.SetBool("Move", true);
        //ani.SetBool("Attack", false);
        agent.speed = agentSpeed;  //移動速度
        if (TrPlayer)
        {
            agent.destination = attackTarget.position; // 設為尋徑目標
        }
        else
        {
            //agent.destination = oriTarget[A_defense].position; // 設為尋徑目標
        }
        attacking = false; // 追踪玩家,不在攻擊狀態
        //print("追擊" + attackTarget);  
    }
    private void Attack()
    {
        if (!StartAttack) return;

        //if (AttackPlay)
        //{
        //    Vector3 atP = new Vector3(attackTarget.position.x, attackTarget.position.y, attackTarget.position.z);
        //    AAT = atP;
        //    //AAT = attackTarget.position;
        //}
        //else
        //{
        //    //AAT = oriTarget[A_defense].position;
        //}
        switch (AttackMode)
        {
            case 1:
                if (BulletNub <= 18)
                {
                    float overheat = 0.91f + BulletNub * (0.09f / 18);  //最小EEW + 當前子彈數 * (最小與最大EEW差值 / 子彈上限) 
                    if (overheat <= 0.91f) overheat = 0.91f;
                    MuzzleMaterial[0].SetFloat("_EmissiveExposureWeight", overheat);
                }
                //print("子彈 " + BulletNub);
                break;
            case 2:
                //ani.SetBool("Attack1", false);
                //print("攻擊2");
                break;
        }
        ani.SetBool("Attack1", true);  //第一階攻擊模式
        ani.SetInteger("AttackMode", AttackMode);

        if (Fire)
        {        
            //ani.SetBool("Move", false);
            AttackAngleT = false;
            agent.speed = 0;
            //Muzzle_vfx.SetActive(true);
            //MuFire.Play();

            //Vector3 targetDir = AAT - transform.position;
            //Quaternion rotate = Quaternion.LookRotation(targetDir);
            //transform.localRotation = Quaternion.Slerp(transform.localRotation, rotate, 40f * Time.smoothDeltaTime);
            //AttackAning(true, 1);
            //print("Fire" + coolDown);
        }
    }
    public void AttackAning(bool attackingB, int BulletAttackNub)
    {
        attacking = attackingB;
        bulletAttack = BulletAttackNub;
    }
    public static void ReMuzzleGrid(int Grid)  //釋放槍口格子
    {
        muzzleGrid[Grid] = 0;
    }
    void OnDisable()  //禁用時
    {
        attacking = false;
    }
}