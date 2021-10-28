using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletLife : MonoBehaviour
{
    public ObjectPool pool_Hit;  //物件池
    public RectTransform HitUI;
    public Camera GunCamera;
    private Transform HIT;
    public GameObject Hit_vfx, Hit_vfx_S;  //彈孔類型
    public int HitType;  //彈孔類型變數
    public GameObject target;
    float speed = 360;//飛行速度
    public float liftTime; //生命時間
    public bool facingRight = true; //是否面向右邊
    private Vector3 moveDir = Vector3.right;
    //public LayerMask collidLayers = -1; //射線判斷的圖層，-1表示全部圖層
    public float power = 1; //子彈威力
    [TagSelector] public string[] damageTags; //要傷害的Tag
    [TagSelector] public string[] ignoreTags; //要忽略的Tag

    public float rayLength = 0.5f;  //1大約x:105-人物到槍口???射線長度??
    public LayerMask[] Ground;  //射線偵測圖層   
    public Vector3 muzzlePOS;  //槍口座標
    public GameObject Tail;
    Quaternion rot;
    Vector3 pos;
    bool YesHit;
    public LayerMask layerMask;
    public LayerMask NolayerMask;
    bool NoActor = false;

    public void Init(bool FacingRight) //初始化子彈時順便給定子彈飛行方向
    {
        facingRight = FacingRight;
    }
    void Awake()
    {
        for (int i = 0; i < Hit_vfx.transform.childCount; i++)
        {        
            Hit_vfx_S = Hit_vfx.transform.GetChild(i).gameObject;         
            Hit_vfx_S.SetActive(false);
        }
    }
    void Start()
    {
        Tail = gameObject.transform.GetChild(2).gameObject;
        pool_Hit = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        GunCamera = GameObject.Find("Gun_Camera").GetComponent<Camera>();
        HitUI = GameObject.Find("HitUI").GetComponent<RectTransform>();
        HitUI.transform.localScale = new Vector3(0f, 0f, 0f);
        liftTime = 1f;
        Hit_vfx_S = null;
        YesHit = true;     

        //if (Muzzle_vfx != null)
        //{
        //    var muzzleVFX = Instantiate(Muzzle_vfx, transform.position, Quaternion.identity);
        //    muzzleVFX.transform.forward = gameObject.transform.forward;
        //    var psMuzzle = Muzzle_vfx.GetComponent<ParticleSystem>();
        //    if (psMuzzle != null) {Destroy(muzzleVFX, psMuzzle.main.duration);}
        //    else
        //    {
        //        var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
        //        Destroy(muzzleVFX, psChild.main.duration);
        //    }
        //}
        //射線長度測試
        //if (Physics.Raycast(transform.position, fwd, out hit, rayLength2, Ground))
        //{
        //    Debug.DrawLine(transform.position, hit.point, Color.red, 0.5f, false);      
        //}
    }
    void Update()
    {
        HIT = GameObject.Find("HIT").GetComponent<Transform>();

        liftTime -= Time.deltaTime;
        if (liftTime <= 0.8f)
        {
            Tail.GetComponent<TrailRenderer>().emitting = true;  //開啟尾巴軌跡
            Tail.GetComponent<TrailRenderer>().time = 0.5f; ;  //開啟尾巴軌跡
        }
        if (liftTime <= 0) 
        {         
            liftTime = 0;
            DestroyGameObject();          
        }
        if (gameObject.activeSelf)
        {           
            if (YesHit == true)
            {
                YesHit = false;
                //判斷圖層
                //int maskGround = 1 << LayerMask.NameToLayer("Ground");
                //int maskMonster = 1 << LayerMask.NameToLayer("Monster");
                //int maskWall = 1 << LayerMask.NameToLayer("Wall");
                //LayerMask layerMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Monster")) |
                //    (1 << LayerMask.NameToLayer("Wall")) | (0 << LayerMask.NameToLayer("Actor"));
                //LayerMask layerMask = (1 << 9) | (1 << 10) | (1 << 11) | (0 << 13); //Ground,Monster,Wall

                //由攝影機射到是畫面正中央的射線
                Ray ray = GunCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));            
                RaycastHit hit; //射線擊中資訊

                //Vector3 fwd = transform.TransformDirection(Vector3.forward);
                //偵測射線判斷，由 自身座標 的 前方 射出，以rayLength為長度，並且只偵測Ground圖層(記得改圖層
                //Raycast(射線初始位置, 射線方向, 儲存所碰到物件, 射線長度(沒設置。無限長), 設定忽略物件)
                if (Physics.Raycast(ray, out hit, layerMask)) //擊中牆壁
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))  //彈孔噴黑煙
                    {
                        HitType = 0;
                        //Debug.DrawLine(ray.origin, hit.point, Color.black, 0.7f, false);
                    }
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                    {
                        HitType = 0;
                        //繪出起點到射線擊中的綠色線段(起點座標,目標座標,顏色,持續時間,??)      
                        //Debug.DrawLine(ray.origin, hit.point, Color.green, 0.7f, false);
                    }
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))  //彈孔噴紅血
                    {
                        HitType = 1;
                        //Debug.DrawLine(ray.origin, hit.point, Color.red, 0.7f, false);
                        //hit.transform.SendMessage("Damage", power); //傷害
                        if (hit.collider.tag == "Enemy")  //綠血
                        {
                            HitUI.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                            HitUI.transform.localScale -= new Vector3(0.5f, 0.5f, 0.5f);
                            HitType = 2;
                            hit.transform.SendMessage("Damage", power);
                            //Debug.DrawLine(ray.origin, hit.point, Color.blue, 0.3f, true);
                        }
                    }
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Actor"))
                    {
                        NoActor = true;
                    }
                }

                //if (Physics.Raycast(ray, out hit, 1 << 10)) //擊中怪物
                //{
                //    if (hit.collider.gameObject.layer == 1 << 10)  //彈孔噴紅血
                //    {
                //        HitType = 1;
                //        print(layerMask.value + "HitType+" + HitType);
                //        //Debug.DrawLine(ray.origin, hit.point, Color.red, 0.7f, false);
                //        //hit.transform.SendMessage("Damage", power); //傷害                  
                //    }
                //    if (hit.collider.tag == "Enemy")  //綠血
                //    {
                //        HitType = 2;
                //        print(layerMask.value + "+HitType+" + HitType);
                //        //hit.transform.SendMessage("Damage", power);
                //        //Debug.DrawLine(ray.origin, hit.point, Color.blue, 0.3f, true);
                //    }
                //}              
                //在到物體上產生彈孔
                rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
                pos = hit.point;             
                if (NoActor)
                {
                    pos = HIT.transform.position;
                    pool_Hit.ReUseHit(pos, rot, HitType);;  //從彈孔池取出彈孔
                    liftTime = 0;
                    return;
                }
                else
                {
                    pool_Hit.ReUseHit(pos, rot, HitType);  //從彈孔池取出彈孔
                }
            }
        }
    }
    void OnDisable()
    {       
        YesHit = true;
        HitType = 0;
        liftTime = 1f;      
    }
    void DestroyGameObject()
    {
        Tail.GetComponent<TrailRenderer>().emitting = false;  //關閉尾巴軌跡        
        Tail.GetComponent<TrailRenderer>().time = 0f;  //關閉尾巴軌跡        
        //回收物件
        GameObject.Find("ObjectPool").GetComponent<ObjectPool>().Recovery(gameObject);       
        HitUI.transform.localScale = new Vector3(0f, 0f, 0f);
    }
    void FixedUpdate()
    {
        if (pos != Vector3.zero)
        {
            float firstSpeed = Vector3.Distance(transform.position, pos);
            if (firstSpeed != 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, pos, speed * Time.deltaTime);
            }
            else
            {
                DestroyGameObject();
            }          
        }
        else
        {
            transform.position += transform.forward * speed * Time.deltaTime;  //往前移動
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
            liftTime = 0;
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        //若碰撞體在作用圖層內才進行運算
        if (InLayerMask(collider.gameObject.layer, Ground[1]))
        {
            NoActor = true;
        }
        if (InLayerMask(collider.gameObject.layer, Ground[0]))
        {
            liftTime = 0;
            for (int i = 0; i < ignoreTags.Length; i++)
            {
                if (collider.gameObject.tag == ignoreTags[i])
                {
                    return; //若對象在忽略Tag，則直接返回不做任何處理
                }
            }
            for (int i = 0; i < damageTags.Length; i++)
            {
                if (collider.gameObject.tag == damageTags[i])
                {
                    collider.gameObject.SendMessage("Damage", power); //傷害
                    break; //結束迴圈
                }
            }
            
        }
    }
}
