using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MonsterAI02 : MonoBehaviour
{
    public NavMeshAgent agent;  //尋徑代理人
    private SpawnRayReg reg;    //生怪註冊器
    private SpawnRay spawnRay;  //生怪后蟲
    private Vector3 target; // 尋徑目標點
    public GameObject MissionTarget;
    Transform OriTarger;

    public Animator ani; //動畫控制器
    public float arriveDistance = 0.5f; // 到達目的地的距離
    private bool moving = false;  //是否要移動角色
    private float speed = 0; //animator裡面用的speed數值

    // run 的%不用指定,因為除了 idle 和 walk,剩下就是 run
    public float idlePercent = 10f;
    public float walkPercent = 90f;


    // 若一段時間沒到達目的或是 idle 一段時間,需強制更換行動
    public float changeActionMin = 5f; // 行為的執行時間範圍
    public float changeActionMax = 8f; // 行為的執行時間範圍
    private float actionTimer = 0; // 行為計時器
    private float nextActionTime = 0; // 到下個行為的時間

    public float heightOfEye = 1.7f; // 眼睛高度
    public float heightOfTarget = 1f; // 敵物要掃描的高度
    public float lookDistance = 10f; // 看見玩家的距離
    [Range(0, 180)] public float lookAngle = 80; // 可見的夾角,左右各 80 度,所以可視範圍為 160 度
    public LayerMask actorLayer = 0; // 角色所在的圖層
    [TagSelector] public string[] playerTags = { "Player" };
    private Transform attackTarget; // 搜尋到最近的攻擊目標
    private float targetDistance = 2000; // 與最近攻擊目標的距離
    [SerializeField] private float attackDistance = 6f; // 攻擊距離
    public static bool attacking;
    public static int buttleAttack;

    public AttackLevel attackLv1 = new AttackLevel(false, 2f, 3f, 80f, 1f); //第一段攻擊力 (威力,距離,角度,高度)

    public GameObject bullet;
    public GameObject muzzle;
    public Vector3 muzzlePOS;  //槍口座標



    private AttackUtility attackUtility = new AttackUtility();

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 eyeHi = new Vector3(0, heightOfEye, 0); // 眼睛高度
        Gizmos.color = Color.cyan; // 設為青色
        Quaternion arcRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -lookAngle, 0));// 計算弧線的起始角度
        GizmosExtension.DrawSector(transform.position + eyeHi, lookDistance, lookAngle * 2, arcRotation);// 畫扇形 

         Vector3 weaponHi; // 武器高度
        if (attackLv1.displayGizmos)
      {
          weaponHi = new Vector3(0, attackLv1.height, 0);
            Gizmos.color = Color.red; // 設為紅色
          arcRotation= Quaternion.Euler(transform.eulerAngles + new Vector3(0, -attackLv1.angle, 0));// 計算弧線的起始角度
         GizmosExtension.DrawSector(transform.position + weaponHi,attackLv1.distance, attackLv1.angle * 2, arcRotation);// 畫扇形
       }
    }
#endif

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //ani = GetComponent<Animator>();

        agent.enabled = true;


        reg = GetComponent<SpawnRayReg>();
        spawnRay = reg.mother;  //取得怪物的母體

        //取得一個尋徑目標點
        //target = GetNavTarget();  //取得一個目標
        //agent.destination = target;  //把目標設到尋徑裡面
        target = transform.position; //先令目標點等於角色所在位置
        agent.destination = target; //設置尋徑目標點
        agent.speed = 5; //要用動畫去移動,因此尋徑的速度要設為 0

        //if (ani == null)
        //{
        //    ani = GetComponent<Animator>(); //自動取得動畫控制器
        //}

        OriTarger = MissionTarget.transform;
    }

    public void AttackLv1()
    {
        attackUtility.AttackTargets(attackLv1, transform, playerTags, actorLayer);
    }
    //Vector3 GetOriTarget()
    //{
        
    //}

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
                    if (NoObstacle(actors[i].transform)) // 若中間沒有障礙物
                    {
                        // 判斷距離看是否是最近的一個玩家
                        float d = Vector3.Distance(transform.position,
                       actors[i].transform.position);
                        if (d < nd)
                        {
                            nd = d;
                            player = actors[i].transform;
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
        Vector3 targetPos = targetActor.position + new Vector3(0,
       heightOfTarget, 0);
        Vector3 direct = targetPos - origin;
        Ray ray = new Ray(origin, direct);
        RaycastHit hit = new RaycastHit();
        float distance = Vector3.Distance(transform.position,
       targetActor.position);
        if (Physics.Raycast(ray, out hit, distance, 0xFFFF - actorLayer)) //若有障礙物
        {

#if UNITY_EDITOR
            Debug.DrawRay(origin, hit.point - origin, Color.yellow);

#endif

            // 射線有打到東西表示角色間有帳礙物
            ret = false;

        }

        return ret;
    }

    void Update()
    {
        muzzlePOS = muzzle.GetComponent<Transform>().position;

        attacking = AnimEvents.attacking;
        buttleAttack = AnimEvents.buttleAttack;
        if (attacking) return; // 若在攻擊狀態中,一定要等攻擊完才做下一次的動作


        if (FindNearestPlayer(playerTags, out attackTarget, out targetDistance))// 若有掃描到玩家
        {
            //--actionTimer = nextActionTime; // 把計時器設為時間已到,當玩家離開視線時能強制更換行為
            // 與攻擊目標的距離
            float d = Vector3.Distance(transform.position, attackTarget.position);

            if (d < attackDistance) // 玩家距離小於攻擊距離,攻擊玩家
            {
                Attack();
            }
            else // 玩家距離大於攻擊距離,進行追踪
            {
                TrackingPlayer();
            }

        }
        else
        {
           //-- actionTimer += Time.deltaTime; //計時器

            //取得角色與目標的距離
            float d = Vector3.Distance(transform.position, OriTarger.position);

            // if (d < arriveDistance || actionTimer > nextActionTime)  //若距離小於停止距離或時間超過下次行為時間

            moving = true;
            //OriTarger = GetOriTarget();  //取得新的移動目標
            agent.destination = OriTarger.position;  //設尋徑為新的目標
            speed = 5f;

            if (d < attackDistance) // 玩家距離小於攻擊距離,攻擊玩家
            {
                Attack();
            }
            else // 玩家距離大於攻擊距離,進行追踪
            {
                //TrackingPlayer();
            }

            //-- actionTimer = 0; //計時器歸０
            nextActionTime = Random.Range(changeActionMin, changeActionMax); //重新取得下次更換動作時間
       
        }
        ani.SetFloat("Speed", speed); //設置動畫播放

        if (moving)   //若要移動，進行方向修正
        {
            transform.rotation = GetNavRotation(true, agent);
        }
        
    }
    void FixedUpdate()
    {
        if (buttleAttack>=1)
        {
            GameObject obj = Instantiate(bullet, muzzlePOS, OriTarger.rotation);
            buttleAttack = 0;
        }
    }
    private void TrackingPlayer()
    {
        agent.destination = attackTarget.position; // 設為尋徑目標
        speed = 5;// 跑向目標
        attacking = false; // 追踪玩家,不在攻擊狀態
    }
    private void Attack()
    {       
        speed = 0;
        attacking = true;
        ani.SetFloat("Speed", speed);
        ani.SetTrigger("Attack");
    }
    //public void AttackCompletion()
    //{
    //    attacking = false;
    //}


}