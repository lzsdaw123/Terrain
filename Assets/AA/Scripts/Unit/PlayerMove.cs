using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public CharacterController controller;
    private Rigidbody _rigidbody;
    public float Speed=4f;
    public float gravity = -9.81f; //重力
    public float jumpHeigh; // 跳躍高度

    public Animator Weapon;   //動畫控制器
    public GameObject[] _Animator;
    public int n, m; //武器種類

    public Transform groundCheck;      //地面檢查
    public GameObject AgroundCheck;
    public float groundDistance = 0.4f;  //球體半徑
    public LayerMask Ground;       //地面圖層
    public float margin = 0.1f;

    public Vector3 move;
    public float h,v;
    bool Squat = false;
    float SquatHeigh;

    public Vector3 velocity;
    public bool isGrounded;

    void Start()
    {
        isGrounded = true;
        jumpHeigh = 2f;
    }
    void Jump()
    {
        //velocity.y = -2f;
        isGrounded = false;   
        velocity.y = Mathf.Sqrt(jumpHeigh * -2 * gravity); //跳躍物理 v=√h*-2*g   
        Debug.Log(velocity.y);
        Weapon.SetTrigger("Jump");
        Weapon.SetTrigger("Idle");

    }
    void Update()  //Input用
    {
        n = GetComponent<Shooting>().n;
        m = GetComponent<Shooting>().m;
        Weapon = _Animator[n].GetComponent<Animator>();

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        h = Input.GetAxis("Horizontal");  //取得輸入橫軸
         v = Input.GetAxis("Vertical");    //取得輸入縱軸            

 
        if (Input.GetButtonDown("Jump")&&(isGrounded==true))   //按下跳躍
        {     
            Jump();        
        }

        //射線偵測groundCheck與地面距離，如果小於0.075f(測試出來) 能跳
        //Ray ray = new Ray(groundCheck.position, -groundCheck.up);      
        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit, Ground))
        //{
        //    Vector3 A = groundCheck.position;
        //    Vector3 B = hit.point;
        //    //Debug.Log(A.y+"/"+B.y);
        //    float distance = (A.y - B.y);
        //    //Debug.Log(distance);
        //    if(distance > 0.075f)
        //    {
        //        Debug.DrawLine(groundCheck.position, hit.point, Color.red, 1f, false);
        //        isGrounded = false;
        //    }else if (distance <= 0.075f)
        //    {
        //        Debug.DrawLine(groundCheck.position, hit.point, Color.green, 1f, false);
        //        isGrounded = true;
        //    }              
        //}
        velocity.y += gravity * Time.deltaTime;  //重力物理
        controller.Move(velocity * Time.deltaTime); //執行跳躍

        if (Input.GetButton("Squat"))  //蹲下
        {
            Speed = 1.5f;
            Squat = true;
            GetComponent<CharacterController>().height = 1.5f;
        }
        else
        {
            Squat = false;
            GetComponent<CharacterController>().height+=0.1f;
            if (GetComponent<CharacterController>().height >= 3.1f)
            {
                GetComponent<CharacterController>().height = 3.1f;
            }
        }


        if (Speed >= 10)
        {
            Speed = 10;
        }
        else if (Speed <= 6 && Squat==false)
        {
            Speed = 6;
        }

        
        if ((v != 0) || (h != 0))
        {

            Weapon.SetBool("Move", true);
            
            if (Input.GetButton("Run") && v > 0.5f)    //人物移動
            {
                Speed += 0.2f;
                
                controller.Move(move * Speed * Time.deltaTime);
                Weapon.SetFloat("Speed", Speed);

            }
            else if (Input.GetButton("Fire2"))
            {
                Speed -= 0.2f;
                controller.Move(move * Speed * Time.deltaTime);
                Weapon.SetBool("Move", false);
                Weapon.SetBool("AimMove", true);

            }
            else
            {
                Speed -= 0.2f;
                controller.Move(move * Speed * Time.deltaTime);
                Weapon.SetFloat("Speed", Speed);
                Weapon.SetBool("AimMove", false);
            }
        }
        else
        {
            Weapon.SetBool("Move", false);
            Weapon.SetBool("AimMove", false);
        }
      

    }
    void FixedUpdate()  //移動用 固定偵數
    {
        move = transform.right * h + transform.forward * v;  //按照面對方向移動
        

        //物理.球體檢查(地面檢查.位置,球體半徑,地面圖層)
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, Ground);
    }

}
