using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletLife : MonoBehaviour
{
    public ObjectPool pool_Hit;  //物件池
    public Camera GunCamera;
    private Transform HIT;
    public GameObject Hit_vfx, Hit_vfx_S;  //彈孔類型
    public int HitType;  //彈孔類型變數
    public GameObject target;
    public float speed = 600;//飛行速度
    public float liftTime; //生命時間
    public bool facingRight = true; //是否面向右邊
    //public LayerMask collidLayers = -1; //射線判斷的圖層，-1表示全部圖層
    public float power = 1; //子彈威力
    [TagSelector] public string[] damageTags; //要傷害的Tag
    [TagSelector] public string[] ignoreTags; //要忽略的Tag

    public float rayLength = 0.5f;  //1大約x:105-人物到槍口???射線長度??
    public LayerMask[] Ground;  //射線偵測圖層   
    public Vector3 muzzlePOS;  //槍口座標
    public ParticleSystem Tail;
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
        //var main = Tail.main;
        //main.simulationSpeed = 3.7f;  //加快粒子開始播放時間
    }
    void Start()
    {
        pool_Hit = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        GunCamera = GameObject.Find("Gun_Camera").GetComponent<Camera>();
        liftTime = 1f;
        Hit_vfx_S = null;
        YesHit = true;
        //射線長度測試
        //if (Physics.Raycast(transform.position, fwd, out hit, rayLength2, Ground))
        //{
        //    Debug.DrawLine(transform.position, hit.point, Color.red, 0.5f, false);      
        //}
    }
    void Update()
    {
        //HIT = GameObject.Find("HIT").GetComponent<Transform>();

        liftTime -= Time.deltaTime;
        if (liftTime <= 0) 
        {         
            liftTime = 0;
            DestroyGameObject();          
        }
        if (YesHit)
        {
            YesHit = false;
            //判斷圖層
            //LayerMask layerMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Monster")) |
            //    (1 << LayerMask.NameToLayer("Wall")) | (0 << LayerMask.NameToLayer("Actor"));
            //LayerMask layerMask = (1 << 9) | (1 << 10) | (1 << 11) | (0 << 13); //Ground,Monster,Wall

            //由攝影機射到是畫面正中央的射線
            //Ray ray = GunCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            //RaycastHit hit; //射線擊中資訊
            //Raycast(射線初始位置, 射線方向, 儲存所碰到物件, 射線長度(沒設置。無限長), 設定忽略物件)
            //if (Physics.Raycast(ray, out hit, layerMask)) //擊中牆壁
            //{
            //    print("擊中");
            //    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))  //彈孔噴黑煙
            //    {
            //        HitType = 0;
            //        //Debug.DrawLine(ray.origin, hit.point, Color.black, 0.7f, false);
            //    }
            //    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            //    {
            //        HitType = 0;
            //        //繪出起點到射線擊中的綠色線段(起點座標,目標座標,顏色,持續時間,??)      
            //        //Debug.DrawLine(ray.origin, hit.point, Color.green, 0.7f, false);                        
            //    }
            //    if (hit.collider.tag == "Metal")
            //    {
            //        HitType = 3;
            //        AudioManager.Hit(0);
            //        //Debug.DrawLine(ray.origin, hit.point, Color.blue, 0.3f, false);
            //    }
            //    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))  //彈孔噴紅血
            //    {
            //        HitType = 1;
            //        //Debug.DrawLine(ray.origin, hit.point, Color.red, 0.7f, false);
            //        //hit.transform.SendMessage("Damage", power); //傷害
            //        if (hit.collider.tag == "Enemy")  //綠血
            //        {
            //            HitUI.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            //            HitUI.transform.localScale -= new Vector3(0.5f, 0.5f, 0.5f);
            //            HitType = 2;
            //            hit.transform.SendMessage("Damage", power);
            //            //Debug.DrawLine(ray.origin, hit.point, Color.blue, 0.3f, true);
            //        }
            //    }
            //    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Actor"))
            //    {
            //        if (hit.collider.tag != "MissionTarget")
            //        {
            //            NoActor = true;
            //        }
            //    }
            //}
            ////在到物體上產生彈孔
            //rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            //pos = hit.point;
            //if (NoActor && pos != Vector3.zero)
            //{
            //    NoActor = false;
            //    pos = HIT.transform.position;
            //    pool_Hit.ReUseHit(pos, rot, HitType); ;  //從彈孔池取出彈孔
            //    liftTime = 0;
            //}
            //else
            //{                
            //    pool_Hit.ReUseHit(pos, rot, HitType);  //從彈孔池取出彈孔
            //    //print("彈孔" + HitType);
            //}
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
        //回收物件
        GameObject.Find("ObjectPool").GetComponent<ObjectPool>().Recovery(gameObject);       
        //HitUI.transform.localScale = new Vector3(0f, 0f, 0f);
    }
    void FixedUpdate()
    {
        if (pos != Vector3.zero)
        {
            float firstSpeed = Vector3.Distance(transform.position, pos);
            float orifirstSpeed = firstSpeed;
            if (firstSpeed != 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, pos, speed * Time.deltaTime);
                firstSpeed = Vector3.Distance(transform.position, pos);
                if (firstSpeed == orifirstSpeed)
                {
                    DestroyGameObject();
                }
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
