using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_BulletLife : MonoBehaviour
{
    public GameObject Muzzle_vfx;
    public GameObject[] Hit_vfx;  //彈孔類型
    public int HitType;
    public Transform target;
    public Transform OriTarger;
    public int aTN;
    public Vector3 Atarget;
    public float AtargetY;
    public float speed ;//飛行速度
    public float liftTime = 5f; //生命時間
    bool Ay = true;  //子彈飛行軌跡
    public Vector3 ABPath;
    Vector3 AAT;
    float FlyDistance; //慣性飛行時間
    public bool facingRight = true; //是否面向右邊
    private Vector3 moveDir = Vector3.right;
    //public LayerMask collidLayers = -1; //射線判斷的圖層，-1表示全部圖層
    public float power = 1; //子彈威力
    [TagSelector] public string[] damageTags; //要傷害的Tag
    [TagSelector] public string[] ignoreTags; //要忽略的Tag

    public float rayLength = 0.5f;  //1大約x:105-人物到槍口???射線長度??
    public float rayLength2 = 1f;
    public LayerMask[] Ground;  //射線偵測圖層

    public void Init(bool FacingRight) //初始化子彈時順便給定子彈飛行方向
    {
        facingRight = FacingRight;


        //Destroy(gameObject, liftTime); //設置生命時間到自動刪除

    }
    void Start()
    {
        speed = 60f; //飛行速度


        //if (Muzzle_vfx != null)
        //{
        //    var muzzleVFX = Instantiate(Muzzle_vfx, transform.position, Quaternion.identity);
        //    muzzleVFX.transform.forward = gameObject.transform.forward;
        //    var psMuzzle = Muzzle_vfx.GetComponent<ParticleSystem>();
        //    if (psMuzzle != null)
        //    {
        //        //Destroy(muzzleVFX, psMuzzle.main.duration);
        //    }
        //    else
        //    {
        //        var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
        //        //Destroy(muzzleVFX, psChild.main.duration);
        //    }
        //}
        int maskGround = 1 << LayerMask.NameToLayer("Ground");
        int maskMonster = 1 << LayerMask.NameToLayer("Monster");
        int maskWall = 1 << LayerMask.NameToLayer("Wall");

       // mask |= 1 << LayerMask.NameToLayer("Wall");
        //射線初始位置,射線方向
        Ray ray = new Ray(transform.position, transform.forward); //*********改****子彈位置到滑鼠點擊位置
        RaycastHit hit; //射線擊中資訊

        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        //偵測射線判斷，由 自身座標 的 前方 射出，以rayLength為長度，並且只偵測Ground圖層(記得改圖層
        //Raycast(射線初始位置, 射線方向, 儲存所碰到物件, 射線長度(沒設置。無限長), 設定忽略物件)
        if(Physics.Raycast(transform.position, fwd * 0.01f, out hit, maskGround))
        {
            HitType = 0;
            //Debug.DrawLine(transform.position, hit.point, Color.green, 0.7f, false);
            //繪出起點到射線擊中的綠色線段(起點座標,目標座標,顏色,持續時間,??)        
        }
        if (Physics.Raycast(transform.position, fwd * 0.01f, out hit, maskWall)) //彈孔噴綠
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                HitType = 0;
                //Debug.DrawLine(transform.position, hit.point, Color.black, 0.7f, false);
                hit.transform.SendMessage("Damage", power); //傷害

            }
        }
        if (Physics.Raycast(transform.position, fwd * 0.01f, out hit, maskMonster)) //彈孔噴綠
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Actor"))
            {
                HitType = 0;
                //Debug.DrawLine(transform.position, hit.point, Color.red, 0.7f, false);
                hit.transform.SendMessage("Damage", power); //傷害
            }      
            //if (hit.collider.tag == "Enemy")
            //{
            //    HitType = 3;
            //    //hit.transform.SendMessage("Damage", power);
            //    Debug.DrawLine(transform.position, hit.point, Color.blue, 0.3f, true);
            //}
        }
        //在到物體上產生彈孔
        //Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
        Vector3 pos = hit.point;

        AAT = MonsterAI02.AAT;
        
    }
    void OnDisable()
    {
        liftTime = 5;
    }
    void Update()
    {
        liftTime -= Time.deltaTime;
        FlyDistance += Time.deltaTime;
        if (liftTime <= 0)
        {
            GameObject.Find("ObjectPool").GetComponent<ObjectPool>().RecoveryM01Bullet(gameObject);
        }
    }
    void FixedUpdate()
    {
        //target = MonsterAI02.attackTarget;

        if (Ay)
        {
            AtargetY = AAT.y + 1.2f;
            Atarget = new Vector3(AAT.x, AtargetY, AAT.z);
            ABPath = Atarget - transform.position;
            ABPath = ABPath / 10;
            Ay = false;
        }
        if (AAT != Vector3.zero)
        {          
            float firstSpeed = Vector3.Distance(transform.position, Atarget);
            float orifirstSpeed = firstSpeed;
            if (firstSpeed != 0)
            {
                if (firstSpeed == orifirstSpeed)
                {
                    transform.localPosition += transform.forward * speed * Time.deltaTime;  //往前移動
                    return;
                }
                transform.position = Vector3.MoveTowards(transform.position, Atarget, speed * Time.deltaTime);
                firstSpeed = Vector3.Distance(transform.position, Atarget);      
            }
            else
            {
                  liftTime = 0;
            }
        }
        else
        {
            liftTime = 0;
        }
        //    float step = speed * Time.deltaTime;
        //transform.localPosition += ABPath * speed * Time.deltaTime ;

        //Vector3.MoveTowards(當前位置.目標位置.速度)
        //transform.localPosition = Vector3.MoveTowards(transform.localPosition, AAT, step);


        if (FlyDistance >= 0.08f)
        {
            //Ay = false;
        }
        //if (transform.position == Atarget)
        //{          
        //    GetComponent<Rigidbody>().useGravity = true;
        //}
    }

    bool InLayerMask(int layer, LayerMask layerMask) //判斷物件圖層是否在LayerMask內
    {
        return layerMask == (layerMask | (1 << layer));
    }

    private void OnCollisionEnter(Collision collision)
    {
        //ContactPoint contact = collision.contacts[0];      //在到物體上產生彈痕
        //Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        //Vector3 pos = contact.point;

        //if (Hit_vfx != null)
        //{
        //    var hixVFX = Instantiate(Hit_vfx, pos, rot);
        //    var psHit = Hit_vfx.GetComponent<ParticleSystem>();
        //    if (psHit != null)
        //    {
        //        Destroy(hixVFX, psHit.main.duration);
        //    }
        //    else
        //    {
        //        var psChild = hixVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
        //        Destroy(hixVFX, psChild.main.duration);
        //    }
        //}

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
                    collision.gameObject.SendMessage("Damage", power); //傷害
                    break; //結束迴圈
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
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
                    collision.gameObject.SendMessage("Damage", power); //傷害
                    break; //結束迴圈
                }
            }
        }
    }
}
