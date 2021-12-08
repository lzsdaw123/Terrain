using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NPC_AI : MonoBehaviour
{
    public NavMeshAgent agent;  //尋徑代理人
    private SpawnRayReg reg;    //生怪註冊器
    private SpawnRay spawnRay;  //生怪后蟲
    private Vector3 target; // 尋徑目標點
    public GameObject MissionTarget;  //任務目標點
    GameObject tagObject;
    public ObjectPool pool;
    [SerializeField] private AnimEvents AnimEvents;
    public Animator ani; //動畫控制器
    public GameObject Weap;
    public float arriveDistance = 4f; // 到達目的地的距離
    private bool moving = false;  //是否要移動角色
    private float speed = 0; //animator裡面用的speed數值

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
    [TagSelector] public string[] playerTags = { "Enemy" };
    public static Transform attackTarget; // 搜尋到最近的攻擊目標
    public static Transform oriTarget; // 任務的攻擊目標
    public static Vector3 AAT; // 搜尋到最近的攻擊目標
    public static int aTN;
    private float targetDistance = 2000; // 與最近攻擊目標的距離
    [SerializeField] private float attackDistance; // 攻擊角度距離
    [SerializeField] private float ArangeDistance; // 攻擊範圍距離
    bool AttackAngleT=false;
    private  bool attacking;
    public bool isEnemy=false;
    public static bool AttackPlay;
    bool TrPlayer;
    public bool Fire=false;
    public static bool Reload;
    public static int WeapAm =30;  //武器彈藥量
    public int T_WeapAm = 300; //武器總彈藥量
    public float coolDown; //冷卻結束時間
    public float coolDownTimer; //冷卻時間計時器
    static int FireButtle;  //開火動畫冷卻
    public bool BFire;  //生成子彈bool
    public GameObject Muzzle_vfx;  //槍口火光  
    public ParticleSystem MuSmoke;  //槍口煙霧
    public ParticleSystem MuFire;  //槍口火
    public LayerMask layerMask;  //圖層
    public int HitType;  //彈孔類型變數
    bool NoActor = false;  //擊中玩家
    Quaternion rot;  //彈孔生成角度
    Vector3 pos;  //彈孔生成位置
    public static float power; //子彈威力
    public ObjectPool pool_Hit;  //物件池
    bool LayDown;  //收槍bool

    public AttackLevel attackLv1 = new AttackLevel(false, 2f, 3f, 80f, 1f); //第一段攻擊力 (威力,距離,角度,高度)

    public GameObject bullet;
    [SerializeField]private Vector3 muzzlePOS;  //槍口座標
    public float targetHP;

    private AttackUtility attackUtility = new AttackUtility();

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

        Gizmos.color = Color.blue; // 設為紅色
        Quaternion ArangeRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -ArangeAngle, 0));// 計算弧線的起始角度
        GizmosExtension.DrawSector(transform.position + eyeHi, ArangeDistance, ArangeAngle * 2, ArangeRotation);// 畫扇形 

    }
#endif

    void Start()
    {
        pool_Hit = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        pool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        agent = GetComponent<NavMeshAgent>();
        coolDown = 0.8f;  //冷卻結束時間
        coolDownTimer = coolDown + 1;
        agent.enabled = true;
        LayDown = false;

        //reg = GetComponent<SpawnRayReg>();
        //spawnRay = reg.mother;  //取得怪物的母體

        //取得一個尋徑目標點
        //target = GetNavTarget();  //取得一個目標
        //agent.destination = target;  //把目標設到尋徑裡面
        target = transform.position; //先令目標點等於角色所在位置
        agent.destination = target; //設置尋徑目標點
        agent.speed = 0; //要用動畫去移動,因此尋徑的速度要設為 0

        //if (ani == null)
        //{
        //    ani = GetComponent<Animator>(); //自動取得動畫控制器
        //}
        MissionTarget = GameObject.Find("MissionTarget").gameObject;
        oriTarget = MissionTarget.transform;
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
                            //print(d + "++" + actors[i]);
                            nd = d;
                            player = actors[i].transform;                      
                        }
                        tagObject = player.gameObject;
                        //判斷在攻擊範圍內
                        if (nd < ArangeDistance || GetXZAngle(transform.forward, transform.position,
                                tagObject.transform.position, false) < ArangeAngle)
                        {
                            //判斷在攻擊角度內
                            if (GetXZAngle(transform.forward, transform.position,
                                tagObject.transform.position, false) < AttackAngle)
                            {
                                AttackAngleT = true;
                            }
                            else
                            {
                                speed = 1f;
                                ani.SetBool("Fire", false);
                                AttackAngleT = false;
                                //若不在攻擊角度內轉向目標
                                Vector3 targetDir = tagObject.transform.position - transform.position;
                                Quaternion rotate = Quaternion.LookRotation(targetDir);
                                //Weap.transform.localRotation = Quaternion.Slerp(transform.localRotation, rotate, 60f * Time.smoothDeltaTime);
                                transform.localRotation = Quaternion.Slerp(transform.localRotation, rotate, 60f * Time.smoothDeltaTime);
                            }
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
        Vector3 targetPos = targetActor.position + new Vector3(0,heightOfTarget, 0);
        Vector3 direct = targetPos - origin;
        Ray ray = new Ray(origin, direct);
        RaycastHit hit = new RaycastHit();
        float distance = Vector3.Distance(transform.position, targetActor.position);
        int maskMonster = 1 << LayerMask.NameToLayer("Actor");

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
        if (attacking)return; // 若在攻擊狀態中,一定要等攻擊完才做下一次的動作

        if (FindNearestPlayer(playerTags, out attackTarget, out targetDistance))// 若有掃描到玩家
        {
            if (LayDown)
            {
                LayDown = false;
                ani.SetBool("LayDown", false);
            }
            //actionTimer = nextActionTime; // 把計時器設為時間已到,當玩家離開視線時能強制更換行為
            // 與攻擊目標的距離
            float d = Vector3.Distance(transform.position, attackTarget.position);
            if (d < attackDistance) // 玩家距離小於攻擊距離,攻擊玩家
            {
                if (AttackAngleT)
                {
                    AttackPlay = true;
                    Fire = true;
                    Attack();
                    //print("小於攻擊距離 攻擊+"+ attackTarget);
                }
                else
                {
                    Fire = false;
                }
            }
            else // 玩家距離大於攻擊距離,進行追踪
            {
                TrPlayer = true;
                Fire = false;
                TrackingPlayer();
            }
        }
        else 
        {
            //float d = Vector3.Distance(transform.position, attackTarget.position);
            //if (d < arriveDistance)  //若距離小於停止距離
            //{
            //    //print("警戒");
            //}
            Fire = false;
            if (!LayDown && !Reload)
            {
                LayDown = true;
                ani.SetTrigger("LayDownT");
                ani.SetBool("LayDown", true);
            }
                
            if (MissionTarget.activeSelf)
            {
                //取得角色與目標的距離
                float dn = Vector3.Distance(transform.position, oriTarget.position);
                //moving = true;
                if (dn < attackDistance) // 玩家距離小於攻擊距離,攻擊玩家
                {
                    AttackPlay = false;
                    Attack();                   
                }
                else // 玩家距離大於攻擊距離,進行追踪
                {
                    TrPlayer = false;
                    TrackingPlayer();
                }
            }
            else
            {
                MissionTarget = null;
            }

        }
        if (moving)   //若要移動，進行方向修正
        {
            transform.rotation = GetNavRotation(true, agent);
        }

        if (coolDownTimer > coolDown) //若冷卻時間已到 可以發射子彈
        {
            Muzzle_vfx.SetActive(false); //關閉火光
            ani.SetBool("Fire", false);
            //若按下滑鼠左鍵開火
            if (Fire)
            {
                if (WeapAm != 0)
                {
                    MuSmoke.Stop();  //關閉槍口煙霧
                    WeapAm--;
                    if (FireButtle == 1)
                    {
                        ani.SetBool("Fire", true);
                        FireButtle = 0;
                    }
                    BFire = true;  //生成子彈
                    //Weapon.SetBool("Aim", false);                    
                }
                else  //沒子彈
                {
                    if (Reload == false)
                    {
                        FireButtle = 0;
                        Reload = true;
                        ani.SetTrigger("Reload");
                    }
                }
                coolDownTimer = 0.66f;  //開火冷卻時間，與coolDown 0.8差越小越快
            }
            else
            {
                ani.SetBool("Fire", false);
            }
        }
        else //否則需要冷卻計時
        {
            coolDownTimer += Time.deltaTime;
            //Weapon.SetBool("Fire", false);
        }

    }
    void FixedUpdate()
    {
        if (BFire) //生成子彈
        {
            muzzlePOS = Muzzle_vfx.transform.position;
            //pool.ReUse(muzzlePOS, transform.rotation);
            Vector3 targetPos = tagObject.transform.position;
            Vector3 direct = targetPos - muzzlePOS;
            Ray ray = new Ray(muzzlePOS, direct);
            RaycastHit hit = new RaycastHit(); //射線擊中資訊       
            float distance = Vector3.Distance(transform.position, tagObject.transform.position);
            //由槍口位置射到是敵物位置的射線
            if (Physics.Raycast(ray, out hit, distance, layerMask)) //擊中圖層
            {
                //if(hit.collider.name == "Scorpion")
                //{

                //}
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))  //彈孔噴黑煙
                {
                    HitType = 0;
                    //Debug.DrawLine(ray.origin, hit.point, Color.black, 0.7f, false);
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
                    //Debug.DrawLine(ray.origin, hit.point, Color.blue, 0.3f, false);
                }
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))  //彈孔噴紅血
                {
                    HitType = 1;
                    //Debug.DrawLine(ray.origin, hit.point, Color.red, 1f, false);
                    if (hit.collider.tag == "Enemy")  //綠血
                    {
                        HitType = 2;
                        power = 1;
                        hit.transform.SendMessage("Unit", false);  //攻擊者是否為玩家
                        hit.transform.SendMessage("Damage", power) ;  //造成傷害
                        //Debug.DrawLine(ray.origin, hit.point, Color.green, 1f, false);
                    }
                    if (hit.collider.tag == "Carapace")  //甲殼
                    {
                        HitType = 4;
                    }
                }
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Actor"))
                {
                    if (hit.collider.tag != "MissionTarget")
                    {
                        NoActor = true;
                    }
                    print("打到Actor");
                }
            }
            //在到物體上產生彈孔
            rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            pos = hit.point;
            if (NoActor && pos != Vector3.zero)
            {
                NoActor = false;
                //pool_Hit.ReUseHit(pos, rot, HitType); ;  //從彈孔池取出彈孔
            }
            else
            {
                pool_Hit.ReUseHit(pos, rot, HitType);  //從彈孔池取出彈孔
            }
            Muzzle_vfx.transform.position = muzzlePOS;
            Muzzle_vfx.transform.rotation = transform.rotation;
            Muzzle_vfx.SetActive(true);
            AnimEvents.NPC_Audio(2);  //開火音效
            MuSmoke.Play();
            BFire = false;
        }
    }
    private void TrackingPlayer()
    {
        ani.SetBool("Move", true);
        ani.SetBool("Fire", false);
        agent.speed = 9;  //移動速度
        if (TrPlayer)
        {
            agent.destination = attackTarget.position; // 設為尋徑目標
        }
        else
        {
            agent.destination = oriTarget.position; // 設為尋徑目標
        }
        speed = 1;// 跑向目標
        ani.SetFloat("Speed", speed);       
        attacking = false; // 追踪玩家,不在攻擊狀態
        //print("追擊" + attackTarget);  
    }
    private void Attack()
    {
        if (AttackPlay)
        {
            AAT = attackTarget.position;
        }
        else
        {
            AAT = oriTarget.position;
        }
        AttackAngleT = false;
        agent.speed = 0;
        speed = 0;
    }
    public static void ReLoad_E()  //換彈結束
    {
        WeapAm = 30;
        Reload = false;
        FireButtle = 1;
    }
    public static void Loaded()
    {
        FireButtle = 1;
    }
}