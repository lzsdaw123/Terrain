﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterLife : MonoBehaviour
{


    [SerializeField] Transform root;

    private Animator ani;
    private Rigidbody rigid;
    private Collider cld;

    public float hpFull = 5; // 血量上限
    public float hp; // 血量
    public Image hpImage;

    private NavMeshAgent agent;
    private MonsterAI01 monster01;


    void Start()
    {
        hp = hpFull; // 遊戲一開始先補滿血量
        RefreshLifebar(); // 更新血條
        ani = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        cld = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
        monster01 = GetComponent<MonsterAI01>();
        RagdollActive(false); // 先關閉物理娃娃

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y)) // 測試用
        {
            monster01.enabled = false; // 關閉控制腳本
            agent.enabled = false;  //立即關閉尋徑功能
            ani.SetTrigger("Die");
        }
    }
    // 開啟或關閉物理娃娃系統
    void RagdollActive(bool active)
    {
        ani.enabled = !active; // 關閉或開啟角色的 Animator
        rigid.isKinematic = active; // 關閉或開啟角色原本的 rigidbody
        cld.enabled = !active; // 關閉或開啟角色原本的(膠囊)Collider

        // 搜尋骨架以下所有的 Rigidbody 關閉或開啟
        Rigidbody[] rigs = root.GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rigs.Length; i++)
        {
            rigs[i].isKinematic = !active;
        }

        // 搜尋骨架以下所有的 Collider 關閉或開啟
        Collider[] clds = root.GetComponentsInChildren<Collider>();
        for (int i = 0; i < clds.Length; i++)
        {
            clds[i].enabled = active;
        }
    }

    // 開啟角色物理娃娃
    public void SetDeathRagdoll()
    {
        RagdollActive(true);
    }
    public void Damage(float Power)
    {
        hp -= Power; // 扣血
        if (hp <= 0)
        {
            hp = 0; // 不要扣到負值
            monster01.enabled = false; // 關閉 AI 腳本
            agent.enabled = false; // 立即關閉尋徑功能
            ani.SetTrigger("Die");
        }

        RefreshLifebar(); // 更新血條
    }

    void RefreshLifebar() // 更新血條 UI
    {
        hpImage.fillAmount = hp / hpFull; //顯示血球
    }


}
