using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ObjectPool : MonoBehaviour
{
    public  GameObject Bullet, Hit;
    public  GameObject BulletPool, HitPool, MBulletPool, MonsterPool_A, MonsterPool_B;  //物件池集中位置
    public GameObject[] Monster;	// 可生的怪種類
    public GameObject MBullet ;	// 怪物子彈
    public SpawnRay _SpawnRay;

    public int inttailSize;  //預置物件數量
    Vector3 muzzlePOS;

    private int uid = 0;								// 怪物編號


    private Queue<GameObject> _pool = new Queue<GameObject>();
    private Queue<GameObject> _pool_Hit = new Queue<GameObject>();

    private Queue<GameObject> Monster_poolA = new Queue<GameObject>();
    private Queue<GameObject> Monster_poolB = new Queue<GameObject>();
    private Queue<GameObject> M_Bullet_pool = new Queue<GameObject>();


    void Awake()
    {
        //DontDestroyOnLoad(gameObject);  //切換場景時保留
        inttailSize = 8;  //物件池大小

        for (int cut =0;cut< inttailSize; cut++)
        {
            //GameObject go = Instantiate(Bullet) as GameObject;
            GameObject go = Instantiate(Bullet, BulletPool.transform) as GameObject; //生成子彈於子彈池
            GameObject go2 = Instantiate(Hit, HitPool.transform) as GameObject;   //生成彈孔於彈孔池
            GameObject Mo1B = Instantiate(MBullet, MBulletPool.transform) as GameObject;   //怪物子彈於怪物子彈池
            //int monsterNum = (int)(Random.value * Monster.Length);	// 亂數取得一隻怪
            GameObject Mo1 = Instantiate(Monster[0], MonsterPool_A.transform) as GameObject;   //生成怪物於怪物池
            GameObject Mo2 = Instantiate(Monster[1], MonsterPool_B.transform) as GameObject;   //生成怪物於怪物池
            uid++;                                      // 編號加1

            if (!Mo1.GetComponent<SpawnRayReg>())   // 怪物一定要有這個腳本
                    Mo1.AddComponent<SpawnRayReg>();
            Mo1.SendMessage("Init", new MonterInfo(uid, _SpawnRay, 0));
            if (!Mo2.GetComponent<SpawnRayReg>())   // 怪物一定要有這個腳本
                Mo2.AddComponent<SpawnRayReg>();
            Mo2.SendMessage("Init", new MonterInfo(uid, _SpawnRay, 1));

            _pool.Enqueue(go);  //Queue.Enqueue() 將物件放入結構中
            _pool_Hit.Enqueue(go2);  //Queue.Enqueue() 將物件放入結構中
            Monster_poolA.Enqueue(Mo1);  //Queue.Enqueue() 將怪物1放入結構中
            Monster_poolB.Enqueue(Mo2);  //Queue.Enqueue() 將怪物2放入結構中
            M_Bullet_pool.Enqueue(Mo1B);  //Queue.Enqueue() 將怪物1子彈放入結構中
            go.SetActive(false);
            go2.SetActive(false);
            Mo1.SetActive(false);
            Mo2.SetActive(false);
            Mo1B.SetActive(false);                       
        }
    }
    //子彈
    public void ReUse (Vector3 positon, Quaternion rotation)  //取出存放在物件池中的物件
    {
        if (_pool.Count > 0)
        {
            GameObject reuse = _pool.Dequeue();  //Queue.Dequeue() 將最先進入的物件取出
            reuse.transform.position = positon;
            reuse.transform.rotation = rotation;         
            reuse.SetActive(true);
        }
        else
        {
            GameObject go = Instantiate(Bullet, BulletPool.transform) as GameObject;
            go.transform.position = positon;
            go.transform.rotation = rotation;
        }
    }
    public void Recovery( GameObject recovery)  //用來回收物件
    {
        _pool.Enqueue(recovery);
        recovery.SetActive(false);
    }

    //彈孔
    public void ReUseHit(Vector3 positon, Quaternion rotation,int HitType)  //取出存放在物件池中的物件
    {
        if (_pool_Hit.Count > 0)
        {
            GameObject reuse = _pool_Hit.Dequeue();  //Queue.Dequeue() 將最先進入的物件取出
            reuse.transform.position = positon;
            reuse.transform.rotation = rotation;
            GameObject reHit;
            reHit = reuse.transform.GetChild(HitType).gameObject;
            reHit.SetActive(true);
            reuse.SetActive(true);
        }
        else
        {
            GameObject go2 = Instantiate(Hit, HitPool.transform) as GameObject;
            go2.transform.position = positon;
            go2.transform.rotation = rotation;
            GameObject reHit;
            reHit = go2.transform.GetChild(HitType).gameObject;
            reHit.SetActive(true);
        }
    }
    public void RecoveryHit(GameObject recovery)  //用來回收物件
    {
        _pool_Hit.Enqueue(recovery);
        GameObject reHit;
        for (int i = 0; i < recovery.transform.childCount; i++)
        {
            reHit = recovery.transform.GetChild(i).gameObject;
            reHit.SetActive(false);
        }
        recovery.SetActive(false);
    }

    //怪物1
    public void ReUseMonster01(Vector3 positon, Quaternion rotation)  //取出存放在物件池中的物件
    {
        if (Monster_poolA.Count > 0)
        {
            GameObject reuse = Monster_poolA.Dequeue();  //Queue.Dequeue() 將最先進入的物件取出
            reuse.transform.position = positon;
            reuse.transform.rotation = rotation;
            reuse.SetActive(true);
        }
        else
        {
            int monsterNum = (int)(Random.value * Monster.Length);	// 亂數取得一隻怪
            GameObject Mo1 = Instantiate(Monster[monsterNum], MonsterPool_A.transform) as GameObject;  //生成怪物於怪物池
            uid++;                                      // 編號加1

            if (!Mo1.GetComponent<SpawnRayReg>())   // 怪物一定要有這個腳本
                Mo1.AddComponent<SpawnRayReg>();
            Mo1.SendMessage("Init", new MonterInfo(uid, _SpawnRay, 0));
            Mo1.transform.position = positon;
            Mo1.transform.rotation = rotation;
        }
    }
    public void RecoveryMonster01(GameObject recovery)  //用來回收物件
    {
        Monster_poolA.Enqueue(recovery);
        recovery.SetActive(false);
    }
    //怪物2
    public void ReUseMonster02(Vector3 positon, Quaternion rotation)  //取出存放在物件池中的物件
    {
        if (Monster_poolB.Count > 0)
        {
            GameObject reuse = Monster_poolB.Dequeue();  //Queue.Dequeue() 將最先進入的物件取出
            reuse.transform.position = positon;
            reuse.transform.rotation = rotation;
            reuse.SetActive(true);
        }
        else
        {
            int monsterNum = (int)(Random.value * Monster.Length);	// 亂數取得一隻怪
            GameObject Mo2 = Instantiate(Monster[monsterNum], MonsterPool_B.transform) as GameObject;  //生成怪物於怪物池
            uid++;                                      // 編號加1

            if (!Mo2.GetComponent<SpawnRayReg>())   // 怪物一定要有這個腳本
                Mo2.AddComponent<SpawnRayReg>();
            Mo2.SendMessage("Init", new MonterInfo(uid, _SpawnRay, 1));
            Mo2.transform.position = positon;
            Mo2.transform.rotation = rotation;
        }
    }
    public void RecoveryMonster02(GameObject recovery)  //用來回收物件
    {
        Monster_poolB.Enqueue(recovery);
        recovery.SetActive(false);
    }

    //怪物1 子彈
    public void ReUseM01Bullet(Vector3 positon, Quaternion rotation)  //取出存放在物件池中的物件
    {
        if (M_Bullet_pool.Count > 0)
        {
            GameObject reuse = M_Bullet_pool.Dequeue();  //Queue.Dequeue() 將最先進入的物件取出
            reuse.transform.position = positon;
            reuse.transform.rotation = rotation;
            reuse.SetActive(true);
        }
        else
        {
            GameObject Mo1B = Instantiate(MBullet, MBulletPool.transform) as GameObject;  //怪物子彈於怪物子彈池
            Mo1B.transform.position = positon;
            Mo1B.transform.rotation = rotation;
        }
    }
    public void RecoveryM01Bullet(GameObject recovery)  //用來回收物件
    {
        M_Bullet_pool.Enqueue(recovery);
        recovery.SetActive(false);
    }


}
