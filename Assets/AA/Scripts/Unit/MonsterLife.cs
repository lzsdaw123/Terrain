﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterLife : MonoBehaviour
{
    [SerializeField] Transform root;

    public Animator ani;
    private Rigidbody rigid;
    private Collider cld;

    public int MonsterType;  //怪物類型 0=蠍子 / 1= 螃蟹
    public float[] hpFull = new float[] { 14, 20 }; // 血量上限
    public float hp; // 血量
    int HpLv;  //生命等級
    int Level;  //難度等級
    //public Image hpImage;

    private NavMeshAgent agent;
    public MonsterAI02 monster02;
    public MonsterAI03 monster03;

    public GameObject PS_Dead;
    [SerializeField] float DeadTime;

    public GameObject HitUI;  //命中UI

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        cld = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
        monster02 = GetComponent<MonsterAI02>();
        monster03 = GetComponent<MonsterAI03>();
        HitUI = GameObject.Find("HitUI").gameObject;
    }
    void Start()
    {
        PS_Dead.SetActive(false);
        hp = hpFull[MonsterType];  //補滿血量
        DeadTime = 0;
        DifficultyUp();  //難度調整
        RefreshLifebar(); // 更新血條
        //ani = GetComponent<Animator>();
       
        //RagdollActive(false); // 先關閉物理娃娃
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Y)) // 測試用
        //{
        //    monster02.enabled = false; // 關閉控制腳本
        //    agent.enabled = false;  //立即關閉尋徑功能
        //    ani.SetTrigger("Die");
        //}
        if (PS_Dead.activeSelf)
        {
            DeadTime += 1.6f * Time.deltaTime;
            if (DeadTime >= 1)
            {
                GameObject.Find("ObjectPool").GetComponent<ObjectPool>().RecoveryMonster01(gameObject);
            }
        }
        if (HitUI.activeSelf)  //命中UI
        {
            HitUI.transform.localScale -= new Vector3(0.15f, 0.15f, 0.15f)*10*Time.deltaTime;
            Vector3 Z = new Vector3(0, 0f, 0f);
            if (HitUI.transform.localScale.x <= Z.x)
            {
                HitUI.SetActive(false);
                HitUI.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
            }
        }
    }
    // 開啟或關閉物理娃娃系統
    void RagdollActive(bool active)
    {
        ani.enabled = !active; // 關閉或開啟角色的 Animator
        rigid.isKinematic = active; // 關閉或開啟角色原本的 rigidbody
        //cld.enabled = !active; // 關閉或開啟角色原本的(膠囊)Collider

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
        if (hp < 0) return;
        hp -= Power; // 扣血
        HitUI.SetActive(true);
        HitUI.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
        HitUI.GetComponent<Image>().color = Color.white;
        if (hp <= 0)
        {
            HitUI.GetComponent<Image>().color = Color.red;
            hp = -1; // 不要扣到負值
            PS_Dead.SetActive(true);
            switch (MonsterType)
            {
                case 0:
                    monster02.enabled = false; // 關閉 AI 腳本
                    break;
                case 1:
                    monster03.enabled = false; // 關閉 AI 腳本
                    break;
            }           
            agent.enabled = false; // 立即關閉尋徑功能
            ani.SetTrigger("Die");           
        }

        RefreshLifebar(); // 更新血條
    }

    void RefreshLifebar() // 更新血條 UI
    {
        //hpImage.fillAmount = hp / hpFull; //顯示血球
    }
    void DifficultyUp()  //難度設定
    {
        HpLv = Level_1.MonsterLevel;
        Level = Settings.Level;
        Level = Level +1;
        if (HpLv > 0)
        {
            hpFull[MonsterType] = 7 + (HpLv * Level);
            if (hpFull[MonsterType] >= 7 + (5 * Level))
            {
                hpFull[MonsterType] = 7 + (5 * Level);
            }
        }
        //print("怪物血量:" + hpFull);  //最終血量 12 / 17 / 22 
        hpFull = new float[] { 14, 20 };
        hp = hpFull[MonsterType];  //補滿血量
    }
    void OnDisable()
    {
        Scoreboard.AddScore(true);  //怪物擊殺分數
        Shop.AddKillScore();  //怪物擊殺分數
        DifficultyUp();      
        PS_Dead.SetActive(false);
        DeadTime = 0;
        switch (MonsterType)
        {
            case 0:
                monster02.enabled = true;
                break;
            case 1: 
                monster03.enabled = true;
                break;
        }
        agent.enabled = true; // 開啟尋徑功能
    }
}
