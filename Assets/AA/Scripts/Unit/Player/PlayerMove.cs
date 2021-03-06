using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Camera PlayCamera;  //原Camera
    public CharacterController controller;
    public Shooting _Shooting;
    public HeroLife heroLife;

    private Rigidbody _rigidbody;
    public static float Speed=6.5f;
    [SerializeField] float SF_Speed;
    public static float MoveSpeed=60f;
    public float gravity = -9.81f; //重力
    public float jumpHeigh; // 跳躍高度

    public Animator Weapon;   //動畫控制器
    public GameObject[] _Animator;
    public int WeaponType; //武器類型
    public GameObject Gun;

    public Transform groundCheck;    //地面檢查
    public Transform SquatCheck;      //蹲下檢查
    public float groundDistance = 0.6f;  //地面判定球體半徑
    public float SquatDistance = 0.5f;  //頭頂判定球體半徑
    public LayerMask Ground, Ceiling;       //地面圖層
    public CollisionFlags m_CollisionFlags;  //碰撞提醒
    public bool m_Jump;  //是否跳躍
    [SerializeField] bool isAir;  //是否跳躍
    public static bool m_Jumping;  //跳躍中
    public bool isVehicle;  //在載具中
    float rotationX;
    public static int Metal;

    public bool inside = false;  //是否碰到梯子
    public float insideTimer;  //離開梯子時間
    float speedUpDown = 3.2f;  //爬梯速度

    public Vector3 move;
    public static float Player_h,Player_v;  //人物位移
    public static bool Squat = false;

    public Vector3 velocity;
    public bool isGrounded;  //在地面上
    public bool isSquat; //正在蹲下
    public bool UpSquat;
    public bool NoMove;
    public Vector3 NoM_V3;

    void Start()
    {
        insideTimer = -1;
        jumpHeigh = 2.5f;
        _rigidbody = GetComponent<Rigidbody>();
        isVehicle = false;
        m_Jumping = false;
        GetComponent<CharacterController>().height = 3.1f;
        UpSquat = true;
    }
    void Update()  //Input用
    {
        SF_Speed = Speed;
        if (Time.timeScale == 0) { return; }
        rotationX = PlayCamera.GetComponent<MouseLook>().rotationX;

        if (inside == false)
        {
            WeaponType = Shooting.WeaponType;
            Weapon = _Animator[WeaponType].GetComponent<Animator>();

            if (!NoMove)
            {
                Player_h = Input.GetAxis("Horizontal");  //取得輸入橫軸
                Player_v = Input.GetAxis("Vertical");    //取得輸入縱軸      
            }
            else
            {
                Player_h = 0;
                Player_v = 0;
                transform.position = new Vector3(NoM_V3.x, transform.position.y, NoM_V3.z);
            }

            //if (Input.GetButtonDown("Jump") && !m_Jump && Shooting.Reload==false && !m_Jumping)   //按下跳躍
            //{
            //    m_Jump = true;
            //    //m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            //}
            if (!isGrounded && controller.isGrounded)
            {
                if (m_Jumping && velocity.y != -2)
                {
                    if (Metal==0)
                    {
                        AudioManager.PlayJumpAudio(0);
                    }
                    else
                    {
                        AudioManager.PlayJumpAudio(1);
                    }                 
                    //print("落地");
                    isAir = true;
                }
                isAir = false;
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
                if (_Shooting.LayDown)
                {
                    Speed = 4f;
                }
                else
                {
                    Speed = 3f;
                }                
                Squat = true;
                GetComponent<CharacterController>().height = 1.6f;
                Gun.transform.localPosition = new Vector3(0,2.29f,0.089f);  //Gun的本地座標修正
            }
            if (Input.GetButtonUp("Squat"))
            {
                isSquat = Physics.CheckSphere(SquatCheck.position, SquatDistance, Ceiling);
                if (isSquat)  //判斷頭頂是否有障礙物
                {
                    Squat = true;
                    UpSquat = true;
                    //if (_Shooting.LayDown)
                    //{
                    //    Speed = 4f;
                    //}
                    //else
                    //{
                    //    Speed = 3f;
                    //}
                    //Squat = true;
                    //GetComponent<CharacterController>().height = 1.6f;
                    //Gun.transform.localPosition = new Vector3(0, 2.29f, 0.089f);
                }
                else
                {
                    Squat = false;
                    UpSquat=false;
                    Gun.transform.localPosition = new Vector3(0, 2.865f, 0.089f);
                    GetComponent<CharacterController>().height += 2f;
                    if (GetComponent<CharacterController>().height >= 3.1f)
                    {
                        GetComponent<CharacterController>().height = 3.1f;
                    }
                }
            }
            if(!isSquat && UpSquat)
            {
                Squat = UpSquat = false;
                Gun.transform.localPosition = new Vector3(0, 2.865f, 0.089f);
                GetComponent<CharacterController>().height += 2f;
                if (GetComponent<CharacterController>().height >= 3.1f)
                {
                    GetComponent<CharacterController>().height = 3.1f;
                }
            }
       
            if (_Shooting.LayDown)
            {
                if (Speed >= 13f)
                {
                    Speed = 13f;
                }
                else if (Speed <= 6.5f && !Squat)
                {
                    Speed = 6.5f;
                }
            }
            else
            {
                if (Speed >= 11f )
                {
                    Speed = 11f;
                }
                else if (Speed <= 6.5f && !Squat)
                {
                    Speed = 6.5f;
                }
            }
            if (Speed <= 3) Speed = 3;
            if ((Player_v != 0) || (Player_h != 0))  //人物移動
            {
                Weapon.SetBool("Move", true);             
                if (Input.GetButton("Run")&& Shooting.Reload!=true)    //人物跑動
                {
                    if (Input.GetButton("Fire1"))
                    {
                        Weapon.SetBool("Move", false);          
                        Speed -= 0.6f;
                    }
                    if (Input.GetButton("Fire2"))
                    {
                        Weapon.SetBool("AimMove", true);
                        Speed -= 0.6f;
                    }
                    else
                    {
                        Weapon.SetBool("AimMove", false);
                        Speed += 0.4f;
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
                    Speed -= 0.6f;
                }
                float moveSpeed = Speed - heroLife.Level * 1.4f;  //水晶感染影響速度
                if (moveSpeed <= 0) moveSpeed = 0;
                Weapon.SetFloat("Speed", moveSpeed);
                controller.Move(move * moveSpeed * Time.deltaTime);  //移動
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
        if (Input.GetButtonDown("Jump") && !m_Jump && Shooting.Reload == false && !m_Jumping)   //按下跳躍
        {
            m_Jump = true;
            //m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            Vector3 fwd = transform.TransformDirection(Vector3.down);
            RaycastHit hit; //射線擊中資訊
            if (Physics.Raycast(groundCheck.position, fwd, out hit, 1f))
            {
                isGrounded = true;
            }
        }

        groundDistance = 1.2f;
        //物理.球體檢查(地面檢查.位置,球體半徑,地面圖層)
        //isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, Ground);
        isSquat = Physics.CheckSphere(SquatCheck.position, SquatDistance, Ceiling);      
        if (controller.isGrounded || isGrounded)
        {
            if (m_Jump)
            {
                m_Jump = false;
                m_Jumping = true;               
                velocity.y = Mathf.Sqrt(jumpHeigh * -2 * gravity); //跳躍物理 v=√h*-2*g
                Weapon.SetTrigger("Jump");
                if (!_Shooting.LayDown)
                {
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

        if (!isVehicle)
        {
            velocity.y += gravity * 1.1f * Time.deltaTime;  //重力物理
        }
        float Vy = velocity.y;
        int Dhp;  //墜落傷害
        if (isGrounded && velocity.y < 0)
        {
            if (Vy <= -20)  //墜落傷害
            {
                Dhp = (int)Vy * -1 - 20;
                HeroLife.DownDamage(Dhp /2);
            }
            velocity.y = -2f;
        }
        if (inside == false)  //是否接觸梯子
        {
            if (!NoMove)
            {
                move = transform.right * Player_h + transform.forward * Player_v * MoveSpeed * Time.deltaTime;  //按照面對方向移動       
            }
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
        if (hit.collider.tag == "Metal")  //金屬
        {
            Metal = 1;
        }
        else
        {
            Metal = 0;
        }
        //if (m_CollisionFlags == CollisionFlags.Below)
        //{
        //    return;
        //}

        //if (body == null || body.isKinematic)
        //{
        //    return;
        //}
        //body.AddForceAtPosition(controller.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }
}
