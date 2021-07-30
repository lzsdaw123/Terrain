﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Camera PlayCamera;

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

    public float rotationX;

    public bool inside = false;  //是否碰到梯子
    public float insideTimer;  //離開梯子時間
    public float speedUpDown = 3.2f;  //爬梯速度

    public Vector3 move;
    public float h,v;
    bool Squat = false;
    float SquatHeigh;

    public Vector3 velocity;
    public bool isGrounded;

    void Start()
    {
        insideTimer = -1;
        isGrounded = true;
        jumpHeigh = 2f;
        _rigidbody = GetComponent<Rigidbody>();
    }
    void Jump()
    {
        if (inside == false)
        {
            isGrounded = false;
            velocity.y = Mathf.Sqrt(jumpHeigh * -2 * gravity); //跳躍物理 v=√h*-2*g   
            //Debug.Log(velocity.y);
            Weapon.SetTrigger("Jump");
            Weapon.SetTrigger("Idle");
        }
    }
    void Update()  //Input用
    {
        rotationX = PlayCamera.GetComponent<MouseLook>().rotationX;

        if (inside == false)
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


            if (Input.GetButtonDown("Jump") && (isGrounded == true))   //按下跳躍
            {
                Jump();
            }

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
                GetComponent<CharacterController>().height += 0.1f;
                if (GetComponent<CharacterController>().height >= 3.1f)
                {
                    GetComponent<CharacterController>().height = 3.1f;
                }
            }


            if (Speed >= 10)
            {
                Speed = 10;
            }
            else if (Speed <= 6 && Squat == false)
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
            if (insideTimer >= 0)
            {
                insideTimer += Time.deltaTime;
            }
            if (insideTimer >= 0.8f)
            {
                _rigidbody.isKinematic = false;
                insideTimer = -1;
            }
        }
        
    }
    void FixedUpdate()  //移動用 固定偵數
    {
        //物理.球體檢查(地面檢查.位置,球體半徑,地面圖層)
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, Ground);

        if (inside == false)  //是否接觸梯子
        {
            move = transform.right * h + transform.forward * v;  //按照面對方向移動       
        }
        else
        {
            if (Input.GetKey("w"))
            {
                if (rotationX >= 60) //往下看
                {
                    if (isGrounded == false)
                    {
                        transform.position += Vector3.down / speedUpDown;
                    }
                    else
                    {
                        inside = false;
                    }
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
                    if(isGrounded == false)
                    {
                        transform.position += Vector3.down / speedUpDown;
                    }
                    else
                    {
                        inside = false;
                    }             
                }
            }
        }      
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Ladder")
        {           
            inside = true;
            insideTimer = -1;
            //transform.position += Vector3.up/2;
            _rigidbody.isKinematic = true;
        }
    }
    

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Ladder")
        {
            inside = false;
            insideTimer = 0;                  
        }
    }

}
