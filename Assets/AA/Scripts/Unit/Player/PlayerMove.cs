using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Camera PlayCamera;
    public CharacterController controller;
    public Shooting _Shooting;

    private Rigidbody _rigidbody;
    public float Speed=6.5f;
    public float gravity = -9.81f; //重力
    public float jumpHeigh; // 跳躍高度

    public Animator Weapon;   //動畫控制器
    public GameObject[] _Animator;
    public int n, m; //武器種類
    public GameObject Gun;

    public Transform groundCheck;    //地面檢查
    public Transform SquatCheck;      //蹲下檢查
    public float groundDistance = 0.6f;  //地面判定球體半徑
    public float SquatDistance = 0.5f;  //頭頂判定球體半徑
    public LayerMask Ground, Ceiling;       //地面圖層
    public CollisionFlags m_CollisionFlags;  //碰撞提醒
    public bool m_Jump;  //是否跳躍
    public static bool m_Jumping;  //跳躍中
    float rotationX;

    public bool inside = false;  //是否碰到梯子
    public float insideTimer;  //離開梯子時間
    float speedUpDown = 3.2f;  //爬梯速度

    public Vector3 move;
    public static float h,v;
    bool Squat = false;

    public Vector3 velocity;
    public bool isGrounded;  //在地面上
    public bool isSquat; //正在蹲下

    void Start()
    {
        insideTimer = -1;
        jumpHeigh = 2.5f;
        _rigidbody = GetComponent<Rigidbody>();

        m_Jumping = false;
    }
    void Update()  //Input用
    {
        if (Time.timeScale == 0) { return; }
        rotationX = PlayCamera.GetComponent<MouseLook>().rotationX;

        if (inside == false)
        {
            n = GetComponent<Shooting>().n;
            m = GetComponent<Shooting>().m;
            Weapon = _Animator[n].GetComponent<Animator>();


            h = Input.GetAxis("Horizontal");  //取得輸入橫軸
            v = Input.GetAxis("Vertical");    //取得輸入縱軸            

            if (Input.GetButtonDown("Jump") && !m_Jump && Shooting.Reload==false && isGrounded)   //按下跳躍
            {
                m_Jump = true;
                //m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
            if (!isGrounded && controller.isGrounded)
            {
                //PlayLandingSound();
                move.y = 0f;
                m_Jumping = false;                
            }
            if (!controller.isGrounded && !m_Jumping && isGrounded)
            {
                move.y = 0f;
            }
            isGrounded = controller.isGrounded;  //是否接觸地面


            if (Input.GetButton("Squat"))  //蹲下
            {
                Speed = 3f;
                Squat = true;
                GetComponent<CharacterController>().height = 1.6f;
                Gun.transform.localPosition = new Vector3(0,2.29f,0.089f);  //Gun的本地座標修正
            }
            else if(isSquat)  //判斷頭頂是否有障礙物
            {
                Speed = 3f;
                Squat = true;
                GetComponent<CharacterController>().height = 1.6f;
                Gun.transform.localPosition = new Vector3(0, 2.29f, 0.089f);
            }
            else
            {
                Squat = false;
                Gun.transform.localPosition = new Vector3(0, 2.865f, 0.089f);
                GetComponent<CharacterController>().height += 0.2f;
                if (GetComponent<CharacterController>().height >= 3.1f)
                {
                    GetComponent<CharacterController>().height = 3.1f;
                }
            }

            if (Speed >= 10)
            {
                Speed = 10;
            }
            else if (Speed <= 6.5f && Squat == false)
            {
                Speed = 6.5f;
            }

            if ((v != 0) || (h != 0))
            {
                Weapon.SetBool("Move", true);             
                if (Input.GetButton("Run")&& Shooting.Reload!=true)    //人物跑動
                {                   
                    if (Input.GetButton("Fire2"))
                    {
                        Weapon.SetBool("AimMove", true);
                        Speed -= 0.2f;
                    }
                    else
                    {
                        Weapon.SetBool("AimMove", false);
                        Speed += 0.2f;
                    }                                
                }            
                else
                {
                    if (Input.GetButton("Fire2"))
                    {
                        Weapon.SetBool("AimMove", true);
                    }
                    else
                    {
                        Weapon.SetBool("AimMove", false);
                    }
                    Speed -= 0.2f;
                }             
                Weapon.SetFloat("Speed", Speed);
                controller.Move(move * Speed * Time.deltaTime);
                //AudioManager.PlayFootstepAudio();
            }
            else
            {
                Weapon.SetBool("Move", false);
                Weapon.SetBool("AimMove", false);
            }
            if (insideTimer >= 0)
            {
                insideTimer += Time.deltaTime;
            }
            if (insideTimer >= 0.1f)
            {
                insideTimer = -1;
                inside = false;
            }
        }
    }


    void FixedUpdate()  //移動用 固定偵數
    {
        //物理.球體檢查(地面檢查.位置,球體半徑,地面圖層)
        //isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, Ground);
        isSquat = Physics.CheckSphere(SquatCheck.position, SquatDistance, Ceiling);      
        if (controller.isGrounded)
        {
            if (m_Jump)
            {
                m_Jump = false;
                m_Jumping = true;               
                velocity.y = Mathf.Sqrt(jumpHeigh * -2 * gravity); //跳躍物理 v=√h*-2*g
                if (!_Shooting.LayDown)
                {
                    Weapon.SetTrigger("Jump");
                    if (Input.GetButton("Fire2"))
                    {
                        Weapon.SetBool("Aim", true);
                    }
                    else
                    {
                        Weapon.SetTrigger("Idle");
                    }
                }              
            }
        }
        m_CollisionFlags = controller.Move(move * Time.fixedDeltaTime);
        velocity.y += gravity * Time.deltaTime;  //重力物理      

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if (inside == false)  //是否接觸梯子
        {           
            move = transform.right * h + transform.forward * v;  //按照面對方向移動       
            controller.Move(velocity * Time.deltaTime); //執行跳躍            
        }
        else
        {
            if (Input.GetKey("w"))
            {
                if (rotationX >= 60) //往下看
                {
                    transform.position += Vector3.down / speedUpDown;
  
                }
                else  //往上看
                {
                    transform.position += Vector3.up / speedUpDown;
                }               
            }
            if (Input.GetKey("s"))
            {
                if (rotationX >= 60)  //往下看
                {
                    transform.position += Vector3.up / speedUpDown;                 
                }
                else  //往上看
                {
                    transform.position += Vector3.down / speedUpDown;
                    if ((controller.collisionFlags & CollisionFlags.Below) != 0)
                    {
                        insideTimer = 0;
                    }
                }
            }         
        }
    }
    void OnTriggerEnter(Collider col)  //觸碰梯子
    {
        if (col.gameObject.tag == "Ladder")
        {
            inside = true;
            move = Vector3.zero;
            Weapon.SetBool("Move", false);
        }
    }
    void OnTriggerExit(Collider col)  //離開梯子
    {
        if (col.gameObject.tag == "Ladder")
        {
            inside = false;
            Weapon.SetBool("Move", true);
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)//角色碰撞偵測
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        //dont move the rigidbody if the character is on top of it
        if (m_CollisionFlags == CollisionFlags.Below)
        {
            return;
        }

        if (body == null || body.isKinematic)
        {
            return;
        }
        body.AddForceAtPosition(controller.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }

}
