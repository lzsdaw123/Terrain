using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    private float h, v; //移動&轉向軸

    public Animator ani; //動畫控制器
    private CharacterController m_CharacterController; //角色控制器
    private Vector2 m_Input;
    private Vector3 targetDirection = Vector3.zero;  //移動方向
    [SerializeField] private float m_JumpSpeed;
    [SerializeField] private float m_StickToGroundForce; //堅持地面力量
    [SerializeField] private float m_GravityMultiplier;  //重力倍增

    private bool m_Jump;
    private bool m_Jumping;

    [SerializeField] private bool smoothTurn = true; //是否平滑轉身 
    public float turnSpeed = 8f;
    [SerializeField] private bool smoothAcce = true; //是否平滑加速
    public float acceleration = 0.1f;

    public float moveSpeed = 10.0f;
    public Transform playerBody;

    //方向靈敏度 
    public float sensitivityX = 0F;
    public float sensitivityY = 0F;
    public float sensitive = 0.5f;
    //上下最大視角(Y視角) 
    public float minimumY = -60F;
    public float maximumY = 60F;
    float rotationX = 0f;
    float rotationY = 0f;

    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        h = Input.GetAxis("Horizontal");  //取得輸入橫軸
        v = Input.GetAxis("Vertical");    //取得輸入縱軸  
   
        // 獲得鼠標當前位置的X和Y                                
        float mouseX = Input.GetAxis("Mouse X") * moveSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * moveSpeed * Time.deltaTime;

        playerBody.Rotate(Vector3.up * mouseX);

        float speed = 0; //移動動畫速度

        //// 鼠標在X軸上的移動轉為主角左右的移動，同時帶動其子物體攝像機的左右移動
        //transform.localRotation = transform.localRotation * Quaternion.Euler(0, mouseX, 0);
        //// 鼠標在Y軸上的移動轉為攝像機的上下運動，即是繞著X軸反向旋轉
        //Camera.main.transform.localRotation = Camera.main.transform.localRotation * Quaternion.Euler(-mouseY, 0, 0);
        

        //根據鼠標移動的快慢(增量), 獲得相機上下旋轉的角度(處理Y) 
        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        //角度限制. rotationY小於min,返回min. 大於max,返回max. 否則返回value 
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        
        Vector3 forward = Camera.main.transform.forward; //攝影機前方向量
        forward.y = 0; //變成水平向量
        forward = forward.normalized; //轉成單位向量
        Vector3 right = new Vector3(forward.z, 0, -forward.x); //右方向量
       
        targetDirection = forward * v + right * h; //算出移動的向量

        var worldDirection = transform.TransformDirection(targetDirection);   //轉換世界方向

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) //若有按下移動鍵
        {
            speed = 4;
            Vector3 target = transform.position + new Vector3(h, 0, v); //獲取以角色自身位置為基準加上偏移的位置
            transform.LookAt(target);                                                     //讓角色面向朝向要移動的目標點
            transform.Translate(Vector3.forward * Time.deltaTime * speed);

            //if (smoothTurn)
            //{
            //    //計算要面向的平滑方向
            //    Vector3 rotDir =
            //    Vector3.RotateTowards(transform.forward, targetDirection, turnSpeed * Time.deltaTime, 0);
            //    transform.rotation = Quaternion.LookRotation(rotDir);
            //}
            //else
            //{
            //    transform.forward = targetDirection; //令角色面向移動方向
            //}
        }

        if (Input.GetButton("Walk"))
        {
            speed *= 0.5f;
        }
        


        //Lerp:照比例從Ａ到Ｂ的數值
        //Toward:等速度從Ａ到Ｂ的數值
        //if (smoothAcce)
        //{
        //    //Mathf.Lerp(原始速度,目標速度,線性比例值) 快進慢出
        //    speed = Mathf.Lerp(ani.GetFloat("Speed"), speed, acceleration);
        //    ani.SetFloat("Speed", speed);
        //}
        //else
        //{
        //    ani.SetFloat("Speed", speed);
        //}
    }
    private void FixedUpdate()
    {
        float speed=0;

        //始終沿相機向前移動，因為它是瞄準的方向
        //Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

        //獲得被觸摸以沿其移動的表面的法線
        //RaycastHit hitInfo;
        //Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
        //                   m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        //desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        //targetDirection.x = desiredMove.x * speed;
        //targetDirection.z = desiredMove.z * speed;


        //if (m_CharacterController.isGrounded)
        //{
        //    targetDirection.y = -m_StickToGroundForce;

        //    if (m_Jump)
        //    {
        //        targetDirection.y = m_JumpSpeed;
        //        m_Jump = false;
        //        m_Jumping = true;
        //    }
        //}
        //else
        //{
        //    targetDirection += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
        //}

    }
    //private void GetInput()
    //{
    //    float horizontal = Input.GetAxis("Horizontal");
    //    float vertical = Input.GetAxis("Vertical");
    //}



}
