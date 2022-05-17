using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MG_Turret_AI : MonoBehaviour
{
    public ObjectPool pool;
    [SerializeField] private AnimEvents AnimEvents;
    public Crystal_Life crystal_Life;
    public Boss_Life boss_Life;
    public Animator ani; //動畫控制器
    public GameObject Player;  //玩家
    public GameObject bullet;
    public LayerMask layerMask;  //圖層
    [SerializeField] private bool attacking;
    private int bulletAttack;
    public static int BulletNub;  //子彈數量
    public float power;  //傷害
    [SerializeField] int SF_BulletNub;  //子彈數量
    [SerializeField] private GameObject[] muzzle;  //槍口
    public GameObject[] RigTarget;  //槍口瞄準目標
    public GameObject[] MA_Rig;  //Rig連結
    public float MA_weight;  //Rig連結權重
    public bool StartAttack;  //進入攻擊狀態
    public int AttackRange;  //攻擊範圍
    public bool[] InAttackRange;
    public float LockTime;  //鎖定時間
    public bool Reload;
    public float ReloadTime;
    public bool overheatLock;  //過熱鎖定
    public float coolDown;
    public bool Fire;
    public bool[] FlyStart;  //子彈開始飛行
    public float[] FlySpeed; //子彈飛行速度
    public float[] distance; //子彈預計飛行距離
    public bool[] SureHit;  //擊出射線
    public Vector3[] O_targetPos;
    public Vector3 NO_targetPos;
    public float[] ETA;  //預計抵達時間
    public bool[] HitTarget;  //命中玩家
    public bool Dead;

    void Start()
    {
        //pool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        attacking = false;
        bulletAttack = 1;
        //Fire = false;
        //AttackStatus = false;
        coolDown = 1;
        BulletNub = 30;  //子彈數
        power = 0.5f;
        StartAttack = false;
        Dead = false;
        ReloadTime = 0;
        SureHit = new bool[] { false, false };
        FlyStart = new bool[] { false, false };
        HitTarget = new bool[] { false, false };
        InAttackRange = new bool[] { true, false };
    }

    void Update()
    {
        Dead = crystal_Life.Dead;
        if (boss_Life.Dead || !boss_Life.gameObject.GetComponent<Boss02_AI>().enabled) StartAttack = false;

        if (StartAttack && !Dead)
        {
            SF_BulletNub = BulletNub;

            ani.SetBool("Start", true);  //進入攻擊模式
            Vector3 origin = muzzle[0].transform.position;
            Vector3 targetPos = Player.transform.position + new Vector3(0, 2, 0);
            Vector3 direct = targetPos - origin;
            Ray ray = new Ray(origin, direct);
            RaycastHit hit = new RaycastHit(); //射線擊中資訊

            if (Physics.Raycast(ray, out hit, 100f, layerMask))
            {
                //Debug.DrawLine(ray.origin, hit.point, Color.black, 0.1f, false);

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Actor"))  //取得玩家
                {
                    if (hit.collider.tag == "Player")  //玩家
                    {

                        //Debug.DrawLine(ray.origin, hit.point, Color.green, 0.1f, false);
                        if (BulletNub <= 0)   //武器1 過熱
                        {
                            Reload = true; //冷卻狀態
                            overheatLock = true;   // 過熱鎖定
                            BulletNub = 0;
                            bulletAttack = 0;
                            ReLoad();
                            attacking = false;
                            //print("BulletNub =0");
                        }
                        else  //武器1 攻擊
                        {
                            LockTime = 0;
                            Fire = true;
                            attacking = true;
                            if(!overheatLock) Reload = false;  //非鎖定狀態 離開ReLoad
                            Attack();
                            //ani.SetBool("Attack", true);  //攻擊
                            //print("攻擊");
                        }
                    }
                }
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))  //玩家躲在掩體
                {
                    if (hit.collider.tag == "Crystal")  //水晶
                    {
                        //Debug.DrawLine(ray.origin, hit.point, Color.yellow, 0.4f, false);
                        HitTarget[0] = false;
                        HitTarget[1] = false;
                        LockTime += Time.deltaTime;
                        if (LockTime >= 0.1f)
                        {
                            //AttackMode = 2;   //攻擊模式2
                            LockTime = 1;
                            overheatLock = false;   // 過熱鎖定
                             //bulletAttack = 0;
                            Reload = true;
                            ReLoad();
                            //print("被擋住了");
                        }
                    }
                }
            }
            if (InAttackRange[0] && InAttackRange[1]) AttackRange = 0;  //在死角外&房間內 =0
            if (!InAttackRange[0] && InAttackRange[1]) AttackRange = 1;  //在死角內&房間內 =1
            if (InAttackRange[0] && !InAttackRange[1]) AttackRange = 1;  //在死角外&房間外 =1

            switch (AttackRange)
            {
                case 0:  //範圍內
                    if (Reload && overheatLock)  //處於冷卻狀態 並過熱鎖定
                    {
                        //AttackMode = 2;
                        //print("過熱中");
                    }
                    if (Reload && !overheatLock)  //處於冷卻狀態 並 非過熱鎖定
                    {
                        Reload = false;
                        //AttackMode = 1;  //攻擊模式1
                        //print("過熱但開火");
                    }
                    else if (!Reload)  //處於非冷卻狀態
                    {
                        //AttackMode = 1;  //攻擊模式1
                        //print("開火");
                    }
                    MA_weight += 0.5f * Time.deltaTime;
                    if (MA_weight >= 1) MA_weight = 1;
                    break;
                case 1:  //範圍外死角
                    Reload = true;
                    overheatLock = false;
                    ReLoad();
                    MA_weight -= 0.5f * Time.deltaTime;
                    if (MA_weight <= 0) MA_weight = 0f;
                    break;
            }

            MA_Rig[0].GetComponent<MultiAimConstraint>().weight = MA_weight;  //槍口連結
            MA_Rig[1].GetComponent<MultiAimConstraint>().weight = MA_weight;  //槍口連結
            //ani.SetInteger("AttackMode", AttackMode);
        }
        else
        {
            ani.SetBool("Start", false);  //進入非攻擊模式
        }
        MA_Rig[0].GetComponent<MultiAimConstraint>().weight = MA_weight;  //槍口連結

    }
    void ReLoad()  //武器冷卻
    {
        if (Reload)
        {
            ani.SetBool("Attack", false);  //不攻擊
            ReloadTime += Time.deltaTime;
            if (ReloadTime >= 2)
            {
                ReloadTime = 0;
                Reload = false;
                BulletNub = 30;
            }
            //MuzzleMaterial.SetFloat("_EmissiveExposureWeight", overheatTime);
        }
    }
    void FixedUpdate()
    {
        if (!StartAttack) return;
        if (bulletAttack >= 1)
        {
            attacking = false;
            bulletAttack = 0;
        }
        if (FlyStart[0])  //第一發子彈
        {
            if (!SureHit[0])
            {
                RigTarget[0].transform.position = Player.transform.position + new Vector3(0, 2f, 0);
                Vector3 O_origin = muzzle[0].transform.position;
                if (!HitTarget[0])
                {
                    O_targetPos[0] = RigTarget[0].transform.position;
                }
                Vector3 O_direct = O_targetPos[0] - O_origin;
                Ray O_ray = new Ray(O_origin, O_direct);
                RaycastHit O_hit = new RaycastHit(); //射線擊中資訊
                if (Physics.Raycast(O_ray, out O_hit, 100f, layerMask))
                {
                    distance[0] = Vector3.Distance(O_ray.origin, O_hit.point);  //取得與目標距離
                    if (!HitTarget[0])
                    {
                        //Debug.DrawLine(O_ray.origin, O_hit.point, Color.green, 0.1f, false);  //綠
                        FlySpeed[0] = 0;
                        SureHit[0] = true;
                    }
                    else
                    {
                        //Debug.DrawLine(O_ray.origin, O_hit.point, Color.red, 0.1f, false);  //紅
                        if (O_hit.collider.gameObject.layer == LayerMask.NameToLayer("Actor"))  //取得玩家
                        {
                            if (O_hit.collider.tag == "Player")  //玩家
                            {
                                O_hit.transform.SendMessage("Damage", power);  //造成傷害
                                O_hit.collider.gameObject.SendMessage("hit_Direction", transform); //命中方位
                            }
                        }
                        FlyStart[0] = false;
                        HitTarget[0] = false;
                    }
                }
            }
            else
            {
                if (FlySpeed[0] >= 0)
                {
                    ETA[0] = 0.0018f * distance[0];  //預計抵達時間 = ((每1距離/ 0.0018秒 ) * 目標距離 ) +開火延遲
                    FlySpeed[0] += Time.deltaTime;
                    if (FlySpeed[0] >= ETA[0])
                    {
                        FlySpeed[0] = -1;
                        HitTarget[0] = true;
                        SureHit[0] = false;
                    }
                }
            }
        }
        if (FlyStart[1])  //第二發子彈
        {
            if (!SureHit[1])
            {
                RigTarget[0].transform.position = Player.transform.position + new Vector3(0, 2f, 0);  //目標一樣
                Vector3 O_origin = muzzle[1].transform.position;
                if (!HitTarget[1])
                {
                    O_targetPos[1] = RigTarget[0].transform.position;
                }
                Vector3 O_direct = O_targetPos[1] - O_origin;
                Ray O_ray = new Ray(O_origin, O_direct);
                RaycastHit O_hit = new RaycastHit(); //射線擊中資訊
                if (Physics.Raycast(O_ray, out O_hit, 100f, layerMask))
                {
                    distance[1] = Vector3.Distance(O_ray.origin, O_hit.point);  //取得與目標距離
                    if (!HitTarget[1])
                    {
                        //Debug.DrawLine(O_ray.origin, O_hit.point, Color.black, 0.1f, false);  //黑
                        FlySpeed[1] = 0;
                        SureHit[1] = true;
                    }
                    else
                    {
                        //Debug.DrawLine(O_ray.origin, O_hit.point, Color.blue, 0.1f, false);  //藍
                        if (O_hit.collider.gameObject.layer == LayerMask.NameToLayer("Actor"))  //取得玩家
                        {
                            if (O_hit.collider.tag == "Player")  //玩家
                            {
                                O_hit.transform.SendMessage("Damage", power);  //造成傷害
                                O_hit.collider.gameObject.SendMessage("hit_Direction", transform); //命中方位
                            }
                        }
                        FlyStart[1] = false;
                        HitTarget[1] = false;
                    }
                }
            }
            else
            {
                if (FlySpeed[1] >= 0)
                {
                    ETA[1] = 0.0018f * distance[1];  //預計抵達時間 = ((每1距離/ 0.0018秒 ) * 目標距離 ) +開火延遲
                    FlySpeed[1] += Time.deltaTime;
                    if (FlySpeed[1] >= ETA[1])
                    {
                        FlySpeed[1] = -1;
                        HitTarget[1] = true;
                        SureHit[1] = false;
                    }
                }
            }
        }
    }
    private void Attack()
    {
        if (!StartAttack) return;
        //switch (AttackMode)
        //{
        //    case 1:
        //        if (BulletNub <= 18)
        //        {
        //            float overheat = 0.91f + BulletNub * (0.09f / 18);  //最小EEW + 當前子彈數 * (最小與最大EEW差值 / 子彈上限) 
        //            if (overheat <= 0.91f) overheat = 0.91f;
        //            MuzzleMaterial.SetFloat("_EmissiveExposureWeight", overheat);
        //        }
        //        //print("子彈 " + BulletNub);
        //        break;
        //    case 2:
        //        //ani.SetBool("Attack1", false);
        //        //print("攻擊2");
        //        break;
        //}
        //ani.SetInteger("AttackMode", AttackMode);

        if (Fire)
        {
            ReloadTime = 0;
            ani.SetBool("Attack", true);  //攻擊
            //print("Attack");
        }
    }
    public void AttackAning(bool attackingB, int BulletAttackNub)
    {
        attacking = attackingB;
        bulletAttack = BulletAttackNub;
    }
    void OnDisable()  //禁用時
    {
        attacking = false;
    }
}
