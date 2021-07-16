using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HeroController : MonoBehaviour
{
    enum MoveType { Type1,Type2,Type3} //列舉
    enum CameraType { Type1, Type2, Type3 }//攝影機類型
    [SerializeField] private MoveType moveType = MoveType.Type1;
    [SerializeField] private CameraType cameraType = CameraType.Type1;

    private float h, v; //移動&轉向軸

    public Animator ani; //動畫控制器
    private Vector3 targetDirection = Vector3.zero;  //移動方向
    [SerializeField] private bool smoothTurn = true; //是否平滑轉身 
    public float turnSpeed = 8f;
    [SerializeField] private bool smoothAcce = true; //是否平滑加速
    public float acceleration = 0.1f;
    [SerializeField] private bool turnByAni = false; //是否用動畫轉身

    public float JumpSpeed = 13f;
    public bool isjump=false; 
    public bool isjumptrue = false;
    public bool isfiy = false;
    public int cameratype;

    public GameObject Camera1, Camera2, Camera3, CM_vcam1;
    public float moveSpeed = 10.0f;

    public float mouseX;
    public float mouseY;
    void Start()
    {
        cameratype = 1;
        if (ani == null)
            ani = GetComponent<Animator>();

    }

    void Update()
    {
        h = Input.GetAxis("Horizontal");  //取得輸入橫軸
        v = Input.GetAxis("Vertical");    //取得輸入縱軸
            // 獲得鼠標當前位置的X和Y                                
         mouseX = Input.GetAxis("Mouse X") * moveSpeed;
         mouseY = Input.GetAxis("Mouse Y") * moveSpeed;

        float speed = 0; //移動動畫速度
        if (Input.GetKeyDown(KeyCode.R)) //切換視角
        {
            if(cameratype == 1)
            {
                cameratype ++;
                cameraType = CameraType.Type2;
            }
            else if(cameratype == 2)
            {
                cameratype ++;
                cameraType = CameraType.Type3;
            }
            else if (cameratype == 3)
            {
                cameratype = 1;
                cameraType = CameraType.Type1;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q)) //跳躍&飛行
        {
            if (isfiy == false) { isfiy = true; } 
            else { isfiy = false; }
        }
        if (isfiy == false)
        {
            if (isjump == false)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    GetComponent<Rigidbody>().velocity = new Vector3(0, 5.5f, 0);
                    GetComponent<Rigidbody>().AddForce(Vector3.up * JumpSpeed); //給剛體一個向上的力，力的大小為Vector3.up*JumpSpeed                  
                }
            }
            if (isjumptrue == true)
            {
                isjump = true;
            }
        }
        else if(isfiy==true)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                GetComponent<Rigidbody>().velocity = new Vector3(0, 5.8f, 0);
                GetComponent<Rigidbody>().AddForce(Vector3.up * JumpSpeed); //給剛體一個向上的力，力的大小為Vector3.up*JumpSpeed
            }
        }

        //移動
        switch (moveType)
        {
            case MoveType.Type1:

                targetDirection = new Vector3(h, 0, v); //算出移動的向量

                if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) //若有按下移動鍵
                {
                    speed = 1;
                    if (smoothTurn)
                    {
                        //計算要面向的平滑方向
                        Vector3 rotDir =
                        Vector3.RotateTowards(transform.forward, targetDirection, turnSpeed * Time.deltaTime, 0);
                        transform.rotation = Quaternion.LookRotation(rotDir);
                    }
                    else
                    {
                        transform.forward = targetDirection; //令角色面向移動方向
                    }
                }

                if (Input.GetButton("Walk"))
                {
                    speed *= 0.5f;
                }
                //Lerp:照比例從Ａ到Ｂ的數值
                //Toward:等速度從Ａ到Ｂ的數值
                if (smoothAcce)
                {
                    //Mathf.Lerp(原始速度,目標速度,線性比例值) 快進慢出
                    speed = Mathf.Lerp(ani.GetFloat("Speed"), speed, acceleration);
                    ani.SetFloat("Speed", speed);
                }
                else
                {
                    ani.SetFloat("Speed", speed);
                }
                break;
            case MoveType.Type2:

                Vector3 forward = Camera.main.transform.forward; //攝影機前方向量
                forward.y = 0; //變成水平向量
                forward = forward.normalized; //轉成單位向量
                Vector3 right = new Vector3(forward.z, 0, -forward.x); //右方向量

                targetDirection = forward * v + right * h; //算出移動的向量

                if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) //若有按下移動鍵
                {
                    speed = 1;
                    if (smoothTurn)
                    {
                        //計算要面向的平滑方向
                        Vector3 rotDir =
                        Vector3.RotateTowards(transform.forward, targetDirection, turnSpeed * Time.deltaTime, 0);
                        transform.rotation = Quaternion.LookRotation(rotDir);
                    }
                    else
                    {
                        transform.forward = targetDirection; //令角色面向移動方向
                    }
                }

                if (Input.GetButton("Walk"))
                {
                    speed *= 0.5f;
                }

                //Lerp:照比例從Ａ到Ｂ的數值
                //Toward:等速度從Ａ到Ｂ的數值
                if (smoothAcce)
                {
                    //Mathf.Lerp(原始速度,目標速度,線性比例值) 快進慢出
                    speed = Mathf.Lerp(ani.GetFloat("Speed"), speed, acceleration);
                    ani.SetFloat("Speed", speed);
                }
                else
                {
                    ani.SetFloat("Speed", speed);
                }
                break;
            case MoveType.Type3:

                if (Input.GetButton("Walk"))
                {
                    v *= 0.5f;
                }

                ani.SetFloat("Speed", Mathf.Lerp(ani.GetFloat("Speed"), v, acceleration));

                if (v != 0)  //若在移動中
                {
                    transform.Rotate(new Vector3(0, h * turnSpeed * Time.deltaTime, 0));
                }
                else
                {
                    ani.SetFloat("Turn", Mathf.Lerp(ani.GetFloat("Turn"), h, acceleration));

                    if (!turnByAni) //若沒有勾選動畫轉身，就要用程式碼進行轉身           
                        transform.Rotate(new Vector3(0, h * turnSpeed * Time.deltaTime, 0));
                }
                break;
            default:

                break;
        }
        switch (cameraType)
        {
            case CameraType.Type1:
                Camera1.SetActive(true);
                Camera2.SetActive(false);
                Camera3.SetActive(false);
                // 鼠標在X軸上的移動轉為主角左右的移動，同時帶動其子物體攝像機的左右移動
                transform.localRotation = transform.localRotation * Quaternion.Euler(0, mouseX, 0);
                // 鼠標在Y軸上的移動號轉為攝像機的上下運動，即是繞著X軸反向旋轉
                Camera.main.transform.localRotation = Camera.main.transform.localRotation * Quaternion.Euler(-mouseY, 0, 0);
                break;
            case CameraType.Type2:
                Camera1.SetActive(false);
                Camera2.SetActive(true);
                Camera3.SetActive(false);

                // 鼠標在X軸上的移動轉為主角左右的移動，同時帶動其子物體攝像機的左右移動
                transform.localRotation = transform.localRotation * Quaternion.Euler(0, mouseX, 0);
                // 鼠標在Y軸上的移動號轉為攝像機的上下運動，即是繞著X軸反向旋轉
                Camera.main.transform.localRotation = Camera.main.transform.localRotation * Quaternion.Euler(-mouseY, 0, 0);
                break;
            case CameraType.Type3:
                Camera1.SetActive(false);
                Camera2.SetActive(false);
                Camera3.SetActive(true);
                break;
            default:
                break;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "平台")
        {
            isjumptrue = true;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "平台")
        {
            isjump = false;
            isjumptrue = false;
        }
    }
}
